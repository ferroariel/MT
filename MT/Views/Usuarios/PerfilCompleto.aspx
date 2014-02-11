<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.DatosdeUsuario>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PerfilCompleto
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>PerfilCompleto</h2>

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>DatosdeUsuario</legend>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.nro_usuario) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.nro_usuario) %>
            <%: Html.ValidationMessageFor(model => model.nro_usuario) %>
        </div>

        <div class="editor-label">
            <%: Html.LabelFor(model => model.avatar) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.avatar) %>
            <%: Html.ValidationMessageFor(model => model.avatar) %>
        </div>

        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>
<% } %>

<div>
    <%: Html.ActionLink("Back to List", "Index") %>
</div>

</asp:Content>
