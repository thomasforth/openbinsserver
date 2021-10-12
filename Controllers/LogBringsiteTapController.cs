using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLite;

namespace BinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogBringsiteTapController : ControllerBase
    {
        // GET: api/LogBringsiteTap
        [HttpGet]
        public string Get()
        {
            string postcode = Request.Query["Postcode"].ToString();
            string site = Request.Query["Site"].ToString();

            SQLiteConnection BinUsageDB = new SQLiteConnection(@"Data/BinUsage.db");
            BinUsageDB.CreateTable<Models.BringSiteInteraction>();

            Models.BringSiteInteraction bringSiteInteraction = new Models.BringSiteInteraction()
            {
                Postcode = postcode,
                DateTime = DateTime.Now,
                Site = site
            };

            BinUsageDB.Insert(bringSiteInteraction);
            BinUsageDB.Close();

            return "success";
        }
    }
}
