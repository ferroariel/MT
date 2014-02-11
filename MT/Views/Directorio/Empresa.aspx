<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.MT_EMPRESA_PERFIL>" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>
        <%: Model.XDE_RAZONSOCIAL %></title>
    <% if (Model.XDE_DOMICILIO != null)
       {%>
    <%: Html.Partial("GoogleMaps") %>
    <% } %>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
    <% if (!String.IsNullOrEmpty(ViewBag.Autor))
       {%>
    <link rel="stylesheet" type="text/css" href="/css/uploadify.css" />
    <script src="/Scripts/jquery.uploadify.js"></script>
    <script src="/Scripts/jquery.imgrefresh-1.0.min.js"></script>
    <%}
       if (!String.IsNullOrEmpty(Model.XDE_TWITTER))
       { %>
    <script src="/Scripts/jquery.tweet.js"></script>
    <link rel="stylesheet" type="text/css" href="/css/jquery.tweet.css" />
    <%} %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% 
        MTrepository mtdb = new MTrepository();
        int num_emp = Model.NRO_EMPRESA;
    %>
    <div class="ten columns">
        <div class="row collapse">
            <div class="nine columns">
                <ul class="breadcrumbs">
                    <li><a href="/Directorio/">Directorio</a></li>
                    <li class="current"><a href="#">
                        <%: Model.XDE_RAZONSOCIAL%></a></li>
                </ul>
            </div>
            <div class="three columns divacciones">
                <ul class="block-grid three-up acciones">
                    <% List<MT_EMPRESA_SEGUIDORE> sg = ViewBag.Seguidores as List<MT_EMPRESA_SEGUIDORE>;
                       if (sg.Count() > 0)
                       { %>
                    <li>
                        <%: Html.Partial("EmpresaSeguidores", ViewBag.Seguidores as List<MT_EMPRESA_SEGUIDORE>) %>
                    </li>
                    <%}%>
                    <li>
                        <%: Html.Partial("SocialSharing") %>
                    </li>
                    <li>
                        <ul class="nav-bar">
                            <li class="has-flyout"><a href="#"><span class="icon icon-star"></span></a><a href="#"
                                class="flyout-toggle"><span></span></a>
                                <div class="flyout">
                                    <h5>
                                        Acciones</h5>
                                    <ul>
                                        <% if (!String.IsNullOrEmpty(ViewBag.Autor))
                                           {%>
                                        <li><a href="/Usuarios/Empresa/<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>" class="icon-pencil">
                                            Modificar Empresa </a></li>
                                        <%}
                                           else
                                           { %>
                                        <li><a href="#" id="bSeguir" data-nro_empresa="<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>"
                                            rel="nofollow" class="icon-star">
                                            <%: ViewBag.Siguiendo %></a></li>
                                        <li><a href="#" id="bMensaje" data-reveal-id="pMensaje" class="icon-comment" title="Mensaje">
                                            Enviar un Mensaje</a></li>
                                        <% if (mtdb.EmpresasDeUsuario(User.Identity.Name))
                                           {%>
                                        <li><a href="#" class="icon-resize-full" title="Intercambiar Links" data-reveal-id="mIntercambio">
                                            Conectar con Empresa</a></li>
                                        <%} %>
                                        <%} %>
                                        <li><a href="#" class="icon-link" data-reveal-id="mReporteEnlaceRoto">&nbsp;Reportar
                                            enlace roto</a></li>
                                        <li><a href="#" class="icon-wrench" data-reveal-id="mReporteImpresicion">&nbsp;Reportar
                                            impresición</a></li>
                                        <li><a href="#" class="icon-minus-sign" data-reveal-id="mReporteAbuso">&nbsp;Reportar
                                            abuso</a></li>
                                        <li><a href="javascript:history.go(-1);" class="icon-circle-arrow-left">&nbsp;Volver</a></li>
                                    </ul>
                                </div>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
        </div>
        <div class="row">
            <div class="two columns">
                <h5>
                    <%: Model.XDE_RAZONSOCIAL%></h5>
            </div>
            <div class="two columns">
                Empresa:<br />
                Dirección:<br />
                Estado/Provincia:<br />
                País:<br />
                Teléfono:<br />
                &nbsp;<br />
                Contacto:
                <br />
                &nbsp;</div>
            <div class="four columns">
                <%: Model.XDE_RAZONSOCIAL%>
                <br />
                <%: Model.XDE_DOMICILIO%>
                <br />
                <%: ViewBag.Provincia as string%>
                <br />
                <%: ViewBag.Pais as string%>
                <br />
                <% 
                    List<MT_TELEFONO> ts = mtdb.EmpresaTelefonos(Model.NRO_EMPRESA);
                    List<string> telefonos = new List<string>();
                    foreach (MT_TELEFONO t in ts)
                    {
                        telefonos.Add(String.Format("<a href='tel:{0}'>{0}</a> ", t.XDE_TELEFONO));
                    }
                    if (telefonos.Count() > 0)
                    {
                %><%: Html.Raw(String.Join(" / ", telefonos)) %><%
                    }
                    else
                    { %>No disponible<%}  %>
                <br />
                &nbsp;<br />
                <% 
                    if (num_emp > 0)
                    {
                        MT_USUARIO_PERFIL kv = mtdb.PerfildeUsuario(mtdb.AutorDeEmpresa(num_emp));                    
                %>
                <a href="#" class="th" id="bRespAv">
                    <img src="/Recursos/Avatar?uid=<%: Html.Raw(kv.NRO_USUARIO.ToString()) %>" style="height: 30px;
                        width: auto; float: left; margin-right: 10px;" />
                    <%: Html.Raw(kv.XDE_NOMBRES +" "+ kv.XDE_APELLIDOS) %>
                </a>
                <% kv = null;
                    }%>
            </div>
            <div class="two columns">
                <% 
                    Uri ures = null;
                    if (Uri.TryCreate(String.Format("http://{0}", Model.XDE_WEB), UriKind.Absolute, out ures))
                    {%>
                <a href="<%: ures %>" target="_blank" class="th">
                    <img src="/P/E/<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>/sitio.jpg" style="width: 100%;
                        height: auto;" /></a>
                <%
                    }%>
            </div>
            <div class="two columns">
                <a href="mailto:" class="icon-envelope">E-mail</a><br />
                <%                     
                    if (ures != null)
                    {%>
                <a href="<%: ures %>" target="_blank" class="icon-globe">Web Site</a><br />
                <%
                    }%>
                <!--
                    <%

                    Uri ufb = null;
                    Uri utw = null;
                    if (Uri.TryCreate(Model.XDE_FACEBOOK, UriKind.Absolute, out ufb) || Uri.TryCreate(Model.XDE_TWITTER, UriKind.Absolute, out utw))
                    {
                %>
                &nbsp;<br />
                Encuentre en<br />
                Redes Sociales<br />
                <%
                        if (ufb != null)
                        { 
                %><a href="<%: ufb %>" target="_blank" class="icon-facebook-sign"></a><%
                        }
                        if (utw != null)
                        { 
                %><a href="<%: utw %>" target="_blank" class="icon-twitter-sign"></a><%
                        }
                    }
                    
                %>
                -->
                &nbsp;<br />
                <a href="http://twitter.com/home?status=<%: Url.Encode( Model.XDE_RAZONSOCIAL +", "+ Model.XDE_CORTA +" "+ Request.Url.AbsoluteUri ) %>"
                    class="twitter-share-button" data-via="MercadoTextil" data-lang="es">Twittear</a><br />
                <script>                    !function (d, s, id) { var js, fjs = d.getElementsByTagName(s)[0]; if (!d.getElementById(id)) { js = d.createElement(s); js.id = id; js.src = "//platform.twitter.com/widgets.js"; fjs.parentNode.insertBefore(js, fjs); } } (document, "script", "twitter-wjs");</script>
                <iframe src="http://www.facebook.com/plugins/like.php?href=<%: Url.Encode(Request.Url.AbsoluteUri)%>&amp;layout=button_count&amp;locale=es_ES&amp;show_faces=false&amp;width=150&amp;action=like&amp;colorscheme=light&amp;font=arial"
                    scrolling="no" frameborder="0" allowtransparency="true" style="border: none;
                    overflow: hidden; width: 150px; height: 21px;"></iframe>
                <!-- Place this tag where you want the +1 button to render. -->
                <div class="g-plusone" data-size="small">
                </div>
                <script type="text/javascript">
                    window.___gcfg = { lang: 'es-419' };
                    (function () {
                        var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
                        po.src = 'https://apis.google.com/js/plusone.js';
                        var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
                    })();
                </script>
            </div>
        </div>
        <div class="row">
            &nbsp;
            <div class="twelve columns">
                <dl class="tabs contained tabsempresa five-up">
                    <dd class="active" onclick="repintarMuro()">
                        <a href="#simpleContained2">Productos y Servicios</a></dd>
                    <dd>
                        <a href="#simpleContained3">Fotos y Descargas</a></dd>
                    <% if (!String.IsNullOrEmpty(ViewBag.Direccion))
                       {%>
                    <dd>
                        <a href="#simpleContained4" onclick="repintarMapa()">Ubicación, Contacto</a></dd>
                    <%} %>
                    <dd>
                        <a href="#simpleContained5">Opiniones, Valorización</a></dd>
                </dl>
                <% 
                    MT_RECURSO f = ViewBag.FotoGrande as MT_RECURSO;
                    MT_RECURSO a = ViewBag.Logo as MT_RECURSO;
                    if (!f.NRO_RECURSO.Equals(0) && !a.NRO_RECURSO.Equals(0))
                    {                     
                %>
                <div class="row" id="pBkgEncabezado">
                    <div class="three columns">
                        <div class="row" id="pBkgLogo">
                            <a href="#" class="th">
                                <img id="eLogo" alt="<%: a.NRO_RECURSO.ToString() %>" src="/Recursos/Avatar?eid=<%: Model.NRO_EMPRESA.ToString() %>" /></a>
                            <script>
                                $(document).ready(function () {
                                    $("#pBkgEncabezado").css({
                                        backgroundImage: "url('/Recursos/Fondo?eid=<%: f.NRO_EMPRESA.ToString() %>&uid=<%: f.NRO_USUARIO.ToString() %>')",
                                        backgroundSize: "100% auto",
                                        backgroundPosition: "0% 50%"
                                    });
                                });
                            </script>
                            <% if (!String.IsNullOrEmpty(ViewBag.Autor))
                               { %>
                            <input type="file" name="archivo" id="archivo" />
                            <script>
                                $(document).ready(function () {
                                    $('#archivo').uploadify({
                                        'swf': '/Scripts/uploadify.swf',
                                        'uploader': '/Varios/Archivo/?uid=<%: a.NRO_USUARIO.ToString() %>&eid=<%: Model.NRO_EMPRESA.ToString() %>&irc=<%: a.NRO_RECURSO.ToString() %>&itc=<%: "10" %>',
                                        'auto': true,
                                        'buttonClass': 'button icon-paper-clip',
                                        'buttonText': '',
                                        'method': 'post',
                                        'onUploadSuccess': function (file, data, response) {
                                            $('#eLogo').imgrefresh();
                                        }
                                    });
                                    $('#archivo2').uploadify({
                                        'swf': '/Scripts/uploadify.swf',
                                        'uploader': '/Varios/Archivo/?uid=<%: f.NRO_USUARIO.ToString() %>&eid=<%: f.NRO_EMPRESA.ToString() %>&irc=<%: f.NRO_RECURSO.ToString() %>&itc=<%: "8" %>',
                                        'auto': true,
                                        'buttonClass': 'button icon-paper-clip',
                                        'buttonText': '',
                                        'method': 'post',
                                        'onUploadSuccess': function (file, data, response) {
                                            $('#pBkgEncabezado').css({ backgroundImage: "url('/Recursos/Fondo?eid=<%: f.NRO_EMPRESA.ToString() %>&uid=<%: f.NRO_USUARIO.ToString() %>&dmy=" + (new Date).getTime() + "')" });
                                        }
                                    });
                                });                    
                            </script>
                            <%} %>
                        </div>
                        <div class="row">
                            <p>
                                &nbsp;</p>
                            <blockquote>
                                <%: Model.XDE_CORTA %>
                            </blockquote>
                        </div>
                    </div>
                    <% if (!String.IsNullOrEmpty(ViewBag.Autor))
                       { %>
                    <input type="file" name="archivo" id="archivo2" />
                    <% } %>
                </div>
                <%} %>
                <ul class="tabs-content contained">
                    <li class="active" id="simpleContained2Tab">
                        <div class="row">
                            <div class="eight columns">
                                <div class="panel">
                                    <%: Html.DisplayFor(model => model.XDE_CORTA) %>
                                    <%: Html.Raw(Model.MEM_LARGA) %>
                                </div>
                            </div>
                            <div class="four columns" style="border-left: solid 1px #E6E6E6">
                                <%                                                    
                                    List<MsgMuro> j = ViewBag.Muro as List<MsgMuro>;
                                    if (j.Count() > 0)
                                    { 
                                %>
                                
                                <%} %>
                                <h5>
                                    Actividad<span style="float:right"><a href="#" class="icon-resize-full" title="Insertar Muro" data-reveal-id="mInsetarMuro">
                                    &nbsp;En tu sitio</a></span></h5>
                                <div class="row" id="pArbol" style="height: 390px; display: block; margin-bottom: 15px">
                                    <div id="scrollbar1" class="twelve columns">
                                        <div class="scrollbar" style="height: auto !important">
                                            <div class="track" style="height: auto !important">
                                                <div class="thumb">
                                                    <div class="end">
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="viewport" style="width: 92%">
                                            <div class="overview" style="width: 100%">
                                                <%      
                                                    if (j.Count() > 0)
                                                    {
                                                        foreach (MsgMuro w in j)
                                                        {
                                                %><div class="row collapse">
                                                    <div class="twelve columns">
                                                        <%: w.t%></div>
                                                    <%: Html.Raw(w.m)%>
                                                    <div class="msg_sep">
                                                        <abbr class="timeago" title="<%: w.x %>">
                                                            <%: w.x%></abbr>
                                                    </div>
                                                </div>
                                                <%}
                                                            }
                                                            else
                                                            {%>
                                                No hay actividad reciente...
                                                <%}%>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <%if (!String.IsNullOrEmpty(Model.XDE_TWITTER))
                                      { %>
                                    <div class="six columns">
                                        <h5>
                                            <%: Model.XDE_RAZONSOCIAL%>
                                            en Twitter</h5>
                                        <div class="panel tweet">
                                            No hay tweets...</div>
                                    </div>
                                    <%} %>
                                    <div class="six columns">
                                    </div>
                                </div>
                    </li>
                    <li id="simpleContained3Tab">
                        <div class="row">
                            <div class="nine columns">
                                <h5>
                                    Galería de Fotos</h5>
                                <% 
                                    IQueryable<MT_RECURSO> r = ViewBag.EmpresaFotosProductos as IQueryable<MT_RECURSO>;
                                    if (r.Count() > 0)
                                    {%>
                                <ul class="block-grid three-up" data-clearing>
                                    <%
                                        foreach (MT_RECURSO i in r)
                                        {
                                    %>
                                    <li><a target="_blank" class="th" href="/P/E/<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>/<%: Html.Raw(i.XDE_ARCHIVO) %>">
                                        <img src="/P/E/<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>/<%: Html.Raw(i.XDE_ARCHIVO) %>" /></a></li>
                                    <%}%>
                                </ul>
                                <%
                                    }
                                    else
                                    { %>
                                No hay fotos cargadas...
                                <%}
                                %>
                            </div>
                            <div class="three columns">
                                <h5>
                                    Descargas</h5>
                                <%       
                                    IQueryable<MT_RECURSO> dw = ViewBag.Descargas as IQueryable<MT_RECURSO>;
                                    if (dw.Count() > 0)
                                    {%>
                                <ul class="block-grid two-up descargas">
                                    <%
                                        foreach (MT_RECURSO iw in dw)
                                        {
                                    %>
                                    <li>
                                        <img src="/Content/img/document.png" title="<%: Html.Raw(iw.XDE_ARCHIVO) %>" /><br />
                                        <a class="button small icon-download-alt" title="Descargar" href="/Recursos/Descarga?eid=<%: Html.Raw(iw.NRO_EMPRESA.ToString()) %>&rid=<%: Html.Raw(iw.NRO_RECURSO.ToString()) %>">
                                        </a></li>
                                    <%}%>
                                </ul>
                                <%
                                    }
                                    else
                                    { %>
                                No hay descargas disponibles...
                                <%}
                                %>
                            </div>
                        </div>
                    </li>
                    <% if (!String.IsNullOrEmpty(ViewBag.Direccion))
                       {%>
                    <li id="simpleContained4Tab">
                        <div class="row">
                            <div class="three columns blanco">
                                <h5>
                                    Resumen
                                </h5>
                                <dl>
                                    <% if (ViewBag.Tags != null)
                                       {%>
                                    <dt>Sector / Rubro</dt>
                                    <% foreach (MT_TAG tag in ViewBag.Tags as List<MT_TAG>)
                                       {%>
                                    <dd>
                                        -&nbsp;<%: tag.OBS_NOMBRE %></dd>
                                    <%}
                                       } %>
                                    <% if (telefonos.Count() > 0)
                                       {%>
                                    <dt>Teléfonos</dt>
                                    <dd>
                                        <%: Html.Raw(String.Join(" / ", telefonos))%></dd>
                                    <%} %>
                                    <dt>Dirección</dt>
                                    <dd>
                                        <%: Model.XDE_DOMICILIO %></dd>
                                    <dt>Ciudad</dt>
                                    <dd>
                                        <%: Model.XDE_CIUDAD %></dd>
                                    <dt>Provincia</dt>
                                    <dd>
                                        <%: ViewBag.Provincia%></dd>
                                    <dt>País</dt>
                                    <dd>
                                        <%: ViewBag.Pais%></dd>
                                    <% if (ures != null)
                                       {%>
                                    <dt>Website</dt>
                                    <dd>
                                        <a href="<%: Model.XDE_WEB %>" target="_blank">
                                            <%: Model.XDE_WEB%></a></dd>
                                    <%} %>
                                    <dt>Productos / Servicios que ofrece</dt>
                                    <dd>
                                        <%: Html.Raw(Varias.StripHTML(Model.MEM_LARGA)) %></dd>
                                </dl>
                            </div>
                            <div class="nine columns">
                                <div class="content">
                                    <div id="map_canvas" style="width: 100%; height: 400px;">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </li>
                    <%} %>
                    <li id="simpleContained5Tab">
                        <div class="row">
                            <div class="four columns">
                                <h5>
                                    Puntaje:</h5>
                                <div id="valoracion">
                                    <div id="bar">
                                    </div>
                                    <div id="stars">
                                        <a href="#" class="voto1" data-valoracion="1"></a><a href="#" class="voto1" data-valoracion="2">
                                        </a><a href="#" class="voto1" data-valoracion="3"></a><a href="#" class="voto1" data-valoracion="4">
                                        </a><a href="#" class="voto1" data-valoracion="5"></a>
                                    </div>
                                </div>
                            </div>
                            <div class="eight columns">
                                <h5>
                                    Opinión de Usarios</h5>
                                <blockquote>
                                    <% 
                                        var o = ViewBag.Opiniones as List<MT_MENSAJE>;
                                        if (o.Count().Equals(0))
                                        { 
                                    %><cite>No hay opiniones. Sé el primero!</cite><%
                                        }
                                        else
                                        {
                                            foreach (MT_MENSAJE w in o)
                                            {
                                    %>
                                    <div class="row">
                                        <div class="two columns">
                                            <a href="#" class="th">
                                                <img src="/Recursos/Avatar?uid=<%: w.NRO_USUARIO.ToString() %>" /></a>
                                        </div>
                                        <div class="ten columns">
                                            <p>
                                                <%: w.MEM_LARGA %></p>
                                            <div class="msg_sep">
                                                <abbr class="timeago" title="<%: w.FEC_PUBLICADO.ToString("o") %>">
                                                    <%: w.FEC_PUBLICADO.ToString("o")%></abbr></div>
                                        </div>
                                    </div>
                                    <%
                                            }
                                        }
                                    %>
                                    <a href="#" id="bCommentario" class="button small icon-comment-alt" style="width: 140px"
                                        data-reveal-id="pMensaje" onclick="$('#mTipo').prop('value',4); $('#mEmpresaDestino').prop('value',<%: Model.NRO_EMPRESA.ToString() %>); $('#mUsuarioDestino').prop('value',0); $('#bASeguidores').prop('value','');">
                                        &nbsp;Deja tu opinión</a>
                                </blockquote>
                            </div>
                        </div>
                    </li>
                    <% 
                        var v = ViewBag.Votos as MT_VOTO;
                        var p = (v != null) ? Convert.ToInt32(v.NRO_SUMA / v.NRO_VOTOS) : 0;                        
                    %>
                    <script>
                        var havotado = false;
                        $(document).ready(function () {
                            var vt = "<%: p * 32 %>";                            
                            $("#bar").css({ width: vt + "px" });
                            $("a.voto1").click(function (e) {
                                var a = $(this);
                                e.preventDefault();
                                if (!havotado) {
                                    $.ajax({
                                        url: "/Varios/Voto/",
                                        contentType: "application/json; charset=utf-8",
                                        data: JSON.stringify( {e : <%: Model.NRO_EMPRESA %>, v : a.data("valoracion") }),
                                        type: "POST"
                                    }).done(function (data) {
                                        if (data.ok) {                                        
                                            $("#bar").css({ width : (data.d * 32) +"px" });
                                        }
                                        havotado = true;
                                        $("#pPuntaje").reveal().delay(2000).trigger('reveal:close');                                            
                                    });                                    
                                }
                            });
                        });
                    </script>
                </ul>
            </div>
        </div>
        <%: Html.Partial("Html_Empresa") %>
    </div>
    <script>
        function repintarMapa() {
            $("#simpleContained4Tab").css({ 'display': 'block' });
            $("#map_canvas2").css({ 'width': '540px', 'height': '400px' });
            mapaEmpresa();
        }
        $(document).ready(function () {
            $("#bSeguir").click(function (e) {
                e.preventDefault();
                var l = $(this);
                $.ajax({
                    url: "/Mensajes/Seguir/" + l.data("nro_empresa"),
                    contentType: "application/json; charset=utf-8",
                    type: "GET"
                }).done(function (data) {
                    l.text(" " + data.label);
                });
            });
            if ($('#scrollbar1').length > 0) {
                var oScrollbar = $('#scrollbar1');
                oScrollbar.tinyscrollbar();
                oScrollbar.tinyscrollbar_update("top");
            }
            //if ($("abbr.timeago").length > 0) {
            $("abbr.timeago").timeago();
            //}
        });
        function repintarMuro() {
            $("#simpleContained2Tab").css({ 'display': 'block' });
            var oScrollbar = $('#scrollbar1');
            oScrollbar.tinyscrollbar();
            oScrollbar.tinyscrollbar_update("top");
        }
        $(window).load(
			function () {
			    if ($('abbr.timeago').length > 0) { 
                    $('abbr.timeago').timeago(); 
                }
			    if ($('.overview img').length > 0) {
                    $('.overview img').each(function () {
			            if (!this.complete || typeof this.naturalWidth == "undefined" || this.naturalWidth == 0) {
			                this.src = '/Content/img/white.gif';
			            }                    
			        });
			    }                    
			});
		
    </script>
    <script src="/Scripts/jquery.foundation.forms.js"></script>
    <script src="/Scripts/jquery.foundation.tabs.js"></script>
    <script src="/Scripts/jquery.smartTruncation.js"></script>
    <script src="/Scripts/jquery.tinyscrollbar.min.js"></script>
    <% if (ViewBag.VP)
       {%>
    <div id="aviso">
        Ud. está viendo en el modo de Vista Previa.<br />
        <a href="#" class="button small" onclick="self.close();" style="margin: 0;">Seguir Editando</a>
    </div>
    <link rel="stylesheet" type="text/css" href="/css/jquery.pnotify.default.css" />
    <script src="/Scripts/jquery.pnotify.min.js"></script>
    <script>
        var permanotice = null;
        $(document).ready(function () {
            $("#aviso").hide();
            if (permanotice) {
                permanotice.pnotify_display();
            } else {
                permanotice = $.pnotify({
                    title: 'Vista previa',
                    text: $("#aviso").html(),
                    nonblock: false,
                    hide: false,
                    closer: false,
                    sticker: false
                });
            }
        });
    </script>
    <%}
       if (!String.IsNullOrEmpty(Model.XDE_TWITTER))
       { %>
    <script>
        jQuery(function ($) {
            $(".tweet").tweet({
                join_text: "auto",
                username: "<%: Model.XDE_TWITTER %>",
                avatar_size: 30,
                count: 3,
                auto_join_text_default: "",
                auto_join_text_ed: "",
                auto_join_text_ing: "",
                auto_join_text_reply: "",
                auto_join_text_url: "",
                loading_text: "Cargando..."
            });
        });
    </script>
    <%}%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterHolder" runat="server">
    <script src="/Scripts/jquery.timeago.js"></script>
    <%: Html.Partial("FormsReporte") %>
    <%: Html.Partial("FormMensaje") %>
    <%: Html.Partial("PopPuntaje") %>
    <% Html.RenderPartial("FormInsertarMuro", Model); %>
</asp:Content>
