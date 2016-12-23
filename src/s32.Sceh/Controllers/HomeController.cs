using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using s32.Sceh.Classes;
using s32.Sceh.Code;
using s32.Sceh.Data;
using s32.Sceh.Models;

namespace s32.Sceh.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("LogIn");

            var viewModel = new IndexViewModel(new IndexModel() { MyProfile = User.Identity.Name });

            var tradeSuggestions = Session[id == null ? "LastResult" : "Result$" + id] as TradeSuggestions;
            if (tradeSuggestions != null)
                FillViewModel(viewModel, tradeSuggestions);

            //using (var context = new ScehContext())
            //{
            //    var u = new SteamUser();
            //    u.Login = "qwer";
            //    u.SteamId = 234567876543L;
            //    u.UserUrl = null;
            //    context.SteamUsers.Add(u);
            //    context.SaveChanges();
            //}

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IndexModel input)
        {
            if (ModelState.IsValid)
            {
                string errorMessage;
                var result = TradeSuggestionsMaker.Generate(User.Identity.Name, input.OtherProfile, out errorMessage);
                if (errorMessage == null)
                {
                    Session["LastResult"] = result;
                    Session["Result$" + input.OtherProfile] = result;
                    return RedirectToAction("Index", new { id = input.OtherProfile });
                }
                ModelState.AddModelError(String.Empty, errorMessage);
            }

            input.MyProfile = User.Identity.Name;
            var viewModel = new IndexViewModel(input);
            return View(viewModel);
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LoginModel input)
        {
            if (ModelState.IsValid)
            {
                var userId = TryAuthenticate(input.MyProfile);
                if (userId != Guid.Empty)
                    return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return JavaScript("window.location.href = '" + VirtualPathUtility.ToAbsolute(FormsAuthentication.LoginUrl) + "';");
        }

        private void FillViewModel(IndexViewModel viewModel, TradeSuggestions tradeSuggestions)
        {
            viewModel.Input.OtherProfile = tradeSuggestions.OtherInv.User;
            viewModel.MyInv = tradeSuggestions.MyInv;
            viewModel.OtherInv = tradeSuggestions.OtherInv;
            viewModel.SteamApps = tradeSuggestions.SteamApps;
            viewModel.OriginalsUsed = tradeSuggestions.OriginalsUsed;
            viewModel.ThumbnailsUsed = tradeSuggestions.ThumbnailsUsed;
        }

        private Guid TryAuthenticate(string login)
        {
            var user = SteamUsers.Get(login);

            if (user == null)
                return Guid.Empty;

            FormsAuthentication.SetAuthCookie(login, true);

            return user.Id;
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
