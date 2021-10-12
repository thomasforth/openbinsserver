using CsvHelper.Configuration;
using SQLite;
using System;

namespace BinsAPI.Models
{
    public class Secrets
    {
        public string host { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    class JobClassMap : ClassMap<Job>
    {
        public JobClassMap()
        {
            Map(m => m.PremiseID).Index(0);
            Map(m => m.BinType).Index(1);
            Map(m => m.CollectionDate).Index(2).TypeConverterOption.Format("dd/MM/yy");
        }
    }

    class PremiseClassMap : ClassMap<Premise>
    {
        public PremiseClassMap()
        {
            Map(m => m.PremiseID).Index(0);
            Map(m => m.Address1).Index(1);
            Map(m => m.Address2).Index(2);
            Map(m => m.Street).Index(3);
            Map(m => m.Locality).Index(4);
            Map(m => m.Town).Index(5);
            Map(m => m.Postcode).Index(6);
        }
    }

    public class ProcessingTime
    {
        public string Action { get; set; }
        public DateTime DateCompleted { get; set; }
        public string TimeSpent { get; set; }
        public int LinesInJobsFile { get; set; }
        public int JobRecordsInDatabase { get; set; }
        public int LinesInPremisesFile { get; set; }
        public int PremiseRecordsInDatabase { get; set; }
    }

    class AppUse
    {
        public DateTime DateTime { get; set; }
        public string Postcode { get; set; }
    }

    class BringSiteInteraction
    {
        public string Postcode { get; set; }
        public DateTime DateTime { get; set; }
        public string Site { get; set; }
    }

    class PostcodeLookup
    {
        [PrimaryKey]
        public string pcds { get; set; }
        public string oa11 { get; set; }

        public double lat { get; set; }

        public double longitude { get; set; }
        public string osward { get; set; }
    }

    public class Job
    {
        [Indexed]
        public int? PremiseID { get; set; }
        public string BinType { get; set; }
        [Indexed]
        public string LocalAuthority { get; set; }
        public DateTime? CollectionDate { get; set; }
    }

    public class Premise
    {
        [Indexed]
        public int PremiseID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Street { get; set; }
        public string Locality { get; set; }
        [Indexed]
        public string LocalAuthority { get; set; }
        public string Town { get; set; }
        [Indexed]
        public string Postcode { get; set; }
    }

    public class Location
    {
        public string Postcode { get; set; }
        public string OutputArea { get; set; }
        public string WardCode { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
    }
}
