using Microsoft.AspNetCore.Mvc;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GetUsageDataController : ControllerBase
    {
        public ReturnObject Get()
        {
            SQLiteConnection AppUsesDB = new SQLiteConnection("Data/postcodelog.db");

            string MinDateText = AppUsesDB.ExecuteScalar<string>("select min(DateText) from AppUse");

            Dictionary<string, int> NewPostcodesUsedByDateDictionary = AppUsesDB.Query<NewPostcodesUsedByDate>(@"select DateText, count(Postcode) as NewPostcodesUsed from (select Postcode, min(DateText) as DateText from AppUse group by Postcode) group by DateText").ToDictionary(x => x.DateText, x => x.NewPostcodesUsed);
            Dictionary<string, int> LookupsByDateDictionary = AppUsesDB.Query<NewPostcodesUsedByDate>(@"select DateText, count(DateText) as NewPostcodesUsed from AppUse group by DateText").ToDictionary(x => x.DateText, x => x.NewPostcodesUsed);

            List<UsageByWard> UseByWard = AppUsesDB.Query<UsageByWard>(@"select Wardcode, Count(Postcode) as totalusage, Count(Distinct(Postcode)) as uniquepostcodeusage from AppUse group by Wardcode").ToList();

            DateTime MinDateDate = DateTime.Parse(MinDateText);
            List<UsageByDate> AppUseByDate = new List<UsageByDate>();
            int CumulativeDailyUse = 0;
            int CumulativeUniques = 0;
            for (DateTime date = MinDateDate; date < DateTime.Now; date = date.AddDays(1))
            {
                int DailyUse = 0;
                LookupsByDateDictionary.TryGetValue(date.ToString("yyyy-MM-dd"), out DailyUse);
                CumulativeDailyUse += DailyUse;

                int NewPostcodesToday = 0;
                NewPostcodesUsedByDateDictionary.TryGetValue(date.ToString("yyyy-MM-dd"), out NewPostcodesToday);
                CumulativeUniques += NewPostcodesToday;

                UsageByDate UBD = new UsageByDate()
                {
                    DateText = date.ToString("yyyy-MM-dd"),
                    DailyUse = DailyUse,
                    CumulativeDailyUse = CumulativeDailyUse,
                    CumulativeUniquePostcodes = CumulativeUniques
                };
                AppUseByDate.Add(UBD);
            }

            ReturnObject RO = new ReturnObject()
            {
                UseByDate = AppUseByDate,
                UseByWard = UseByWard
            };

            return RO;
        }
    }

    public class ReturnObject
    {
        public List<UsageByWard> UseByWard { get; set; }
        public List<UsageByDate> UseByDate { get; set; }
    }

    public class PCDTOWARD
    {
        public string pcds { get; set; }
        public string ward { get; set; }
    }
    public class UsageByWard
    {
        public string Wardcode { get; set; }
        public int totalusage { get; set; }
        public int uniquepostcodeusage { get; set; }
    }
    public class UsageByDate
    {
        public string DateText { get; set; }
        public int DailyUse { get; set; }
        public int CumulativeDailyUse { get; set; }
        public int CumulativeUniquePostcodes { get; set; }
    }
    public class NewPostcodesUsedByDate
    {
        public string DateText { get; set; }
        public int NewPostcodesUsed { get; set; }
    }

    public class AppUse
    {
        [Indexed]
        public string DateTimeText { get; set; }
        [Indexed(Name = "PCDTIndex", Order = 2)]
        public string DateText { get; set; }
        [Indexed(Name = "PCDTIndex", Order = 1)]
        public string Postcode { get; set; }
        [Indexed]
        public string Wardcode { get; set; }
    }
}
