<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<MT.Models.MT_TELEFONO>>" %>
<ul>
    <% foreach (var item in Model)
       { %>
    <li>
        <%: Html.DisplayFor(modelItem => item.XDE_TELEFONO) %>
    </li>
    <% } %>
</ul>
