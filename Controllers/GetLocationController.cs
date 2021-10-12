using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLite;

namespace BinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GetLocationController : ControllerBase
    {
        [HttpGet]
        public List<Models.Location> Get()
        {
            string postcode = Request.Query["Postcode"].ToString();            
            List<Models.Location> Locations = new List<Models.Location>();

            SQLiteConnection BinsDB = new SQLiteConnection(@"Data/Bins_active.db");
            List<Models.PostcodeLookup> Addresses = BinsDB.Table<Models.PostcodeLookup>().Where(x => x.pcds == postcode.ToUpper()).ToList();

            foreach (Models.PostcodeLookup address in Addresses)
            {
                Locations.Add(new Models.Location()
                {
                    Latitude = address.lat,
                    Longitude = address.longitude,
                    OutputArea = address.oa11,
                    Postcode = address.pcds,
                    WardCode = address.osward
                });
            }

            return Locations;
        }
    }
}
