using SocialApp.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.Data
{
    public class clsUserInformation
    {
        public UserInformation GetUserInformation(string email)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var userInformation = new UserInformation();

                var userinfo = context.AspNetUsers.Where(x => x.Email == email).SingleOrDefault();
                if(userinfo!=null)
                {
                   
                    userInformation.userid = userinfo.Id;
                    userInformation.email = userinfo.UserName;
                }

                return userInformation;
            }
        }
    }

    public class UserInformation

    {
        public string userid { get; set; }
        public string email { get; set; }
    }
}
