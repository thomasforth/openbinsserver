using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLite;

namespace BinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogAppUseController : ControllerBase
    {
        // GET: api/LogAppUse
        [HttpGet]
        public string Get()
        {
            string postcode = Request.Query["Postcode"].ToString();

            SQLiteConnection BinUsageDB = new SQLiteConnection(@"Data/BinUsage.db");
            BinUsageDB.CreateTable<Models.AppUse>();

            Models.AppUse appUse = new Models.AppUse()
            {
                Postcode = postcode,
                DateTime = DateTime.Now
            };

            BinUsageDB.Insert(appUse);
            BinUsageDB.Close();

            return "success";
        }
    }
}
