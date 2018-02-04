using SocialApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialApp.Data.Model;

namespace SocialApp.Data
{
    public class clsMedia
    {
        public long AddMedia(MediaInfo mediaInfo)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                UserMedia userMedia = new UserMedia();
                userMedia.UserID = mediaInfo.userid;
                userMedia.Title = mediaInfo.title;
                userMedia.Description = mediaInfo.description;
                userMedia.MediaPath = mediaInfo.mediapath;
                userMedia.ThumbPath = mediaInfo.thumbpath;
                userMedia.MediaType = mediaInfo.mediatype;
                userMedia.MediaType = mediaInfo.mediatype;
                userMedia.InsertedBy = mediaInfo.userid;
                userMedia.InsertedDate = DateTime.Now;

                context.UserMedias.Add(userMedia);
                context.SaveChanges();

                return  userMedia.MediaID;
            }
        }

        public List<MediaInfo> GetMedia(string userID, int mediaType)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var mediaList = context.UserMedias.Where(x=>x.UserID==userID && x.MediaType==mediaType).ToList();

                List<MediaInfo> mediaInfoList = new List<MediaInfo>();

                foreach (var item in mediaList)
                {
                    MediaInfo mediaInfo = new MediaInfo();
                    mediaInfo.mediaid = item.MediaID;
                    mediaInfo.mediatype = item.MediaType;
                    mediaInfo.title = item.Title;
                    mediaInfo.description = item.Description;
                    mediaInfo.mediapath = item.MediaPath;
                    mediaInfo.thumbpath = item.ThumbPath;
                    mediaInfo.userid = item.UserID;
                    mediaInfoList.Add(mediaInfo);
                }
                return mediaInfoList;
            }
        }

        public MediaInfo GetMediaDetail(string userID, int mediaID)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var mediaDetails = context.UserMedias.Where(x=>x.UserID==userID && x.MediaID==mediaID).Single();

                MediaInfo mediaInfo = new MediaInfo();
                mediaInfo.mediaid = mediaDetails.MediaID;
                mediaInfo.mediatype = mediaDetails.MediaType;
                mediaInfo.mediapath = mediaDetails.MediaPath;
                mediaInfo.title = mediaDetails.Title;
                mediaInfo.description = mediaDetails.Description;
                mediaInfo.userid = mediaDetails.UserID;

                if (mediaDetails.MediaType == 4)
                {
                    clsMediaLocation mediaLocation = new clsMediaLocation();
                    var medialoc=mediaLocation.GetMediaLocation(mediaDetails.MediaID);
                    if(medialoc!=null)
                    { 
                    mediaInfo.address1 = medialoc.Address1;
                    mediaInfo.address1 = medialoc.Address2;
                    mediaInfo.city = medialoc.City;
                    mediaInfo.zip = medialoc.ZIP;
                    mediaInfo.latitude = medialoc.Lat;
                    mediaInfo.longitude = medialoc.Long;
                    }
                }
                return mediaInfo;
            }
        }

        public bool DeleteMedia(string userID, int mediaID)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var mediaDetails = context.UserMedias.Where(x => x.UserID == userID && x.MediaID == mediaID).Single();

                context.UserMedias.Remove(mediaDetails);

                return Convert.ToBoolean( context.SaveChanges());
            }
        }

        public bool SaveMediaDetails(string userID, MediaInfo mediaInfo)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var mediaDetails = context.UserMedias.Where(x => x.UserID == userID && x.MediaID == mediaInfo.mediaid).Single();
                
                mediaDetails.Title = mediaInfo.title;
                mediaDetails.Description = mediaInfo.description;

                return Convert.ToBoolean( context.SaveChanges() );
            }
        }
    }
    
    public class MediaInfo: MediaLocationInfo
    {
        public long mediaid { get; set; }
        public string userid { get; set; }
        public string title { get; set; }
        public int mediatype { get; set; }
        public string media { get; set; }
        public string mediapath { get; set; }
        public string thumbpath { get; set; }
        public string description { get; set; }
       // public string latitude { get; set; }
      //  public string longitude { get; set; }
    }

}