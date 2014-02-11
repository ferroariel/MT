<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MT.Models.MT_EMPLEO_OFERTA>>" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Bolsa de Trabajo</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% 
        BuscarEmpleoModel m = new BuscarEmpleoModel()
        {
            Buscar = String.Empty
        };
        if (ViewBag.BuscarEmpleo != null)
        {
            m = ViewBag.BuscarEmpleo as BuscarEmpleoModel;            
        }            
    %>
    <div class="two columns">
        <h5>
            Buscar</h5>
        <% Html.RenderPartial("BuscarEmpleo", m); %>
    </div>
    <div class="eight columns">
        <h4>
            Bolsa de Trabajo</h4>
        <% foreach (var item in Model)
           { %>
        <blockquote>
            <h5>
                <b>
                    <%: Html.ActionLink(item.XDE_TITULO, "Oferta", "Empleo", new { id = item.NRO_OFERTA.ToString() }, null)%></b></h5>
            &laquo;<%: Varias.Fragmento(item.MEM_LARGA,150," ", "...") %>&raquo;<cite><%: String.Format("{0:dd/MM/yyyy}", item.FEC_DESDE)%></cite></blockquote>
        <hr />
        <% } %>
        <p>
            &nbsp;</p>
    </div>    
</asp:Content>
