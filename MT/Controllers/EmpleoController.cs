using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Web.Mvc;
using MT.Models;

namespace MT.Controllers
{
    public class EmpleoController : Controller
    {
        private MTrepository mtdb = new MTrepository();

        [Authorize]
        public ActionResult Agregar(int id, string nombre)
        {
            ViewBag.Tipos = mtdb.EmpleoTipos();
            ViewBag.Categorias = mtdb.EmpleoCategorias();
            ViewBag.Empresa = new string[] { id.ToString(), nombre };
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Agregar(MT_EMPLEO_OFERTA model, FormCollection formcollection)
        {
            if (!String.IsNullOrEmpty(formcollection["_MT_TIPO"].ToString()) &&
                !String.IsNullOrEmpty(formcollection["_MT_CATEGORIA"].ToString()))
            {
                MT_EMPLEO_OFERTA eo = new MT_EMPLEO_OFERTA()
                {
                    EST_ADMINISTRADOR = 'S',
                    FEC_DESDE = Convert.ToDateTime(model.FEC_DESDE),
                    FEC_HASTA = Convert.ToDateTime(model.FEC_HASTA),
                    NRO_EMPRESA = model.NRO_EMPRESA,
                    XDE_TITULO = model.XDE_TITULO,
                    MEM_LARGA = model.MEM_LARGA,
                    NRO_USUARIO = mtdb.NumerodeUsuario(User.Identity.Name)
                };
                mtdb.NuevaOfertaDeEmpleo(eo);
                bool b = mtdb.Save();

                List<string> tip = (formcollection["_MT_TIPO"].Contains(",")) ? formcollection["_MT_TIPO"].Split(new char[] { ',' }).ToList<string>() : (new List<string>(new string[] { formcollection["_MT_TIPO"].ToString() }));
                List<string> cat = (formcollection["_MT_CATEGORIA"].Contains(",")) ? formcollection["_MT_CATEGORIA"].Split(new char[] { ',' }).ToList<string>() : (new List<string>(new string[] { formcollection["_MT_CATEGORIA"].ToString() }));

                List<MT_EMPLEO> e = new List<MT_EMPLEO>();
                foreach (string t in tip)
                {
                    MT_EMPLEO empleo = new MT_EMPLEO()
                    {
                        NRO_OFERTA = eo.NRO_OFERTA,
                        NRO_TIPO = Convert.ToInt32(t)
                    };
                    e.Add(empleo);
                }
                foreach (string c in cat)
                {
                    MT_EMPLEO empleo = new MT_EMPLEO()
                    {
                        NRO_OFERTA = eo.NRO_OFERTA,
                        NRO_CATEGORIA = Convert.ToInt32(c)
                    };
                    e.Add(empleo);
                }
                mtdb.ActualizarOfertaDeEmpleo(e);
                b = mtdb.Save();
                Notificaciones.NotificarGrupo(eo, mtdb.PerfildeEmpresa(eo.NRO_EMPRESA));
            }
            return RedirectToAction("Perfil", "Usuarios");
        }

        [OutputCache(CacheProfile = "DosMinutosEnCliente")]
        public ActionResult EmailRasterizado(int id)
        {
            Bitmap bmp = new Bitmap(200, 20);
            var g = Graphics.FromImage(bmp);
            SolidBrush drawBrush = new SolidBrush(Color.Silver);
            StringFormat genericFormat = new StringFormat(StringFormatFlags.NoClip);
            var Rect = new Rectangle(5, 4, 200, 20);
            String e = "[Email protegido]";
            if (User.Identity.IsAuthenticated)
            {
                drawBrush = new SolidBrush(Color.Gray);
                e = mtdb.EmpresaEmails(id).FirstOrDefault().XDE_EMAIL;
            }
            using (var headerFont = new Font("Tahoma", 13, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.DrawString(e, headerFont, drawBrush/*Brushes.Gray*/, Rect, genericFormat);
            }
            return new ImageResult { Image = bmp, ImageFormat = ImageFormat.Png };
        }

        [OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult Index()
        {
            IEnumerable<MT_EMPLEO_OFERTA> o = mtdb.OfertaDeEmpleo();
            return View(o);
        }

        [HttpPost]
        [OutputCache(CacheProfile = "DosMinutos")]
        public ActionResult Index(BuscarEmpleoModel m)
        {
            IEnumerable<MT_EMPLEO_OFERTA> o = mtdb.OfertaDeEmpleo(m);
            ViewBag.BuscarEmpleo = m;
            return View(o);
        }

        [OutputCache(CacheProfile = "DosMinutos")]
        public ActionResult Oferta(int? id)
        {
            if (id.HasValue)
            {
                MT_EMPLEO_OFERTA o = mtdb.OfertaDeEmpleo(id.Value).FirstOrDefault();
                ViewBag.Tipos = mtdb.EmpleoTipos(o.NRO_OFERTA);
                ViewBag.Categorias = mtdb.EmpleoCategorias(o.NRO_OFERTA);
                ViewBag.Empresa = mtdb.PerfildeEmpresa(o.NRO_EMPRESA);
                ViewBag.Telefonos = mtdb.EmpresaTelefonos(o.NRO_EMPRESA);

                //ViewBag.Emails = mtdb.EmpresaEmails(o.NRO_EMPRESA);
                return View(o);
            }
            else
                return RedirectToAction("Index");
        }
    }
}

namespace MT.Models
{
    public class BuscarEmpleoModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Busqueda")]
        public string Buscar { get; set; }
    }
}
