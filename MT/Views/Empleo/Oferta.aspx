<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.MT_EMPLEO_OFERTA>" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Bolsa de Trabajo</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns">
        <div class="row">
            <div class="ten columns">
                <h4>
                    &laquo;
                    <%: Model.XDE_TITULO%>
                    &raquo;
                </h4>
            </div>
            <% if (User.Identity.IsAuthenticated)
               {%>
            <div class="two columns">
                <a href="#" id="bMensaje" class="button small icon-comment" data-reveal-id="mRepPostulacion" title="Mensaje"> Postulate!</a>
            </div>            
            <%} %>
        </div>
        <fieldset>
            <%: Html.HiddenFor(model => model.NRO_EMPRESA) %>
            <%: Html.HiddenFor(model => model.NRO_USUARIO)%>
            <%: Html.HiddenFor(model => model.XDE_TITULO)%>
            <h5>
                Detalle</h5>
            <%: Html.Raw(StringMethodExtensions.ToHtml(Model.MEM_LARGA))%>
            <div class="row">
                <div class="six columns">
                    <% 
                        MT_EMPRESA_PERFIL e = ViewBag.Empresa as MT_EMPRESA_PERFIL;
                        string[] direccionymapa = Varias.DireccionyMapa(e);
                    %>
                    <h5>
                        Empresa</h5>
                    <p>
                        <b>
                            <%: Html.ActionLink(e.XDE_RAZONSOCIAL, "Empresa", "Directorio", new { id = Varias.mt_Escape(e.XDE_RAZONSOCIAL) }, null)%>
                        </b>
                        <br />
                        <%: direccionymapa[0]%><br />
                        <% 
                            List<MT_TELEFONO> f = ViewBag.Telefonos as List<MT_TELEFONO>;
                            foreach (MT_TELEFONO fn in f)
                            { %>
                        <span class="icon-phone"></span>
                        <%: fn.XDE_TELEFONO %><br />
                        <%}
                            //List<MT_EMAIL> w = ViewBag.Emails as List<MT_EMAIL>;
                            /*foreach (MT_EMAIL z in w)
                            { */%>
                        <span class="icon-envelope" style="float: left;"></span>
                        <img src="/Empleo/EmailRasterizado/<%: e.NRO_EMPRESA.ToString() %>" /><br />
                        <%//}%>
                        <%if (!String.IsNullOrEmpty(e.XDE_WEB))
                          { %>
                        <span class="icon-globe"></span><a href="<%: e.XDE_WEB %>" target="_blank">
                            <%:e.XDE_WEB%></a><br />
                        <%}%>
                    </p>
                </div>
                <div class="three columns">
                    <h5>
                        Ãrea</h5>
                    <ul class="disc">
                        <% 
                            List<MT_EMPLEO_CATEGORIA> c = ViewBag.Categorias as List<MT_EMPLEO_CATEGORIA>;
                            foreach (MT_EMPLEO_CATEGORIA s in c)
                            { 
                        %><li>
                            <%: s.XDE_NOMBRE %></li><%
                            }
                            %>
                    </ul>
                </div>
                <div class="three columns">
                    <h5>
                        Modalidad</h5>
                    <ul class="circle">
                        <% 
                            List<MT_EMPLEO_TIPO> t = ViewBag.Tipos as List<MT_EMPLEO_TIPO>;
                            foreach (MT_EMPLEO_TIPO s in t)
                            { 
                        %><li>
                            <%: s.XDE_NOMBRE %></li><%
                            }
                            %>
                    </ul>
                </div>
            </div>
        </fieldset>
        <p>
            <%: Html.ActionLink("Ver otras Ofertas", "Index", null, new { @onclick = "history.go(-1); return false", @class = "button small" })%>
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterHolder" runat="server">
    <%: Html.Partial("FormPostulacion") %>
</asp:Content>
