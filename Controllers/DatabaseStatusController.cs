using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BinsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DatabaseStatusController : ControllerBase
    {
        // GET: api/DatabaseStatus
        [HttpGet]
        public List<Models.ProcessingTime> Get()
        {
            List<Models.ProcessingTime> ProcessingTimes = JsonSerializer.Deserialize<List<Models.ProcessingTime>>(System.IO.File.ReadAllText("DatabaseStatus.json"));
            return ProcessingTimes;
        }
    }
}