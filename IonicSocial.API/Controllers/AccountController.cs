using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using SocialApp.API.Models;
using SocialApp.API.Providers;
using SocialApp.API.Results;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace SocialApp.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }



        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }



        [HttpPost]
        [AllowAnonymous]
        [Route("FacebookLogin")]
        public async Task<IHttpActionResult> FacebookLogin([FromBody] FacebookLoginModel model)
        {
            var responseStatus = new ResponseStatus();
             
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(model.token))
            {
                
                return BadRequest("No access token");
            }

            var tokenExpirationTimeSpan = TimeSpan.FromDays(300);
            ApplicationUser user = null;
            string username;
            // Get the fb access token and make a graph call to the /me endpoint
            var fbUser = await VerifyFacebookAccessToken(model.token);
            if (fbUser == null)
            {
                return BadRequest("Invalid OAuth access token");
            }

            UserLoginInfo loginInfo = new UserLoginInfo("Facebook", model.userid);
            user = await UserManager.FindAsync(loginInfo);

            // If user not found, register him with username.
            if (user == null)
            {
                //check local user account exists or not 

                //check user is exists
                var isLocaUserExists = UserManager.FindByEmail(model.username);

                if (isLocaUserExists!=null)
                {
                    responseStatus.status = 2;
                    responseStatus.message="USERALREADYEXISTS";
                    return Json(responseStatus);
                }

                //check google account exists
                UserLoginInfo googleLoginInfo = new UserLoginInfo("Google", model.userid);
                user = await UserManager.FindAsync(googleLoginInfo);

                if (isLocaUserExists != null)
                {
                    responseStatus.status = 2;
                    responseStatus.message = "USERALREADYEXISTS";
                    return Json(responseStatus);
                }

                //is confirmed that no account exists in databsae create new account
                if (String.IsNullOrEmpty(model.username))
                    return BadRequest("unregistered user");

                user = new ApplicationUser { UserName = model.username ,Email = model.username };

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo);
                    username = model.username;
                    if (!result.Succeeded)
                        return BadRequest("cannot add facebook login");
                }
                else
                {
                    return BadRequest("cannot create user");
                }
            }
            else
            {
                // existed user.
                username = user.UserName;
            }

            // common process: Facebook claims update, Login token generation
            user = await UserManager.FindByNameAsync(username);

            // Optional: make email address confirmed when user is logged in from Facebook.
            user.Email = model.username;
            user.EmailConfirmed = true;
            await UserManager.UpdateAsync(user);

            // Sign-in the user using the OWIN flow
            var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);

            var claims = await UserManager.GetClaimsAsync(user.Id);
            var newClaim = new Claim("FacebookAccessToken", model.token); // For compatibility with ASP.NET MVC AccountController
            var oldClaim = claims.FirstOrDefault(c => c.Type.Equals("FacebookAccessToken"));
            if (oldClaim == null)
            {
                var claimResult = await UserManager.AddClaimAsync(user.Id, newClaim);
                if (!claimResult.Succeeded)
                    return BadRequest("cannot add claims");
            }
            else
            {
                await UserManager.RemoveClaimAsync(user.Id, oldClaim);
                await UserManager.AddClaimAsync(user.Id, newClaim);
            }

            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            properties.IssuedUtc = currentUtc;
            properties.ExpiresUtc = currentUtc.Add(tokenExpirationTimeSpan);
            AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
            var accesstoken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesstoken);
            Authentication.SignIn(identity);

            // Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
            JObject blob = new JObject(
                new JProperty("userName", user.UserName),
                new JProperty("access_token", accesstoken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", tokenExpirationTimeSpan.TotalSeconds.ToString()),
                new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()),
                new JProperty("model.token", model.token),
                new JProperty("userid", user.Id)

            );
            // Return OK
            return Ok(blob);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("GoogleLogin")]
        public async Task<IHttpActionResult> GoogleLogin([FromBody] GoogleLoginModel model)
        {
            var responseStatus = new ResponseStatus();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(model.token))
            {
                return BadRequest("No access token");
            }

            var tokenExpirationTimeSpan = TimeSpan.FromDays(300);
            ApplicationUser user = null;
            string username;
            // Get the fb access token and make a graph call to the /me endpoint
            var googleUser = await VerifyGoogleAccessToken(model.token);
            if (googleUser == null)
            {
                return BadRequest("Invalid OAuth access token");
            }

            UserLoginInfo loginInfo = new UserLoginInfo("Google", model.userid);
            user = await UserManager.FindAsync(loginInfo);

            // If user not found, register him with username.
            if (user == null)
            {

                //check user is exists
                var isLocaUserExists = UserManager.FindByEmail(model.username);

                if (isLocaUserExists != null)
                {
                    responseStatus.status = 2;
                    responseStatus.message = "USERALREADYEXISTS";
                    return Json(responseStatus);
                }

                //check google account exists
                UserLoginInfo googleLoginInfo = new UserLoginInfo("Facebook", model.userid);
                user = await UserManager.FindAsync(googleLoginInfo);

                if (isLocaUserExists != null)
                {
                    responseStatus.status = 2;
                    responseStatus.message = "USERALREADYEXISTS";
                    return Json(responseStatus);
                }

                if (String.IsNullOrEmpty(model.username))
                    return BadRequest("unregistered user");

                user = new ApplicationUser { UserName = model.username, Email = model.username };

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo);
                    username = model.username;
                    if (!result.Succeeded)
                        return BadRequest("cannot add google login");
                }
                else
                {
                    return BadRequest("cannot create user");
                }
            }
            else
            {
                // existed user.
                username = user.UserName;
            }

            // common process: google claims update, Login token generation
            user = await UserManager.FindByNameAsync(username);

            // Optional: make email address confirmed when user is logged in from Facebook.
            user.Email = model.username;
            user.EmailConfirmed = true;
            await UserManager.UpdateAsync(user);

            // Sign-in the user using the OWIN flow
            var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);

            var claims = await UserManager.GetClaimsAsync(user.Id);
            var newClaim = new Claim("GoogleAccessToken", model.token); // For compatibility with ASP.NET MVC AccountController
            var oldClaim = claims.FirstOrDefault(c => c.Type.Equals("GoogleAccessToken"));
            if (oldClaim == null)
            {
                var claimResult = await UserManager.AddClaimAsync(user.Id, newClaim);
                if (!claimResult.Succeeded)
                    return BadRequest("cannot add claims");
            }
            else
            {
                await UserManager.RemoveClaimAsync(user.Id, oldClaim);
                await UserManager.AddClaimAsync(user.Id, newClaim);
            }

            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            properties.IssuedUtc = currentUtc;
            properties.ExpiresUtc = currentUtc.Add(tokenExpirationTimeSpan);
            AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
            var accesstoken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesstoken);
            Authentication.SignIn(identity);

            // Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
            JObject blob = new JObject(
                new JProperty("userName", user.UserName),
                new JProperty("access_token", accesstoken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", tokenExpirationTimeSpan.TotalSeconds.ToString()),
                new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()),
                new JProperty("model.token", model.token),
                new JProperty("userid", user.Id)

            );
            // Return OK
            return Ok(blob);
        }


        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }


        // POST api/Account/RemoveLogin
        [Route("UserInformation")]
        public async Task<IHttpActionResult> UserInformation(string email)
        {
            var userInformation = new SocialApp.Data.clsUserInformation().GetUserInformation(email);

            return Ok(userInformation);
        }


        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            var responseStatus = new ResponseStatus();
            responseStatus.status = 0; //set internal error.

            //invalid model
            if (!ModelState.IsValid)
            {
                responseStatus.status = -1 ;
                return Ok(responseStatus);
            }   

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            //success
            if (result.Succeeded)
            {
                responseStatus.status = 1;
            }
            else
            {
                //check user is exists
                var isUserExists = UserManager.FindByEmail(model.Email);

                if(isUserExists!=null)
                {
                    responseStatus.status = 2;
                }
            }

            return Ok(responseStatus);
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        private async Task<FacebookUserViewModel> VerifyFacebookAccessToken(string accessToken)
        {
            FacebookUserViewModel fbUser = null;
            var path = "https://graph.facebook.com/me?access_token=" + accessToken;
            var client = new HttpClient();
            var uri = new Uri(path);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserViewModel>(content);
            }
            return fbUser;
        }

        private async Task<GoogleUserViewModel> VerifyGoogleAccessToken(string accessToken)
        {
            GoogleUserViewModel googleUser = null;
            var path = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken;
            var client = new HttpClient();
            var uri = new Uri(path);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                googleUser = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleUserViewModel>(content);
            }
            return googleUser;
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }

    public class FacebookLoginModel
    {
        public string token { get; set; }
        public string username { get; set; }
        public string userid { get; set; }
    }

    public class FacebookUserViewModel
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }


    public class GoogleLoginModel
    {
        public string token { get; set; }
        public string username { get; set; }
        public string userid { get; set; }
    }

    public class GoogleUserViewModel
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
    }

}
