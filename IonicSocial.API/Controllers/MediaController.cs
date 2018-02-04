using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using SocialApp.Data;
using System.Net.Http.Formatting;
using System.Runtime.InteropServices;

namespace SocialApp.API
{
    [Authorize]
    [RoutePrefix("api/media")]
    public class MediaController : ApiController
    {
        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;

        [HttpDelete]
        public bool DeleteMedia(int mediaid)
        {
            var userID = Request.Headers.GetValues("UserID").FirstOrDefault();
            return new clsMedia().DeleteMedia(userID, mediaid);
        }

        [HttpPut]
        public bool SaveMediaDetails([FromBody] MediaInfo mediaInfo)
        {
            var userID = Request.Headers.GetValues("UserID").FirstOrDefault();
            return new clsMedia().SaveMediaDetails(userID, mediaInfo);
        }

        [HttpPut]
        [Route("UpdateLocation")]
        public int UpdateLocation(long mediaid, [FromBody] MediaLocationInfo mediaLocationInfo)
        {
            return new clsMediaLocation().UpdateLocation(mediaid, mediaLocationInfo);
        }

        [HttpGet]
        public List<MediaInfo> GetMedia(int mediaType)
        {
            var userID = Request.Headers.GetValues("UserID").FirstOrDefault();
            return new clsMedia().GetMedia(userID,mediaType);
        }

        [HttpGet]
        [Route("MediaDetails")]
        public MediaInfo GetMediaDetail(int mediaID)
        {
            var userID = Request.Headers.GetValues("UserID").FirstOrDefault();
            return new clsMedia().GetMediaDetail(userID, mediaID);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddMedia()
        {
            MediaInfo returnMediaInfo=null;
            long mediaid = 0;

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = System.Web.HttpContext.Current.Server.MapPath("~/Media");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                string userID =provider.FormData.Get("userID");
                string uploadedFrom=provider.FormData.Get("uploadedFrom");
                int mediaType = Convert.ToInt32(provider.FormData.Get("mediaType"));
                string latitude= provider.FormData.Get("latitude");
                string longitude= provider.FormData.Get("longitude");

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    string newFileName = "";
                    string thumb = "";

                    if (mediaType==1 || mediaType==4)   //photo and geo
                    { 
                        newFileName =  userID.ToString()+"_"+Guid.NewGuid()+ ".jpg";
                    }
                    if(mediaType==2)    // audio
                    {
                        newFileName = userID.ToString() + "_" + Guid.NewGuid() + ".3gpp";
                    }
                    if (mediaType == 3)    // video
                    {
                        newFileName = userID.ToString() + "_" + Guid.NewGuid() + ".mp4";
                        
                        thumb= userID.ToString() + "_" + Guid.NewGuid() + ".jpg";
 
                    }
                    if (File.Exists(file.LocalFileName))
                    {
                        File.Copy(file.LocalFileName, root+"\\"+newFileName, true);
                        File.Delete(file.LocalFileName);
                    }

                    if(mediaType==3)
                    {
                        while (!IsFileLocked(root + "\\" + newFileName))
                        {
                            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                            ffMpeg.GetVideoThumbnail(root + "\\" + newFileName, root + "\\thumb\\" + thumb, 1);
                            break;
                        }
                    }

                    MediaInfo mediaInfo = new MediaInfo();
                    mediaInfo.userid = userID;
                    mediaInfo.mediapath = newFileName;
                    mediaInfo.thumbpath = thumb;
                    mediaInfo.mediatype = mediaType;
                    mediaInfo.latitude = latitude;
                    mediaInfo.longitude = longitude;
                    mediaInfo.title = "";
                    mediaInfo.description = "";

                    clsMedia media = new clsMedia();

                    mediaid=media.AddMedia(mediaInfo);

                    if(mediaid>0)
                    {
                        //add media location 

                        if (mediaType == 4)
                        {
                            MediaLocationInfo mediaLocationInfo = new MediaLocationInfo();
                            mediaLocationInfo.latitude = latitude;
                            mediaLocationInfo.longitude = longitude;
                            mediaLocationInfo.locationmediaid = mediaid;

                            clsMediaLocation mediaLocation = new clsMediaLocation();
                            mediaLocation.AddMediaLocation(mediaLocationInfo);
                        }

                        mediaInfo.mediaid = mediaid;
                        returnMediaInfo = mediaInfo;
                    }
                }

                return Request.CreateResponse<MediaInfo>(HttpStatusCode.OK, returnMediaInfo);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        private bool IsFileLocked(string file)
        {
            //check that problem is not in destination file
            if (File.Exists(file) == true)
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex2)
                {
                    //_log.WriteLog(ex2, "Error in checking whether file is locked " + file);
                    int errorCode = Marshal.GetHRForException(ex2) & ((1 << 16) - 1);
                    if ((ex2 is IOException) && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }

        //public bool CreateImage(string outfile, string imagefile, string iname)
        //{
        //    BL_ImageResize Objresize = new BL_ImageResize();
        //    Process procVideoConvert = new Process();
        //    try
        //    {

        //        //set working directory (give directory in which execution permission is given)
        //        procVideoConvert.StartInfo.WorkingDirectory = System.Web.HttpContext.Current.Server.MapPath("~/Bin");
        //        //set process to execute in shell (for console programs)
        //        procVideoConvert.StartInfo.UseShellExecute = true;
        //        //Hide process window (hide console here)
        //        procVideoConvert.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //        //suppress error dialogs
        //        procVideoConvert.StartInfo.ErrorDialog = false;

        //        //set process executable file/batch file
        //        procVideoConvert.StartInfo.FileName = "ffmpeg.exe";
        //        //Set arguments to be passed to process
        //        procVideoConvert.StartInfo.Arguments = " -i " + outfile + " -ss 0:0:2.0  -vframes 1  -vcodec png  -s 400x300 -y -f image2 " + imagefile;

        //        //start conversion process
        //        procVideoConvert.Start();

        //        //lblInfo.Text += DateTime.Now.ToString("hh:mm:ss") + " : started conversion <br/>";

        //        //wait for conversion process to exit
        //        procVideoConvert.WaitForExit();

        //        //Image resize

        //        System.IO.FileInfo objFileInfo = new System.IO.FileInfo(imagefile);

        //        string LogoPath = System.Web.HttpContext.Current.Server.MapPath("~/Media/Thumb/") + iname;
        //        string EnlargePath = System.Web.HttpContext.Current.Server.MapPath("~/MerchantVideos/VideoImage/Enlarge/") + iname;
        //        int LogoWidth = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Drawing_thumbWidth"]);
        //        int LogoHeight = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Drawing_thumbHeight"]);
        //        int EnlargeWidth = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Drawing_EnlargeWidth"]);
        //        int EnlargeHeight = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["Drawing_EnlargeHeight"]);

        //        ////Save Enlarged image
        //        //System.Drawing.Bitmap gridthumbnail = Objresize.CreateThumbNail(new System.Drawing.Bitmap(objFileInfo.OpenRead(), false), EnlargeWidth, EnlargeHeight);
        //        //System.Drawing.Image tbig = new System.Drawing.Bitmap(gridthumbnail.Width, gridthumbnail.Height);
        //        //System.Drawing.Graphics g1 = System.Drawing.Graphics.FromImage(tbig);
        //        //g1.DrawImage(gridthumbnail, 0, 0);
        //        //gridthumbnail.Dispose();
        //        //g1.Dispose();
        //        //tbig.Save(EnlargePath, System.Drawing.Imaging.ImageFormat.Jpeg);

        //        //Save Logo Image     
        //        System.Drawing.Bitmap gridthumbnail_2 = Objresize.CreateThumbNail(new System.Drawing.Bitmap(objFileInfo.OpenRead(), false), LogoWidth, LogoHeight);
        //        tbig = new System.Drawing.Bitmap(gridthumbnail_2.Width, gridthumbnail_2.Height);
        //        g1 = System.Drawing.Graphics.FromImage(tbig);
        //        g1.DrawImage(gridthumbnail_2, 0, 0);
        //        gridthumbnail_2.Dispose();
        //        g1.Dispose();
        //        tbig.Save(LogoPath, System.Drawing.Imaging.ImageFormat.Jpeg);

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

    }
}
