using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using MT.Models;

namespace MT.Controllers
{
    public class MensajesController : Controller
    {
        private static Dictionary<int, bool> UsuarioEscribiendo = new Dictionary<int, bool>();
        private Mensajes m = new Mensajes();
        private MTrepository mtdb = new MTrepository();
        [HttpGet]
        public JsonResult AlertaMensajeOK()
        {
            if (Request.IsAjaxRequest())
            {
                mtdb.MensajesLeidos(User.Identity.Name);
                mtdb.Save();
            }
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EstaEscribiendo(int iUsr, bool b)
        {
            bool o = false;
            if (Request.IsAjaxRequest())
            {
                o = true;
                if (UsuarioEscribiendo.ContainsKey(iUsr))
                    UsuarioEscribiendo[iUsr] = b;
                else
                    UsuarioEscribiendo.Add(iUsr, b);
            }
            return Json(new { ok = o, s = b }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult HayMensajes()
        {
            int cont = -1;
            List<int> iMsgs = new List<int>();
            if (Request.IsAjaxRequest())
            {
                cont = m.ContarMensajesNuevos(mtdb.NumerodeUsuario(User.Identity.Name), out iMsgs);
            }
            return Json(new { ok = true, cnt = cont, c = iMsgs }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            mtdb.MensajesLeidos(User.Identity.Name);
            mtdb.Save();
            int k = mtdb.NumerodeUsuario(User.Identity.Name);
            ViewBag.MiID = k;
            ViewBag.MiNombre = mtdb.NombreyApellidoUsuario(k);
            return View();
        }
        [HttpGet]
        [Authorize]
        public JsonResult ListadoArbol()
        {
            if (Request.IsAjaxRequest())
            {
                int k = mtdb.NumerodeUsuario(User.Identity.Name);
                List<Remitente> x = mtdb.AutoresDeMensajes(k);
                if (x.Count() > 0)
                    return Json(new { ok = true, r = x }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListadoMensajes(int u, int e, int n = 0)
        {
            if (Request.IsAjaxRequest())
            {
                int k = mtdb.NumerodeUsuario(User.Identity.Name);

                List<MT_MENSAJE> msgs = mtdb.Mensajes(k, n).ToList();

                List<Conversacion> cnv = new List<Conversacion>();
                Conversacion c = null;
                foreach (MT_MENSAJE m in msgs)
                {
                    c = null;
                    if (e != 0)
                    {
                        // mensajes escritos a la empresa pasada por id
                        if (Convert.ToInt32(m.NRO_EMPRESA_DESTINO).Equals(e))
                        {
                            c = new Conversacion()
                            {
                                i = m.NRO_MENSAJE,
                                d = m.FEC_PUBLICADO.ToString("o"),
                                m = Varias.LinksHtml(m.XDE_CORTA + "\n" + m.OBS_MEDIA + "\n" + m.MEM_LARGA),
                                ei = (m.NRO_EMPRESA != 0) ? m.NRO_EMPRESA : 0,
                                en = (m.NRO_EMPRESA != 0) ? mtdb.PerfildeEmpresa(m.NRO_EMPRESA).XDE_RAZONSOCIAL : String.Empty,
                                ui = (m.NRO_USUARIO != 0) ? m.NRO_USUARIO : 0,
                                un = (m.NRO_USUARIO != 0) ? mtdb.NombreyApellidoUsuario(m.NRO_USUARIO) : String.Empty,
                                t = m.NRO_TIPO
                            };
                        }

                        // mensajes escritos por la empresa pasada por id
                        else if (m.NRO_EMPRESA.Equals(e))
                        {
                            c = new Conversacion()
                            {
                                i = m.NRO_MENSAJE,
                                d = m.FEC_PUBLICADO.ToString("o"),
                                m = Varias.LinksHtml(m.XDE_CORTA + "\n" + m.OBS_MEDIA + "\n" + m.MEM_LARGA),
                                ei = (m.NRO_EMPRESA != 0) ? m.NRO_EMPRESA : 0,
                                en = (m.NRO_EMPRESA != 0 && mtdb.PerfildeEmpresa(m.NRO_EMPRESA) != null) ? mtdb.PerfildeEmpresa(m.NRO_EMPRESA).XDE_RAZONSOCIAL : String.Empty,
                                ui = (m.NRO_USUARIO != 0) ? m.NRO_USUARIO : 0,
                                un = (m.NRO_USUARIO != 0) ? mtdb.NombreyApellidoUsuario(m.NRO_USUARIO) : String.Empty,
                                t = m.NRO_TIPO
                            };
                        }
                    }
                    else if (u != 0)
                    {
                        // mensajes escritos a mi
                        if (Convert.ToInt32(m.NRO_USUARIO_DESTINO).Equals(u))
                        {
                            c = new Conversacion()
                             {
                                 i = m.NRO_MENSAJE,
                                 d = m.FEC_PUBLICADO.ToString("o"),
                                 m = Varias.LinksHtml(m.XDE_CORTA + "\n" + m.OBS_MEDIA + "\n" + m.MEM_LARGA),
                                 ei = (m.NRO_EMPRESA != 0) ? m.NRO_EMPRESA : 0,
                                 en = (m.NRO_EMPRESA != 0) ? mtdb.PerfildeEmpresa(m.NRO_EMPRESA).XDE_RAZONSOCIAL : String.Empty,
                                 ui = (m.NRO_USUARIO != 0) ? m.NRO_USUARIO : 0,
                                 un = (m.NRO_USUARIO != 0) ? mtdb.NombreyApellidoUsuario(m.NRO_USUARIO) : String.Empty,
                                 t = m.NRO_TIPO
                             };
                        }

                        // mensajes escritos por mi
                        else if (m.NRO_USUARIO.Equals(u))
                        {
                            c = new Conversacion()
                            {
                                i = m.NRO_MENSAJE,
                                d = m.FEC_PUBLICADO.ToString("o"),
                                m = Varias.LinksHtml(m.XDE_CORTA + "\n" + m.OBS_MEDIA + "\n" + m.MEM_LARGA),
                                ei = (m.NRO_EMPRESA != 0) ? m.NRO_EMPRESA : 0,
                                en = (m.NRO_EMPRESA != 0) ? mtdb.PerfildeEmpresa(m.NRO_EMPRESA).XDE_RAZONSOCIAL : String.Empty,
                                ui = (m.NRO_USUARIO != 0) ? m.NRO_USUARIO : 0,
                                un = (m.NRO_USUARIO != 0) ? mtdb.NombreyApellidoUsuario(m.NRO_USUARIO) : String.Empty,
                                t = m.NRO_TIPO
                            };
                        }
                    }
                    if (c != null)
                        cnv.Add(c);
                }
                msgs = null;
                bool b = (ActividadDeUsuario.tipeando != null) ? (ActividadDeUsuario.tipeando.ContainsKey(u)) ? true : false : false;
                return Json(new { ok = true, c = cnv, o = b }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
        }

        #region "estan escribiendo"

        public JsonResult Tipea(int id)
        {
            string usr = String.Empty;
            bool o = false;
            if (Request.IsAjaxRequest())
            {
                if (ActividadDeUsuario.tipeando == null)
                {
                    ActividadDeUsuario.tipeando = new Dictionary<int, bool>();
                }
                if (ActividadDeUsuario.tipeando.ContainsKey(id))
                {
                    if (ActividadDeUsuario.tipeando[id].Equals(true))
                    {
                        o = true;
                    }
                    usr = mtdb.NombreyApellidoUsuario(id);
                }
            }
            return Json(new { ok = o, u = usr }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Tipear(int id, bool b)
        {
            bool o = false;
            if (Request.IsAjaxRequest())
            {
                o = true;
                if (ActividadDeUsuario.tipeando == null)
                {
                    ActividadDeUsuario.tipeando = new Dictionary<int, bool>();
                }
                if (ActividadDeUsuario.tipeando.ContainsKey(id))
                {
                    ActividadDeUsuario.tipeando[id] = b;
                }
                else
                {
                    ActividadDeUsuario.tipeando.Add(id, b);
                }
            }
            return Json(new { ok = o }, JsonRequestBehavior.AllowGet);
        }
        #endregion "estan escribiendo"
        [Authorize]
        [HttpPost]
        public JsonResult Mensaje(int ed, int ud, string m, string a, int edm = 0, int s = 0, int t = 3)
        {
            bool o = false;
            if (Request.IsAjaxRequest())
            {
                o = true;
                if (s > 0) // si viene con valor entonces es un mensaje a los seguidores
                {
                    List<MT_EMPRESA_SEGUIDORE> seg = mtdb.SeguidoresDeEmpresa(ed);
                    foreach (MT_EMPRESA_SEGUIDORE sg in seg)
                    {
                        MT_MENSAJE n = new MT_MENSAJE()
                        {
                            EST_ADMINISTRADOR = 'S',
                            FEC_PUBLICADO = DateTime.Now,
                            EST_VIGENTE = 'S',
                            NRO_PADRE = 0,
                            XDE_TITULO = a,
                            MEM_LARGA = Varias.StripHTML(m),
                            NRO_USUARIO = mtdb.NumerodeUsuario(User.Identity.Name),
                            NRO_USUARIO_DESTINO = sg.NRO_USUARIO,
                            NRO_EMPRESA_DESTINO = 0,
                            NRO_EMPRESA = ed,
                            NRO_TIPO = t/*1*/
                        };
                        mtdb.NuevoMensaje(n);
                    }
                }
                else
                {
                    MT_MENSAJE n = new MT_MENSAJE()
                    {
                        EST_ADMINISTRADOR = 'S',
                        FEC_PUBLICADO = DateTime.Now,
                        EST_VIGENTE = 'S',
                        NRO_PADRE = 0,
                        MEM_LARGA = Varias.StripHTML(m),
                        XDE_TITULO = a,
                        NRO_USUARIO = mtdb.NumerodeUsuario(User.Identity.Name),
                        NRO_USUARIO_DESTINO = ud,
                        NRO_EMPRESA_DESTINO = ed,
                        NRO_EMPRESA = edm,
                        NRO_TIPO = t/*1*/
                    };
                    mtdb.NuevoMensaje(n);
                }
            }
            return Json(new { ok = o }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Muro(int e, int u, string m, string a, int s)
        {
            if (s.Equals(1))
            {
                List<MT_EMPRESA_SEGUIDORE> seg = mtdb.SeguidoresDeEmpresa(e);
                foreach (MT_EMPRESA_SEGUIDORE sg in seg)
                {
                    MT_MENSAJE n = new MT_MENSAJE()
                    {
                        EST_ADMINISTRADOR = 'S',
                        FEC_PUBLICADO = DateTime.Now,
                        EST_VIGENTE = 'S',
                        NRO_PADRE = 0,
                        XDE_TITULO = a,
                        MEM_LARGA = m,
                        NRO_USUARIO = u,
                        NRO_USUARIO_DESTINO = sg.NRO_USUARIO,
                        NRO_EMPRESA_DESTINO = 0,
                        NRO_EMPRESA = e,
                        NRO_TIPO = 3/*1*/
                    };
                    mtdb.NuevoMensaje(n);
                }
                return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                MT_MENSAJE n = new MT_MENSAJE()
                {
                    EST_ADMINISTRADOR = 'S',
                    FEC_PUBLICADO = DateTime.Now,
                    EST_VIGENTE = 'S',
                    NRO_PADRE = 0,
                    MEM_LARGA = m,
                    XDE_TITULO = a,
                    NRO_USUARIO = u,
                    NRO_USUARIO_DESTINO = 0,
                    NRO_EMPRESA_DESTINO = 0,
                    NRO_EMPRESA = e,
                    NRO_TIPO = 17
                };
                mtdb.NuevoMensaje(n);
                return Json(new { ok = true, d = mtdb.Muro(e) }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public JsonResult MuroOcultar(int i)
        {
            if (Request.IsAjaxRequest())
            {
                MT_MENSAJE m = mtdb.Mensaje(i);
                m.EST_VIGENTE = 'N';
                mtdb.Save();
                return Json(new { ok = true, d = mtdb.Muro(m.NRO_EMPRESA) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult Reporte(string a, string m, int t = 0)
        {
            if (User.Identity.IsAuthenticated && Request.IsAjaxRequest())
            {
                try
                {
                    MT_MENSAJE r = new MT_MENSAJE()
                    {
                        EST_ADMINISTRADOR = 'S',
                        EST_VIGENTE = 'S',
                        FEC_PUBLICADO = DateTime.Now,
                        MEM_LARGA = m,
                        NRO_USUARIO = mtdb.NumerodeUsuario(User.Identity.Name),
                        NRO_USUARIO_DESTINO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]),
                        NRO_EMPRESA = 0,
                        NRO_EMPRESA_DESTINO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admemp"]),
                        XDE_TITULO = String.Empty,
                        NRO_TIPO = t,
                        NRO_PADRE = 0
                    };
                    mtdb.NuevoMensaje(r);
                    return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { ok = false, d = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { ok = false, d = "Debes ingresar como usuario primero!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Respuesta(RespuestaPrivada model)
        {
            int k = mtdb.NumerodeUsuario(User.Identity.Name);
            if (Convert.ToInt32(model.msg_id) != 0)
            {
                MT_MENSAJE o = mtdb.Mensaje(model.msg_id);
                if (o != null)
                {
                    MT_MENSAJE m = new MT_MENSAJE()
                    {
                        EST_ADMINISTRADOR = 'S',
                        FEC_PUBLICADO = DateTime.Now,
                        FEC_LEIDO = null,
                        EST_VIGENTE = 'S',
                        MEM_LARGA = model.rep_mensaje,
                        NRO_PADRE = o.NRO_MENSAJE,
                        NRO_USUARIO = k,
                        NRO_EMPRESA = Convert.ToInt32(o.NRO_EMPRESA_DESTINO),
                        NRO_USUARIO_DESTINO = o.NRO_USUARIO,
                        NRO_EMPRESA_DESTINO = o.NRO_EMPRESA,
                        NRO_TIPO = 3/*o.NRO_TIPO*/
                    };
                    mtdb.NuevoMensaje(m);
                    if (ActividadDeUsuario.tipeando.ContainsKey(k))
                    {
                        ActividadDeUsuario.tipeando[k] = false;
                    }
                    else
                    {
                        ActividadDeUsuario.tipeando.Add(k, false);
                    }
                    return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Seguir(int id)
        {
            String s = String.Empty;
            if (Request.IsAjaxRequest())
            {
                s = (m.ToogleSeguirEmpresa(mtdb.NumerodeUsuario(User.Identity.Name), id)) ? "No Seguir" : "Seguir";
            }
            return Json(new { ok = true, label = s }, JsonRequestBehavior.AllowGet);
        }
        /*
        public JsonResult CargarMuro(int eid = 0)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(new { ok = true, d = mtdb.Muro(eid, Convert.ToInt32(ConfigurationManager.AppSettings["MT_msgmurohome"])) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }*/
    }
}