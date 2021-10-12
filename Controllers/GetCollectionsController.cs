using Microsoft.AspNetCore.Mvc;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace BinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GetCollectionsController : ControllerBase
    {
        [HttpGet]
        public List<Models.Job> Get()
        {
            int PremisesID = int.Parse(Request.Query["PremisesID"]);
            string LocalAuthority = Request.Query["LocalAuthority"].ToString();
            SQLiteConnection BinsDB = new SQLiteConnection(@"Data/Bins_active.db");
            List<Models.Job> Jobs = BinsDB.Table<Models.Job>().Where(x => x.PremiseID == PremisesID).OrderBy(x => x.CollectionDate).ToList();

            return Jobs.Where(x => x.LocalAuthority == LocalAuthority).ToList();
        }

    }
}
