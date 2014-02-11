<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Inicio</title>
    <script type="text/javascript" src="/Scripts/moment.min.js"></script>
    <style type="text/css">
        table thead tr th {
            background:#000;
            color:#fff;
            text-align:left;
            font-weight:bold;
        }
        table tr td {
            border:solid 1px silver;
            border-bottom-width:0;
            border-right-width:0;
        }
        table {
            width:60%;
            border:solid 1px silver;
            border-top-width:0;
            border-left-width:0;
        }
    </style>
</asp:Content> 
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <form id="ajax_test">
    <p>
        Ingresar un texto para buscar en la tabla de categorías:</p>
    <p>
        <label for="query">
            Buscar:</label><input type="text" id="query" value="ropa" placeholder="Ej: ropa, short, bikini" />
        <input type="checkbox" id="lucene" checked="checked" value="1" /><label for="lucene">Usar
            Lucene</label><br />
        <label for="pagesize">
            Paginar cada
        </label>
        <input type="number" value="10" id="pagesize" style="width: 40px;" /><br />
        <label for="pagecurrent">
            Pagina:
        </label>
        <input type="number" value="1" id="pagecurrent" disabled="disabled" style="width: 40px;" /><br />
    </p>
    <hr />
    <p>
        Aplicar filtros:<br />
        <input type="checkbox" id="checkpaises" value="1" /><label for="paises">Valor1:
        </label>
        <select size="1" id="paises" style="width: 120px">
            <% 
                List<string> p = ViewData["p"] as List<string>;
                foreach (var x in p)
                {
                    Response.Write("<option value='" + x.ToString() + "'>" + x.ToString() + "</option>");
                }
            %>
        </select><br />
        <input type="checkbox" id="checknumeros" value="1" /><label for="numeros">Valor2:
        </label>
        <select size="1" id="numeros" style="width: 120px">
            <% 
                for (int i = 1000; i <= 1100; i++)
                {
                    Response.Write("<option value='" + i + "'>" + i + "</option>");
                }
            %>
        </select></p>
    <hr />    
    <p>
        <input type="button" id="buscar" value="Buscar" />
    </p>
    </form>
    <p>
        <span id="beg"></span><span id="end"></span><span id="tspan"></span><span id="cont">
        </span><span id="paginate"></span>
    </p>
    <div id="ajax">
        <p>
            Nada ...</p>
    </div>
    <div id="paginado">
    </div>
    <script type="text/javascript">
        var muro = null;
        $(document).ready(function () {
            $("#buscar").click(function () {
                if ($("#query").val() != "") {
                    $("#buscar, #lucene").attr("disabled", true);
                    $("#ajax").html("Cargando...");
                    var luc = $('#lucene:checked').length ? $('#lucene').val() : 0;
                    var BusquedaFiltros = {
                        XDE_PAIS: ($("#checkpaises:checked").length > 0) ? $("#paises").val() : "",
                        NRO_VALOR: ($("#checknumeros:checked").length > 0) ? $("#numeros").val() : 0
                    };
                    var Busqueda = {
                        UsarLucene: luc,
                        Texto: $("#query").val(),
                        PageSize: $("#pagesize").val(),
                        PageCurrent: $("#pagecurrent").val(),
                        Filtros: BusquedaFiltros
                    };
                    modoBusqueda(Busqueda, "#ajax");
                }
            });
        });
        function modoBusqueda(busq, divdestino) {
            var day = new Date();
            var myday = moment(day).format("DD/MM/YYYY H:mm:ss");
            $("#beg").html("REQ => " + myday + "<br />");
            $("#end, #tspan").html("");
            muro = $.ajax({
                url: "/Home/BuscarRubro/",
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: JSON.stringify(busq)
            }).done(function (data) {
                if (!data.success) {
                    alert("El modelo de búsqueda es inválido");
                } else {
                    var items = [];
                    $("#cont").html("RESULTADOS: <b>" + data.res.res_Contador + "</b><br />");
                    //$("#paginate").html("PAGINAS: " + data.res_Pages + "<br />");
                    uiPaginado(data.res.res_Pages, data.res.res_Current);
                    var res = "<table cellpadding=0 cellspacing=0><thead><tr><th>Rubro</th><th>Valor1</th><th>Valor2</th></tr></thead>";
                    $.each(data.res.res_Tags, function (key, val) {
                        res += "<tr>";
                        res += "<td>" + val["OBS_NOMBRE"] + "</td>";
                        res += "<td>" + val["XDE_PAIS"] + "</td>";
                        res += "<td>" + val["NRO_VALOR"] + "</td>";
                        res += "</tr>";
                    });
                    res += "</ol>";
                    $("#ajax").html(res);
                }
                $("#buscar, #lucene").attr("disabled", false);
                var day2 = new Date();
                var myday2 = moment(day2).format("DD/MM/YYYY H:mm:ss");
                $("#end").html("RES <= " + myday2 + "<br />");
                $("#tspan").html("Tardó " + moment(day2).diff(moment(day)) + " ms<br />");
            });
        }
        function uiPaginado(paginas, actual) {
            var p = parseInt(paginas);
            var a = parseInt(actual);
            var h = "...";
            var desde, hasta = 0;
            var pagsize = parseInt($("#pagesize").val());
            if (actual > 3) {
                desde = actual - 3;
            } else {
                desde = 1;
            }
            if (actual < (paginas - 3)) {
                hasta = actual + 3;
            } else {
                hasta = paginas;
            }
            if (p > 1) {
                if (a != 1) { h += '[<a href="javascript:setPg(1)" class=pgn>&laquo;</a>]'; }
                //for (var i = 1; i <= p; i++) {
                if (a > 5) {
                    h += '[<a href="javascript:setPg(1)" class=pgn>1</a>] ...';
                }
                var i = 0;
                for (var i = desde; i <= hasta; i++) {
                    h += " [";
                    if (i == a) { h += "<b>"; } else { h += '<a href="javascript:setPg(' + i + ');" class=pgn>'; }
                    h += i;
                    if (i == a) { h += "</b>"; } else { h += "</a>" }
                    h += "] ";
                }
                if (i < p) {
                    h += '... [<a href="javascript:setPg(' + p + ')" class=pgn>'+p+'</a>]';
                }
                if (a < p) { h += '[<a href="javascript:setPg(' + p + ')" class=pgn>&raquo;</a>]'; }
            } else {
                h = "1";
            }            
            $("#paginate").html("PAGS: " + h);
        }
        function setPg(i) {
            var luc = $('#lucene:checked').length ? $('#lucene').val() : 0;
            var BusquedaFiltros = {
                XDE_PAIS: ($("#checkpaises:checked").length > 0) ? $("#paises").val() : "",
                NRO_VALOR: ($("#checknumeros:checked").length > 0) ? $("#numeros").val() : 0
            };
            var Busqueda = {
                UsarLucene: luc,
                Texto: $("#query").val(),
                PageSize: $("#pagesize").val(),
                PageCurrent: i,
                Filtros: BusquedaFiltros
            };
            modoBusqueda(Busqueda, "#ajax");
        }
        $(window).unload(function () { muro.abort(); });
    </script>
</asp:Content>
