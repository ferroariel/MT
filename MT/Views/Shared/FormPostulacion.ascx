<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="MT.Models" %>
<% 
    MTrepository mtdb = new MTrepository();
    if (String.IsNullOrEmpty(ViewBag.Autor))
    {%>
<div id="mRepPostulacion" class="reveal-modal">
    <h4>
        Postulación a Oferta</h4>
    <form id="fMensaje">
    <div class="row">
        <div class="two columns th">
            <img src="/Recursos/Avatar" style="width: 60px; height: 60px" />
        </div>
        <div class="nine columns">
            <div class="row">
                <input type="hidden" id="mEmpresaDestino" value="<%: Model.NRO_EMPRESA.ToString() %>" />
                <input type="hidden" id="mUsuarioDestino" value="<%: mtdb.AutorDeEmpresa(Model.NRO_EMPRESA).ToString() %>" />
                <input type="hidden" name="mAsunto" id="mAsunto" value="PostulaciÃ³n/Ref#<%: Model.NRO_OFERTA.ToString() %>" />
                <textarea name="mMensaje" spellcheck="true" id="mMensaje">Deseo postularme a esta oferta de empleo</textarea>
                <input type="submit" value="Enviar" id="bEnviarMensaje" class="button small" />
            </div>
        </div>
        <div class="one columns"></div>
    </div>
    </form>
    <a class="close-reveal-modal">&#215;</a>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#bEnviarMensaje").click(function (e) {
            e.preventDefault();
            $.ajax({
                url: "/Mensajes/Mensaje",
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: JSON.stringify({ ud: $("#mUsuarioDestino").val(), ed: $("#mEmpresaDestino").val(), a: $("#mAsunto").val(), m: $("#mMensaje").val() })
            }).done(function (data) {
                //$("#pMensaje").hide();
            });
        });
    });        
</script>
<%}%>