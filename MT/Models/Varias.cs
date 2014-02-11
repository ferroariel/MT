using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;

namespace MT.Helpers
{
    public static class FoundationDropDown
    {
        public static MvcHtmlString AFDropDown(this HtmlHelper html, IEnumerable<SelectListItem> Paises, string eID /*object htmlAttributes*/)
        {
            if (Paises != null)
            {
                var tb = new TagBuilder("select");
                tb.GenerateId(eID);
                tb.Attributes.Add("style", "display:none;");
                var tb2 = new TagBuilder("div");
                tb2.Attributes.Add("class", "custom dropdown");
                if (Paises.Count() > 0)
                {
                    tb2.InnerHtml = String.Format("<a href=\"#\" class=\"current\">{0}</a><a href=\"#\" class=\"selector\"></a>", Paises.First().Text);
                    tb2.InnerHtml += "<ul>";

                    //tb.MergeAttribute("style","display:none;");
                    //tb.MergeAttributes(new RouteValueDictionary(htmlAttributes));
                    foreach (SelectListItem p in Paises)
                    {
                        var o = new TagBuilder("option");
                        var o2 = new TagBuilder("li");
                        o.Attributes.Add("value", p.Value.ToString());
                        o.SetInnerText(p.Text);
                        tb.InnerHtml += o.ToString();
                        o2.SetInnerText(p.Text);
                        tb2.InnerHtml += o2.ToString();
                    }
                    tb2.InnerHtml += "</ul>";
                }
                /*
                <!-- Custom Selects -->
                  <label for="customDropdown">Dropdown Label</label>
                  <select style="display:none;" id="customDropdown">
                    <option SELECTED>This is a dropdown</option>
                    <option>This is another option</option>
                    <option>Look, a third option</option>
                  </select>
                  <div class="custom dropdown">
                    <a href="#" class="current">
                      This is a dropdown
                    </a>
                    <a href="#" class="selector"></a>
                    <ul>
                      <li>This is a dropdown</li>
                      <li>This is another option</li>
                      <li>Look, a third option</li>
                    </ul>
                  </div>
                */

                //tb.InnerHtml += tb2.ToString();
                return MvcHtmlString.Create(String.Format("{0}{1}", tb, tb2));
            }
            else
                return MvcHtmlString.Create("--");
        }

        public static MvcHtmlString RegisterScriptTimeAgo(this HtmlHelper html)
        {
            var script = String.Format(@"<script src=""{0}"" type=""text/javascript""></script>
                                    <script type=""text/javascript"">
                                        jQuery(document).ready(function() {{
                                            jQuery(""abbr.timeago"").timeago();
                                        }});
                                    </script>",
                            UrlHelper.GenerateContentUrl("~/Scripts/jquery.timeago.js", html.ViewContext.HttpContext));
            return MvcHtmlString.Create(script);
        }
    }
}

namespace MT.Models
{
    public static class StringMethodExtensions
    {
        private static string _link = "<a href=\"{0}\">{1}</a>";
        private static string _linkNoFollow = "<a href=\"{0}\" rel=\"nofollow\">{1}</a>";
        private static string _paraBreak = "\r\n\r\n";
        private static int Prime = 1580030173;
        private static int PrimeInverse = 59260789;

        #region encodingsencillo - http://stackoverflow.com/questions/8755713/user-id-obfuscation

        public static int DecodeId(int input)
        {
            return (input * PrimeInverse) & int.MaxValue;
        }

        public static int EncodeId(int input)
        {
            return (input * Prime) & int.MaxValue;
        }

        #endregion encodingsencillo - http://stackoverflow.com/questions/8755713/user-id-obfuscation

        /// <summary>
        /// Returns a copy of this string converted to HTML markup.
        /// </summary>
        public static string ConvertRelativePathsToAbsolute(String text, String absoluteUrl)
        {
            String value = Regex.Replace(text,
                "<(.*?)(src|href)=\"(?!http)(.*?)\"(.*?)>",
                "<$1$2=\"" + absoluteUrl + "$3\"$4>",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return value.Replace(absoluteUrl + "/", absoluteUrl);
        }

        public static string ToHtml(this string s)
        {
            return ToHtml(s, false);
        }

        /// <summary>
        /// Returns a copy of this string converted to HTML markup.
        /// </summary>
        /// <param name="nofollow">If true, links are given "nofollow"
        /// attribute</param>
        public static string ToHtml(this string s, bool nofollow)
        {
            StringBuilder sb = new StringBuilder();

            int pos = 0;
            while (pos < s.Length)
            {
                int start = pos;
                pos = s.IndexOf(_paraBreak, start);
                if (pos < 0)
                    pos = s.Length;
                string para = s.Substring(start, pos - start).Trim();

                if (para.Length > 0)
                    EncodeParagraph(para, sb, nofollow);

                pos += _paraBreak.Length;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Encodes [[URL]] and [[Text][URL]] links to HTML.
        /// </summary>
        /// <param name="text">Text to encode</param>
        /// <param name="sb">StringBuilder to write results</param>
        /// <param name="nofollow">If true, links are given "nofollow"
        /// attribute</param>
        private static void EncodeLinks(string s, StringBuilder sb, bool nofollow)
        {
            int pos = 0;
            while (pos < s.Length)
            {
                int start = pos;
                pos = s.IndexOf("[[", pos);
                if (pos < 0)
                    pos = s.Length;
                sb.Append(s.Substring(start, pos - start));
                if (pos < s.Length)
                {
                    string label, link;

                    start = pos + 2;
                    pos = s.IndexOf("]]", start);
                    if (pos < 0)
                        pos = s.Length;
                    label = s.Substring(start, pos - start);
                    int i = label.IndexOf("][");
                    if (i >= 0)
                    {
                        link = label.Substring(i + 2);
                        label = label.Substring(0, i);
                    }
                    else
                    {
                        link = label;
                    }

                    sb.Append(String.Format(nofollow ? _linkNoFollow : _link, link, label));

                    pos += 2;
                }
            }
        }

        /// <summary>
        /// Encodes a single paragraph to HTML.
        /// </summary>
        /// <param name="s">Text to encode</param>
        /// <param name="sb">StringBuilder to write results</param>
        /// <param name="nofollow">If true, links are given "nofollow"
        /// attribute</param>
        private static void EncodeParagraph(string s, StringBuilder sb, bool nofollow)
        {
            sb.AppendLine("<p>");
            s = HttpUtility.HtmlEncode(s);
            s = s.Replace(Environment.NewLine, "<br />\r\n");
            EncodeLinks(s, sb, nofollow);
            sb.AppendLine("\r\n</p>");
        }
    }

    public class ImageResult : ActionResult
    {
        static ImageResult()
        {
            CreateContentTypeMap();
        }

        public Image Image { get; set; }

        public ImageFormat ImageFormat { get; set; }

        private static Dictionary<ImageFormat, string> FormatMap { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (Image == null) throw new ArgumentNullException("Image");
            if (ImageFormat == null) throw new ArgumentNullException("ImageFormat");
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = FormatMap[ImageFormat];
            Image.Save(context.HttpContext.Response.OutputStream, ImageFormat);
        }

        private static void CreateContentTypeMap()
        {
            FormatMap = new Dictionary<ImageFormat, string>{
              { ImageFormat.Bmp,  "image/bmp"                },
              { ImageFormat.Gif,  "image/gif"                },
              { ImageFormat.Icon, "image/vnd.microsoft.icon" },
              { ImageFormat.Jpeg, "image/Jpeg"               },
              { ImageFormat.Png,  "image/png"                },
              { ImageFormat.Tiff, "image/tiff"               },
              { ImageFormat.Wmf,  "image/wmf"                }
            };
        }
    }

    public class LabelImage
    {
        private int F_Height;
        private int F_Width;
        private string MyURL;
        private int S_Height;
        private int S_Width;

        public LabelImage(SitioDeEmpresa sitiodeempres)
        {
            //int[] SizePreview = new int[] { 800,600,120,90 };
            this.WebSite = sitiodeempres.SITE_URL.ToString();
            this.ScreenWidth = 800; //ScreenWidth;
            this.ScreenHeight = 600; //ScreenHeight;
            this.ImageWidth = sitiodeempres.NRO_ANCHO; //ImageHeight;
            this.ImageHeight = sitiodeempres.NRO_ALTO; //ImageWidth;
        }

        public int ImageHeight
        {
            get { return F_Height; }
            set { F_Height = value; }
        }

        public int ImageWidth
        {
            get { return F_Width; }
            set { F_Width = value; }
        }

        public int ScreenHeight
        {
            get { return S_Height; }
            set { S_Height = value; }
        }

        public int ScreenWidth
        {
            get { return S_Width; }
            set { S_Width = value; }
        }

        public string WebSite
        {
            get { return MyURL; }
            set { MyURL = value; }
        }

        public Bitmap GetBitmap()
        {
            WebPageBitmap Shot = new WebPageBitmap(this.WebSite, this.ScreenWidth, this.ScreenHeight);
            Shot.GetIt();
            return Shot.DrawBitmap(this.ImageHeight, this.ImageWidth);
        }

        public void New(int h, int w, int ih, int iw, string u)
        {
            this.S_Height = h;
        }
    }

    public class Provincia
    {
        public int NRO_PAIS { get; set; }

        public int NRO_PROVINCIA { get; set; }

        public string XDE_PROVINCIA { get; set; }
    }

    // http://www.blackbeltcoder.com/Articles/strings/converting-text-to-html
    public class SitioDeEmpresa
    {
        public int NRO_ALTO { get { return 160; } }

        public int NRO_ANCHO { get { return 250; } }

        public int NRO_EMPRESA { get; set; }

        public string ROOT_PATH { get; set; }

        public Uri SITE_URL { get; set; }
    }

    public class Varias
    {
        /// <summary>
        /// devuelve dos strings con la direccion para mostrar y para cargar el mapa
        /// </summary>
        /// <param name="e">objeto perfil de empresa</param>
        /// <returns></returns>
        public static string[] DireccionyMapa(MT_EMPRESA_PERFIL e)
        {
            string[] d = new string[] { String.Empty, String.Empty };

            //return d;

            List<string> data = new List<string>();
            List<string> data2 = new List<string>();
            data.Add(e.XDE_DOMICILIO);
            data.Add(e.XDE_CIUDAD);
            data.Add(e.XDE_CP);
            if (e.NRO_PROVINCIA > 0)
            {
                if (e.MT_PROVINCIA != null)
                    data.Add(e.MT_PROVINCIA.XDE_PROVINCIA);
                else
                {
                    MTrepoDataContext m = new MTrepoDataContext();
                    data.Add((from p in m.MT_PROVINCIAs
                              where p.NRO_PROVINCIA.Equals(e.NRO_PROVINCIA)
                              select p.XDE_PROVINCIA).SingleOrDefault());
                    m.Dispose();
                    m = null;
                }
            }
            if (e.NRO_PAIS > 0)
            {
                if (e.MT_PAISE != null)
                    data.Add(e.MT_PAISE.XDE_PAIS);
                else
                {
                    MTrepoDataContext m = new MTrepoDataContext();
                    data.Add((from p in m.MT_PAISEs
                              where p.NRO_PAIS.Equals(e.NRO_PAIS)
                              select p.XDE_PAIS).SingleOrDefault());
                    m.Dispose();
                    m = null;
                }
            }
            foreach (string k in data)
            {
                if (!String.IsNullOrEmpty(k))
                {
                    data2.Add(k);
                }
            }
            if (data2.Count > 2)
            {
                d[0] = String.Join(", ", data2);
                d[1] = String.Join(" - ", data2);
            }
            return d;
        }

        public static string Fragmento(string s, int l, string c, string f)
        {
            if (s.Length > l)
            {
                s = s.Substring(0, l);
                s = (s.Contains(c)) ? s.Substring(0, s.LastIndexOf(c)) : s;
                s = s + f;
            }
            return s;
        }

        public static string GetSizeReadable(long i)
        {
            string sign = (i < 0 ? "-" : "");
            double readable = (i < 0 ? -i : i);
            string suffix;
            if (i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (double)(i >> 50);
            }
            else if (i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (double)(i >> 40);
            }
            else if (i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (double)(i >> 30);
            }
            else if (i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (double)(i >> 20);
            }
            else if (i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (double)(i >> 10);
            }
            else if (i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = (double)i;
            }
            else
            {
                return i.ToString(sign + "0 B"); // Byte
            }
            readable = readable / 1024;

            return sign + readable.ToString("0.### ") + suffix;
        }

        public static string LinksHtml(string s)
        {
            string strContent = s;
            Regex urlregex = new Regex(@"(http:\/\/([\w.]+\/?)\S*)",
                             RegexOptions.IgnoreCase | RegexOptions.Compiled);
            strContent = urlregex.Replace(strContent,
                         " <a href=\"$1\" target=\"_blank\">$1</a>");
            return strContent;
        }

        /// <summary>
        /// funcion amigable para URLs
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string mt_Escape(string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(input, "");
        }

        public static string StripHTML(string htmlString)
        {
            if (!String.IsNullOrEmpty(htmlString))
            {
                string pattern = @"<(.|\n)*?>";
                return Regex.Replace(htmlString, pattern, string.Empty);
            }
            else
            {
                return htmlString;
            }
        }

        public string HashClave(string pass)
        {
            string salt = WebConfigurationManager.AppSettings["hash"].ToString();
            return FormsAuthentication.HashPasswordForStoringInConfigFile(pass + salt, "SHA1");
        }

        public bool IsDate(string sdate)
        {
            DateTime dt;
            bool isDate = true;
            try
            {
                dt = DateTime.Parse(sdate);
            }
            catch
            {
                isDate = false;
            }
            return isDate;
        }
    }

    public class WebPageBitmap
    {
        private int Height;
        private WebBrowser MyBrowser;
        private string URL;
        private int Width;

        public WebPageBitmap(string url, int width, int height)
        {
            this.Height = height;
            this.Width = width;
            this.URL = url;
            MyBrowser = new WebBrowser()
            {
                ScrollBarsEnabled = false,
                ScriptErrorsSuppressed = true,
                Size = new Size(this.Width, this.Height)
            };
        }

        public Bitmap DrawBitmap(int theight, int twidth)
        {
            DateTime d1;
            DateTime d2 = DateTime.Now.AddSeconds(15);
            do
            {
                d1 = DateTime.Now;
            } while (d1 < d2);

            Bitmap myBitmap = new Bitmap(Width, Height);
            myBitmap.SetResolution(600, 600);
            Rectangle DrawRect = new Rectangle(0, 0, Width, Height);
            MyBrowser.DrawToBitmap(myBitmap, DrawRect);
            Image imgOutput = myBitmap;
            Bitmap thisBitmap = new Bitmap(twidth, theight, imgOutput.PixelFormat);
            thisBitmap.SetResolution(600, 600);
            Image oThumbNail = thisBitmap;
            Graphics g = Graphics.FromImage(oThumbNail);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle orectangle = new Rectangle(0, 0, twidth, theight);
            g.DrawImage(imgOutput, orectangle);
            g.Dispose();
            g = null;
            myBitmap.Dispose();
            myBitmap = null;
            imgOutput.Dispose();
            imgOutput = null;
            MyBrowser.Dispose();
            MyBrowser = null;
            return oThumbNail as Bitmap;
        }

        public void GetIt()
        {
            MyBrowser.Navigate(this.URL);
            while (MyBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }
    }
}