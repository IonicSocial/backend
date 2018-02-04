using SocialApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialApp.Data.Model;

namespace SocialApp.Data
{
    public class clsTextTo
    {
        public bool AddTextTo(TextToInfo textToInfo)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                TextTo textTo = new TextTo();
                textTo.UserID = textToInfo.userid;
                textTo.TextTo1 = textToInfo.textto;
                textTo.InsertedDate = DateTime.Now;
                textTo.InsertedBy = textToInfo.userid;

                context.TextToes.Add(textTo);
                context.SaveChanges();

                return (textTo.TextToID > 0);
            }
        }

        public List<TextTo> List()
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                return context.TextToes.ToList();
            }
        }
        //public List<MediaInfo> GetTextToList(int userID)
        //{
        //    using (SocialAppEntities context = new SocialAppEntities())
        //    {
        //        var mediaList = context.UserMedias.Where(x=>x.UserID==userID && x.MediaType==mediaType).ToList();

        //        List<MediaInfo> mediaInfoList = new List<MediaInfo>();

        //        foreach (var item in mediaList)
        //        {
        //            MediaInfo mediaInfo = new MediaInfo();
        //            mediaInfo.MediaID = item.MediaID;
        //            mediaInfo.MediaType = item.MediaType;
        //            mediaInfo.Title = item.Title;
        //            mediaInfo.Description = item.Description;
        //            mediaInfo.UserID = item.UserID;
        //            mediaInfoList.Add(mediaInfo);
        //        }
        //        return mediaInfoList;
        //    }
        //}

        public List<TextToInfo> GetTextToDetail(string userID)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var textToList = context.TextToes.Where(x=>x.UserID==userID).ToList();
                var texttoInfoList = new List<TextToInfo>();

                foreach (var item in textToList)
                {
                    TextToInfo textToInfo = new TextToInfo();
                    textToInfo.texttoid = item.TextToID;
                    textToInfo.textto = item.TextTo1;
                    textToInfo.userid = item.UserID;

                    texttoInfoList.Add (textToInfo);
                }
                return texttoInfoList;
            }
        }

        public bool SaveTextTo(TextToInfo textToInfo)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var textToDetail = context.TextToes.Where(x => x.UserID == textToInfo.userid).SingleOrDefault();
                if(textToDetail==null)
                {
                    AddTextTo(textToInfo);
                    return true;
                }
                textToDetail.TextTo1 = textToInfo.textto;
                context.SaveChanges();

                if (textToDetail.TextToID > 0)
                    return true;
            }
            return false;
        }
    }

    public class TextToInfo
    {
        public long texttoid { get; set; }
        public string userid { get; set; }
        public string textto { get; set; }
    }

}