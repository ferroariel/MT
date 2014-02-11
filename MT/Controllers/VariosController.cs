using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MT.Models;

namespace MT.Controllers
{
    public class VariosController : Controller
    {
        private MTrepoDataContext mtdc = new MTrepoDataContext();

        /// <summary>
        /// upload de recurso
        /// </summary>
        /// <param name="uid">id del usuario</param>
        /// <param name="trc">id del tipo de recurso, o 0</param>
        /// <param name="irc">id del recurso, o 7</param>
        /// <param name="eid">id de la empresa, o 0</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Archivo(int uid, int irc = 0, int itc = 7, int eid = 0)
        {
            Boolean ret_ok = false;
            String ret_data = String.Empty;
            MTrepository mtdb = new MTrepository();
            try
            {
                HttpPostedFileBase file = null;
                foreach (string inputTagName in Request.Files)
                {
                    file = Request.Files[inputTagName];
                    if (file.ContentLength > Convert.ToInt32(ConfigurationManager.AppSettings["MT_maxfilesize"]))
                    {
                        ModelState.AddModelError(string.Empty, String.Format("Por favor, use archivos de hasta {0}", Varias.GetSizeReadable(file.ContentLength)));
                    }
                    if (itc != 23)
                    {
                        List<string> ext = file.FileName.ToLower().Split('.').ToList();
                        bool IsJpg = ext.Last() == "jpg";
                        bool IsJpeg = ext.Last() == "jpeg";
                        bool IsPng = ext.Last() == "png";
                        bool IsGif = ext.Last() == "gif";
                        if (!IsJpeg && !IsJpg && !IsPng && !IsGif)
                        {
                            ModelState.AddModelError(string.Empty, "Use archivos .jpg, .gif y .png solamente");
                        }
                    }
                    if (file == null || file.ContentLength <= 0)
                    {
                        ModelState.AddModelError(string.Empty, "Debe elegir una imagen primero");
                    }
                    else
                    {
                        string serverPath = String.Empty;
                        if (eid > 0)
                        {
                            if (mtdb.EmpresaFotosProductos(eid).Count() >= Convert.ToInt32(ConfigurationManager.AppSettings["MT_maxrecxprod"]) && itc != 8)
                            {
                                ModelState.AddModelError(string.Empty, String.Format("Solo se permiten hasta {0} fotos!", ConfigurationManager.AppSettings["MT_maxrecxprod"]));
                            }
                            else
                            {
                                serverPath = Server.MapPath(String.Format("/P/E/{0}", eid));
                            }
                        }
                        else
                        {
                            serverPath = Server.MapPath(String.Format("/P/U/{0}", uid));
                        }
                        if (ModelState.IsValid)
                        {
                            Directory.CreateDirectory(serverPath);
                            string filePath = Path.Combine(serverPath, Path.GetFileName(file.FileName));
                            file.SaveAs(filePath);
                            MT_RECURSO r = null;
                            if (irc > 0)
                            {
                                r = mtdb.Recurso(irc);
                                r.XDE_ARCHIVO = file.FileName;
                                bool b = mtdb.Save();
                            }
                            else
                            {
                                r = new MT_RECURSO()
                                {
                                    NRO_EMPRESA = eid,
                                    NRO_GALERIA = 0,
                                    NRO_USUARIO = uid,
                                    NRO_ORDEN = 0,
                                    NRO_TIPO = itc,
                                    XDE_ARCHIVO = file.FileName
                                };
                                mtdb.NuevoRecurso(r);
                            }
                            Notificaciones.NotificarGrupo(r);
                            ret_ok = true;
                            ret_data = r.NRO_RECURSO.ToString();
                            r = null;
                        }
                        else
                        {
                            var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                            ret_data = String.Join(". ", errorList.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret_data = ex.Message;
            }
            finally
            {
                mtdb = null;
            }
            return Json(new { ok = ret_ok, d = ret_data }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(CacheProfile = "DosMinutosFijos")]
        public ActionResult Index(string AllValues)
        {
            switch (AllValues.ToLower())
            {
                case "legales": return View("Legales");
                case "terminos_de_uso": return View("Terminos_de_Uso");
                case "organizaciones": return View("Organizaciones");
                case "medios": return View("Medios");
                case "contacto": return View("Contacto");
                case "index": return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "DosMinutos")]
        public JsonResult ProvinciasxPais(string id)
        {
            bool success = false;
            List<Provincia> res = (from p in mtdc.MT_PROVINCIAs
                                   where p.NRO_PAIS.Equals(Convert.ToInt32(id))
                                   orderby p.XDE_PROVINCIA
                                   select new Provincia
                                   {
                                       XDE_PROVINCIA = p.XDE_PROVINCIA,
                                       NRO_PAIS = p.NRO_PAIS,
                                       NRO_PROVINCIA = p.NRO_PROVINCIA
                                   })
                    .ToList();
            if (res != null)
            {
                success = true;
            }
            return Json(new { success, res }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Voto(int e, int v)
        {
            MTrepository mtdb = new MTrepository();
            MT_VOTO vt = mtdb.EmpresaPuntaje(e);
            if (vt != null)
            {
                vt.NRO_SUMA += v;
                vt.NRO_VOTOS++;
                mtdb.Save();
            }
            else
            {
                vt = new MT_VOTO()
                {
                    NRO_EMPRESA = e,
                    NRO_VOTOS = 1,
                    NRO_SUMA = v
                };
                mtdc.MT_VOTOs.InsertOnSubmit(vt);
                mtdc.SubmitChanges();
            }
            return Json(new { ok = true, d = (int)(vt.NRO_SUMA / vt.NRO_VOTOS) }, JsonRequestBehavior.AllowGet);
        }
    }
}