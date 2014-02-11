<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<MT.Models.MT_TAG>>" %>
<%@ Import Namespace="MT.Models" %>

<ul class="panel">
    <% 
        MTrepository mtdb = new MTrepository();
        foreach (var item in Model)
       { %>
    <li>
        <%: Html.ActionLink(item.OBS_NOMBRE, "Tag", new { id = item.NRO_RUBRO }) %>
        <b><%: mtdb.TagEmpresas(item.NRO_RUBRO) %></b> /
        <%: mtdb.TagHijas(item.NRO_RUBRO) %>
        
        </li>
    <% } %>
</ul>

