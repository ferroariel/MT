using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MT.Models;

namespace MT.Controllers
{
    public class DirectorioController : Controller
    {
        private MTrepository mtdb = new MTrepository();
        private Varias v = new Varias();

        [HttpPost]
        public JsonResult Buscar(Busqueda busqueda)
        {
            Resultados res = new Resultados();
            bool success = false;

            if (ModelState.IsValid)
            {
                string h = String.Empty;
                int contadorResultados = 0;

                //Varias v = new Varias();
                IEnumerable<MT_EMPRESA_PERFIL> emps = LuceneSearch.Search_E(busqueda, ref contadorResultados, "CAMPO_BUSQUEDA");
                List<List<string>> e_n = new List<List<string>>();
                List<string> d;
                foreach (MT_EMPRESA_PERFIL e in emps)
                {
                    string pais = mtdb.ObtenerProvincia(e.NRO_PROVINCIA);
                    string provincia = mtdb.ObtenerPais(e.NRO_PAIS);
                    d = new List<string>();
                    d.Add(e.XDE_RAZONSOCIAL);
                    d.Add(pais);
                    d.Add(provincia);
                    d.Add(Varias.mt_Escape(e.XDE_RAZONSOCIAL));
                    if (!String.IsNullOrEmpty(e.XDE_LAT) && !String.IsNullOrEmpty(e.XDE_LON))
                    {
                        d.Add(e.XDE_LAT);
                        d.Add(e.XDE_LON);
                    }
                    else
                    {
                        d.Add(String.Empty);
                        d.Add(String.Empty);
                    }
                    d.Add(mtdb.CodigoEmpresa(e.NRO_EMPRESA, e.NRO_PAIS));
                    d.Add(e.XDE_CORTA + "<br />" + e.MEM_LARGA + "<br />" + e.OBS_MEDIA);
                    d.Add(e.NRO_EMPRESA.ToString());
                    e_n.Add(d);
                }
                res = new Resultados()
                {
                    res_Contador = contadorResultados--,
                    res_Pages = (contadorResultados / busqueda.PageSize) + 1,
                    res_Emps = e_n,
                    res_Tags = mtdb.TagsDeEmpresas(emps),
                    res_Current = busqueda.PageCurrent
                };
                success = true;
            }
            return Json(new { success, res }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ValidateInput(false)]
        public ActionResult Empresa(string id = "", FormCollection fc = null)
        {
            Boolean vp = false;
            if (fc != null && fc.HasKeys())
            {
                vp = (fc["VistaPrevia"].Equals("1")) ? true : false;
            }
            MT_EMPRESA_PERFIL e = null;
            if (!vp)
                e = mtdb.PerfildeEmpresa(id);
            else
            {
                e = new MT_EMPRESA_PERFIL()
                {
                    NRO_EMPRESA = Convert.ToInt32(fc["NRO_EMPRESA"]),
                    XDE_RAZONSOCIAL = fc["XDE_RAZONSOCIAL"].ToString(),
                    XDE_CORTA = fc["XDE_CORTA"].ToString(),
                    OBS_MEDIA = fc["OBS_MEDIA"].ToString(),
                    MEM_LARGA = fc["MEM_LARGA"].ToString(),
                    XDE_CIUDAD = fc["XDE_CIUDAD"].ToString(),
                    XDE_DOMICILIO = fc["XDE_DOMICILIO"].ToString(),
                    XDE_CP = fc["XDE_CP"].ToString(),
                    XDE_WEB = fc["XDE_WEB"].ToString(),
                    XDE_FACEBOOK = fc["XDE_FACEBOOK"].ToString(),
                    XDE_TWITTER = fc["XDE_TWITTER"].ToString(),
                    NRO_PAIS = Convert.ToInt32(fc["NRO_PAIS"]),
                    NRO_PROVINCIA = Convert.ToInt32(fc["NRO_PROVINCIA"]),
                    XDE_LAT = fc["XDE_LAT"].ToString(),
                    XDE_LON = fc["XDE_LON"].ToString()
                };
                ViewBag.VP = true;
            }
            string[] direccionymapa = Varias.DireccionyMapa(e);

            // esto es solo para capturar las miniaturas de los sitios que no la tienen
            if (!vp && !System.IO.File.Exists(Server.MapPath(String.Format("/P/E/{0}/sitio.jpg", e.NRO_EMPRESA))) && !String.IsNullOrEmpty(e.XDE_WEB))
            {
                Uri validatedUri;
                if (Uri.TryCreate(e.XDE_WEB, UriKind.RelativeOrAbsolute, out validatedUri))
                {
                    SitioDeEmpresa sitiodeempresa = new SitioDeEmpresa()
                    {
                        NRO_EMPRESA = e.NRO_EMPRESA,
                        ROOT_PATH = Server.MapPath("/"),
                        SITE_URL = validatedUri
                    };
                    mtdb.ForzarCapturaDeSitio(sitiodeempresa);
                }
            }
            ViewBag.Direccion = (direccionymapa[0] != null) ? direccionymapa[0] : String.Empty;/* mtdb.ObtenerProvincia(e.NRO_PROVINCIA);*/
            ViewBag.Mapa = (direccionymapa[1] != null) ? direccionymapa[0] : String.Empty; /* String.Format("{0} - {1} - {2} - {3} - {4}", e.XDE_DOMICILIO, e.XDE_CIUDAD, e.XDE_CP, ViewBag.NombreProvincia, ViewBag.NombrePais);*/
            ViewBag.EmpresaFotosProductos = mtdb.EmpresaFotosProductos(e.NRO_EMPRESA);
            ViewBag.Descargas = mtdb.EmpresaDescargas(e.NRO_EMPRESA);
            int num_usuario = mtdb.NumerodeUsuario(User.Identity.Name);
            ViewBag.Logo = mtdb.LogoDeEmpresa(e.NRO_EMPRESA, num_usuario);
            ViewBag.FotoGrande = mtdb.EmpresaFotoGrande(e.NRO_EMPRESA, num_usuario);
            ViewBag.VP = vp;
            ViewBag.Autor = (mtdb.EsAutorDeEmpresa(e.NRO_EMPRESA, num_usuario)) ? num_usuario.ToString() : String.Empty;
            ViewBag.Siguiendo = (mtdb.EstaSiguiendoEmpresa(e.NRO_EMPRESA, num_usuario)) ? "No Seguir" : "Seguir";
            ViewBag.Seguidores = mtdb.SeguidoresDeEmpresa(id);
            ViewBag.MiID = mtdb.NumerodeUsuario(User.Identity.Name);
            ViewBag.Provincia = mtdb.ObtenerProvincia(e.NRO_PROVINCIA);
            ViewBag.Pais = mtdb.ObtenerPais(e.NRO_PAIS);
            ViewBag.Muro = mtdb.Muro(e.NRO_EMPRESA);
            ViewBag.Votos = mtdb.EmpresaPuntaje(e.NRO_EMPRESA);
            List<MT_TAG> tg = null;
            mtdb.TagsdeEmpresa(e.NRO_EMPRESA, out tg);
            ViewBag.Tags = (tg.Count() > 0) ? tg : null;
            ViewBag.Opiniones = mtdb.EmpresaOpiniones(e.NRO_EMPRESA);
            return View(e);
        }

        [HttpPost]
        public JsonResult Filtrar(BusquedaFiltros bf)
        {
            bool success = false;
            IEnumerable<MT_TAG> res = new List<MT_TAG>();
            if (Request.IsAjaxRequest())
            {
                success = true;
                res = mtdb.TagsRelacionados(bf);
            }
            return Json(new { success, res }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "DosMinutos")]
        public ActionResult Home()
        {
            return View();
        }
        [OutputCache(CacheProfile = "DosMinutos")]
        public ActionResult Index()
        {
            List<MT_EMPRESA_PERFIL> es = mtdb.PerfilesEmpresas();
            ViewBag.Tags = mtdb.Tags(0);
            return View(es);
        }

        public ActionResult Tag(int id = 0)
        {
            ViewBag.NombreTag = mtdb.NombreTag(id);
            ViewBag.Tags = mtdb.Tags(id);
            List<string> path = new List<string>();
            ViewBag.TagsPath = mtdb.TagsPath(id, ref path);
            ViewBag.TagEmpresas = mtdb.TagEmpresasPerfiles(id);
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult VistaPrevia(FormCollection f)
        {
            return RedirectToAction("Empresa", f);
        }
    }
}