<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MT.Models.MT_EMPRESA_PERFIL>" %>
<% using (Html.BeginForm("Empresa", "Directorio", FormMethod.Post, new { @id = "VP", @target = "_blank" }))
   {%>
<input type="hidden" name="VistaPrevia" value="1" />
<%: Html.HiddenFor(model => model.NRO_EMPRESA, new { @id = "vp_NRO_EMPRESA" })%>
<%: Html.HiddenFor(model => model.XDE_RAZONSOCIAL, new { @id = "vp_XDE_RAZONSOCIAL" })%>
<%: Html.HiddenFor(model => model.XDE_CORTA, new { @id = "vp_XDE_CORTA" })%>
<%: Html.HiddenFor(model => model.OBS_MEDIA, new { @id = "vp_OBS_MEDIA" })%>
<%: Html.HiddenFor(model => model.MEM_LARGA, new { @id = "vp_MEM_LARGA" })%>
<%: Html.HiddenFor(model => model.XDE_CIUDAD, new { @id = "vp_XDE_CIUDAD" })%>
<%: Html.HiddenFor(model => model.XDE_DOMICILIO, new { @id = "vp_XDE_DOMICILIO" })%>
<%: Html.HiddenFor(model => model.XDE_CP, new { @id = "vp_XDE_CP" })%>
<%: Html.HiddenFor(model => model.XDE_WEB, new { @id = "vp_XDE_WEB" })%>
<%: Html.HiddenFor(model => model.XDE_FACEBOOK, new { @id = "vp_XDE_FACEBOOK" })%>
<%: Html.HiddenFor(model => model.XDE_TWITTER, new { @id = "vp_XDE_TWITTER" })%>
<%: Html.HiddenFor(model => model.NRO_PAIS, new { @id = "vp_NRO_PAIS" })%>
<%: Html.HiddenFor(model => model.NRO_PROVINCIA, new { @id = "vp_NRO_PROVINCIA" })%>
<%: Html.HiddenFor(model => model.XDE_LAT, new { @id = "vp_XDE_LAT" })%>
<%: Html.HiddenFor(model => model.XDE_LON, new { @id = "vp_XDE_LON" })%>
<% } %>
