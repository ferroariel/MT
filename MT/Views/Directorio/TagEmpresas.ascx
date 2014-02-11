<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<MT.Models.MT_EMPRESA_PERFIL>>" %>
<%@ Import Namespace="MT.Models" %>

<ol>
<% 
    Varias v = new Varias();
    foreach (var item in Model) { %>
    <li><b><%: Html.ActionLink(item.XDE_RAZONSOCIAL, "Empresa", new { id = Varias.mt_Escape(item.XDE_RAZONSOCIAL) })%></b></li>        
<% } %>
</ol>
