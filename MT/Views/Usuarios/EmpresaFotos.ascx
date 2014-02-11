<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<MT.Models.MT_RECURSO>>" %>
<%@ Import Namespace="MT.Models" %>
<fieldset>
    <legend>Galería de Fotos</legend>
    <% 
        string gal_id = "";
        string emp_id = "";
        string usr_id = "";
        foreach (var item in Model)
        { %>
    <img src="/P/E/<%: Html.Raw(item.NRO_EMPRESA.ToString()) %>/<%: Html.Raw(item.XDE_ARCHIVO) %>"
    width="40" height="40" />
    <b><%: Html.ActionLink("X", "BorrarFoto", "Usuarios", new { id = item.NRO_RECURSO }, null)%></b>
    
    <% 
            gal_id = item.NRO_GALERIA.ToString();
            emp_id = item.NRO_EMPRESA.ToString();
            usr_id = item.NRO_USUARIO.ToString();
        } %>
<% using (Html.BeginForm("AgregarFoto", "Usuarios", FormMethod.Post, new { enctype = "multipart/form-data" }))
        { %>
<%: Html.Hidden("NRO_EMPRESA", emp_id) %>
<%: Html.Hidden("NRO_USUARIO", usr_id) %>
<%: Html.Hidden("NRO_GALERIA", gal_id) %>
<input type=file name=XDE_ARCHIVO value />
<input type="submit" value="Agregar Foto" />
<%} %>
</fieldset>