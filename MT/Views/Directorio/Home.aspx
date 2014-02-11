<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Mercado Textil - Red Social del Sector</title>
    <script src="/Scripts/jcarousellite_1.0.1.min.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns home1" style="margin-top: -30px">
        <div class="row home1">
            <h5>
                Aquí hay un lugar para vos<br />
                y para tu negocio<br />
                <a href="/Directorio" class="button icon-thumbs-up icon-large">&nbsp;Unite!</a></h5>
        </div>
        <div class="row" id="HomeSlider">
            <div class="twelve columns">
                <div class="row">
                    <div class="seven columns centered">
                        <% using (Html.BeginForm("Index", "Directorio", FormMethod.Get, new { @id = "bsHm" }))
                           {%>
                        <div class="row">
                            <div class="nine mobile-six columns">
                                <input type="text" name="buscar" id="bs" placeholder="Ej: ropa femenina" />
                            </div>
                            <div class="three mobile-six columns">
                                <input type="submit" id="bsHmSch" class="button big" value="   Buscar   " />
                            </div>
                        </div>
                        <%} %>
                    </div>
                </div>
                <div class="row">
                    <div class="twelve columns" id="vSlider">
                        <ul>
                            <li><a title="Hilandería" href="/Directorio?buscar=hilanderia">
                                <img alt="Hilandería" src="/Content/img/slider_hilanderia.jpeg" /><br />
                                Hilandería</a></li>
                            <li><a title="Tejeduría" href="/Directorio?buscar=tejeduria">
                                <img alt="Tejeduría" src="/Content/img/slider_tejeduria.jpeg" /><br />
                                Tejeduría</a></li>
                            <li><a title="Tintorería" href="/Directorio?buscar=tintoreria">
                                <img alt="Tintorería" src="/Content/img/slider_tintoreria.jpeg" /><br />
                                Tintorería</a></li>
                            <li><a title="Confección de Indumentaria" href="/Directorio?buscar=confeccion">
                                <img alt="Confección" src="/Content/img/slider_confeccion.jpeg" /><br />
                                Confección</a></li>
                            <li><a title="Marroquinería" href="/Directorio?buscar=marroquineria">
                                <img alt="Marroquinería" src="/Content/img/slider_marroquineria.jpeg" /><br />
                                Marroquinería</a></li>
                            <li><a title="Prendas de Cuero" href="/Directorio?buscar=cuero">
                                <img alt="Prendas de Cuero" src="/Content/img/slider_cuero.jpeg" /><br />
                                Prendas de Cuero</a></li>
                            <li><a title="Ropa Teen" href="/Directorio?buscar=teens">
                                <img alt="Ropa Teen" src="/Content/img/slider_teens.jpeg" /><br />
                                Teens</a></li>
                            <li><a title="Novias y 15 años" href="/Directorio?buscar=novias+y+15+anos">
                                <img alt="Novias y 15 años" src="/Content/img/slider_novias.jpeg" /><br />
                                Novias y 15 años</a></li>
                            <li><a title="Alta Costura" href="/Directorio?buscar=alta+costura">
                                <img alt="Alta Costura" src="/Content/img/slider_costura.jpeg" /><br />
                                Alta Costura</a></li>
                            <li><a href="#">
                                <img src="/Content/img/ban-1.gif" /><br />
                                &nbsp;&nbsp;&nbsp;&nbsp;</a></li>
                            <li><a title="Proveedores" href="/Directorio?buscar=proveedores">
                                <img alt="Proveedores" src="/Content/img/slider_proveedores.jpeg" /><br />
                                Proveedores</a></li>
                            <li><a title="Lencería" href="/Directorio?buscar=lenceria">
                                <img alt="Lencería" src="/Content/img/slider_lenceria.jpeg" /><br />
                                Lenceria</a></li>
                            <li><a title="Diseño" href="/Directorio?buscar=diseno">
                                <img alt="Diseño" src="/Content/img/slider_diseno.jpeg" /><br />
                                Diseño</a></li>
                            <li><a title="Moldería" href="/Directorio?buscar=molderia">
                                <img alt="Moldería" src="/Content/img/slider_molderia.jpeg" /><br />
                                Molderia</a></li>
                            <li><a title="Hilos para Bordados" href="/Directorio?buscar=hilos+para+bordar">
                                <img alt="Hilos para Bordados" src="/Content/img/slider_hilos.jpeg" /><br />
                                Hilos para Bordados</a></li>
                            <li><a title="Lavado" href="/Directorio?buscar=lavado">
                                <img alt="Lavado" src="/Content/img/slider_lavado.jpeg" /><br />
                                Lavado</a></li>
                            <li><a title="Jeans" href="/Directorio?buscar=jean">
                                <img alt="Jeans" src="/Content/img/slider_jean.jpeg" /><br />
                                Jeans</a></li>
                        </ul>
                        <a href="#" class="prev"><span></span></a><a href="#" class="next"><span></span>
                        </a>
                    </div>
                </div>
            </div>
        </div>
        <div class="row home2 panel">
            <div class="one columns">
            </div>
            <div class="ten columns">
                <ul class="block-grid three-up mobile-one-up">
                    <li>
                        <h5>
                            Unite a la Comunidad Textil más grande de Latinoamérica</h5>
                    </li>
                    <li>
                        <h5>
                            Agregá tus productos y servicios a nuestra Comunidad</h5>
                    </li>
                    <li>
                        <h5>
                            Conectate con clientes, proveedores y empresas</h5>
                    </li>
                </ul>
                <ul class="block-grid three-up">
                    <li>
                        <img src="/Content/img/exchange.jpg" />
                    </li>
                    <li>
                        <img src="/Content/img/buss.jpg" /></li>
                    <li>
                        <img src="/Content/img/office-people.jpg" /></li>
                </ul>
                <ul class="block-grid three-up mobile-one-up">
                    <li>
                        <p>
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec adipiscing facilisis
                            libero sit amet vulputate. Phasellus at lacus eu diam venenatis imperdiet. Duis
                            ac nibh et libero vehicula sagittis.
                        </p>
                    </li>
                    <li>
                        <p>
                            Sed tempor, arcu ut consequat posuere, lorem tellus sollicitudin ipsum, vitae consequat
                            massa dui ut sapien. Nulla dapibus, urna quis molestie tristique, arcu nulla dapibus
                            sapien
                        </p>
                    </li>
                    <li>
                        <p>
                            Ac placerat leo diam sit amet enim. Vestibulum libero sapien, blandit et dignissim
                            vel, molestie ac justo. Cras rhoncus, tortor id suscipit dictum, urna ligula interdum
                            nulla, eu malesuada lorem velit ut neque.
                        </p>
                    </li>
                </ul>
            </div>
            <div class="one columns">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="BannerContent" runat="server">
    <div class="two columns" id="MuroContenedor">
        <h5>
            Poné tu empresa aquí!</h5>
        <div class="row" id="pArbol" style="display: block; margin-bottom: 15px">
            <div id="scrollbar1" class="twelve columns">
                <div class="scrollbar">
                    <div class="track">
                        <div class="thumb">
                            <div class="end">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="viewport" style="width: 85% !important">
                    <div class="overview" style="width: 95%">
                        <div id="vMensajes" class="murohome">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="/Scripts/jquery.tinyscrollbar.min.js"></script>
    <script>
        (function ($) {
            $.fn.shuffle = function () {
                var allElems = this.get(),
            getRandom = function (max) {
                return Math.floor(Math.random() * max);
            },
            shuffled = $.map(allElems, function () {
                var random = getRandom(allElems.length),
                    randEl = $(allElems[random]).clone(true)[0];
                allElems.splice(random, 1);
                return randEl;
            });
                this.each(function (i) {
                    $(this).replaceWith($(shuffled[i]));
                });
                return $(shuffled);
            };
        })(jQuery);

        var oScrollbar = null;
        $(document).ready(function () {
            $("#vSlider ul li").shuffle();
            $("#vSlider").jCarouselLite({
                btnNext: ".next",
                btnPrev: ".prev",
                circular: true,
                hoverPause: true,
                visible: 6,
                animation: "slow",
                auto: 1,
                scroll: 1,
                speed: 1000
            });
            oScrollbar = $('#scrollbar1');
            oScrollbar.tinyscrollbar();
            $.getJSON(
                "/Recursos/CargarMuro",
                null,
                function (data) {
                    d = data;
                    var li = "";
                    if (data.ok && data.d.length > 0) {
                        $.each(data.d, function (key, val) {
                            li += '<p class=conversacion>';
                            li += (val["t"] != null) ? '<b>' + val["t"] + '</b>' : '';
                            li += val["m"] + '</p><div class="msg_sep">' +
                                '   <abbr class="timeago" title="' + val["x"] + '">' +
                                    val["x"] +
                                '   </abbr>' +
                                '</div>';
                        });
                        //$("#pArbol").css({ "height": $("#MuroContenedor").height() + "px" })
                        $("#vMensajes").empty()/*.css({ height: "550px" }).*/.append(li);
                        $("#pArbol, #scrollbar1").css({ "height": $(".home1").height() });
                        $("abbr.timeago").timeago();
                        //$("img").error(function () { $(this).unbind("error").attr("src", "/Content/img/imagennoencontrada.png"); });
                        $("#vMensajes p:first").before('<p><a href="#"><img src="/Content/img/ban-1.gif" /></a></p>');
                    }
                    NuevosMensajes();
                });
        });
        $(window).load(function () {
            oScrollbar.tinyscrollbar_update("top");
            $('.overview img').each(function () {
                if (!this.complete || typeof this.naturalWidth == "undefined" || this.naturalWidth == 0) {
                    this.src = '/Content/img/white.gif';
                }
            });
        });
        function NuevosMensajes() {
            $.getJSON(
                "/Recursos/ChequearMuro",
                null,
                function (data) {
                    if (data.ok && data.d.length > 0) {
                        var li = "";
                        $.each(data.d, function (key, val) {
                            li += '<p class=conversacion>';
                            li += (val["t"] != null) ? '<b>' + val["t"] + '</b>' : '';
                            li += val["m"] + '</p><div class="msg_sep">' +
                                    '   <abbr class="timeago" title="' + val["x"] + '">' +
                                        val["x"] +
                                    '   </abbr>' +
                                    '</div>';
                        });
                        $("#vMensajes").prepend(li);
                        $("abbr.timeago").timeago();
                        oScrollbar.tinyscrollbar_update("top");
                    }
                    NuevosMensajes();
                });
        }
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="FooterHolder" runat="server">
    <script src="/Scripts/jquery.timeago.js"></script>
</asp:Content>
