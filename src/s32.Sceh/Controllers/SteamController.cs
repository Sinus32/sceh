using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using s32.Sceh.Code.OpenId;
using s32.Sceh.Models;

namespace s32.Sceh.Controllers
{
    public class SteamController : Controller
    {
        private readonly OpenIdMembershipService openidemembership;

        public SteamController()
        {
            openidemembership = new OpenIdMembershipService();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            var user = openidemembership.GetUser();
            if (user != null)
            {
                var cookie = openidemembership.CreateFormsAuthenticationCookie(user);
                HttpContext.Response.Cookies.Add(cookie);

                return new RedirectResult(Request.Params["ReturnUrl"] ?? "/");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string provider)
        {
            var response = openidemembership.ValidateAtOpenIdProvider(provider);

            if (response != null)
            {
                return response.RedirectingResponse.AsActionResult();
            }

            return View();
        }
    }
}
