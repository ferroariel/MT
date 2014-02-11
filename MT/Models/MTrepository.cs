
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Transactions;

namespace MT.Models
{
    /// <summary>
    /// http://www.albahari.com/nutshell/predicatebuilder.aspx
    /// </summary>
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }
    }

    /// <summary>
    /// http://nerddinnerbook.s3.amazonaws.com/Part3.htm
    /// </summary>
    public class MTrepository
    {
        private MTrepoDataContext mtdb = new MTrepoDataContext();
        private Random rnd = new Random();
        private SitioDeEmpresa sitiodeempresa;
        private Varias v = new Varias();
        /* = new SitioDeEmpresa();*/

        #region "Directorio"

        public int AutorDeEmpresa(int e)
        {
            return (from k in mtdb.MT_EMPRESAs
                    where k.NRO_EMPRESA.Equals(e)
                    select k.NRO_USUARIO).SingleOrDefault();
        }

        public void BorrarFoto(int id, ref int emp_id)
        {
            MT_RECURSO rec = (from r in mtdb.MT_RECURSOs
                              where r.NRO_RECURSO.Equals(id)
                              select r).SingleOrDefault();
            if (rec != null)
            {
                emp_id = Convert.ToInt32(rec.NRO_EMPRESA);
                FileInfo rc = new FileInfo(HttpContext.Current.Server.MapPath(String.Format("/P/E/{0}/{1}", rec.NRO_EMPRESA, rec.XDE_ARCHIVO)));
                rc.Delete();
                mtdb.MT_RECURSOs.DeleteOnSubmit(rec);
                bool b = Save();
            }
        }

        public IQueryable<MT_RECURSO> EmpresaDescargas(int id, int rid = 0)
        {
            var rc = (from r in mtdb.MT_RECURSOs
                      select r)
                          .AsQueryable()
                          .Where(r => r.NRO_EMPRESA.Equals(id) && r.NRO_TIPO.Equals(23));
            if (rid > 0)
            {
                rc.Where(r => r.NRO_RECURSO.Equals(rid));
            }
            return rc;
        }

        public List<MT_EMAIL> EmpresaEmails(int id)
        {
            var em = (from t in mtdb.MT_EMAILs
                      where t.NRO_EMPRESA.Equals(id)
                      select t).ToList();
            if (em.Count().Equals(0))
            {
                int k = AutorDeEmpresa(id);
                return (from g in mtdb.MT_EMAILs
                        where g.NRO_USUARIO.Equals(k)
                        select g).ToList();
            }
            else
            {
                return em;
            }
        }

        public IQueryable<MT_RECURSO> EmpresaFotos(int id)
        {
            return (from r in mtdb.MT_RECURSOs
                    select r)
                          .AsQueryable()
                          .Where(r => r.NRO_EMPRESA.Equals(id) /*&& r.NRO_TIPO.Equals(1)*/);
        }

        public IQueryable<MT_RECURSO> EmpresaFotosProductos(int id)
        {
            return (from r in mtdb.MT_RECURSOs
                    select r)
                          .AsQueryable()
                          .Where(r => r.NRO_EMPRESA.Equals(id) && r.NRO_TIPO.Equals(19));
        }

        public List<MT_MENSAJE> EmpresaOpiniones(int eid)
        {
            return (from s in mtdb.MT_MENSAJEs
                    where s.NRO_EMPRESA_DESTINO.Equals(eid) && s.NRO_TIPO.Equals(4)
                    select s).ToList();
        }

        public MT_VOTO EmpresaPuntaje(int eid)
        {
            return (from n in mtdb.MT_VOTOs
                    where n.NRO_EMPRESA.Equals(eid)
                    select n).SingleOrDefault();
        }

        public List<MT_TELEFONO> EmpresaTelefonos(int id)
        {
            return (from t in mtdb.MT_TELEFONOs
                    where t.NRO_EMPRESA.Equals(id)
                    select t).ToList();
        }

        public bool EsAutorDeEmpresa(int nro_empresa, int nro_usuario)
        {
            var e = (from n in mtdb.MT_EMPRESAs
                     where n.NRO_EMPRESA.Equals(nro_empresa) && n.NRO_USUARIO.Equals(nro_usuario)
                     select n);
            return (e.Count() > 0) ? true : false;
        }

        public bool EstaSiguiendoEmpresa(int nro_empresa, int nro_usuario)
        {
            int i = (from n in mtdb.MT_EMPRESA_SEGUIDOREs
                     where n.NRO_EMPRESA.Equals(nro_empresa) && n.NRO_USUARIO.Equals(nro_usuario)
                     select n).Count();
            return (i > 0) ? true : false;
        }

        public List<MsgMuro> Muro(int eid = 0, int cont = 0)
        {
            int[] tipos = new int[] { 17, 1 };
            List<MsgMuro> mr;
            if (eid > 0)
            {
                mr = (from m in mtdb.MT_MENSAJEs.AsEnumerable()
                      where m.NRO_EMPRESA.Equals(eid) && tipos.Contains(m.NRO_TIPO) && m.EST_VIGENTE.Equals('S')
                      orderby m.NRO_MENSAJE descending
                      select new MsgMuro
                      {
                          i = m.NRO_MENSAJE,
                          t = m.XDE_TITULO,
                          m = m.MEM_LARGA,
                          d = m.FEC_PUBLICADO,
                          x = m.FEC_PUBLICADO.ToString("o")
                      }).ToList();
            }

            //  sino, tengo que leer los mensajes enviados al usuario administrador
            //  porque se me está pidiendo ver el muro público del home
            else
            {
                mr = (from m in mtdb.MT_MENSAJEs.AsEnumerable()
                      where tipos.Contains(m.NRO_TIPO) && m.NRO_USUARIO_DESTINO.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"])) && m.EST_VIGENTE.Equals('S')
                      orderby m.NRO_MENSAJE descending
                      select new MsgMuro
                      {
                          i = m.NRO_MENSAJE,
                          t = m.XDE_TITULO,
                          m = m.MEM_LARGA,
                          d = m.FEC_PUBLICADO,
                          x = m.FEC_PUBLICADO.ToString("o")
                      }).ToList();
            }
            if (cont > 0)
            {
                mr = mr.Take(cont).ToList();
            }
            return mr;
        }

        public List<MsgMuro> Muro(bool formatURLs, int eid = 0, int cont = 0)
        {
            List<MsgMuro> mr = Muro(eid, cont);
            if (mr.Count() > 0)
            {
                foreach (MsgMuro mm in mr)
                {
                    mm.m = mm.m.Replace("=\"/", "=\"" + ConfigurationManager.AppSettings["MT_sitiourl"] + "/");
                }
            }
            return mr;
        }

        public List<MsgMuro> Muro(bool formatURLs, DateTime desde, int eid = 0, int cont = 0)
        {
            int[] tipos = new int[] { 17, 1 };
            
            List<MsgMuro> mr = (from m in mtdb.MT_MENSAJEs
                                where m.FEC_PUBLICADO > desde && tipos.Contains(m.NRO_TIPO) && m.NRO_USUARIO_DESTINO.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["MT_admusr"])) && m.EST_VIGENTE.Equals('S')
                                select new MsgMuro
                                {
                                    i = m.NRO_MENSAJE,
                                    t = m.XDE_TITULO,
                                    m = m.MEM_LARGA,
                                    d = m.FEC_PUBLICADO,
                                    x = m.FEC_PUBLICADO.ToString("o")
                                }).ToList();
            
            
            
            
            return mr;
        }

        public void NuevoRecurso(MT_RECURSO r)
        {
            mtdb.MT_RECURSOs.InsertOnSubmit(r);
            bool b = Save();
        }

        public String ObtenerPais(int id)
        {
            return (from p in mtdb.MT_PAISEs
                    where p.NRO_PAIS.Equals(id)
                    select p.XDE_PAIS).SingleOrDefault();
        }

        public String ObtenerProvincia(int id)
        {
            return (from p in mtdb.MT_PROVINCIAs
                    where p.NRO_PROVINCIA.Equals(id)
                    select p.XDE_PROVINCIA).SingleOrDefault();
        }

        /// <summary>
        /// http://stackoverflow.com/questions/1698921/linq-to-sql-custom-function?answertab=active#tab-top
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MT_EMPRESA_PERFIL PerfildeEmpresa(string id)
        {
            var mp = (from p in mtdb.MT_EMPRESA_PERFILs
                      select p)
               .AsEnumerable()
               .Where(p => Varias.mt_Escape(p.XDE_RAZONSOCIAL).Equals(id));
            if (mp != null && mp.Count() > 0)
            {
                return mp.First();
            }
            else
            {
                return new MT_EMPRESA_PERFIL();
            }
        }

        public MT_EMPRESA_PERFIL PerfildeEmpresa(int id)
        {
            var mp = (from p in mtdb.MT_EMPRESA_PERFILs
                      select p)
               .AsEnumerable()
               .Where(p => p.NRO_EMPRESA.Equals(id));
            if (mp.Count() > 0)
            {
                var m = mp.First();
                if (!String.IsNullOrEmpty(m.XDE_WEB))
                {
                    m.XDE_WEB = (m.XDE_WEB.Contains("http://")) ? m.XDE_WEB.Replace("http://", String.Empty) : m.XDE_WEB;
                }
                return m;
            }
            else
            {
                return null;
            }
        }

        public List<MT_EMPRESA_PERFIL> PerfilesEmpresas()
        {
            return (from r in mtdb.MT_EMPRESA_PERFILs
                    select r).Take(10).ToList();
        }

        public List<MT_EMPRESA_SEGUIDORE> SeguidoresDeEmpresa(string nombre)
        {
            if (!String.IsNullOrEmpty(nombre))
            {
                return SeguidoresDeEmpresa((from o in mtdb.MT_EMPRESA_PERFILs
                                            where o.XDE_RAZONSOCIAL.Equals(nombre)
                                            select o.NRO_EMPRESA).SingleOrDefault());
            }
            else
            {
                return new List<MT_EMPRESA_SEGUIDORE>();
            }
        }

        public List<MT_EMPRESA_SEGUIDORE> SeguidoresDeEmpresa(int e)
        {
            return (from o in mtdb.MT_EMPRESA_SEGUIDOREs
                    where o.NRO_EMPRESA.Equals(e)
                    select o).ToList();
        }

        public int[] TagsdeEmpresa(int empid)
        {
            return (from e in mtdb.MT_EMPRESAS_TAGs
                    where e.NRO_EMPRESA.Equals(empid)
                    select e.NRO_RUBRO).ToArray();
        }

        public void TagsdeEmpresa(int empid, out List<MT_TAG> t)
        {
            int[] i = TagsdeEmpresa(empid);
            t = (from a in mtdb.MT_TAGs
                 where i.Contains(a.NRO_RUBRO)
                 select a).ToList();
        }

        public IEnumerable<MT_TAG> TagsDeEmpresas(IEnumerable<MT_EMPRESA_PERFIL> emps)
        {
            // cargamos los ids de las empresas en una lista
            List<int> eid = new List<int>();
            foreach (MT_EMPRESA_PERFIL e in emps)
            {
                eid.Add(e.NRO_EMPRESA);
            }

            // tiramos un query contra la tabla de empresas tags
            // para traer ids unicos de tags
            List<int> etg = (from t in mtdb.MT_EMPRESAS_TAGs
                             where eid.Contains(t.NRO_EMPRESA)
                             select t.NRO_RUBRO).Distinct().ToList();

            // devolvemos los objetos tags de esos ids
            return (from q in mtdb.MT_TAGs
                    where etg.Contains(q.NRO_RUBRO)
                    select q).AsEnumerable();
        }

        public MT_MENSAJE UltimoMensajeAEmpresa(int nro_remitente, int nro_empresa_destino)
        {
            MT_MENSAJE m = (from n in mtdb.MT_MENSAJEs
                            where n.EST_VIGENTE.Equals('S') && n.NRO_USUARIO.Equals(nro_remitente) && n.NRO_EMPRESA_DESTINO.Equals(nro_empresa_destino)
                            orderby n.FEC_PUBLICADO ascending
                            select n).Last();
            return m;
        }

        #endregion "Directorio"

        #region "Membresia"

        public List<MT_USUARIO> GetAllUsers()
        {
            return (from u in mtdb.MT_USUARIOs
                    select u).ToList();
        }

        public MT_USUARIO_PERFIL GetUserObjByUserName(string username, string password)
        {
            MT_USUARIO_PERFIL up = null;
            MT_USUARIO us = (from u in mtdb.MT_USUARIOs
                             where u.XDE_USERID.Equals(username) && u.XDE_CLAVE.Equals(password) && u.EST_HABILITADO.Equals('S')
                             select u).SingleOrDefault();
            if (us != null)
                up = PerfildeUsuario(us.NRO_USUARIO);
            return up;
        }

        public int NumerodeUsuario(string userid)
        {
            return (from u in mtdb.MT_USUARIOs
                    where u.XDE_USERID.Equals(userid) && u.EST_HABILITADO.Equals('S')
                    select u.NRO_USUARIO).SingleOrDefault();
        }

        public MT_USUARIO_PERFIL PerfildeUsuario(int numusuario)
        {
            return (from u in mtdb.MT_USUARIO_PERFILs
                    where u.NRO_USUARIO.Equals(numusuario)
                    select u).SingleOrDefault();
        }

        public void RegisterUser(MT_USUARIO u)
        {
            u.EST_ACTIVADO = 'S';
            u.EST_HABILITADO = 'S';
            mtdb.MT_USUARIOs.InsertOnSubmit(u);
            if (Save())
            {
                MT_EMAIL e = new MT_EMAIL()
                {
                    NRO_EMPRESA = 0,
                    XDE_EMAIL = u.XDE_USERID,
                    NRO_USUARIO = u.NRO_USUARIO
                };
                mtdb.MT_EMAILs.InsertOnSubmit(e);
                MT_USUARIO_PERFIL p = new MT_USUARIO_PERFIL()
                {
                    NRO_USUARIO = u.NRO_USUARIO,
                    OBS_DOMICILIO = String.Empty,
                    XDE_CIUDAD = String.Empty,
                    XDE_APELLIDOS = String.Empty,
                    XDE_NOMBRES = String.Empty,
                    FEC_NACIMIENTO = null,
                    XDE_CP = null
                };
                mtdb.MT_USUARIO_PERFILs.InsertOnSubmit(p);
                Boolean guardar = Save();
            }
        }

        public string UserID(int id)
        {
            return (from u in mtdb.MT_USUARIOs
                    where u.NRO_USUARIO.Equals(id) && u.EST_HABILITADO.Equals('S')
                    select u.XDE_USERID).SingleOrDefault();
        }

        #endregion "Membresia"

        #region "Mensajeria"

        public List<Remitente> AutoresDeMensajes(int k)
        {
            List<MT_EMPRESA> oMisEmps = EmpresasDeUsuario(k);
            List<int> iMisEmps = new List<int>();
            foreach (MT_EMPRESA e in oMisEmps)
            {
                if (!iMisEmps.Contains(e.NRO_EMPRESA))
                    iMisEmps.Add(e.NRO_EMPRESA);
            }
            List<Remitente> at = (from m in mtdb.MT_MENSAJEs
                                  select m)
                   .Where(x => x.EST_VIGENTE.Equals('S') && (x.NRO_USUARIO_DESTINO.Equals(k) || x.NRO_USUARIO.Equals(k) || iMisEmps.Contains(Convert.ToInt32(x.NRO_EMPRESA_DESTINO)) || iMisEmps.Contains(x.NRO_EMPRESA)))
                   .OrderByDescending(x => x.FEC_LEIDO)
                   .GroupBy(x => x.NRO_USUARIO)
                   .Select(x => new Remitente(x.First()))
                   .Distinct()
                   .ToList<Remitente>();
            return at;
        }

        public MT_MENSAJE Mensaje(int k)
        {
            return (from d in mtdb.MT_MENSAJEs
                    where d.NRO_MENSAJE.Equals(k) && d.EST_VIGENTE.Equals('S')
                    select d).SingleOrDefault();
        }

        /// <summary>
        /// devuelvo todos los mensajes escritos por mi o por mis empresas
        /// y para mí o para mis empresas
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<MT_MENSAJE> Mensajes(int k, int n = 0)
        {
            List<MT_EMPRESA> oMisEmps = EmpresasDeUsuario(k);
            List<int> iMisEmps = new List<int>();
            foreach (MT_EMPRESA e in oMisEmps)
            {
                if (!iMisEmps.Contains(e.NRO_EMPRESA))
                {
                    iMisEmps.Add(e.NRO_EMPRESA);
                }
            }
            if (n > 0) // solo cargo los no leídos, y que no son del usuario, ya que esos los cargo localmente
            {
                return (from m in mtdb.MT_MENSAJEs
                        select m)
                .Where(x => x.NRO_USUARIO_DESTINO.Equals(k) || iMisEmps.Contains(Convert.ToInt32(x.NRO_EMPRESA_DESTINO)))
                .Where(x => x.EST_VIGENTE.Equals('S'))
                .Where(x => x.FEC_LEIDO.Equals(null))
                .OrderBy(x => x.FEC_PUBLICADO)
                .AsQueryable();
            }
            else
            {
                return (from m in mtdb.MT_MENSAJEs
                        select m)
                .Where(x => x.NRO_USUARIO_DESTINO.Equals(k) || x.NRO_USUARIO.Equals(k) || iMisEmps.Contains(Convert.ToInt32(x.NRO_EMPRESA_DESTINO)) || iMisEmps.Contains(x.NRO_EMPRESA))
                .Where(x => x.EST_VIGENTE.Equals('S'))
                .OrderBy(x => x.FEC_PUBLICADO)
                .AsQueryable();
            }
        }

        public IQueryable<MT_MENSAJE> Mensajes(String userid)
        {
            int k = NumerodeUsuario(userid);
            return Mensajes(k);
        }

        public void MensajesLeidos(String userid)
        {
            var ms = (from m in mtdb.MT_MENSAJEs
                      select m)
                   .AsQueryable()
                   .Where(x => x.NRO_USUARIO_DESTINO.Equals(NumerodeUsuario(userid)))
                   .Where(x => x.FEC_LEIDO.Equals(null))
                   .Where(x => x.EST_VIGENTE.Equals('S'));
            foreach (MT_MENSAJE n in ms)
            {
                n.FEC_LEIDO = DateTime.Now;
            }
            Save();
        }

        public void MensajesLeidos(List<MT_MENSAJE> msg, int iUsr)
        {
            foreach (MT_MENSAJE m in msg)
            {
                if (m.NRO_USUARIO_DESTINO.Equals(iUsr))
                {
                    m.FEC_LEIDO = DateTime.Now;
                }
            }
            Save();
        }

        public int MensajesSinLeer(int id)
        {
            return (from n in mtdb.MT_MENSAJEs
                    where n.NRO_USUARIO_DESTINO.Equals(id) && n.FEC_LEIDO == null && n.EST_VIGENTE.Equals('S')
                    select n).Count();
        }

        public List<MT_MENSAJE> MsgsRecibidos(int nro_usuario)
        {
            return (from a in mtdb.MT_MENSAJEs
                    where a.NRO_USUARIO_DESTINO.Equals(nro_usuario) && a.EST_VIGENTE.Equals('S')
                    select a).ToList();
        }

        public void NuevoMensaje(MT_MENSAJE m)
        {
            mtdb.MT_MENSAJEs.InsertOnSubmit(m);
            bool b = Save();
        }

        #endregion "Mensajeria"

        #region Empleos

        public bool ActualizarOfertaDeEmpleo(List<MT_EMPLEO> em)
        {
            List<MT_EMPLEO> vj = (from g in mtdb.MT_EMPLEOs
                                  where g.NRO_OFERTA.Equals(em.First().NRO_OFERTA)
                                  select g).ToList();
            mtdb.MT_EMPLEOs.DeleteAllOnSubmit(vj);
            mtdb.MT_EMPLEOs.InsertAllOnSubmit(em);
            return Save();
        }

        public List<MT_EMPLEO_CATEGORIA> EmpleoCategorias(int id = 0)
        {
            if (id == 0)
            {
                return (from s in mtdb.MT_EMPLEO_CATEGORIAs
                        select s).ToList();
            }
            else
            {
                List<int> c = (from n in mtdb.MT_EMPLEOs
                               where n.NRO_OFERTA.Equals(id) && n.NRO_CATEGORIA > 0
                               select n.NRO_CATEGORIA).ToList();
                return (from g in mtdb.MT_EMPLEO_CATEGORIAs
                        where c.Contains(g.NRO_CATEGORIA)
                        select g).ToList();
            }
        }

        public List<MT_EMPLEO_TIPO> EmpleoTipos(int id = 0)
        {
            if (id == 0)
            {
                return (from s in mtdb.MT_EMPLEO_TIPOs
                        select s).ToList();
            }
            else
            {
                List<int> c = (from n in mtdb.MT_EMPLEOs
                               where n.NRO_OFERTA.Equals(id) && n.NRO_TIPO > 0
                               select n.NRO_TIPO).ToList();
                return (from g in mtdb.MT_EMPLEO_TIPOs
                        where c.Contains(g.NRO_TIPO)
                        select g).ToList();
            }
        }

        public List<MT_EMPLEO_OFERTA> EmpresaEmpleos(int eId)
        {
            return (from n in mtdb.MT_EMPLEO_OFERTAs
                    where n.NRO_EMPRESA.Equals(eId)
                    select n).ToList();
        }

        public void NuevaOfertaDeEmpleo(MT_EMPLEO_OFERTA e)
        {
            mtdb.MT_EMPLEO_OFERTAs.InsertOnSubmit(e);
            Save();
        }

        public IEnumerable<MT_EMPLEO_OFERTA> OfertaDeEmpleo(BuscarEmpleoModel b)
        {
            var o = (from s in mtdb.MT_EMPLEO_OFERTAs
                     select s)
                .Where(s => s.MEM_LARGA.Contains(b.Buscar));
            return o;
        }

        public IEnumerable<MT_EMPLEO_OFERTA> OfertaDeEmpleo(int id = 0)
        {
            var o = (from s in mtdb.MT_EMPLEO_OFERTAs
                     select s);
            if (id != 0)
            {
                o.Where(s => s.NRO_OFERTA.Equals(id) && s.FEC_DESDE <= DateTime.Now && s.FEC_HASTA >= DateTime.Now);
            }
            else
            {
                o.Where(s => s.FEC_DESDE <= DateTime.Now && s.FEC_HASTA >= DateTime.Now)
                .OrderByDescending(s => s.FEC_DESDE);
            }
            return o;
        }

        #endregion Empleos

        #region "Perfil"

        public void ActualizarEmails(int nro_usuario, List<MT_EMAIL> emails)
        {
            List<MT_EMAIL> old_emails = (from e in mtdb.MT_EMAILs
                                         where e.NRO_USUARIO.Equals(nro_usuario)
                                         select e).ToList();
            foreach (MT_EMAIL em in old_emails)
            {
                foreach (MT_EMAIL eu in emails)
                {
                    if (em.NRO_EMAIL.Equals(eu.NRO_EMAIL))
                    {
                        em.XDE_EMAIL = eu.XDE_EMAIL;
                        break;
                    }
                }
            }
            bool b = Save();
        }

        /// <summary>
        /// Agrega una nueva empresa y perfil de empresa
        /// </summary>
        /// <param name="num_usuario"></param>
        /// <param name="ep"></param>
        /// <returns></returns>
        public MT_EMPRESA_PERFIL AgregarEmpresa(int num_usuario, MT_EMPRESA_PERFIL ep)
        {
            MT_EMPRESA emp = new MT_EMPRESA()
            {
                NRO_USUARIO = num_usuario,
                FEC_REGISTRO = DateTime.Now
            };
            mtdb.MT_EMPRESAs.InsertOnSubmit(emp);
            bool b = Save();
            ep.NRO_EMPRESA = emp.NRO_EMPRESA;
            mtdb.MT_EMPRESA_PERFILs.InsertOnSubmit(ep);
            b = Save();
            Notificaciones.NotificarGrupo(emp);
            if (!String.IsNullOrEmpty(ep.XDE_WEB))
            {
                sitiodeempresa = new SitioDeEmpresa()
                {
                    NRO_EMPRESA = ep.NRO_EMPRESA,
                    ROOT_PATH = HttpContext.Current.Server.MapPath("/"),
                    SITE_URL = (!ep.XDE_WEB.Contains("http://")) ? new Uri(String.Format("http://{0}", ep.XDE_WEB)) : new Uri(ep.XDE_WEB)
                };
                Thread NewTh = new Thread(ScreenshotDeSitio);
                NewTh.SetApartmentState(ApartmentState.STA);
                NewTh.Start();
            }
            return ep;
        }

        public string AvatarUsuario(int num_usuario)
        {
            var a = (from r in mtdb.MT_RECURSOs
                     where r.NRO_TIPO.Equals(7) && r.NRO_USUARIO.Equals(num_usuario)
                     select r).FirstOrDefault();
            return (a != null) ? a.XDE_ARCHIVO : String.Empty;
        }

        public int BorrarTagDeEmpresa(int empid, int rubid)
        {
            int k = 0;
            using (TransactionScope t = new TransactionScope())
            {
                MT_EMPRESAS_TAG et = (from e in mtdb.MT_EMPRESAS_TAGs
                                  where e.NRO_RUBRO.Equals(rubid) && e.NRO_EMPRESA.Equals(empid)
                                  select e).FirstOrDefault();
                k = et.NRO_EMPRESA;
                mtdb.MT_EMPRESAS_TAGs.DeleteOnSubmit(et);
                Save();
                MT_EMPRESA_PERFIL em = (from g in mtdb.MT_EMPRESA_PERFILs
                                        where g.NRO_EMPRESA.Equals(empid)
                                        select g).FirstOrDefault();
                LuceneSearch.AddUpdateLuceneIndex(em);
                t.Complete();
            }            
            return k;
        }

        public void BorrarTodoDeEmpresa(string userid, int empid)
        {
            // primero vemos si la empresa pertenece a este usuario
            // si es asi....
            int u = NumerodeUsuario(userid);
            MT_EMPRESA e = (from m in mtdb.MT_EMPRESAs
                            where m.NRO_USUARIO.Equals(u) && m.NRO_EMPRESA.Equals(empid)
                            select m).FirstOrDefault();
            if (e != null)
            {
                MT_EMPRESA_PERFIL w = (from d in mtdb.MT_EMPRESA_PERFILs
                                       where d.NRO_EMPRESA.Equals(empid)
                                       select d).FirstOrDefault();

                // borramos el perfil
                if (w != null)
                {
                    //borramos de Lucene
                    LuceneSearch.RemoveDocFromLuceneIndex(w);
                    LuceneSearch.Optimize();

                    // borramos los recursos
                    List<int> r = (from n in mtdb.MT_RECURSOs
                                   where n.NRO_EMPRESA.Equals(empid)
                                   select n.NRO_RECURSO).ToList();
                    foreach (int g in r)
                    {
                        BorrarFoto(g, ref empid);
                    }
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(String.Format("/P/E/{0}", empid))))
                    {
                        HttpContext.Current.Application.Lock();
                        Directory.Delete(HttpContext.Current.Server.MapPath(String.Format("/P/E/{0}", empid)), true);
                        HttpContext.Current.Application.UnLock();
                    }

                    // borramos la empresa
                    MT_EMPRESA j = (from f in mtdb.MT_EMPRESAs
                                    where f.NRO_EMPRESA.Equals(empid)
                                    select f).FirstOrDefault();
                    if (j != null)
                    {
                        mtdb.MT_EMPRESAs.DeleteOnSubmit(j);
                    }

                    //borramos los tags de la empresa
                    List<MT_EMPRESAS_TAG> t = (from a in mtdb.MT_EMPRESAS_TAGs
                                               where a.NRO_EMPRESA.Equals(empid)
                                               select a).ToList();
                    foreach (MT_EMPRESAS_TAG i in t)
                    {
                        mtdb.MT_EMPRESAS_TAGs.DeleteOnSubmit(i);
                    }

                    //borramos los emails de esa empresa
                    List<MT_EMAIL> l = (from x in mtdb.MT_EMAILs
                                        where x.NRO_EMPRESA.Equals(empid)
                                        select x).ToList();
                    foreach (MT_EMAIL o in l)
                    {
                        mtdb.MT_EMAILs.DeleteOnSubmit(o);
                    }

                    //borramos los telefonos
                    List<MT_TELEFONO> m = (from y in mtdb.MT_TELEFONOs
                                           where y.NRO_EMPRESA.Equals(empid)
                                           select y).ToList();
                    foreach (MT_TELEFONO q in m)
                    {
                        mtdb.MT_TELEFONOs.DeleteOnSubmit(q);
                    }

                    //borramos el perfil
                    mtdb.MT_EMPRESA_PERFILs.DeleteOnSubmit(w);
                    w = null;

                    //guardamos cambios
                    Save();
                }
            }
        }

        public bool CambiarClave(int usr, string newpass)
        {
            MT_USUARIO u = (from g in mtdb.MT_USUARIOs
                            where g.NRO_USUARIO.Equals(usr)
                            select g).SingleOrDefault();
            if (u != null)
            {
                u.XDE_CLAVE = v.HashClave(newpass);
                return Save();
            }
            else
                return false;
        }

        public string CodigoEmpresa(int id, int pais)
        {
            string p = (from a in mtdb.MT_PAISEs
                        where a.NRO_PAIS.Equals(pais)
                        select a.XDE_PAIS).SingleOrDefault();
            if (String.IsNullOrEmpty(p))
                p = "XX";
            return FormatoCodigoEmpresa(id, p);
        }

        public List<MT_EMAIL> Emails(int id)
        {
            return (from e in mtdb.MT_EMAILs
                    where e.NRO_USUARIO.Equals(id)
                    select e).ToList();
        }

        public MT_RECURSO EmpresaFotoGrande(int id, int u)
        {
            List<MT_RECURSO> g = (from m in mtdb.MT_RECURSOs
                                  where m.NRO_EMPRESA.Equals(id) && m.NRO_TIPO.Equals(8)
                                  select m).ToList();
            MT_RECURSO r;
            if (g.Count().Equals(0))
            {
                r = new MT_RECURSO()
                {
                    NRO_TIPO = 8,
                    NRO_USUARIO = u,
                    NRO_ORDEN = 0,
                    NRO_GALERIA = 0,
                    NRO_EMPRESA = id,
                    XDE_ARCHIVO = String.Empty
                };
            }
            else
            {
                r = g.Last();
            }
            return r;
        }

        public bool EmpresasDeUsuario(string uname)
        {
            int k = NumerodeUsuario(uname);
            return (EmpresasDeUsuario(k).Count() > 0) ? true : false;
        }

        public List<MT_EMPRESA> EmpresasDeUsuario(int nro_usuario)
        {
            return (from a in mtdb.MT_EMPRESAs
                    where a.NRO_USUARIO.Equals(nro_usuario)
                    select a).ToList();
        }

        public List<int> iEmpresasDeUsuario(int nro_usuario)
        {
            List<MT_EMPRESA> oMisEmps = EmpresasDeUsuario(nro_usuario);
            List<int> iMisEmps = new List<int>();
            foreach (MT_EMPRESA e in oMisEmps)
            {
                if (!iMisEmps.Contains(e.NRO_EMPRESA))
                {
                    iMisEmps.Add(e.NRO_EMPRESA);
                }
            }
            return iMisEmps;
        }

        public MT_RECURSO LogoDeEmpresa(int id, int u)
        {
            List<MT_RECURSO> g = (from m in mtdb.MT_RECURSOs
                                  where m.NRO_EMPRESA.Equals(id) && m.NRO_TIPO.Equals(10)
                                  select m).ToList();
            MT_RECURSO r;
            if (g.Count().Equals(0))
            {
                r = new MT_RECURSO()
                {
                    NRO_TIPO = 10,
                    NRO_USUARIO = u,
                    NRO_ORDEN = 0,
                    NRO_GALERIA = 0,
                    NRO_EMPRESA = id,
                    XDE_ARCHIVO = String.Empty
                };
            }
            else
            {
                r = g.Last();
            }
            return r;
        }

        public MT_RECURSO LogoDeEmpresa(int id)
        {
            return (from m in mtdb.MT_RECURSOs
                    where m.NRO_EMPRESA.Equals(id) && m.NRO_TIPO.Equals(10)
                    orderby m.NRO_RECURSO descending
                    select m).FirstOrDefault();
        }

        public string NombreyApellidoUsuario(int num_usuario)
        {
            var a = (from r in mtdb.MT_USUARIO_PERFILs
                     where r.NRO_USUARIO.Equals(num_usuario)
                     select r).SingleOrDefault();
            return (a != null && !String.IsNullOrEmpty(a.XDE_NOMBRES)) ? a.XDE_NOMBRES + " " + a.XDE_APELLIDOS : UserID(num_usuario);
        }

        public void NuevoEmail(MT_EMAIL em)
        {
            mtdb.MT_EMAILs.InsertOnSubmit(em);
            Save();
        }

        public void NuevoTelefono(MT_TELEFONO t)
        {
            mtdb.MT_TELEFONOs.InsertOnSubmit(t);
            Save();
        }

        public MT_USUARIO_PERFIL ObtenerPerfilUsuarioActual(int num_usuario)
        {
            return (from x in mtdb.MT_USUARIO_PERFILs
                    where x.NRO_USUARIO.Equals(num_usuario)
                    select x).SingleOrDefault();
        }

        public DatosdeUsuario PerfilCompletodeUsuario(string userid)
        {
            DatosdeUsuario ud = new DatosdeUsuario();
            ud.nro_usuario = (from x in mtdb.MT_USUARIOs
                              where x.XDE_USERID.Equals(userid)
                              select x.NRO_USUARIO).SingleOrDefault();
            ud.perfil = ObtenerPerfilUsuarioActual(ud.nro_usuario);
            ud.emails = (from e in mtdb.MT_EMAILs
                         where e.NRO_USUARIO.Equals(ud.nro_usuario)
                         select e).ToList();
            ud.tels = (from e in mtdb.MT_TELEFONOs
                       where e.NRO_USUARIO.Equals(ud.nro_usuario) && e.NRO_EMPRESA.Equals(0)
                       select e).ToList();
            ud.empresas = (from m in mtdb.MT_EMPRESAs
                           where m.NRO_USUARIO.Equals(ud.nro_usuario)
                           select m).Take(20).ToList();
            ud.avatar = (from r in mtdb.MT_RECURSOs
                         where r.NRO_TIPO.Equals(7) && r.NRO_USUARIO.Equals(ud.nro_usuario)
                         select r).FirstOrDefault();
            return ud;
        }

        public List<MT_EMPRESA_PERFIL> PerfilesDeEmpresasDeUsuario(string uname)
        {
            int k = NumerodeUsuario(uname);
            List<int> e = iEmpresasDeUsuario(k);
            List<MT_EMPRESA_PERFIL> ep = new List<MT_EMPRESA_PERFIL>();
            if (e.Count > 0)
            {
                ep = (from j in mtdb.MT_EMPRESA_PERFILs
                      where e.Contains(j.NRO_EMPRESA)
                      orderby j.XDE_RAZONSOCIAL ascending
                      select j).Take(50).ToList();
            }
            return ep;
        }

        public MT_RECURSO Recurso(int id)
        {
            return (from r in mtdb.MT_RECURSOs
                    where r.NRO_RECURSO.Equals(id)
                    select r).SingleOrDefault();
        }

        private string FormatoCodigoEmpresa(int id, string pais)
        {
            pais = pais.ToUpper().Substring(0, 2);
            return String.Format("{0}{1,22:D6}", pais, id);
        }

        #endregion "Perfil"

        #region "Varios"

        public List<MT_PAISE> cargarPaises()
        {
            return (from p in mtdb.MT_PAISEs
                    orderby p.XDE_PAIS
                    select p).ToList();
        }

        public List<MT_PROVINCIA> cargarProvincias(int pais)
        {
            return (from p in mtdb.MT_PROVINCIAs
                    where p.NRO_PAIS.Equals(pais)
                    orderby p.XDE_PROVINCIA
                    select p).ToList();
        }

        public void ForzarCapturaDeSitio(SitioDeEmpresa s)
        {
            sitiodeempresa = s;
            if (!String.IsNullOrEmpty(s.SITE_URL.ToString()) && !System.IO.File.Exists(Path.Combine(s.ROOT_PATH, String.Format("/P/E/{0}/sitio.jpg", s.NRO_EMPRESA))))
            {
                Thread NewTh = new Thread(ScreenshotDeSitio);
                NewTh.SetApartmentState(ApartmentState.STA);
                NewTh.Start();
            }
        }

        public IQueryable<MT_EMPRESA_PERFIL> GetAll()
        {
            Busqueda b = new Busqueda();
            return GetAll(b);
        }

        public IQueryable<MT_EMPRESA_PERFIL> GetAll(Busqueda busqueda)
        {
            if (busqueda != null && busqueda.Filtros != null)
            {
                Expression<Func<MT_EMPRESA_PERFIL, bool>> p1, bigP;

                p1 = p => p.XDE_RAZONSOCIAL.Contains(busqueda.Texto);
                bigP = p1;

                return mtdb.MT_EMPRESA_PERFILs.Where(bigP);
            }
            else
            {
                return from r in mtdb.MT_EMPRESA_PERFILs
                       select r;
            }
        }

        public ImageCodecInfo getEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        public Boolean Save()
        {
            mtdb.SubmitChanges();
            return true;
        }

        public IQueryable<MT_TAG> SelectAll<TKey>(List<Expression<Func<MT_TAG, bool>>> whereCls)
        {
            var selectAllQry = from t in mtdb.MT_TAGs
                               select t;
            if (whereCls != null)
            {
                foreach (Expression<Func<MT_TAG, bool>> whereStmt in whereCls)
                {
                    selectAllQry = selectAllQry.Where(whereStmt);
                }
            }
            return selectAllQry;
        }

        private void ScreenshotDeSitio()
        {
            LabelImage thumb;
            Bitmap x;
            try
            {
                thumb = new LabelImage(sitiodeempresa);
                x = thumb.GetBitmap();
                EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, 100L);
                ImageCodecInfo jpegCodec = this.getEncoderInfo("image/jpeg");
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

        #endregion "Varios"

        #region "Tags"

        /// <summary>
        /// asocia el tag a la empresa
        /// </summary>
        public void AgregarTagaEmpresa(int e, int r)
        {
            MT_EMPRESAS_TAG t = new MT_EMPRESAS_TAG()
            {
                NRO_EMPRESA = e,
                NRO_RUBRO = r
            };
            mtdb.MT_EMPRESAS_TAGs.InsertOnSubmit(t);
            Save();
            MT_EMPRESA_PERFIL emp = (from g in mtdb.MT_EMPRESA_PERFILs
                                     where g.NRO_EMPRESA.Equals(e)
                                     select g).FirstOrDefault();
            LuceneSearch.AddUpdateLuceneIndex(emp);
        }

        public string NombreTag(int id)
        {
            return (from n in mtdb.MT_TAGs
                    where n.NRO_RUBRO.Equals(id)
                    select n.OBS_NOMBRE).SingleOrDefault();
        }

        public string TagEmpresas(int rubro)
        {
            string c = String.Empty;
            var h = (from t in mtdb.MT_EMPRESAS_TAGs
                     where t.NRO_RUBRO.Equals(rubro)
                     select t);
            if (h != null && h.Count() > 0)
                c = String.Format("{0} empresas", h.Count() - 1);
            return c;
        }

        public IQueryable<MT_EMPRESA_PERFIL> TagEmpresasPerfiles(int rubro)
        {
            BusquedaFiltros filtros = new BusquedaFiltros()
            {
                NRO_RUBRO = new int[] { rubro }
            };
            Busqueda busqueda = new Busqueda()
            {
                Filtros = filtros,
                UsarLucene = 1,
                Texto = "+NRO_RUBRO:" + rubro.ToString(),
                PageSize = 100,
                PageCurrent = 1
            };
            int contadorResultados = 0;
            return LuceneSearch.Search_E(busqueda, ref contadorResultados, "NRO_RUBRO").AsQueryable();
        }

        public string TagHijas(int rubro)
        {
            string c = String.Empty;
            var h = (from t in mtdb.MT_TAGs
                     where t.NRO_PADRE.Equals(rubro)
                     select t);
            if (h != null && h.Count() > 0)
                c = String.Format("{0} tags", h.Count());
            return c;
        }

        public IQueryable<MT_TAG> Tags(int padre = 0, int[] usados = null)
        {
            if (padre >= 0)
            {
                return (from t in mtdb.MT_TAGs
                        where t.NRO_PADRE.Equals(padre)
                        select t).AsQueryable();
            }

            // si le paso un valor negativo, me devuelve todos los tags
            else
            {
                if (usados != null)
                {
                    return (from t in mtdb.MT_TAGs
                            where !usados.Contains(t.NRO_RUBRO)
                            orderby t.OBS_NOMBRE ascending
                            select t).AsQueryable();
                }
                else
                {
                    return (from t in mtdb.MT_TAGs
                            select t).AsQueryable();
                }
            }
        }

        public string TagsPath(int rubro, ref List<string> h)
        {
            MT_TAG t = (from r in mtdb.MT_TAGs
                        where r.NRO_RUBRO.Equals(rubro)
                        select r).SingleOrDefault();
            if (t != null)
            {
                h.Add(String.Format("<li><a href=\"/Directorio/Tag/{0}\">{1}</a></li>", t.NRO_RUBRO, t.OBS_NOMBRE));
                if (t.NRO_PADRE > 0)
                {
                    string borrar = TagsPath(t.NRO_PADRE, ref h);
                }
            }
            return string.Join(String.Empty, h.ToArray().Reverse());
        }

        public IEnumerable<MT_TAG> TagsRelacionados(BusquedaFiltros bf)
        {
            var tg = from t in mtdb.MT_TAGs
                     select t;
            if (bf.NRO_RUBRO != null)
                if (bf.NRO_RUBRO.Length > 0)
                {
                    {
                        tg = tg.Where(t => bf.NRO_RUBRO.Contains(t.NRO_RUBRO));
                    }
                }
            if (bf.OBS_NOMBRE != null)
            {
                tg = tg.Where(t => t.OBS_NOMBRE.Contains(bf.OBS_NOMBRE));
            }
            return tg.AsQueryable();
        }

        #endregion "Tags"
    }

    public class TagPath
    {
        private int id { get; set; }

        private string nombre { get; set; }
    }
}