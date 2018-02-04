//using GetNearByLocation.Models;
//using SocialApp.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Web.Http.Cors;
//using System.Threading.Tasks;

//namespace SocialApp.API
//{
//    [RoutePrefix("api/user")]
//    [EnableCors(origins: "*", headers: "*", methods: "*")]
//    public class UserController : ApiController
//    {
//        private ApplicationUserManager _userManager;

//        //[Route("Authenticate")]
//        //[HttpGet]
//        //public bool Authenticate(string userEmail, string password)
//        //{

//        //}

//        //[Route("GetUserInformation")]
//        //[HttpGet]
//        //public User GetUserInformation(string latitude, string longitude)
//        //{

//        //}

//        [Route("AddUser")]
//        [HttpPost]
//        [AllowAnonymous]
//        public async Task<bool> Register([FromBody]UserInfo userInfo)
//        {
//            var user = new ApplicationUser { UserName = userInfo.Email, Email = userInfo.Email };
//            var result = await _userManager.CreateAsync(user, userInfo.Password);
//             return true;
//        }

//        //[Route("IsUserExists")]
//        //[HttpGet]
//        //public bool IsUserExists(string email)
//        //{

//        //}

//        //[Route("Forgot")]
//        //[HttpGet]
//        //public bool Forgot(string email)
//        //{

//        //}

//        //[Route("Forgot")]
//        //[HttpGet]
//        //public User(string latitude, string longitude)
//        //{

//        //}

 
//    }

//    public class UserInfo
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//        public string Name { get; set; }
//    }
//}
