using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using s32.Sceh.Classes;
using s32.Sceh.Code;
using s32.Sceh.Models;

namespace s32.Sceh.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string me)
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (TryAuthenticate(me))
                    return RedirectToAction("Index");
            }



            var viewModel = Session["Result"] as TradeSuggestions;
            if (viewModel == null)
            {
                viewModel = new IndexViewModel(new IndexModel()
                {
                    MyProfile = me,
                    OtherProfile = null
                });
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexModel input)
        {
            if (ModelState.IsValid)
            {
                string errorMessage;
                var result = TradeSuggestionsMaker.Generate(input.MyProfile, input.OtherProfile, out errorMessage);
                if (errorMessage == null)
                {
                    Session["Result"] = result;
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(String.Empty, errorMessage);
            }

            var viewModel = new IndexViewModel(input);
            return View(viewModel);
        }

        public ActionResult Login()
        {
            return View();
        }

        private Guid? TryAuthenticate(string me)
        {
            var user = SteamUsers.Get(me);
        }

        public ActionResult GetCardPrice(string marketHashName)
        {
            var url = String.Concat("http://steamcommunity.com/market/priceoverview/?country=PL&currency=3&appid=753&market_hash_name=", HttpUtility.UrlEncode(marketHashName));
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10000;
            request.Accept = "application/json";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Referer = "http://steamcommunity.com/";

            string rawJson;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (!response.ContentType.StartsWith("application/json"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    rawJson = reader.ReadToEnd();
                }
            }
            return Content(rawJson, "application/json");
        }
    }
}
