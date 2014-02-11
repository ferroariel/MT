using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.RelyingParty;
using MT.Models;

namespace MT.Controllers
{
    public class UsuariosController : Controller
    {
        private static OpenIdRelyingParty openid = new OpenIdRelyingParty();

        private string[] msgs = new string[] {
            "Usuario / Clave incorrecto/s!",
            "Clave actual o nueva incorrecta/s!",
            "El Usuario ya existe. Por favor, use otro.",
            "Ya existe un usuario para esa dirección de email. Por favor, use otra dirección de email.",
            "La clave es incorrecta. Por favor, introduzca una clave válida.",
            "La dirección de email es incorrecta. Por favor, intente nuevamente.",
            "La respuesta a la pregunta de seguridad provista es incorrecta.",
            "La pregunta para recuperar su clave es incorrecta. Por favor, intente nuevamente.",
            "El usuario provisto es incorrecto. Por favor, intente nuevamente.",
            "El sistema devolvió un error. Por favor, intente nuevamente. Si el problema persiste, por favor contáctenos.",
            "El registro de usuario ha sido cancelado. Por favor, intente nuevamente. Si el problema persiste, por favor contáctenos.",
            "Ocurrió un error no especificado. Por favor, intente nuevamente. Si el problema persiste, por favor contáctenos."
        };

        private MTrepository mtdb = new MTrepository();

        private SitioDeEmpresa sitiodeempresa;

        public IFormsAuthenticationService FormsService { get; set; }

        public IMembershipService MembershipService { get; set; }

        #region "autenticacion terceros"

        public ActionResult Authenticate(string returnUrl)
        {
            var response = openid.GetResponse();
            if (response == null)
            {
                // Stage 2: user submitting Identifier
                Identifier id;
                if (Identifier.TryParse(Request.Form["openid_identifier"], out id))
                {
                    // si es LIVE, vamos por otro lado
                    if (id.ToString().Contains("live.com"))
                    {
                        return Redirect("https://login.live.com/oauth20_authorize.srf?client_id=00000000400E961A&display=page&locale=es&redirect_uri=http%3A%2F%2Fwww.mercadotextil.com&response_type=code&scope=wl.signin&state=redirect_type%3Dauth%26display%3Dpage%26request_ts%3D1355834472702%26response_method%3Dcookie%26secure_cookie%3Dfalse");
                    }
                    else
                    {
                        try
                        {
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                            var request = openid.CreateRequest(Request.Form["openid_identifier"]);
                            var fetch = new FetchRequest();
                            fetch.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                            request.AddExtension(fetch);
                            return request.RedirectingResponse.AsActionResult();
                        }
                        catch (ProtocolException ex)
                        {
                            ViewData["Message"] = ex.Message;
                            return View("LogOn");
                        }
                    }
                }
            }

            // Stage 3: OpenID Provider sending assertion response
            string u_email = "";    // email del usuario

            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    MembershipUser user = Membership.GetUser(response.ClaimedIdentifier);
                    if (user == null)
                    {
                        var fetch = response.GetExtension<FetchResponse>();

                        switch (Request.Params["openid_provider"])
                        {
                            case "live":

                                //WindowsLiveIdController w = new WindowsLiveIdController();
                                //Redirect("https://login.live.com/wlogin.srf?appid=00000000400E961A&alg=wsignin1.0&lc=1033");
                                if (fetch != null)
                                {
                                    u_email = fetch.GetAttributeValue(WellKnownAttributes.Contact.Email);
                                }
                                break;

                            case "google":
                                /*if (Request.Params["openid.ext1.value.alias1"] != null)
                                    u_email = Request.Params["openid.ext1.value.alias1"];*/
                                if (fetch != null)
                                {
                                    u_email = fetch.GetAttributeValue(WellKnownAttributes.Contact.Email);
                                }
                                break;

                            case "yahoo":
                                /*if (Request.Params["openid.ax.value.email"] != null)
                                    u_email = Request.Params["openid.ext1.value.alias1"];
                                */
                                if (fetch != null)
                                {
                                    u_email = fetch.GetAttributeValue(WellKnownAttributes.Contact.Email);
                                }
                                break;
                        }

                        Guid u_guid = Guid.NewGuid();   //<-- clave única

                        if (u_email.Length > 0)
                        {
                            if (Membership.GetUser(u_email) != null)
                            {
                                FormsAuthentication.SetAuthCookie(u_email, false);
                                int k = mtdb.NumerodeUsuario(u_email);
                            }
                            else
                            {
                                // Attempt to register the user
                                MembershipCreateStatus createStatus;
                                Membership.CreateUser(u_email, "000000", u_email, null, null, true, null, out createStatus);

                                if (createStatus == MembershipCreateStatus.Success)
                                {
                                    FormsAuthentication.SetAuthCookie(u_email, false /* createPersistentCookie */);
                                    Mensajes m = new Mensajes();
                                    m.MT_Mensaje_Bienvenida_Usuario(u_email);
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                        else
                        {
                            ViewData["Message"] = "El proveedor no ha devuelto un usuario válido";
                            return View("LogOn");
                        }
                    }
                    FormsAuthentication.SetAuthCookie(u_email, false);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    TempData["Message"] = "Felicitaciones! Se ha logueado con éxito";
                    /*ViewData["Message"] = "Felicitaciones! Se ha logueado con éxito";*/
                    return RedirectToAction("Index", "Home");
            }

            return new EmptyResult();
        }

        #endregion "autenticacion terceros"

        [Authorize]
        [HttpPost]
        public ActionResult AgregarFoto(FormCollection formcollection)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null)
                {
                    try
                    {
                        Directory.CreateDirectory(Server.MapPath(String.Format("/P/E/{0}", formcollection["NRO_EMPRESA"])));
                        string newFile = Path.GetFileName(file.FileName);
                        file.SaveAs(Server.MapPath(String.Format("/P/E/{0}/{1}", formcollection["NRO_EMPRESA"], newFile)));
                        MT_RECURSO rec = new MT_RECURSO()
                        {
                            NRO_EMPRESA = Convert.ToInt32(formcollection["NRO_EMPRESA"]),
                            NRO_USUARIO = Convert.ToInt32(formcollection["NRO_USUARIO"]),
                            NRO_GALERIA = Convert.ToInt32(formcollection["NRO_GALERIA"]),
                            NRO_ORDEN = 0,
                            NRO_TIPO = 1,
                            XDE_ARCHIVO = newFile
                        };
                        mtdb.NuevoRecurso(rec);
                        mtdb.Save();

                        // notificar a los usuarios que siguen a esta empresa
                        Notificaciones.NotificarGrupo(rec);
                    }
                    catch (Exception ex)
                    {
                        string l = ex.Message;
                    }
                }
            }
            return RedirectToAction("Perfil");
        }

        [Authorize]
        [HttpPost]
        public ActionResult AgregarTagDeEmpresa(FormCollection f)
        {
            int rid = 0;
            int eid = 0;
            if (int.TryParse(f["NRO_RUBRO"], out rid) && int.TryParse(f["NRO_EMPRESA"], out eid))
            {
                mtdb.AgregarTagaEmpresa(eid, rid);
                return RedirectToAction("Empresa", new { id = eid });
            }
            else
            {
                return RedirectToAction("Perfil");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AgregarTelefono(MT_TELEFONO model)
        {
            model.NRO_USUARIO = mtdb.NumerodeUsuario(User.Identity.Name);
            TryUpdateModel(model);
            mtdb.NuevoTelefono(model);
            return RedirectToAction("Empresa", new { id = model.NRO_EMPRESA });
        }

        [Authorize]
        public ActionResult AvatarU()
        {
            return Content(mtdb.NombreyApellidoUsuario(mtdb.NumerodeUsuario(User.Identity.Name)));
        }

        [Authorize]
        public ActionResult BorrarEmpresa(int id)
        {
            mtdb.BorrarTodoDeEmpresa(User.Identity.Name, id);
            return RedirectToAction("Perfil");
        }

        [Authorize]
        public JsonResult BorrarFoto(int id)
        {
            int emp_id = 0;
            try
            {
                mtdb.BorrarFoto(id, ref emp_id);
                return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult BorrarTagDeEmpresa(FormCollection formcollection)
        {
            int k = mtdb.BorrarTagDeEmpresa(Convert.ToInt32(formcollection["NRO_EMPRESA"]), Convert.ToInt32(formcollection["NRO_RUBRO_ABORRAR"]));
            return RedirectToAction("Empresa", new { id = k });
        }

        [Authorize]
        public ActionResult CrearEmpresaPerfil()
        {
            return View();
        }

        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CrearEmpresaPerfil(FormCollection formcollection)
        {
            MT_EMPRESA_PERFIL m = new MT_EMPRESA_PERFIL()
            {
                MEM_LARGA = String.Empty,
                NRO_EMPRESA = 0,
                NRO_PAIS = 0,
                NRO_PROVINCIA = 0,
                OBS_MEDIA = String.Empty,
                XDE_CIUDAD = String.Empty,
                XDE_CORTA = String.Empty,
                XDE_CP = String.Empty,
                XDE_RAZONSOCIAL = formcollection["XDE_RAZONSOCIAL"].ToString(),
                NRO_EMPRESA_PERFIL = 0,
                XDE_DOMICILIO = String.Empty
            };
            MT_EMPRESA_PERFIL ne = mtdb.AgregarEmpresa(mtdb.NumerodeUsuario(User.Identity.Name), m);
            return RedirectToAction("Empresa", new { id = ne.NRO_EMPRESA });
        }

        [Authorize]
        public ActionResult Empresa(int? id)
        {
            if (id.HasValue)
            {
                MT_EMPRESA_PERFIL ep = mtdb.PerfildeEmpresa(id.Value);
                List<SelectListItem> items = new List<SelectListItem>();
                List<SelectListItem> itms = new List<SelectListItem>();
                List<MT_PAISE> paises = mtdb.cargarPaises();
                List<MT_PROVINCIA> provincias = mtdb.cargarProvincias(ep.NRO_PAIS);
                foreach (MT_PAISE pais in paises)
                {
                    bool s = (pais.NRO_PAIS.Equals(ep.NRO_PAIS)) ? true : false;
                    items.Add(new SelectListItem { Text = pais.XDE_PAIS, Value = pais.NRO_PAIS.ToString(), Selected = s });
                }
                ViewBag.ComboPaises = items;
                foreach (MT_PROVINCIA prov in provincias)
                {
                    bool s = (prov.NRO_PROVINCIA.Equals(ep.NRO_PROVINCIA)) ? true : false;
                    itms.Add(new SelectListItem { Text = prov.XDE_PROVINCIA, Value = prov.NRO_PROVINCIA.ToString(), Selected = s });
                }
                ViewBag.ComboProvincias = itms;
                ViewBag.EmpresaFotos = mtdb.EmpresaFotosProductos(id.Value);
                ViewBag.EmpresaDescargas = mtdb.EmpresaDescargas(id.Value);
                ViewBag.Telefonos = mtdb.EmpresaTelefonos(id.Value).AsQueryable();
                int n = mtdb.NumerodeUsuario(User.Identity.Name);
                ViewBag.NroUsuario = n;
                List<MT_TAG> t = new List<MT_TAG>();
                mtdb.TagsdeEmpresa(id.Value, out t);
                ViewBag.TagsdeEmpresa = t;
                ViewBag.Logo = mtdb.LogoDeEmpresa(ep.NRO_EMPRESA, n);
                ViewBag.FotoGrande = mtdb.EmpresaFotoGrande(ep.NRO_EMPRESA, n);
                ViewBag.Empleos = mtdb.EmpresaEmpleos(id.Value);
                ViewBag.Muro = mtdb.Muro(id.Value);
                return View(ep);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Empresa(MT_EMPRESA_PERFIL model)
        {
            MT_EMPRESA_PERFIL ep = mtdb.PerfildeEmpresa(model.NRO_EMPRESA);
            if (ModelState.IsValid & TryUpdateModel(model))
            {
                UpdateModel(ep);
                mtdb.Save();
                if (!String.IsNullOrEmpty(ep.XDE_WEB))
                {
                    sitiodeempresa = new SitioDeEmpresa()
                    {
                        NRO_EMPRESA = ep.NRO_EMPRESA,
                        ROOT_PATH = Server.MapPath("/"),
                        SITE_URL = (!ep.XDE_WEB.Contains("http://")) ? new Uri(String.Format("http://{0}", ep.XDE_WEB)) : new Uri(ep.XDE_WEB)
                    };
                    Thread NewTh = new Thread(ScreenshotDeSitio);
                    NewTh.SetApartmentState(ApartmentState.STA);
                    NewTh.Start();
                }
                LuceneSearch.AddUpdateLuceneIndex(ep);
                LuceneSearch.Optimize();

                return RedirectToAction("Empresa", new { id = model.NRO_EMPRESA });
            }
            else
            {
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Empresa_Logo(FormCollection formcollection)
        {
            MT_RECURSO r = null;
            var file = Request.Files[0];
            if (file != null)
            {
                if (file.ContentLength > 512000)
                {
                    ModelState.AddModelError(string.Empty, "Por favor, use archivos de hasta 500 Kb");
                }
                bool IsJpeg = file.ContentType == "image/jpeg";
                bool IsPng = file.ContentType == "image/png";
                bool IsGif = file.ContentType == "image/gif";
                if (!IsJpeg && !IsPng && !IsGif)
                {
                    ModelState.AddModelError(string.Empty, "Use archivos .jpg, .gif y .png solamente");
                }
                if (file == null || file.ContentLength <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Debe elegir una imagen primero");
                }
                else
                {
                    string newFile = Path.GetFileName(file.FileName);
                    if (!String.IsNullOrEmpty(formcollection["a.NRO_RECURSO"]) && !formcollection["a.NRO_RECURSO"].Equals("0"))
                    {
                        r = mtdb.Recurso(Convert.ToInt32(formcollection["a.NRO_RECURSO"]));
                        r.XDE_ARCHIVO = newFile;
                        bool b = mtdb.Save();
                    }
                    else
                    {
                        r = new MT_RECURSO()
                        {
                            NRO_EMPRESA = Convert.ToInt32(formcollection["a.NRO_EMPRESA"]),
                            NRO_GALERIA = Convert.ToInt32(formcollection["a.NRO_GALERIA"]),
                            NRO_USUARIO = mtdb.NumerodeUsuario(User.Identity.Name),
                            NRO_ORDEN = Convert.ToInt32(formcollection["a.NRO_ORDEN"]),
                            NRO_TIPO = 10,
                            XDE_ARCHIVO = newFile
                        };
                        mtdb.NuevoRecurso(r);

                        // notificar a los usuarios que siguen a esta empresa
                        Notificaciones.NotificarGrupo(r);
                    }
                    try
                    {
                        Directory.CreateDirectory(Server.MapPath(String.Format("/P/E/{0}", r.NRO_EMPRESA)));
                        file.SaveAs(Server.MapPath(String.Format("/P/E/{0}/{1}", r.NRO_EMPRESA, newFile)));
                    }
                    catch (Exception ex)
                    {
                        string n = ex.Message;
                    }
                }
            }
            return RedirectToAction("Perfil");
        }

        /// <summary>
        /// Devuelve todos los tags, menos los ya usados por une empresa
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="emp_id"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Tags(string modo, int emp_id)
        {
            int[] usados = { };
            usados = mtdb.TagsdeEmpresa(emp_id);
            IQueryable<MT_TAG> res = mtdb.Tags(-1, usados);
            bool success = (res.Count() > 0) ? true : false;
            return Json(new { success, res }, JsonRequestBehavior.AllowGet);
        }

        #region "Usuarios"

        [Authorize]
        public ActionResult Empresas()
        {
            List<MT_EMPRESA_PERFIL> es = mtdb.PerfilesDeEmpresasDeUsuario(User.Identity.Name);
            return View(es);
        }

        public ActionResult Ingreso()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ingreso(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    int k = mtdb.NumerodeUsuario(model.UserName);
                    if (ActividadDeUsuario.tipeando == null)
                    {
                        ActividadDeUsuario.tipeando = new Dictionary<int, bool>();
                    }
                    if (!ActividadDeUsuario.tipeando.ContainsKey(k))
                    {
                        ActividadDeUsuario.tipeando.Add(k, false);
                    }

                    if (!String.IsNullOrEmpty(returnUrl) && /*Url.IsLocalUrl(returnUrl) &&*/ returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        /*&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")*/)
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Perfil", "Usuarios");
                    }
                }
                else
                {
                    ModelState.AddModelError("", msgs[0]);
                }
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Perfil()
        {
            DatosdeUsuario ud = mtdb.PerfilCompletodeUsuario(User.Identity.Name);
            List<SelectListItem> items = new List<SelectListItem>();
            List<SelectListItem> itms = new List<SelectListItem>();
            List<MT_PAISE> paises = mtdb.cargarPaises();
            List<MT_PROVINCIA> provincias = mtdb.cargarProvincias(ud.perfil.NRO_PAIS);
            foreach (MT_PAISE pais in paises)
            {
                bool s = (pais.NRO_PAIS.Equals(ud.perfil.NRO_PAIS)) ? true : false;
                items.Add(new SelectListItem { Text = pais.XDE_PAIS, Value = pais.NRO_PAIS.ToString(), Selected = s });
            }
            ViewBag.ComboPaises = items;
            foreach (MT_PROVINCIA prov in provincias)
            {
                bool s = (prov.NRO_PROVINCIA.Equals(ud.perfil.NRO_PROVINCIA)) ? true : false;
                itms.Add(new SelectListItem { Text = prov.XDE_PROVINCIA, Value = prov.NRO_PROVINCIA.ToString(), Selected = s });
            }
            ViewBag.ComboProvincias = itms;
            return View(ud);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Perfil(DatosdeUsuario model)
        {
            MT_USUARIO_PERFIL up = mtdb.PerfildeUsuario(model.perfil.NRO_USUARIO);
            up.XDE_CIUDAD = model.perfil.XDE_CIUDAD + string.Empty;
            up.XDE_CP = model.perfil.XDE_CP + String.Empty;
            up.XDE_FACEBOOK = model.perfil.XDE_FACEBOOK + String.Empty;
            up.XDE_TWITTER = model.perfil.XDE_TWITTER + String.Empty;
            up.XDE_WEB = model.perfil.XDE_WEB + String.Empty;
            up.OBS_DOMICILIO = model.perfil.OBS_DOMICILIO + String.Empty;
            up.NRO_PAIS = model.perfil.NRO_PAIS + 0;
            up.NRO_PROVINCIA = model.perfil.NRO_PROVINCIA + 0;
            up.FEC_NACIMIENTO = model.perfil.FEC_NACIMIENTO;
            up.XDE_NOMBRES = model.perfil.XDE_NOMBRES + String.Empty;
            up.NRO_USUARIO = model.perfil.NRO_USUARIO + 0;
            up.XDE_APELLIDOS = model.perfil.XDE_APELLIDOS + String.Empty;
            TryUpdateModel(up);
            bool b = mtdb.Save();
            return RedirectToAction("Perfil");
        }

        /// <summary>
        /// Recibe una lista de emails para actualizar el usuario actual
        /// No pude pasar el modelo, siempre me da nulo, asi que tuve que
        /// pasar el form como una coleccion y maparealo a mano al objeto MT_EMAILS
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Perfil_Emails(FormCollection formcollection)
        {
            // mapeo a manopla
            List<MT_EMAIL> emails = new List<MT_EMAIL>();
            int i = 0;
            foreach (var _key in formcollection.Keys)
            {
                if (!String.IsNullOrEmpty(formcollection["emails[" + i.ToString() + "].XDE_EMAIL"]))
                {
                    MT_EMAIL email = new MT_EMAIL()
                    {
                        XDE_EMAIL = formcollection["emails[" + i.ToString() + "].XDE_EMAIL"],
                        NRO_USUARIO = Convert.ToInt32(formcollection["emails[" + i.ToString() + "].NRO_USUARIO"]),
                        NRO_EMAIL = Convert.ToInt32(formcollection["emails[" + i.ToString() + "].NRO_EMAIL"]),
                        NRO_EMPRESA = Convert.ToInt32(formcollection["emails[" + i.ToString() + "].NRO_EMPRESA"])
                    };
                    emails.Add(email);
                    i++;
                }
            }
            if (emails.Count > 0)
            {
                int num_usuario = mtdb.NumerodeUsuario(User.Identity.Name);
                mtdb.ActualizarEmails(num_usuario, emails);
            }
            if (!String.IsNullOrEmpty(formcollection["nuevoemail"]))
            {
                MT_EMAIL nuevo = new MT_EMAIL()
                {
                    XDE_EMAIL = formcollection["nuevoemail"].ToString(),
                    NRO_EMPRESA = 0,
                    NRO_USUARIO = mtdb.NumerodeUsuario(User.Identity.Name)
                };
                mtdb.NuevoEmail(nuevo);
            }
            return RedirectToAction("Perfil");
        }

        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("Captcha is not valid")]
        public ActionResult Registro(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.UserName, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    Mensajes m = new Mensajes();
                    m.MT_Mensaje_Bienvenida_Usuario(model.UserName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Salir()
        {
            FormsAuthentication.SignOut();
            if (ActividadDeUsuario.tipeando != null)
            {
                int k = mtdb.NumerodeUsuario(User.Identity.Name);
                if (ActividadDeUsuario.tipeando.ContainsKey(k))
                    ActividadDeUsuario.tipeando.Remove(k);
            }
            return RedirectToAction("Ingreso", "Usuarios");
        }

        #endregion "Usuarios"

        #region "adicionales"

        [Authorize]
        public ActionResult CambiarClave()
        {
            //ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CambiarClave(ChangePasswordModel model)
        {
            TempUserData tempuser = new TempUserData();
            tempuser = (TempUserData)Session["TempUser"];
            model.OldPassword = tempuser.UserTempPass;
            try
            {
                AccountMembershipService c = new AccountMembershipService();
                if (c.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("CambiarClaveOk");
                }
            }
            catch { }
            tempuser = null;
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        public ActionResult CambiarClaveOk()
        {
            if (Session["TempUser"] != null) { Session.Contents.Remove("TempUser"); };
            return View();
        }

        [HttpGet]
        public ActionResult NuevaClave(ResetPasswordModel model)
        {
            try
            {
                AccountMembershipService c = new AccountMembershipService();
                TempUserData userdata = new TempUserData()
                {
                    UserResetCode = model.ResetCode
                };
                c.GetUsernameFromResetCode(ref userdata, Server.MapPath("/P/U/T/"));
                MembershipUser user = Membership.GetUser(userdata.UserName);
                if (user != null)
                {
                    string newPassword = userdata.UserTempPass;

                    // grabamos la clave en el repositorio

                    if (mtdb.CambiarClave(mtdb.NumerodeUsuario(userdata.UserName), userdata.UserTempPass))
                    {
                        LogOnModel logon = new LogOnModel()
                        {
                            Password = userdata.UserTempPass,
                            RememberMe = false,
                            UserName = user.ToString()
                        };
                        if (c.ValidateUser(logon.UserName, logon.Password))
                        {
                            //FormsService.SignIn(logon.UserName, logon.RememberMe);
                            FormsAuthentication.SetAuthCookie(logon.UserName, true);
                            Session.Contents.Add("TempUser", userdata);
                            return RedirectToAction("CambiarClave");
                        }
                    }
                }
                c = null;
                userdata = null;
            }
            catch (Exception ex)
            {
                string b = ex.Message;
            }
            return View("Error");
        }

        public ActionResult ResetearClave()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetearClave(ForgotPasswordModel model)
        {
            string username = String.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    MembershipUser user = null;
                    if (Session["ValidateWithUsername"] != null)
                    {
                        user = Membership.GetUser(Request.Form["_MUsername"]);
                    }
                    else
                    {
                        //username = Membership.GetUserNameByEmail(model.Email);
                        username = model.Email;
                        if (username != null)
                        {
                            user = Membership.GetUser(username);
                        }
                    }
                    if (user != null)
                    {
                        AccountMembershipService c = new AccountMembershipService();
                        String newresetcode = System.Guid.NewGuid().ToString();
                        String newpassword = Membership.GeneratePassword(8, 2);
                        c.SetUsernameFromResetCode(newresetcode, user.UserName, newpassword, Server.MapPath("/P/U/T/"));
                        c = null;

                        if (model.Email.ToLower() == user.Email.ToLower())
                        {
                            StreamReader s;
                            s = System.IO.File.OpenText(Server.MapPath("/P/A/msj_usr_resetoclave.txt"));
                            String body = s.ReadToEnd();
                            s = null;

                            body = body.Replace("%userid%", user.UserName);
                            body = body.Replace("%sitio_url%", ConfigurationManager.AppSettings["MT_sitiourl"].ToString());
                            body = body.Replace("%sitio%", ConfigurationManager.AppSettings["MT_sitio"].ToString());
                            string eurl = String.Format("http://{0}:{1}/Usuarios/NuevaClave/?ResetCode={2}", Request.Url.Host, Request.Url.Port, newresetcode);
                            body = body.Replace("%reset_pass_url%", eurl);

                            Mail m = new Mail();
                            int res = m.doMail("Datos de Membresía", user.Email, body);
                            m = null;

                            if (Session["ValidateWithUsername"] != null)
                            {
                                Session.Contents.Remove("ValidateWithUsername");
                            }

                            return RedirectToAction("ResetearClaveOk");
                        }
                    }
                }
                catch { }
            }
            return View(model);
        }

        public ActionResult ResetearClaveOk()
        {
            return View();
        }

        private void ScreenshotDeSitio()
        {
            LabelImage thumb;
            Bitmap x;
            try
            {
                thumb = new LabelImage(sitiodeempresa);
                x = thumb.GetBitmap();
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                ImageCodecInfo jpegCodec = mtdb.getEncoderInfo("image/jpeg");
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;
                Directory.CreateDirectory(String.Format(@"{0}\P\E\{1}", sitiodeempresa.ROOT_PATH, sitiodeempresa.NRO_EMPRESA));
                x.Save(String.Format(@"{0}\P\E\{1}\sitio.jpg", sitiodeempresa.ROOT_PATH, sitiodeempresa.NRO_EMPRESA), jpegCodec, encoderParams);
            }
            catch
            { }
            finally
            {
                x = null;
                thumb = null;
            }
        }

        #endregion "adicionales"

        #region Services

        // El tipo FormsAuthentication está sellado y contiene miembros estáticos, por lo que es difícil
        // realizar pruebas unitarias en el código que llama a sus miembros. La interfaz y la clase auxiliar siguientes muestran
        // cómo crear un contenedor abstracto en torno a un tipo como este para que puedan realizarse pruebas unitarias en el código de AccountController
        // .

        public interface IFormsAuthenticationService
        {
            void SignIn(string userName, bool createPersistentCookie);

            void SignOut();
        }

        public interface IMembershipService
        {
            int MinPasswordLength { get; }

            bool ChangePassword(string userName, string oldPassword, string newPassword);

            MembershipCreateStatus CreateUser(string userName, string password, string email);

            bool ValidateUser(string userName, string password);
        }

        public class AccountMembershipService : IMembershipService
        {
            private readonly MembershipProvider _provider;

            public AccountMembershipService()
                : this(null)
            {
            }

            public AccountMembershipService(MembershipProvider provider)
            {
                _provider = provider ?? Membership.Provider;
            }

            public int MinPasswordLength
            {
                get
                {
                    return _provider.MinRequiredPasswordLength;
                }
            }

            public bool ChangePassword(string userName, string oldPassword, string newPassword)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");
                if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "oldPassword");
                if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "newPassword");

                // El elemento ChangePassword() subyacente iniciará una excepción en lugar de
                // devolver false en determinados escenarios de error.
                try
                {
                    MembershipUser currentUser = _provider.GetUser(userName, true);

                    Varias v = new Varias();
                    MTrepository userRep = new MTrepository();
                    MT_USUARIO user = userRep.GetAllUsers().SingleOrDefault
                            (u => u.XDE_USERID.Equals(userName));
                    user.XDE_CLAVE = v.HashClave(newPassword);
                    return userRep.Save();

                    //return currentUser.ChangePassword(oldPassword, newPassword);
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (MembershipPasswordException)
                {
                    return false;
                }
            }

            public MembershipCreateStatus CreateUser(string userName, string password, string email)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");
                if (String.IsNullOrEmpty(password)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "password");
                if (String.IsNullOrEmpty(email)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "email");

                MembershipCreateStatus status;
                _provider.CreateUser(userName, password, email, null, null, true, null, out status);
                return status;
            }

            public void GetUsernameFromResetCode(ref TempUserData ud, string path)
            {
                TextReader tw;
                try
                {
                    tw = new StreamReader(Path.Combine(path, ud.UserResetCode + ".txt"));
                    ud.UserName = tw.ReadLine();
                    ud.UserTempPass = tw.ReadLine();
                    tw.Close();
                    tw.Dispose();

                    //File.Delete(String.Format(@"/P{0}\Usuarios\TempData\{1}.txt", ConfigurationManager.AppSettings["SitePath"].ToString(), ud.UserResetCode));
                }
                catch (Exception ex)
                {
                    string exm = ex.Message;
                }
                finally
                {
                    tw = null;
                }
            }

            public void SetUsernameFromResetCode(string resetcode, string username, string userpass, string path)
            {
                if (resetcode.Length > 1)
                {
                    TextWriter tw;
                    try
                    {
                        tw = new StreamWriter(Path.Combine(path, resetcode + ".txt"));
                        tw.WriteLine(username);
                        tw.WriteLine(userpass);
                        tw.Close();
                        tw.Dispose();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        tw = null;
                    }
                }
            }

            public bool ValidateUser(string userName, string password)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");
                if (String.IsNullOrEmpty(password)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "password");
                try
                {
                    return _provider.ValidateUser(userName, password);
                }
                catch
                {
                    return false;
                }
            }
        }

        public class FormsAuthenticationService : IFormsAuthenticationService
        {
            public void SignIn(string userName, bool createPersistentCookie)
            {
                if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");

                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            }

            public void SignOut()
            {
                FormsAuthentication.SignOut();
            }
        }

        #endregion Services

        #region Status Codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.

            UsuariosController c = new UsuariosController();

            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return c.msgs[2];

                case MembershipCreateStatus.DuplicateEmail:
                    return c.msgs[3];

                case MembershipCreateStatus.InvalidPassword:
                    return c.msgs[4];

                case MembershipCreateStatus.InvalidEmail:
                    return c.msgs[5];

                case MembershipCreateStatus.InvalidAnswer:
                    return c.msgs[6];

                case MembershipCreateStatus.InvalidQuestion:
                    return c.msgs[7];

                case MembershipCreateStatus.InvalidUserName:
                    return c.msgs[8];

                case MembershipCreateStatus.ProviderError:
                    return c.msgs[9];

                case MembershipCreateStatus.UserRejected:
                    return c.msgs[10];

                default:
                    return c.msgs[11];
            }
        }

        #endregion Status Codes
    }
}