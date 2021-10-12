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
    [Produces("application/json")]
    public class GetAddressController : ControllerBase
    {
        [HttpGet]
        public List<Models.Premise> Get()
        {
            string Postcode = Request.Query["Postcode"].ToString();

            // add a space back into a postcode that doesn't have one
            Postcode = Postcode.Trim();
            if (Postcode.Substring(Postcode.Length - 3, 1) != " ")
            {
                Postcode = Postcode.Insert(Postcode.Length - 3, " ");
            }

            SQLiteConnection BinsDB = new SQLiteConnection(@"Data/Bins_active.db");
            List<Models.Premise> Premises = BinsDB.Table<Models.Premise>().Where(x => x.Postcode == Postcode.ToUpper()).ToList();

            return Premises;
        }

    }
}
