using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace MT.Models
{
    /// <summary>
    /// esta clase la uso para guardar los usuarios que están tipeando mensajes
    /// y de paso para saber cuáles son los que están en línea
    /// </summary>
    public static class ActividadDeUsuario
    {
        public static Dictionary<int, bool> tipeando { get; set; }
    }

    public class Conversacion
    {
        public string d { get; set; }
        public int ei { get; set; }
        public string en { get; set; }
        public int i { get; set; }  // id de usuario
        public string m { get; set; } // mensaje
        public int t { get; set; } // fecha en formato <time> de HTML5
        public int ui { get; set; } // label de usuario
        public string un { get; set; } // tipo de mensaje, 1 es notificacion, 3 es mensaje
    }

    public class Mensajes
    {
        #region "propiedades"

        private string[] msj_body = new string[] {
            String.Empty,
            "msj_usr_bienvenida.txt",
            "msj_emp_pendiente.txt",
            "msj_emp_bienvenida.txt",
            "msj_emp_anulada.txt"
        };
        private MTrepoDataContext mtdb = new MTrepoDataContext();
        // en esta clase uso directamente el datacontext
        private MTrepository mtr = new MTrepository();
        private int MT_admemp { get { return Convert.ToInt32(ConfigurationManager.AppSettings["MT_admemp"]); } }
        // constantes
        private int MT_admusr { get { return Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]); } }
        private String MT_sitio { get { return ConfigurationManager.AppSettings["MT_sitio"].ToString(); } }
        private String MT_sitiourl { get { return ConfigurationManager.AppSettings["MT_sitiourl"].ToString(); } }

        #endregion "propiedades"

        #region "Chequeo"

        public int ContarMensajesNuevos(int nro_usuario, out List<int> cnv)
        {
            var ms = (from m in mtdb.MT_MENSAJEs
                      where m.NRO_USUARIO_DESTINO.Equals(nro_usuario) && m.EST_VIGENTE.Equals('S') && m.FEC_LEIDO.Equals(null)
                      select m).ToList();
            cnv = new List<int>();

            foreach (MT_MENSAJE m in ms)
            {
                /*Conversacion c = new Conversacion() {
                    i = m.NRO_MENSAJE,
                    d = m.FEC_PUBLICADO.ToString("o"),
                    m = Varias.LinksHtml(m.XDE_CORTA + "\n" + m.OBS_MEDIA + "\n" + m.MEM_LARGA),
                    ei = (m.NRO_EMPRESA != 0) ? m.NRO_EMPRESA : 0,
                    en = (m.NRO_EMPRESA != 0) ? mtr.PerfildeEmpresa(m.NRO_EMPRESA).XDE_RAZONSOCIAL : String.Empty,
                    ui = (m.NRO_USUARIO != 0) ? m.NRO_USUARIO : 0,
                    un = (m.NRO_USUARIO != 0) ? mtr.NombreyApellidoUsuario(m.NRO_USUARIO) : String.Empty,
                    t = m.NRO_TIPO
                };*/
                cnv.Add(m.NRO_MENSAJE);
            }
            return (ms != null) ? ms.Count() : 0;
        }

        #endregion "Chequeo"

        #region "Creación"

        /// <summary>
        /// para enviar el mensaje de bienvenida solamente necesito el userid y algunas
        /// cosntantes del web.config
        /// </summary>
        /// <param name="userid"></param>
        public void MT_Mensaje_Bienvenida_Usuario(string userid)
        {
            int tipo = 1; // mensaje de bienvenida
            int usr = (from u in mtdb.MT_USUARIOs
                       where u.XDE_USERID.Equals(userid)
                       select u.NRO_USUARIO).SingleOrDefault();
            Dictionary<string, string> vals = new Dictionary<string, string>();
            vals.Add("userid", userid);
            vals.Add("sitio", MT_sitio);
            vals.Add("sitio_url", MT_sitiourl);
            String msj = MensajePersonalizado(tipo, vals);
            MT_Mensaje("Bienvenido!", msj, tipo, MT_admusr, MT_admemp, usr);
        }

        /// <summary>
        /// rutina que recibe el texto en crudo de la plantilla de un mensaje, junto con el diccionario
        /// de claves y valores para reemplazar, y lo devuelve
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        private string MensajePersonalizado(int tipo, Dictionary<string, string> vals)
        {
            String msj = String.Empty;
            TextReader tr = new StreamReader(HttpContext.Current.Server.MapPath("/P/A/" + msj_body[tipo]));
            msj = tr.ReadToEnd();
            tr.Close();
            tr.Dispose();
            foreach (KeyValuePair<string, string> par in vals)
            {
                msj = msj.Replace("%" + par.Key + "%", par.Value);
            }
            return msj;
        }

        /// <summary>
        /// esta rutina crea un nuevo objeto mensaje y lo agrega a la tabla
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="msj"></param>
        /// <param name="tipo"></param>
        /// <param name="num_usuario"></param>
        /// <param name="num_usuario_destino"></param>
        /// <param name="num_empresa"></param>
        private void MT_Mensaje(string titulo, string msj, int tipo, int num_usuario, int num_empresa, int num_usuario_destino, int num_empresa_destino = 0)
        {
            MT_MENSAJE m = new MT_MENSAJE()
            {
                EST_ADMINISTRADOR = 'S',
                EST_VIGENTE = 'S',
                NRO_PADRE = 0,
                NRO_USUARIO = num_usuario,
                NRO_USUARIO_DESTINO = num_usuario_destino,
                NRO_EMPRESA = num_empresa,
                NRO_EMPRESA_DESTINO = num_empresa_destino,
                FEC_PUBLICADO = DateTime.Now,
                NRO_TIPO = tipo,
                XDE_TITULO = titulo,
                MEM_LARGA = msj,
                XDE_CORTA = String.Empty,
                OBS_MEDIA = String.Empty
            };
            mtdb.MT_MENSAJEs.InsertOnSubmit(m);
            mtdb.SubmitChanges();
        }

        #endregion "Creación"

        #region Seguimiento de Empresas

        public Boolean ToogleSeguirEmpresa(int nro_usuario, int nro_empresa)
        {
            Boolean siguiendo = false;
            MT_EMPRESA_SEGUIDORE s = (from r in mtdb.MT_EMPRESA_SEGUIDOREs
                                      where r.NRO_EMPRESA.Equals(nro_empresa) && r.NRO_USUARIO.Equals(nro_usuario)
                                      select r).SingleOrDefault();

            // si existe hay que borrarlo
            if (s != null)
            {
                mtdb.MT_EMPRESA_SEGUIDOREs.DeleteOnSubmit(s);
            }
            else
            {
                s = new MT_EMPRESA_SEGUIDORE()
                {
                    NRO_USUARIO = nro_usuario,
                    NRO_EMPRESA = nro_empresa
                };
                mtdb.MT_EMPRESA_SEGUIDOREs.InsertOnSubmit(s);
                siguiendo = true;
                MT_USUARIO_PERFIL u = (from x in mtdb.MT_USUARIO_PERFILs
                                       where x.NRO_USUARIO.Equals(nro_usuario)
                                       select x).SingleOrDefault();
                MT_EMPRESA_PERFIL e = (from x in mtdb.MT_EMPRESA_PERFILs
                                       where x.NRO_EMPRESA.Equals(nro_empresa)
                                       select x).SingleOrDefault();
                Notificaciones.NotificarGrupo(u, e);
            }
            mtdb.SubmitChanges();
            return siguiendo;
        }

        #endregion Seguimiento de Empresas
    }

    public class MsgMuro
    {
        public DateTime d { get; set; }

        public int i { get; set; }

        public string m { get; set; }

        public string t { get; set; }

        public string x { get; set; }
    }

    public class Remitente
    {
        private MTrepository mtdb = new MTrepository();

        public Remitente(MT_MENSAJE m)
        {
            this.ei = m.NRO_EMPRESA;
            this.en = (mtdb.PerfildeEmpresa(m.NRO_EMPRESA) != null) ? mtdb.PerfildeEmpresa(m.NRO_EMPRESA).XDE_RAZONSOCIAL : String.Empty;
            this.ui = m.NRO_USUARIO;
            this.un = (!String.IsNullOrEmpty(mtdb.NombreyApellidoUsuario(m.NRO_USUARIO))) ? mtdb.NombreyApellidoUsuario(m.NRO_USUARIO) : String.Empty;
            this.on = (ActividadDeUsuario.tipeando != null) ? (ActividadDeUsuario.tipeando.ContainsKey(m.NRO_USUARIO)) ? true : false : false;
            this.mc = mtdb.MensajesSinLeer(m.NRO_USUARIO);
        }

        public int ei { get; set; }

        public string en { get; set; }

        public int mc { get; set; }

        public bool on { get; set; }

        public int ui { get; set; } // id de usuario

        public string un { get; set; } // label de usuario

        // id de empresa

        // label de empresa

        // si está online

        // nro de mensajes sin leer
    }

    public class RespuestaPrivada
    {
        public int msg_id { get; set; }

        public string rep_mensaje { get; set; }
    }
}