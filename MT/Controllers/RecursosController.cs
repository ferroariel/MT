using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MT.Models;

// http://dariosantarelli.wordpress.com/2010/10/16/asp-net-mvc-2-handling-timeouts-in-asynchronous-controllers/

namespace MT.Controllers
{
    public class RecursosController : AsyncController
    {
        private static int[] tipos = new int[] { 17, 1 };
        private static MTrepository mtdb = new MTrepository();
        private static MTrepoDataContext mtdc = new MTrepoDataContext() { ObjectTrackingEnabled = false };

        #region "Avatar"

        public void AvatarAsync(int? eid, int? uid)
        {
            AsyncManager.OutstandingOperations.Increment();
            var task = Task.Factory.StartNew(() =>
            {
                try { AsyncManager.Parameters["buffer"] = AvatarCarga(eid, uid); }
                catch { }
                finally { AsyncManager.OutstandingOperations.Decrement(); }
            });
        }
        public FileResult AvatarCompleted(byte[] buffer)
        {
            return File(buffer, MediaTypeNames.Application.Octet, "pec");
        }
        private byte[] AvatarCarga(int? eid, int? uid)
        {
            String pth = String.Empty;
            String icon_default = "default-avatar.png";

            // si me pasan un id de empresa, tengo que mostrar el logo, o la primer imagen relacionada con esta empresa, o la imagen generica
            if (eid != null && eid != 0)
            {
                icon_default = "default-store.png";
                MT_RECURSO e = mtdb.LogoDeEmpresa(Convert.ToInt32(eid));
                if (e != null)
                {
                    pth = Server.MapPath(String.Format("/P/E/{0}/{1}", Convert.ToInt32(eid), e.XDE_ARCHIVO));
                }
                else
                {
                    IQueryable<MT_RECURSO> i = mtdb.EmpresaFotos(Convert.ToInt32(eid)).Take(1);
                    if (i.Count() > 0)
                    {
                        pth = Server.MapPath(String.Format("/P/E/{0}/{1}", Convert.ToInt32(eid), i.First().XDE_ARCHIVO));
                    }
                }
            }
            else
            {
                // si me pasan un id de usuario, muestro su avatar o la imagen generica
                if (uid == null)
                {
                    uid = mtdb.NumerodeUsuario(User.Identity.Name);
                }
                pth = Server.MapPath(String.Format("/P/U/{0}/{1}", uid, mtdb.AvatarUsuario(Convert.ToInt32(uid))));
            }
            if (!System.IO.File.Exists(pth))
            {
                pth = Server.MapPath("/Content/img/" + icon_default);
            }
            byte[] buffer = null;
            try
            {
                FileStream fileStream = new FileStream(pth, FileMode.Open, FileAccess.Read);
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
                fileStream.Close();
            }
            catch { }
            return buffer;
        }

        #endregion

        #region "Muro - Carga Inicial"

        [AsyncTimeout(600000)]
        public void CargarMuroAsync(int _e = 0, int _c = 0)
        {
            if (_e > 0) { _e = StringMethodExtensions.DecodeId(_e); }
            AsyncManager.OutstandingOperations.Increment();
            var task = Task.Factory.StartNew(() =>
            {
                try { AsyncManager.Parameters["Msgs"] = CargarMuroNuevos(_e, _c); }
                catch { }
                finally { AsyncManager.OutstandingOperations.Decrement(); }
            });
        }
        public JsonResult CargarMuroCompleted(IQueryable<MsgMuro> Msgs)
        {
            return Json(new { ok = true, d = Msgs }, JsonRequestBehavior.AllowGet);
        }
        private IQueryable<MsgMuro> CargarMuroNuevos(int _e = 0, int _c = 0)
        {
            IQueryable<MsgMuro> mr;
            if (_e > 0)
            {
                mr = (from n in mtdc.MT_MENSAJEs.AsEnumerable()
                      where n.NRO_EMPRESA.Equals(_e) && tipos.Contains(n.NRO_TIPO) && n.EST_VIGENTE.Equals('S') && n.NRO_USUARIO_DESTINO.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]))
                      orderby n.FEC_PUBLICADO descending
                      select new MsgMuro
                      {
                          i = n.NRO_MENSAJE,
                          t = n.XDE_TITULO,
                          m = StringMethodExtensions.ConvertRelativePathsToAbsolute(n.MEM_LARGA, ConfigurationManager.AppSettings["MT_sitiourl"] + "/"),
                          d = n.FEC_PUBLICADO,
                          x = n.FEC_PUBLICADO.ToString("o")
                      }).AsQueryable();
            }
            else
            {
                mr = (from n in mtdc.MT_MENSAJEs.AsEnumerable()
                      where tipos.Contains(n.NRO_TIPO) && n.EST_VIGENTE.Equals('S') && n.NRO_USUARIO_DESTINO.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]))
                      orderby n.FEC_PUBLICADO descending
                      select new MsgMuro
                      {
                          i = n.NRO_MENSAJE,
                          t = n.XDE_TITULO,
                          m = StringMethodExtensions.ConvertRelativePathsToAbsolute(n.MEM_LARGA, ConfigurationManager.AppSettings["MT_sitiourl"] + "/"),
                          d = n.FEC_PUBLICADO,
                          x = n.FEC_PUBLICADO.ToString("o")
                      }).AsQueryable();
            }
            if (mr.Count().Equals(0) && Response.IsClientConnected)
            {
                Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["MT_delaychequeomensajes"]));
                CargarMuroNuevos(_e, _c);
            }
            mr = (_c > 0) ? mr.Take(_c) : mr.Take(Convert.ToInt32(ConfigurationManager.AppSettings["MT_msgmurohome"]));
            return mr;
        }

        #endregion

        #region "Muro - Chequeo de mensajes nuevos"

        [AsyncTimeout(600000)]
        public void ChequearMuroAsync(int _e = 0)
        {
            if (_e > 0) { _e = StringMethodExtensions.DecodeId(_e); }
            DateTime n = DateTime.Now;
            AsyncManager.OutstandingOperations.Increment();
            var task = Task.Factory.StartNew(() =>
            {
                try { AsyncManager.Parameters["mensajesnuevos"] = NuevoEnMuro(n, _e); }
                catch { }
                finally { AsyncManager.OutstandingOperations.Decrement(); }
            });
        }
        public JsonResult ChequearMuroCompleted(IQueryable<MsgMuro> mensajesnuevos)
        {
            return Json(new { ok = true, d = mensajesnuevos }, JsonRequestBehavior.AllowGet);
        }
        private IQueryable<MsgMuro> NuevoEnMuro(DateTime d, int _e)
        {
            IQueryable<MsgMuro> mr;
            int c = 0;
            if (_e > 0)
            {
                mr = (from n in mtdc.MT_MENSAJEs.AsEnumerable()
                      where n.NRO_EMPRESA.Equals(_e) && n.FEC_PUBLICADO >= d && tipos.Contains(n.NRO_TIPO) && n.EST_VIGENTE.Equals('S') && n.NRO_USUARIO_DESTINO.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]))
                      orderby n.FEC_PUBLICADO descending
                      select new MsgMuro
                      {
                          i = n.NRO_MENSAJE,
                          t = n.XDE_TITULO,
                          m = StringMethodExtensions.ConvertRelativePathsToAbsolute(n.MEM_LARGA, ConfigurationManager.AppSettings["MT_sitiourl"] + "/"),
                          d = n.FEC_PUBLICADO,
                          x = n.FEC_PUBLICADO.ToString("o")
                      }).AsQueryable();
            }
            else
            {
                mr = (from n in mtdc.MT_MENSAJEs.AsEnumerable()
                      where n.FEC_PUBLICADO >= d && tipos.Contains(n.NRO_TIPO) && n.EST_VIGENTE.Equals('S') && n.NRO_USUARIO_DESTINO.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]))
                      orderby n.FEC_PUBLICADO descending
                      select new MsgMuro
                       {
                           i = n.NRO_MENSAJE,
                           t = n.XDE_TITULO,
                           m = StringMethodExtensions.ConvertRelativePathsToAbsolute(n.MEM_LARGA, ConfigurationManager.AppSettings["MT_sitiourl"] + "/"),
                           d = n.FEC_PUBLICADO,
                           x = n.FEC_PUBLICADO.ToString("o")
                       }).AsQueryable();
            }
            if (mr.Count().Equals(0) && Response.IsClientConnected)
            {
                Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["MT_delaychequeomensajes"]));
                NuevoEnMuro(d, _e);
            }
            else
            {
                // cayó acá dentro
                int z = 0;
                String mierda = "asdfadfadf";
            }
            return mr;
        }

        #endregion

        #region "Descarga de recurso"

        public void DescargaAsync(int eid, int rid)
        {
            AsyncManager.OutstandingOperations.Increment();
            var task = Task.Factory.StartNew(() =>
            {
                try
                {
                    AsyncManager.Parameters["buffer"] = Descarga(eid, rid);
                    AsyncManager.Parameters["filename"] = mtdb.EmpresaDescargas(eid, rid).SingleOrDefault().XDE_ARCHIVO;
                }
                catch { }
                finally { AsyncManager.OutstandingOperations.Decrement(); }
            });
        }
        public FileResult DescargaCompleted(byte[] buffer, string filename)
        {
            return File(buffer, MediaTypeNames.Application.Octet, filename);
        }
        private byte[] Descarga(int eid, int rid)
        {
            String pth = String.Empty;
            MT_RECURSO d = mtdb.EmpresaDescargas(eid, rid).SingleOrDefault();
            if (d != null)
            {
                pth = Server.MapPath(String.Format("/P/E/{0}/{1}", eid, d.XDE_ARCHIVO));
            }
            byte[] buffer = null;
            try
            {
                FileStream fileStream = new FileStream(pth, FileMode.Open, FileAccess.Read);
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                         // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
                fileStream.Close();
            }
            catch { }
            return buffer;
        }

        #endregion

        #region "Fondo"

        public void FondoAsync(int eid, int uid)
        {
            AsyncManager.OutstandingOperations.Increment();
            var task = Task.Factory.StartNew(() =>
            {
                try { AsyncManager.Parameters["buffer"] = FondoCarga(eid, uid); }
                catch { }
                finally { AsyncManager.OutstandingOperations.Decrement(); }
            });
        }
        public FileResult FondoCompleted(byte[] buffer)
        {
            return File(buffer, MediaTypeNames.Application.Octet, "fnd");
        }
        private byte[] FondoCarga(int eid, int uid)
        {
            String pth = String.Empty;
            String icon_default = "default-store.png";
            MT_RECURSO e = mtdb.EmpresaFotoGrande(eid, uid);
            if (e != null && !String.IsNullOrEmpty(e.XDE_ARCHIVO))
            {
                pth = Server.MapPath(String.Format("/P/E/{0}/{1}", eid, e.XDE_ARCHIVO));
            }
            else
            {
                pth = Server.MapPath("/Content/img/default-store.png");
            }
            if (!System.IO.File.Exists(pth))
            {
                pth = Server.MapPath("/Content/img/" + icon_default);
            }
            byte[] buffer = null;
            try
            {
                FileStream fileStream = new FileStream(pth, FileMode.Open, FileAccess.Read);
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
                fileStream.Close();
            }
            catch { }
            return buffer;
        }

        #endregion


        /*
        protected override void OnException(ExceptionContext filterContext)
        
            if (filterContext.Exception is TimeoutException)
            {
                filterContext.Result = RedirectToAction("Error");
                filterContext.ExceptionHandled = true;
            }
            base.OnException(filterContext);
        }
        
        
        */






    }
}