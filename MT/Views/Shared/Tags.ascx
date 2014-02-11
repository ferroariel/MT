<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<MT.Models.MT_TAG>>" %>
<ul>
    <% foreach (var item in Model)
       { %>
    <li>
        <%: Html.DisplayFor(modelItem => item.OBS_NOMBRE) %>
        (<%: Html.DisplayFor(modelItem => item.NRO_RUBRO) %>)
        <% } %>
</ul>
