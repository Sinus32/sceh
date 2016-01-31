using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
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
        [HttpPost]
        public ActionResult BeginLogin()
        {
            var openId = new OpenIdRelyingParty();
            var userSuppliedIdentifier = Identifier.Parse("https://steamcommunity.com/openid");
            string hostHeader = Request.Headers["host"];
            var returnToUrl = new Uri(String.Format("{0}://{1}", Request.Url.Scheme, hostHeader));
            var realm = new Realm(returnToUrl);
            returnToUrl = new Uri(returnToUrl, Url.Action("FinalizeLogin"));
            IAuthenticationRequest openIdRequest = openId.CreateRequest(userSuppliedIdentifier, realm, returnToUrl);
            return openIdRequest.RedirectingResponse.AsActionResult();
        }

        [AllowAnonymous]
        public ActionResult FinalizeLogin()
        {
            var openId = new OpenIdRelyingParty();
            IAuthenticationResponse openIdResponse = openId.GetResponse();
            if (openIdResponse != null && openIdResponse.Status == AuthenticationStatus.Authenticated)
            {
                var userId = openIdResponse.FriendlyIdentifierForDisplay.Substring(openIdResponse.FriendlyIdentifierForDisplay.LastIndexOf('/') + 1);
                var ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddDays(7), true, openIdResponse.ClaimedIdentifier);
                var encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                HttpContext.Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Home");
        }
    }
}
