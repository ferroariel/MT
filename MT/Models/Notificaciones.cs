using System;
using System.Collections.Generic;
using System.Configuration;

namespace MT.Models
{
    public class Notificaciones
    {
        private static string[] ntf = {
                             "<a href=\"/Directorio/Empresa/{1}\">{0}</a> agregó una nueva foto en su perfil.<br /><img src=\"/P/E/{2}/{3}\" /><br />",
                             "<a href=\"/Directorio/Empresa/{1}\">{0}</a> actualizó su perfil.<br />",
                             "<a href=\"/Directorio/Empresa/{1}\">{0}</a> actualizó su logo.<br /><img src=\"/P/E/{2}/{3}\" /><br />",
                             "<img src=\"/Recursos/Avatar?uid={3}\" class=\"th\" /><b>{2}</b> es seguidor de <b><a href=\"/Directorio/Empresa/{0}\">{1}</a></b><br />",
                             "La empresa <b>{0}</b> agregó una nueva oferta de empleo: &ldquo;<a href=\"/Empleo/Oferta/{1}\" target=_blank>{2}</a>&bdquo;.<br />",
                             "<a href=\"/Directorio/Empresa/{1}\">{0}</a> agregó un archivo para descargar.<br /><a href=\"/Recursos/Descarga/?eid={2}&rid={3}\"><img src=\"/Content/img/document.png\" />&nbsp;{4}</a><br />",
                             "<a href=\"/Directorio/Empresa/{1}\">{0}</a> se ha unido a <b>{2}</b><br />"
                              };

        // en la mayoría de los casos, se agrega al usuario administrador
        // para que la notificación se visible en los muros
        public static void NotificarGrupo(MT_RECURSO r)
        {
            MTrepository mtdb = new MTrepository();
            int emp_id = Convert.ToInt32(r.NRO_EMPRESA);
            MT_EMPRESA_PERFIL e = mtdb.PerfildeEmpresa(emp_id);
            String s = String.Empty;
            if (r.NRO_TIPO.Equals(10))
            {
                s = ntf[2];
                s = String.Format(s, e.XDE_RAZONSOCIAL, Varias.mt_Escape(e.XDE_RAZONSOCIAL), emp_id, r.XDE_ARCHIVO);
            }
            else if (r.NRO_TIPO.Equals(23))
            {
                s = ntf[5];
                s = String.Format(s, e.XDE_RAZONSOCIAL, Varias.mt_Escape(e.XDE_RAZONSOCIAL), emp_id, r.NRO_RECURSO, r.XDE_ARCHIVO);
            }
            else
            {
                s = ntf[0];
                s = String.Format(s, e.XDE_RAZONSOCIAL, Varias.mt_Escape(e.XDE_RAZONSOCIAL), emp_id, r.XDE_ARCHIVO, r.NRO_RECURSO);
            }
            List<MT_EMPRESA_SEGUIDORE> gs = mtdb.SeguidoresDeEmpresa(emp_id);
            MT_EMPRESA_SEGUIDORE n = new MT_EMPRESA_SEGUIDORE()
            {
                NRO_EMPRESA = emp_id,
                NRO_USUARIO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"])
            };
            gs.Add(n);
            foreach (MT_EMPRESA_SEGUIDORE g in gs)
            {
                MT_MENSAJE m = new MT_MENSAJE()
                {
                    EST_ADMINISTRADOR = 'S',
                    EST_VIGENTE = 'S',
                    FEC_PUBLICADO = DateTime.Now,
                    MEM_LARGA = s,
                    NRO_EMPRESA = emp_id,
                    NRO_EMPRESA_DESTINO = 0,
                    NRO_PADRE = 0,
                    NRO_USUARIO = 0,
                    NRO_TIPO = 1,
                    NRO_USUARIO_DESTINO = g.NRO_USUARIO
                };
                mtdb.NuevoMensaje(m);
            }
        }

        public static void NotificarGrupo(MT_EMPRESA e)
        {
            MTrepository mtdb = new MTrepository();
            int emp_id = e.NRO_EMPRESA;
            MT_EMPRESA_PERFIL emp_pf = mtdb.PerfildeEmpresa(emp_id);
            String s = String.Format(ntf[6], emp_pf.XDE_RAZONSOCIAL, Varias.mt_Escape(emp_pf.XDE_RAZONSOCIAL), ConfigurationManager.AppSettings["MT_sitio"]);
            MT_MENSAJE m = new MT_MENSAJE()
            {
                EST_ADMINISTRADOR = 'S',
                EST_VIGENTE = 'S',
                FEC_PUBLICADO = DateTime.Now,
                MEM_LARGA = s,
                NRO_EMPRESA = emp_id,
                NRO_EMPRESA_DESTINO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admemp"]),
                NRO_PADRE = 0,
                NRO_USUARIO = 0,
                NRO_TIPO = 1,
                NRO_USUARIO_DESTINO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]),
            };
            mtdb.NuevoMensaje(m);
        }

        public static void NotificarGrupo(MT_EMPRESA_PERFIL e)
        {
            MTrepository mtdb = new MTrepository();
            int emp_id = Convert.ToInt32(e.NRO_EMPRESA);
            String s = String.Format(ntf[1], e.XDE_RAZONSOCIAL, Varias.mt_Escape(e.XDE_RAZONSOCIAL));
            List<MT_EMPRESA_SEGUIDORE> gs = mtdb.SeguidoresDeEmpresa(emp_id);
            MT_EMPRESA_SEGUIDORE n = new MT_EMPRESA_SEGUIDORE()
            {
                NRO_EMPRESA = emp_id,
                NRO_USUARIO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"])
            };
            gs.Add(n);
            foreach (MT_EMPRESA_SEGUIDORE g in gs)
            {
                MT_MENSAJE m = new MT_MENSAJE()
                {
                    EST_ADMINISTRADOR = 'S',
                    EST_VIGENTE = 'S',
                    FEC_PUBLICADO = DateTime.Now,
                    MEM_LARGA = s,
                    NRO_EMPRESA = emp_id,
                    NRO_EMPRESA_DESTINO = 0,
                    NRO_PADRE = 0,
                    NRO_USUARIO = 0,
                    NRO_TIPO = 1,
                    NRO_USUARIO_DESTINO = g.NRO_USUARIO
                };
                mtdb.NuevoMensaje(m);
            }
        }

        public static void NotificarGrupo(MT_USUARIO_PERFIL u, MT_EMPRESA_PERFIL e)
        {
            MTrepository mtdb = new MTrepository();
            int emp_id = Convert.ToInt32(e.NRO_EMPRESA);
            String s = String.Format(ntf[3],
                Varias.mt_Escape(e.XDE_RAZONSOCIAL),
                e.XDE_RAZONSOCIAL,
                mtdb.NombreyApellidoUsuario(u.NRO_USUARIO),
                u.NRO_USUARIO,
                25);
            MT_MENSAJE m = new MT_MENSAJE()
            {
                EST_ADMINISTRADOR = 'S',
                EST_VIGENTE = 'S',
                FEC_PUBLICADO = DateTime.Now,
                MEM_LARGA = s,
                NRO_EMPRESA = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admemp"]) /*0*/,
                NRO_EMPRESA_DESTINO = e.NRO_EMPRESA,
                NRO_PADRE = 0,
                NRO_USUARIO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]) /*u.NRO_USUARIO*/,
                NRO_TIPO = 1,
                NRO_USUARIO_DESTINO = mtdb.AutorDeEmpresa(e.NRO_EMPRESA)
            };
            mtdb.NuevoMensaje(m);
        }

        public static void NotificarGrupo(MT_EMPLEO_OFERTA o, MT_EMPRESA_PERFIL e)
        {
            MTrepository mtdb = new MTrepository();
            int emp_id = Convert.ToInt32(e.NRO_EMPRESA);
            String s = String.Format(ntf[4],
                e.XDE_RAZONSOCIAL,
                o.NRO_OFERTA,
                o.XDE_TITULO);
            List<MT_EMPRESA_SEGUIDORE> gs = mtdb.SeguidoresDeEmpresa(e.NRO_EMPRESA);
            MT_EMPRESA_SEGUIDORE n = new MT_EMPRESA_SEGUIDORE()
            {
                NRO_EMPRESA = emp_id,
                NRO_USUARIO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"])
            };
            gs.Add(n);
            foreach (MT_EMPRESA_SEGUIDORE g in gs)
            {
                MT_MENSAJE m = new MT_MENSAJE()
                {
                    EST_ADMINISTRADOR = 'S',
                    EST_VIGENTE = 'S',
                    FEC_PUBLICADO = DateTime.Now,
                    MEM_LARGA = s,
                    NRO_EMPRESA = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admemp"]) /*0*/,
                    NRO_EMPRESA_DESTINO = e.NRO_EMPRESA,
                    NRO_PADRE = 0,
                    NRO_USUARIO = Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"]) /*u.NRO_USUARIO*/,
                    NRO_TIPO = 1,
                    NRO_USUARIO_DESTINO = mtdb.AutorDeEmpresa(e.NRO_EMPRESA)
                };
                mtdb.NuevoMensaje(m);
            }
        }
    }
}