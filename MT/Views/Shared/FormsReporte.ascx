<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="mReporteEnlaceRoto" class="reveal-modal">
    <h4>
        Reportar Enlace Roto</h4>
    <form class="custom" id="mRepEnlaceRoto">
    <input type="hidden" name="tipo" value="20" />
    <div class="row">
        <div class="three columns">
            Reporte:</div>
        <div class="nine columns">
            <select style="display: none" name="sRER" id="sRER">
                <option value="Reporte - vínculo a sitio roto">El vínculo al sitio web está roto</option>
            </select></div>
    </div>
    <div class="row">
        <div class="three columns">
            Mensaje:</div>
        <div class="nine columns">
            <textarea name="mensaje"></textarea></div>
    </div>
    <div class="row mSuccess">
        <div class="eight columns centered">
            Tu mensaje ha sido enviado!
        </div>
    </div>
    <div class="row">
        <div class="ten columns">
        </div>
        <div class="two columns">
            <input type="button" data-form="mRepEnlaceRoto" id="bRepMee" value="Enviar" class="button small" /></div>
    </div>
    </form>
    <a class="close-reveal-modal">&#215;</a>
</div>
<div id="mReporteImpresicion" class="reveal-modal">
    <h4>
        Reportar Impresición</h4>
    <form class="custom" id="mRepImpresicion">
    <input type="hidden" name="tipo" value="21" />
    <div class="row">
        <div class="three columns">
            Mensaje:</div>
        <div class="nine columns">
            <textarea name="mensaje"></textarea></div>
    </div>
    <div class="row mSuccess">
        <div class="eight columns centered">
            Tu mensaje ha sido enviado!
        </div>
    </div>
    <div class="row">
        <div class="ten columns">
        </div>
        <div class="two columns">
            <input type="button" data-form="mRepImpresicion" id="bRepMim" value="Enviar" class="button small" /></div>
    </div>
    </form>
    <a class="close-reveal-modal">&#215;</a>
</div>
<div id="mReporteAbuso" class="reveal-modal">
    <h4>
        Reportar Abuso</h4>
    <form class="custom" id="mRepAbuso">
    <input type="hidden" name="tipo" value="22" />
    <div class="row">
        <div class="three columns">
            Mensaje:</div>
        <div class="nine columns">
            <textarea name="mensaje"></textarea></div>
    </div>
    <div class="row mSuccess">
        <div class="eight columns centered">
            Tu mensaje ha sido enviado!
        </div>
    </div>
    <div class="row">
        <div class="ten columns">
        </div>
        <div class="two columns">
            <input type="button" data-form="mRepAbuso" id="bRepMab" value="Enviar" class="button small" /></div>
    </div>
    </form>
    <a class="close-reveal-modal">&#215;</a>
</div>
<div id="mIntercambio" class="reveal-modal">
    <h4>
        Conección B2B</h4>
    <form class="custom" id="mRepIntercambio">
    <input type="hidden" name="tipo" value="20" />
    <div class="row">
        <div class="three columns">
            Tipo:</div>
        <div class="nine columns">
            <select style="display: none" id="Select1">
                <option value="Reporte - vínculo a sitio roto">El vínculo al sitio web está roto</option>
            </select></div>
    </div>
    <div class="row">
        <div class="three columns">
            Mensaje:</div>
        <div class="nine columns">
            <textarea name="mensaje"></textarea></div>
    </div>
    <div class="row mSuccess">
        <div class="eight columns centered">
            Tu mensaje ha sido enviado!
        </div>
    </div>
    <div class="row">
        <div class="ten columns">
        </div>
        <div class="two columns">
            <input type="button" data-form="mRepIntercambio" id="bRepInt" value="Enviar" class="button small" /></div>
    </div>
    </form>
    <a class="close-reveal-modal">&#215;</a>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#mRepImpresicion").validate({
            rules: {
                mensaje: { required: true }
            },
            messages: {
                mensaje: { required: "Ingrese el mensaje" }
            }
        });
        $("#mRepEnlaceRoto").validate({
            rules: {
                mensaje: { required: true }
            },
            messages: {
                mensaje: { required: "Ingrese el mensaje" }
            }
        });
        $("#mRepAbuso").validate({
            rules: {
                mensaje: { required: true }
            },
            messages: {
                mensaje: { required: "Ingrese el mensaje" }
            }
        });
        $("#mRepIntercambio").validate({
            rules: {
                mensaje: { required: true }
            },
            messages: {
                mensaje: { required: "Ingrese el mensaje" }
            }
        });
        $(".mSuccess, .mError").hide();
        $("#bRepMim, #bRepMee, #bRepMab, #bRepInt").click(function (e) {
            e.preventDefault();
            var fm = $(this).data("form");
            if ($("#" + fm).valid()) {
                $.ajax({
                    url: "/Mensajes/Reporte",
                    contentType: "application/json; charset=utf-8",
                    type: "POST",
                    async: false,
                    data: JSON.stringify(
                {
                    t: $("#" + fm).find("input[type='hidden']").val(), /*fm,*/
                    a: ($("#sRER").length > 0) ? $("#sRER option:selected").val() : "",
                    m: $("#" + fm).find("textarea").val()
                })
                }).done(function (data) {
                    if (data.ok) {
                        $("#" + fm).find(".mSuccess").removeClass("mError").show("slow").delay(2000).hide("slow", function () {
                            $("#" + fm).trigger('reveal:close');
                        });
                    } else {
                        $("#" + fm).find(".mSuccess > .centered").text("Ocurrió un error... <br />" + data.d);
                        $("#" + fm).find(".mSuccess").addClass("mError").show("slow");
                    }
                });
            }
        });
    });
</script>
