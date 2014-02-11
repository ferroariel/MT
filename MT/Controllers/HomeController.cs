using System.Collections.Generic;
using System.Web.Mvc;
using MT.Models;

namespace MT.Controllers
{
    public class HomeController : Controller
    {
        private MTrepository mtdata = new MTrepository();

        public ActionResult ActualizarIndice()
        {
            IEnumerable<MT_EMPRESA_PERFIL> emps = mtdata.GetAll();
            LuceneSearch.AddUpdateLuceneIndex(emps);
            LuceneSearch.Optimize();
            return View();
        }

        public ActionResult BorrarIndice()
        {
            bool b = LuceneSearch.ClearLuceneIndex();
            ViewData["borrado"] = b;
            return View();
        }

        [OutputCache(CacheProfile = "DosMinutos")]
        public ActionResult CargarPais()
        {
            List<string> ps = null /*mtdata.cargarPaises()*/;

            //mtdata.Save();
            return Content(string.Join(", ", ps.ToArray()));
        }

        [OutputCache(CacheProfile = "DosMinutos")]
        public ActionResult Index()
        {
            List<string> ps = null /*mtdata.ListarPaises()*/;
            ViewData["p"] = ps;
            return View();
        }
    }
}

namespace MT.Models
{
    public class Busqueda
    {
        public BusquedaFiltros Filtros { get; set; }
        public int PageCurrent { get; set; }
        public int PageSize { get; set; }
        public string Texto { get; set; }
        public int UsarLucene { get; set; }
    }

    public class BusquedaFiltros
    {
        public int[] NRO_RUBRO { get; set; }
        public string OBS_NOMBRE { get; set; }
    }

    // este objeto debiera ser el germen de un objeto json complejo que se pueda manipular
    // a nivel de cliente
    public class Resultados
    {
        public int res_Contador { get; set; }
        public int res_Current { get; set; }
        public List<List<string>> res_Emps { get; set; }
        public int res_Pages { get; set; }
        public IEnumerable<MT_TAG> res_Tags { get; set; }
    }
}
