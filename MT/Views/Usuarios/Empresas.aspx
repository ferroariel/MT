<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<MT.Models.MT_EMPRESA_PERFIL>>" %>
<%@ Import Namespace="MT.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Perfil</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div class="ten columns">
        <dl class="tabs contained tabsempresa">
            <dd>
                <a href="/Mensajes">Mensajes y Notificaciones</a></dd>
            <dd>
                <a href="/Usuarios/Perfil">Mi Perfil</a></dd>
            <dd class="active">
                <a href="#simpleContained1">Mis Empresas</a></dd>
        </dl>
        <ul class="tabs-content contained">            
            <li class="simpleContained1Tab active">
                <h4>
                    Mis Empresas:</h4>
                <% 
                    MTrepository mtdb = new MTrepository();
                    %>
                <fieldset>
                    
                    <%
                        if (Model.Count()>0)
                        {%>
                    <table cellspacing=0 cellpadding=0>
                        <%  
                            foreach (MT_EMPRESA_PERFIL em in Model)
                            {   
                        %>
                        <tr>
                            <td><b><%: Html.Raw(em.XDE_RAZONSOCIAL) %></b></td>
                            <td><%: Html.ActionLink(" ", "Empresa", "Usuarios", new { id = em.NRO_EMPRESA.ToString() }, new { @style = "width:32px", @class = "button small icon-wrench", @title = "Modificar" })%></td>
                            <td><%: Html.ActionLink(" ", "Empresa", "Directorio", new { id = Varias.mt_Escape(em.XDE_RAZONSOCIAL) }, new { @style = "width:32px", @target = "_blank", @class = "button small icon-external-link", @title = "Ver" })%></td>
                            <td><%: Html.ActionLink(" ", "BorrarEmpresa", "Usuarios", new { id = em.NRO_EMPRESA }, new { @onclick = "if (confirm('Está seguro?')) { return true } else { return false }", @style = "width:32px", @class = "button small icon-minus-sign", @title = "Eliminar" })%></td>                            
                        </tr>
                        <%
                                //}
                            }%>
                    </table>
                    <%}
                        else { %>
                        <p><i>No has agregado ninguna empresa...</i></p>
                        <%}%>
                    <p>
                        <b>
                            <%: Html.ActionLink("Agregar Empresa", "CrearEmpresaPerfil", "Usuarios", null, new { @class = "button" })%></b></p>
                </fieldset>
            </li>            
        </ul>
    </div>
    <script src="/Scripts/foundation.min.js"></script>
    <script src="/Scripts/app.js"></script>

</asp:Content>
