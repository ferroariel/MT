<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Tag</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns">
        <ul class="breadcrumbs">
            <li>
                <%:Html.ActionLink("Directorio","Index","Directorio") %></li>
            <%:Html.Raw( ViewBag.TagsPath)%>
        </ul>
        
        <h4><%: Html.Raw( ViewBag.NombreTag ) %></h4>
        <%: Html.Partial("IndiceTags", ViewBag.Tags as IQueryable<MT_TAG>) %>
        <%: Html.Partial("TagEmpresas", ViewBag.TagEmpresas as IQueryable<MT_EMPRESA_PERFIL>) %>
        
    </div>
</asp:Content>
