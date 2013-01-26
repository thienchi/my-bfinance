using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;

using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm.cmm
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class FPUploadHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private const string extension_img = ".gif.jpg.jpeg.png";
        
        public void ProcessRequest(HttpContext context)
        {
            string vlreturn = "", error = "";
            BasePage basepage = new BasePage();
            if (context.Request.Files.Count == 0)
            {
                vlreturn = "";
                error = CCommon.Get_Definephrase(Definephrase.Invalid_file);
            }
            else
            {
                HttpPostedFile fileUpload = context.Request.Files[0];
                string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                if (extension_img.IndexOf(fileExtension) == -1)
                {
                    vlreturn = "";
                    error = CCommon.Get_Definephrase(Definephrase.Invalid_filetype_image);
                }
                else
                {
                    string uploadPath = context.Request.QueryString["up"];
                    uploadPath = CFunctions.IsNullOrEmpty(uploadPath) ? "../commup/upload/" : CFunctions.MBDecrypt(uploadPath);
                    string uploadPathDir = context.Server.MapPath(uploadPath);

                    DirectoryInfo pathInfo = new DirectoryInfo(uploadPathDir);
                    string fileName;
                    if (pathInfo.GetFiles(fileUpload.FileName).Length == 0)
                    {
                        fileName = Path.GetFileName(fileUpload.FileName);
                    }
                    else
                    {
                        fileName = this.Get_Filename() + fileExtension;
                        error = CCommon.Get_Definephrase(Definephrase.Notice_fileupload_duplicate);
                    }

                    if (CConstants.FILEUPLOAD_THUMBNAIL)
                    {
                        string uploadLocation_thumb = uploadPathDir + "\\thumb_" + fileName;
                        this.CreateThumbnail(fileUpload, uploadLocation_thumb, true);
                    }

                    string uploadLocation = uploadPathDir + "\\" + fileName;
                    fileUpload.SaveAs(uploadLocation);
                    vlreturn = uploadPath.Replace("../", "") + fileName;
                    error += CCommon.Get_Definephrase(Definephrase.Notice_fileupload_done);
                }
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write("{\"name\":\"" + vlreturn + "\", \"error\":\"" + error + "\"}");
        }

        public void CreateThumbnail(HttpPostedFile fileUpload, string NewFile, bool OnlyResizeIfWider)
        {
            string[] thumbnailsize = System.Configuration.ConfigurationSettings.AppSettings["FILEUPLOAD_THUMBNAILSIZE"].Split(':');
            int NewWidth = Convert.ToInt32(thumbnailsize[0]);
            int MaxHeight = Convert.ToInt32(thumbnailsize[1]);

            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromStream(fileUpload.InputStream);
            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (OnlyResizeIfWider)
            {
                if (FullsizeImage.Width <= NewWidth)
                {
                    NewWidth = FullsizeImage.Width;
                }
            }

            int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                NewHeight = MaxHeight;
            }

            System.Drawing.Image oThumbNail = new System.Drawing.Bitmap(NewWidth, NewHeight, FullsizeImage.PixelFormat);
            System.Drawing.Graphics oGraphic = System.Drawing.Graphics.FromImage(oThumbNail);
            oGraphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            oGraphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            oGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            System.Drawing.Rectangle oRectangle = new System.Drawing.Rectangle(0, 0, NewWidth, NewHeight);
            oGraphic.DrawImage(FullsizeImage, oRectangle);
            oThumbNail.Save(NewFile, this.Get_ImageFormat(fileUpload.FileName));

            FullsizeImage.Dispose();
        }
        private System.Drawing.Imaging.ImageFormat Get_ImageFormat(string fileName)
        {
            System.Drawing.Imaging.ImageFormat imageformat = System.Drawing.Imaging.ImageFormat.Jpeg;
            string extension = fileName.Substring(fileName.LastIndexOf("."));
            switch (extension)
            {
                case "gif":
                    imageformat = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case "png":
                    imageformat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                case "jpg":
                case "jpeg":
                    imageformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
            }
            return imageformat;
        }
        private string Get_Filename()
        {
            return DateTime.Now.Ticks.ToString();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
