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
    public class CheckForMessagesController : ControllerBase
    {
        // GET: api/DatabaseStatus
        [HttpGet]
        public ResponseMessage Get()
        {
            string Postcode = Request.Query["Postcode"].ToString();

            // add a space back into a postcode that doesn't have one
            Postcode = Postcode.Trim();
            if (Postcode.Substring(Postcode.Length - 4, 1) != " ")
            {
                Postcode = Postcode.Insert(Postcode.Length - 3, " ");
            }

            SQLiteConnection BinsDB = new SQLiteConnection(@"Data/Bins_active.db");
            string localAuthority = BinsDB.Table<Models.Premise>().Where(x => x.Postcode == Postcode.ToUpper()).Select(x => x.LocalAuthority).FirstOrDefault();

            ResponseMessage responseMessage = new ResponseMessage()
            {
                RequestPostcode = Postcode,
                LocalAuthority = localAuthority,
                MessageLink = "https://publichealthmatters.blog.gov.uk/2020/01/23/wuhan-novel-coronavirus-what-you-need-to-know/",
                MessageContentHTML = "The data in this app is updated daily but collections may be changed at short notice in April, May, and June.",
                MessageBackgroundColour = "#ffeb3b",
                MessageTextColour = "#202a30"
            };

            if (localAuthority == "Leeds")
            {
                //responseMessage.MessageContentHTML = "Covid-19 Update: lockdown may be easing but there is still 20% more black bin waste every day to collect, with 10% of our staff off work shielding. Black and green bins will continue to be prioritised. All 8 Council Household Waste and Recycling Centres are open – to book a visit tap here.";
                //responseMessage.MessageContentHTML = "Covid19 Lockdown 3 update: black and green bins should still be collected on the scheduled day. If your street is missed leave your bin out for a couple of days and we will try to return. All 8 household waste and recycling centres are OPEN. You must book a slot. Tap to book now.";
                //responseMessage.MessageContentHTML = "If your street is missed, please leave your bin out for 2 days. If we don’t empty it within this time, return it to your property. The 8 household waste and recycling centres can be visited by booking a slot. Tap to book now.";
                responseMessage.MessageContentHTML = "You can put most plastics, metals/cans, packaging, pots, tubs and trays in your GREEN bin.";
                responseMessage.MessageLink = "https://www.leeds.gov.uk/residents/bins-and-recycling/your-bins/green-recycling-bin";
            }

            return responseMessage;
        }
    }

    public class ResponseMessage
    {
        public string RequestPostcode { get; set; }
        public string LocalAuthority { get; set; }
        public string MessageLink { get; set; }
        public string MessageContentHTML { get; set; }
        public string MessageBackgroundColour { get; set; }
        public string MessageTextColour { get; set; }
    }
}
