<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% 
    using (Html.BeginForm("Respuesta", "Mensajes", FormMethod.Post, new { @id = "rspM" }))
    { %>        
    <input type="hidden" id="msg_id" name="msg_id" value="" />        
    <%: Html.TextArea("rep_mensaje", new { @spellcheck = "true" })%>
    <input type="submit" value="Responder" id="bResp" class="button small" />
<% } %>
