using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Web.Script.Serialization;

namespace WCAPP.Utils
{
    public class QrCoder
    {
        /*
        static object ZBarDecoder = null;
        static MethodInfo ZBarDecode = null;

       
        static QrCoder()
        {
            string zbarDir;
            string opencvDir;
            if (Environment.Is64BitProcess)
            {
                zbarDir = AppDomain.CurrentDomain.BaseDirectory + "3rdparty/zbar/x64";
                opencvDir = AppDomain.CurrentDomain.BaseDirectory + "3rdparty/opencv/x64/vc15/bin";
            }
            else
            {
                zbarDir = AppDomain.CurrentDomain.BaseDirectory + "3rdparty/zbar/x86";
                opencvDir = AppDomain.CurrentDomain.BaseDirectory + "3rdparty/opencv/x86/vc15/bin";
            }

            var path = Environment.GetEnvironmentVariable("Path");
            Environment.SetEnvironmentVariable("Path", zbarDir + ";" + opencvDir + ";" + path);

            Assembly zbar = null;
            Type ZBarDecoderType = null;
            Type[] inputType = new Type[] { typeof(Stream) };

            zbar = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "bin/ZBar/ZBar.dll");
            ZBarDecoderType = zbar.GetType("ZBar.Decoder");
            ZBarDecoder = zbar.CreateInstance("ZBar.Decoder");
            ZBarDecode = ZBarDecoderType.GetMethod("Decode", inputType);
        }//*/

        public static byte[] EncodeWithCompress(string str)
        {
            return EncodeWithCompress(str.ToUtf8Bytes());
        }

        public static byte[] EncodeWithCompress(byte[] bytes)
        {
            var base64Str = "";
            using (var mscode = new MemoryStream())
            {
                using (var zipStream = new GZipStream(mscode, CompressionMode.Compress))
                    zipStream.Write(bytes, 0, bytes.Length);

                base64Str = mscode.ToArray().Base64();
            }

            return Encode(base64Str);
        }

        public static byte[] Encode(string str)
        {
            var qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            var qrCode = qrEncoder.Encode(str);

            var render = new GraphicsRenderer(new FixedModuleSize(3, QuietZoneModules.Zero));
            var ms = new MemoryStream();
            //render.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);


            DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
            Graphics g = Graphics.FromImage(map);
            render.Draw(g, qrCode.Matrix);

            int logosize = 20;
            using (Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + @"Images\innerWhite-logo.png"))
            {
                Point imgPoint = new Point((map.Width - logosize) / 2, (map.Height - logosize) / 2);
                g.DrawImage(img, imgPoint.X, imgPoint.Y, logosize, logosize);
                map.Save(ms, ImageFormat.Png);

            }

            return ms.GetBuffer();
        }

        public static byte[] Encode(object o)
        {
            var json = new JavaScriptSerializer().Serialize(o);
            return EncodeWithCompress(json);
        }

        /*
        public static string Decode(Stream inputStream)
        {
            return ZBarDecode.Invoke(ZBarDecoder, new[] { inputStream }) as string;
        }//*/
    }
}