<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="MT.Models" %>
<% if (Model != null)
   {
       MTrepository mtdb = new MTrepository();
%>
<div id="pMensaje" class="reveal-modal">
    <h4>
        Nuevo Mensaje</h4>
    <form id="fMensaje">
    <div class="row">
        <div class="two columns">
            <a href="#" class="th">
                <img src="/Recursos/Avatar" style="width: 60px; height: 60px" /></a>
        </div>
        <div class="ten columns">
            <input type="hidden" id="mTipo" value="3" />
            <input type="hidden" id="mEmpresaDestino" value="<%: Model.NRO_EMPRESA.ToString() %>" />
            <input type="hidden" id="mUsuarioDestino" value="<%: mtdb.AutorDeEmpresa(Model.NRO_EMPRESA).ToString() %>" />
            <textarea name="mMensaje" id="mMensaje" placeholder="Mensaje"></textarea>
            <input type="button" value="Enviar" onclick="enviarMsg()" id="bEnviarMensaje" class="button" />
            <%
       if (!String.IsNullOrEmpty(ViewBag.Autor))
       {%>
            <input type="hidden" id="bASeguidores" value="1" />            
            <%} %>
        </div>
    </div>
    </form>
    <a class="close-reveal-modal">&#215;</a>
</div>
<script>
    $(document).ready(function () {
        $("#fMensaje").validate({
            rules: {
                mMensaje: { required: true }
            },
            messages: {
                mMensaje: { required: "Ingrese el mensaje" }
            }
        });
    });
    function enviarMsg() {
        if ($("#fMensaje").valid()) {
            var t = $("mTipo").val();
            $.ajax({
                url: "/Mensajes/Mensaje",
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: JSON.stringify({ ud: $("#mUsuarioDestino").val(), ed: $("#mEmpresaDestino").val(), a: ""/*$("#mAsunto").val()*/, m: $("#mMensaje").val(), s: $("#bASeguidores").val(), t: $("#mTipo").val() })
            }).done(function (data) {
                $('#pMensaje').trigger('reveal:close');
            });
        }
    }
</script>
<% } %>