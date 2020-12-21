using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using NCCSAN.Source.Entity;

namespace NCCSAN.Source.Logic
{
    public class Utils
    {
        public static void convertImageFileToJpeg(Archivo archivo)
        {
            if (archivo == null)
            {
                return;
            }

            if (archivo.getNombre().ToUpper().EndsWith("JPG"))
            {
                archivo.setNombre(archivo.getNombre().Substring(0, archivo.getNombre().Length - 3) + "jpeg");
                archivo.setContenido(Utils.pngToJpeg(archivo.getContenido()));
            }
            else if (archivo.getNombre().ToUpper().EndsWith("PNG"))
            {
                archivo.setNombre(archivo.getNombre().Substring(0, archivo.getNombre().Length - 3) + "jpeg");
                archivo.setContenido(Utils.pngToJpeg(archivo.getContenido()));
            }
            else if (archivo.getNombre().ToUpper().EndsWith("BMP"))
            {
                archivo.setNombre(archivo.getNombre().Substring(0, archivo.getNombre().Length - 3) + "jpeg");
                archivo.setContenido(Utils.pngToJpeg(archivo.getContenido()));
            }
        }


        private static byte[] pngToJpeg(byte[] bytesPNG)
        {
            MemoryStream msPNG = new MemoryStream(bytesPNG);
            Image img = Image.FromStream(msPNG);
            MemoryStream msJPEG = new MemoryStream();
            img.Save(msJPEG, System.Drawing.Imaging.ImageFormat.Jpeg);
            return msJPEG.ToArray();
        }


        public static double getPercentage(double value, double total)
        {
            if((value < 0) || (total < 0))
            {
                return -1;
            }

            if(value > total)
            {
                return -1;
            }

            if((value == 0) || (total == 0))
            {
                return 0;
            }

            return (value * 100) / total;
        }


        public static string validateFilename(string filename)
        {
            if (filename == null)
            {
                return "El nombre de archivo es inválido";
            }

            if (filename.Length > 90)
            {
                return "El nombre de archivo es demasiado largo. La longitud máxima permitida son 85 caracteres";
            }

            return null;

        }


        public static bool validateRUTPersona(string rut)
        {
            string pattern = @"([0-9]+)(\-[0-9|K|k])?";
            if (Utils.isFullMatched(rut, pattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool validateEmail(string email)
        {
            string pattern = @"([A-z]+[\.]?[A-z]+)+(@)([A-z]+[\.]?[A-z]+)+[\.][A-z]+";
            if (Utils.isFullMatched(email, pattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static string getAppFullURL(HttpRequest request, string home_path)
        {
            return "http://" + request.Url.Authority.ToString() + home_path;
        }


        public static int getEventoType(string fuente, double irc)
        {
            if (fuente.ToUpper().Equals("RECLAMO DE CLIENTE"))
            {
                //No conformidad
                return 1;
            }

            if (irc < 10)
            {
                //Hallazgo
                return 0;
            }
            else
            {
                //No conformidad
                return 1;
            }
        }


        public static bool isFullMatched(string text, string pattern)
        {
            if ((text == null) || (pattern == null))
            {
                return false;
            }

            if ((Regex.IsMatch(text, pattern)) && (Regex.Match(text, pattern).Length == text.Length))
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public static bool validateFecha(string fecha)
        {
            if (fecha.Length < 10)
                return false;

            DateTime dt;

            if (DateTime.TryParse(fecha, out dt))
                return true;
            else
                return false;
        }


        public static bool validateNumber(string number)
        {
            if (number == null)
                return false;

            int i;

            if (Int32.TryParse(number, out i))
                return true;
            else
                return false;
        }



        public static string getUniqueCode()
        {
            Guid guid = Guid.NewGuid();
            String code = guid.ToString();

            //return code.Substring(24, 12);
            return code;
        }


        public static string getMD5Hash(string input)
        {
            if (input == null)
            {
                return null;
            }

            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(input));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }


        public static string getSHA1Hash(string input)
        {
            if (input == null)
            {
                return null;
            }

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(input));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }


        public static string getFileExtension(string filename)
        {
            if (filename == null)
                return null;
            else
                return System.IO.Path.GetExtension(filename);
        }


        public static String getFileTypeGroup(String extension)
        {
            extension = extension.ToLower();

            //Image
            if (extension.Equals(".bmp"))
                return "Imagen";
            if (extension.Equals(".gif"))
                return "Imagen";
            if (extension.Equals(".jpeg"))
                return "Imagen";
            if (extension.Equals(".jpg"))
                return "Imagen";
            if (extension.Equals(".png"))
                return "Imagen";
            if (extension.Equals(".tif"))
                return "Imagen";
            if (extension.Equals(".tiff"))
                return "Imagen";
            //Documents
            if (extension.Equals(".msg"))
                return "Documento";
            if (extension.Equals(".doc"))
                return "Documento";
            if (extension.Equals(".docx"))
                return "Documento";
            if (extension.Equals(".pdf"))
                return "Documento";
            //Slideshows
            if (extension.Equals(".ppt"))
                return "Diapositivas";
            if (extension.Equals(".pptx"))
                return "Diapositivas";
            //Data
            if (extension.Equals(".xlsx"))
                return "Documento";
            if (extension.Equals(".xlsm"))
                return "Documento";
            if (extension.Equals(".xls"))
                return "Documento";
            if (extension.Equals(".csv"))
                return "Documento";
            if (extension.Equals(".xml"))
                return "Documento";
            if (extension.Equals(".txt"))
                return "Documento";
            //Compressed Folders
            if (extension.Equals(".zip"))
                return "Comprimido";
            if (extension.Equals(".rar"))
                return "Comprimido";
            //Audio
            if (extension.Equals(".ogg"))
                return "Audio";
            if (extension.Equals(".mp3"))
                return "Audio";
            if (extension.Equals(".wma"))
                return "Audio";
            if (extension.Equals(".wav"))
                return "Audio";
            //Video
            if (extension.Equals(".wmv"))
                return "Video";
            if (extension.Equals(".swf"))
                return "Video";
            if (extension.Equals(".avi"))
                return "Video";
            if (extension.Equals(".mp4"))
                return "Video";
            if (extension.Equals(".mpeg"))
                return "Video";
            if (extension.Equals(".mpg"))
                return "Video";
            if (extension.Equals(".qt"))
                return "Video";

            return null;
        }


        public static String getContentType(String extension)
        {
            extension = extension.ToLower();

            //Image
            if (extension.Equals(".bmp"))
                return "image/bmp";
            if (extension.Equals(".gif"))
                return "image/gif";
            if (extension.Equals(".jpeg"))
                return "image/jpeg";
            if (extension.Equals(".jpg"))
                return "image/jpeg";
            if (extension.Equals(".png"))
                return "image/png";
            if (extension.Equals(".tif"))
                return "image/tiff";
            if (extension.Equals(".tiff"))
                return "image/tiff";
            //Documents
            if (extension.Equals(".msg"))
                return "application/vnd.ms-outlook";
            if (extension.Equals(".doc"))
                return "application/msword";
            if (extension.Equals(".docx"))
                return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            if (extension.Equals(".pdf"))
                return "application/pdf";
            //Slideshows
            if (extension.Equals(".ppt"))
                return "application/vnd.ms-powerpoint";
            if (extension.Equals(".pptx"))
                return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            //Data
            if (extension.Equals(".xlsx"))
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            if (extension.Equals(".xlsm"))
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            if (extension.Equals(".xls"))
                return "application/vnd.ms-excel";
            if (extension.Equals(".csv"))
                return "text/csv";
            if (extension.Equals(".xml"))
                return "text/xml";
            if (extension.Equals(".txt"))
                return "text/plain";
            //Compressed Folders
            if (extension.Equals(".zip"))
                return "application/zip";
            if (extension.Equals(".rar"))
                return "application/x-rar-compressed";
            //Audio
            if (extension.Equals(".ogg"))
                return "application/ogg";
            if (extension.Equals(".mp3"))
                return "audio/mpeg";
            if (extension.Equals(".wma"))
                return "audio/x-ms-wma";
            if (extension.Equals(".wav"))
                return "audio/x-wav";
            //Video
            if (extension.Equals(".wmv"))
                return "audio/x-ms-wmv";
            if (extension.Equals(".swf"))
                return "application/x-shockwave-flash";
            if (extension.Equals(".avi"))
                return "video/avi";
            if (extension.Equals(".mp4"))
                return "video/mp4";
            if (extension.Equals(".mpeg"))
                return "video/mpeg";
            if (extension.Equals(".mpg"))
                return "video/mpeg";
            if (extension.Equals(".qt"))
                return "video/quicktime";

            return null;
        }


        public static String getFileType(String extension)
        {
            extension = extension.ToLower();

            //Image
            if (extension.Equals(".bmp"))
                return "Imagen BMP";
            if (extension.Equals(".gif"))
                return "Imagen animada";
            if (extension.Equals(".jpeg"))
                return "Imagen JPEG";
            if (extension.Equals(".jpg"))
                return "Imagen JPG";
            if (extension.Equals(".png"))
                return "Imagen PNG";
            if (extension.Equals(".tif"))
                return "Imagen TIFF";
            if (extension.Equals(".tiff"))
                return "Imagen TIFF";
            //Documents
            if (extension.Equals(".msg"))
                return "Correo electrónico de Outlook";
            if (extension.Equals(".doc"))
                return "Documento de Microsoft Word";
            if (extension.Equals(".docx"))
                return "Documento de Microsoft Word";
            if (extension.Equals(".pdf"))
                return "Documento PDF";
            //Slideshows
            if (extension.Equals(".ppt"))
                return "Presentación de Miscrosoft Power Point";
            if (extension.Equals(".pptx"))
                return "Presentación de Miscrosoft Power Point";
            //Data
            if (extension.Equals(".xlsx"))
                return "Hoja de cálculo de Microsoft Excel";
            if (extension.Equals(".xlsm"))
                return "Hoja de cálculo de Microsoft Excel con macros";
            if (extension.Equals(".xls"))
                return "Hoja de cálculo de Microsoft Excel";
            if (extension.Equals(".csv"))
                return "Documento de Texto separado por comas";
            if (extension.Equals(".xml"))
                return "Documento de Texto estructurado";
            if (extension.Equals(".txt"))
                return "Documento de Texto plano";
            //Compressed Folders
            if (extension.Equals(".zip"))
                return "Carpeta comprimidos ZIP";
            if (extension.Equals(".rar"))
                return "Carpeta comprimida RAR";
            //Audio
            if (extension.Equals(".ogg"))
                return "Audio OGG";
            if (extension.Equals(".mp3"))
                return "Audio MP3";
            if (extension.Equals(".wma"))
                return "Audio WMA";
            if (extension.Equals(".wav"))
                return "Audio WAV";
            //Video
            if (extension.Equals(".wmv"))
                return "Video WMV";
            if (extension.Equals(".swf"))
                return "Animación Flash";
            if (extension.Equals(".avi"))
                return "Video AVI";
            if (extension.Equals(".mp4"))
                return "Video MP4";
            if (extension.Equals(".mpeg"))
                return "Video MPEG";
            if (extension.Equals(".mpg"))
                return "Video MPG";
            if (extension.Equals(".qt"))
                return "Video de QuickTime";

            return null;
        }
    }
}