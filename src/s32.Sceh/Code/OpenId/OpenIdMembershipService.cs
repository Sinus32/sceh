using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
using s32.Sceh.Classes.OpenId;

namespace s32.Sceh.Code.OpenId
{
    public class OpenIdMembershipService
    {
        private readonly OpenIdRelyingParty openId;

        public OpenIdMembershipService()
        {
            openId = new OpenIdRelyingParty();
        }

        public IAuthenticationRequest ValidateAtOpenIdProvider(string openIdIdentifier)
        {
            var userSuppliedIdentifier = Identifier.Parse(openIdIdentifier);
            IAuthenticationRequest openIdRequest = openId.CreateRequest(userSuppliedIdentifier);
            
            return openIdRequest;
        }

        public OpenIdUser GetUser()
        {
            OpenIdUser user = null;
            IAuthenticationResponse openIdResponse = openId.GetResponse();

            if (openIdResponse != null && openIdResponse.Status == AuthenticationStatus.Authenticated)
            {
                user = ResponseIntoUser(openIdResponse);
            }

            return user;
        }

        private OpenIdUser ResponseIntoUser(IAuthenticationResponse response)
        {
            OpenIdUser user = null;
            var claimResponseUntrusted = response.GetUntrustedExtension<ClaimsResponse>();
            var claimResponse = response.GetExtension<ClaimsResponse>();

            if (claimResponse != null)
            {
                user = new OpenIdUser(claimResponse, response.ClaimedIdentifier);
            }
            else if (claimResponseUntrusted != null)
            {
                user = new OpenIdUser(claimResponseUntrusted, response.ClaimedIdentifier);
            }

            return user;
        }

        public HttpCookie CreateFormsAuthenticationCookie(OpenIdUser user)
        {
            var ticket = new FormsAuthenticationTicket(1, user.Nickname, DateTime.Now, DateTime.Now.AddDays(7), true, user.ToString());
            var encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);

            return cookie;
        }
    }
}