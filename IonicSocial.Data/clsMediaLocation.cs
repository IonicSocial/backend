using SocialApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialApp.Data.Model;

namespace SocialApp.Data
{
    public class clsMediaLocation
    {
        public long AddMediaLocation(MediaLocationInfo mediaLocationInfo)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                MediaLocation mediaLocation = new MediaLocation();
                mediaLocation.MediaID = mediaLocationInfo.locationmediaid;
                mediaLocation.Address1 = mediaLocationInfo.address1;
                mediaLocation.Address2 = mediaLocationInfo.address2;
                mediaLocation.Lat = mediaLocationInfo.latitude;
                mediaLocation.Long = mediaLocationInfo.longitude;
                mediaLocation.ZIP = mediaLocationInfo.zip;

                context.MediaLocations.Add(mediaLocation);
                context.SaveChanges();

                return mediaLocation.MediaLocationID;
            }
        }

        public int UpdateLatLong(long mediaID,string latitude,string longitude)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var mediaLocation = context.MediaLocations.Where(x => x.MediaID == mediaID).Single();

                mediaLocation.Lat = latitude;
                mediaLocation.Long = longitude;

                return context.SaveChanges();
                
            }
        }

        public int UpdateLocation(long mediaID, MediaLocationInfo mediaLocationInfo)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                var mediaLocation = context.MediaLocations.Where(x => x.MediaID == mediaID).Single();

                mediaLocation.Lat = mediaLocationInfo.latitude;
                mediaLocation.Long = mediaLocationInfo.longitude;
                mediaLocation.Address1 = mediaLocationInfo.address1;
                mediaLocation.Address2 = mediaLocationInfo.address2;
                mediaLocation.City= mediaLocationInfo.city;
                mediaLocation.ZIP = mediaLocationInfo.zip;
                mediaLocation.Address1 = mediaLocationInfo.address1;

                return context.SaveChanges();

            }
        }

        public MediaLocation GetMediaLocation(long mediaID)
        {
            using (SocialAppEntities context = new SocialAppEntities())
            {
                return context.MediaLocations.Where(x=>x.MediaID== mediaID).SingleOrDefault();
            }
        }
    }

    public class MediaLocationInfo
    {
        public long medialocationinfoId { get; set; }
        public long locationmediaid { get; set; }
        public string zip { get; set; }
        public string address2 { get; set; }
        public string address1 { get; set; }
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

}