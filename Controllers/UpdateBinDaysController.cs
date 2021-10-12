using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using BinsAPI.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Hangfire;
using Hangfire.Storage.Monitoring;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using SQLite;

namespace BinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UpdateBinDaysController : ControllerBase
    {
        static Secrets _secrets = null;
        
        // GET: api/UpdateBinDays
        [HttpGet]
        public string Get()
        {
            if (_secrets == null)
            {
                _secrets = System.Text.Json.JsonSerializer.Deserialize<Secrets>(System.IO.File.ReadAllText("secrets.json"));
            }

            List<QueueWithTopEnqueuedJobsDto> CurrentlyRunningQueues = JobStorage.Current.GetMonitoringApi().Queues().ToList();
            if (CurrentlyRunningQueues.Count > 0)
            {
                return "Database update is still running. Only one update can run at a time.";
            }
            else
            {
                string jobId = BackgroundJob.Enqueue(() => UpdateAllBinsWithTiming());
                return "Database update has been started. This is a background process (it could be automated if required in the future). You can check for progress at /api/databasestatus";
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public static void UpdateAllBinsWithTiming()
        {
            if (Directory.Exists("Data") == false)
            {
                Directory.CreateDirectory("Data");
            }
            List<ProcessingTime> ProcessingTimes = new List<ProcessingTime>();
            ProcessingTimes.Add(UpdateBinsData("Leeds"));
            ProcessingTimes.Add(UpdateBinsData("Fenland"));
            ProcessingTimes.Add(UpdateBinsData("Luton"));
            LoadPostcodeData();
            System.IO.File.WriteAllText("DatabaseStatus.json", JsonSerializer.Serialize(ProcessingTimes));
        }

        static void LoadPostcodeData()
        {
            Console.WriteLine("Reading postcode details.");
            List<PostcodeLookup> PostcodeLookups = new List<PostcodeLookup>();

            using (TextReader textReader = System.IO.File.OpenText("Assets/ONSPD_NOV_2019_UK.csv"))
            {
                CsvConfiguration csvconfig = new CsvConfiguration(CultureInfo.InvariantCulture) { MissingFieldFound = null };
                using (CsvReader csvReader = new CsvReader(textReader, csvconfig))
                {
                    PostcodeLookups = csvReader.GetRecords<PostcodeLookup>().ToList();
                }
            }
            using (SQLiteConnection FullOrgs = new SQLiteConnection(@"Data/Bins_active.db"))
            {
                FullOrgs.DropTable<PostcodeLookup>();
                FullOrgs.CreateTable<PostcodeLookup>();
                FullOrgs.InsertAll(PostcodeLookups);
            }
        }

        static ProcessingTime UpdateBinsData(string localauthority)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"Connecting to Bartec server and downloading jobs and premises for {localauthority}.");

            ProcessingTime PR = new ProcessingTime()
            {
                Action = $"Load {localauthority} bin days"
            };

            using (var sftp = new SftpClient(_secrets.host, _secrets.username, _secrets.password))
            {
                sftp.Connect();
                Console.WriteLine("Downloading jobs and premises.");
                if (System.IO.File.Exists($"{localauthority}_premises.csv"))
                {
                    System.IO.File.Delete($"{localauthority}_premises.csv");
                }
                using (var file = System.IO.File.OpenWrite($"{localauthority}_premises.csv"))
                {
                    sftp.DownloadFile($"{localauthority}/{localauthority}_premises.csv", file);
                }

                if (System.IO.File.Exists($"{localauthority}_jobs.csv"))
                {
                    System.IO.File.Delete($"{localauthority}_jobs.csv");
                }
                using (var file = System.IO.File.OpenWrite($"{localauthority}_jobs.csv"))
                {

                    sftp.DownloadFile($"{localauthority}/{localauthority}_jobs.csv", file);
                }
                sftp.Disconnect();
            }

            // Read jobs and premises files
            Console.WriteLine("Reading jobs and premises.");
            List<Premise> Premises = new List<Premise>();
            using (TextReader textReader = System.IO.File.OpenText($"{localauthority}_premises.csv"))
            {
                CsvConfiguration csvconfig = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false};                
                using (CsvReader csvReader = new CsvReader(textReader,csvconfig))
                {                    
                    csvReader.Context.RegisterClassMap<PremiseClassMap>();
                    Premises = csvReader.GetRecords<Premise>().ToList();
                    PR.LinesInPremisesFile = Premises.Count;
                }
            }
            List<Job> Jobs = new List<Job>();
            using (TextReader textReader = System.IO.File.OpenText($"{localauthority}_jobs.csv"))
            {
                CsvConfiguration csvconfig = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false};
                using (CsvReader csvReader = new CsvReader(textReader,csvconfig))
                {
                    csvReader.Context.RegisterClassMap<JobClassMap>();
                    Jobs = new List<Job>(csvReader.GetRecords<Job>());
                    PR.LinesInJobsFile = Jobs.Count;
                }
            }

            Premises.ForEach(x => x.LocalAuthority = localauthority);
            Jobs.ForEach(x => x.LocalAuthority = localauthority);

            // Connect to MySQL database            
            Console.WriteLine("Connecting to SQLite database.");
            using (SQLiteConnection FullOrgs = new SQLiteConnection(@"Data/Bins_active.db"))
            {
                Console.WriteLine($"Writing {localauthority} jobs data to SQLite database.");
                FullOrgs.CreateTable<Job>();
                FullOrgs.Query<Job>($"delete from Job where LocalAuthority = '{localauthority}'");
                FullOrgs.InsertAll(Jobs);
                PR.JobRecordsInDatabase = FullOrgs.Table<Job>().Where(x => x.LocalAuthority == localauthority).Count();
                Console.WriteLine($"Writing {localauthority} premises data to SQLite database.");
                FullOrgs.CreateTable<Premise>();
                FullOrgs.Query<Premise>($"delete from Premise where LocalAuthority = '{localauthority}'");
                FullOrgs.InsertAll(Premises);
                PR.PremiseRecordsInDatabase = FullOrgs.Table<Premise>().Where(x => x.LocalAuthority == localauthority).Count();
            }

            PR.DateCompleted = DateTime.Now;
            PR.TimeSpent = $"{Math.Round(stopwatch.Elapsed.TotalSeconds, 0)} seconds.";

            return PR;
        }
    }
}
