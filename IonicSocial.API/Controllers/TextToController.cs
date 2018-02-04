using SocialApp.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SocialApp.API
{
    [Authorize]
    [RoutePrefix("api/textto")]
    public class TextToController : ApiController
    {

        [HttpPost]
        public bool AddTextTo([FromBody] TextToInfo textToInfo)
        {
            return new clsTextTo().AddTextTo(textToInfo);
        }

        [HttpDelete]
        public bool DeleteMedia(int textToID)
        {
            return true;
        }

        [HttpGet]
        public List<TextToInfo> GetTextTo()
        {
            string userID =  Request.Headers.GetValues("UserID").FirstOrDefault();
            return new clsTextTo().GetTextToDetail(userID);
        }

        [HttpPut]
        public bool SaveTextTo([FromBody] TextToInfo textToInfo)
        {
            textToInfo.userid = Request.Headers.GetValues("UserID").FirstOrDefault();
            return new clsTextTo().SaveTextTo(textToInfo);
        }
    }
}
