<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="MT.Models" %>
<div class="two columns">
    <h5>
        Filtros&nbsp;Aplicados</h5>
    <form id="ajax_test" class="custom">
        <input type="hidden" value="1000" id="pagesize" />
        <input type="hidden" value="1" id="pagecurrent" />
        <label for="query">
            Buscar:</label>
        <input type="search" id="query" placeholder="tu búsqueda" /><br />
        <small>3 letras como mínimo</small>
        <div id="filts">
        </div>
        <dl id="aplicarfiltros" class="sub-nav">
        </dl>
        <label for="pagecurrent">
        </label>
    </form>
    <%: Html.Partial("Html_Rubros") %>
</div>
<div class="eight columns">
    <dl class="tabs pill contained vistas">
        <dt><span id="cont">0</span> empresas</dt>
        <dd class="active">
            <a href="#simpleContained1" class="icon-align-left">&nbsp;Lista</a></dd>
        <dd class="hide-for-small">
            <a href="#simpleContained2" id="MapaTodos" class="icon-map-marker">&nbsp;Mapa</a></dd>
        <dd class="hide-for-small">
            <a href="#simpleContained3" class="icon-columns">&nbsp;Dual</a></dd>
        <dd class="hide-for-small">
            <a href="#simpleContained4" class="icon-th-list">&nbsp;Detalle</a></dd>
    </dl>
    <ul class="tabs-content contained vistas">
        <li class="active" id="simpleContained1Tab">            
            <table cellpadding="0" cellspacing="0" border="0" id="tablaresultados" class="sombra_gris">
                <thead>
                    <tr>
                        <th width="40%">
                            Empresa
                        </th>
                        <th width="35%">
                            Provincia
                        </th>
                        <th width="25%">
                            País
                        </th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
            <p>
                &nbsp;</p>
        </li>
        <li id="simpleContained2Tab">
            <div id="map_canvas2" class="map_canvas sombra_gris" style="display: block; width: 540px; height: 400px;">
            </div>            
        </li>
        <li id="simpleContained3Tab">
            <div class="row">
                <div class="seven columns">
                    <table cellpadding="0" cellspacing="0" border="0" id="tablaresultadosdual" class="sombra_gris">
                        <thead>
                            <tr>
                                <th width="200">
                                    Empresa
                                </th>
                                <th width="50">
                                    &nbsp;
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div class="five columns">
                    <div class="map_canvas sombra_gris" id="map_canvas" style="display: block; width: 100%; height: 400px">
                    </div>
                </div>
            </div>
        </li>
        <li id="simpleContained4Tab">
            <table cellpadding="0" cellspacing="0" border="0" id="tablaresultadosdetalle" class="sombra_gris">
                <thead>
                    <tr>
                        <th width="200">
                            Empresa
                        </th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </li>
    </ul>
    
    <script>
        var mt_data = null;
        var mapt = null;
        var oTable, oTableDual, oTableDet = null;
        var rbs = []; // ids de rubros
        var rbsn = []; // nombres de rubros
        var rb_f = []; // ids de rubros para filtrado
        var rb_fn = []; // nombres de rubros para filtrado        
        
        $(document).ready(function () {
            //$("dl.vistas, ul.vistas").hide();
            
            //$('#scrollbar1').tinyscrollbar();            
            $("#query")
            .keyup(function () {                                                
                $("dl.vistas dd").removeClass("active");
                $("dl.vistas dd:eq(0)").addClass("active");
                $("ul.vistas li").css({"display":"none"});
                $("ul.vistas li#simpleContained1Tab").css({"display":"block"});
                $("#map_canvas2").hide();
                
                if ($(this).val() != "") {                    
                    
                    var BusquedaFiltros = {
                        NRO_RUBRO: rb_f,
                        OBS_NOMBRE: ""
                    };
                    var Busqueda = {
                        UsarLucene: 1,
                        Texto: $(this).val(),
                        PageSize: $("#pagesize").val(),
                        PageCurrent: $("#pagecurrent").val(),
                        Filtros: BusquedaFiltros
                    };
                    modoBusqueda(Busqueda, "#ajax");
                }
            })
            .mouseleave(function(){
                $(this).blur();
            });
            <% if (Request.QueryString["buscar"]!=null) {%>
            $("#query").prop("value", "<%:Html.Raw(Request.QueryString["buscar"].ToString()) %>").trigger("keyup");
            <%}%>
        });        
        function modoBusqueda(busq, divdestino) {
            $.ajax({
                url: "/Directorio/Buscar/",
                contentType: "application/json; charset=utf-8",
                type: "POST",
                data: JSON.stringify(busq)
            }).done(function (data) {
                if (data.success) {
                    mt_data = data;
                    $("#cont").text(mt_data.res.res_Emps.length);
                    var items = [];                    
                    //if (mt_data.res.res_Emps.length > 0) {                                                
                        if (oTable)
                            oTable.fnDestroy();
                        if (oTableDual)
                            oTableDual.fnDestroy();
                        if (oTableDet)    
                            oTableDet.fnDestroy();                                        
                        $("#map_canvas").hide();
                        oTable = $('#tablaresultados').dataTable({
                            aaData: mt_data.res.res_Emps,
                            /*"oLanguage": { "sUrl": "/Scripts/es_AR.txt" },*/
                            "fnRowCallback":
                                function (nRow, aData, iDisplayIndex) {
                                    $('td:eq(0)', nRow).html('<a href="/Directorio/Empresa/' + aData[3] + '" title="' + aData[0] + '">' + aData[0] + '</a>');
                                },
                            "sPaginationType": "full_numbers",
                            "bAutoWidth": false,
                            "bStateSave": true,
                            "bFilter": false,
                            "bLengthChange": false,
                            /*"bSort": false,*/
                            "aaSorting": [ ]
                        });
                        oTableDual = $('#tablaresultadosdual').dataTable({
                            aaData: mt_data.res.res_Emps,
                            /*"oLanguage": { "sUrl": "/Scripts/es_AR.txt" },*/
                            "fnRowCallback":
                                function (nRow, aData, iDisplayIndex) {
                                    if (aData[4] != "" && aData[5] != "") {
                                        $('td:eq(1)', nRow).html('<a href="#" onclick="verenmapa(\'' + aData[4] + '\',\'' + aData[5] + '\'); return false;" title="Ver en mapa" class="icon-map-marker">&nbsp;</a>');                                                                                
                                    } else {
                                        $('td:eq(1)', nRow).html('');
                                    }
                                    $('td:eq(0)', nRow).html('<a href="/Directorio/Empresa/' + aData[3] + '" title="' + aData[0] + '">' + aData[0] + '</a>');
                                },
                            "sPaginationType": "full_numbers",
                            "bAutoWidth": false,
                            "aoColumns": [
                                    { sWidth: '200px' },
                                    { sWidth: '50px' }
                                ],
                            "bStateSave": true,
                            "bFilter": false,
                            "bLengthChange": false,
                            /*"bSort": false,*/
                            "aaSorting": [ ]
                        });
                        oTableDet = $('#tablaresultadosdetalle').dataTable({
                            aaData: mt_data.res.res_Emps,
                            "fnRowCallback":
                                function (nRow, aData, iDisplayIndex) {
                                    var r = '<table width="100%"><tr valign=top align=left>' +
                                        '<td width="20%">' +
                                        '<img src="/P/E/' + aData[8] + '/sitio.jpg" onerror="$(this).hide();" class="wait" width="120" height="80" /></td>' +
                                        '<td width="80%"><small>' + aData[6] + '</small><a class="th" href="/Directorio/Empresa/' + aData[3] + '" title="' + aData[0] + '"><h5>' + aData[0] + '</h5></a>' +
                                        '<p>' + aData[7] + '</p></td>' +
                                        '</tr></table>';
                                    $('td:eq(0)', nRow).html(r);
                                },
                            "sPaginationType": "full_numbers",
                            "bAutoWidth": false,
                            "bStateSave": true,
                            "bFilter": false,
                            "bLengthChange": false,
                            /*"bSort": false,*/
                            "aaSorting": [ ]
                        });                        
                        $("#filts").empty();
                        if (mt_data.res.res_Tags.length > 0) {
                            res = "Filtrar por:<br />";
                            res += '<select style="display:none;" id="selFilts">';
                            $.each(mt_data.res.res_Tags, function (key, val) {
                                rbs.push(parseInt(val["NRO_RUBRO"]));
                                rbsn.push(val["OBS_NOMBRE"]);
                                res += "<option value=\"addTag(" + val["NRO_RUBRO"] + ",'" + val["OBS_NOMBRE"] + "');\">" + val["OBS_NOMBRE"] + "</option>";
                            });
                            res += '</select>';
                            res += '<div class="custom dropdown">' +
                              '<a href="#" class="current opcionelegida">' + mt_data.res.res_Tags[0]["OBS_NOMBRE"] + '</a>' +
                              '<a href="#" class="selector"></a>' +
                              '<ul>';
                            $.each(mt_data.res.res_Tags, function (key, val) {
                                rbs.push(parseInt(val["NRO_RUBRO"]));
                                rbsn.push(val["OBS_NOMBRE"]);
                                //res += "<li><a href=\"addTag(" + val["NRO_RUBRO"] + ",'" + val["OBS_NOMBRE"] + "');\">" + val["OBS_NOMBRE"] + "</a></li>";
                                res += "<li>" + val["OBS_NOMBRE"] + "</li>";
                            });
                            res += '</ul>' +
                            '</div>';
                            $("#filts").append(res);
                            // sort de opciones
                            $("#selFilts").html($('#selFilts option').sort(function (x, y) {
                                return $(x).text() < $(y).text() ? -1 : 1;
                            }))
                            $("#selFilts").get(0).selectedIndex = 0;
                            // -----
                        }
                    //}
                }
                $("#selFilts").change(function () {
                    eval($(this).val())
                });                
            });
            $("#MapaTodos").click(function () {
                $("#simpleContained2Tab").css({ 'display': 'block' });
                $("#map_canvas2").css({ 'width': '540px', 'height': '400px' });
                MapearTodos();
            });
        }
        function addTag(i, n) {
            if (rb_f.indexOf(i) < 0) {
                rb_f.push(i);
                rb_fn.push(n);
                refrescarTags();
            }
        }
        function delTag(i, n) {
            var indice = rb_f.indexOf(i);
            rb_f.splice(indice, 1);
            rb_fn.splice(indice, 1);
            refrescarTags();
        }
        function refrescarTags() {
            $("#aplicarfiltros").empty().append("Fitros:");
            for (var i = 0; i < rb_f.length; i++) {
                //$("#aplicarfiltros").append("<dd class='active'><a href=\"javascript:delTag(" + rb_f[i] + ",'" + rb_fn[i] + "'); \">&nbsp;" + rb_fn[i] + "&nbsp;x</a></dd>");
                $("#aplicarfiltros").append('<span class="radius label filtro">'+ rb_fn[i] +'<a href="javascript:delTag(' + rb_f[i] + ',\'' + rb_fn[i] + '\');" class="icon-ban-circle"></a></span> ');
            }
            $("#query").trigger("keyup");
        }
        function uiPaginado(paginas, actual) {
            var p = parseInt(paginas);
            var a = parseInt(actual);
            var h = "";
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
                if (a != 1)
                    h += '<li class="arrow"><a href="javascript:setPg(1)" class=pgn>&laquo;</a></li>';
                else
                    h += '<li class="arrow unavailable"><a href="#" onclick="return false;">&laquo;</a></li>';
                var i = 0;
                for (i = desde; i <= hasta; i++) {
                    h += "<li ";
                    if (i == a)
                        h += "class=current";
                    h += '><a href="javascript:setPg(' + i + ');" class=pgn>';
                    h += i;
                    h += '</a></li>';
                }
                if (i < p) {
                    h += '<li><a href="javascript:setPg(' + p + ')" class=pgn>' + p + '</a></li>';
                }
                if (a < p) {
                    h += '<li class="arrow"><a href="javascript:setPg(' + p + ')" class=pgn>&laquo;</a></li>';
                } else {
                    h += '<li class="arrow unavailable"><a href="#" onclick="return false;">&raquo;</a></li>';
                }
            }
            $("#paginate").empty().append(h);
        }
        function setPg(i) {
            var BusquedaFiltros = {
                XDE_PAIS: "",
                NRO_VALOR: 0
            };
            var Busqueda = {
                UsarLucene: 1,
                Texto: $("#query").val(),
                PageSize: $("#pagesize").val(),
                PageCurrent: i,
                Filtros: BusquedaFiltros
            };
            modoBusqueda(Busqueda, "#ajax");
        } 
        function MapearTodos() {            
            $("#map_canvas2_nomapa").show();
            if (mt_data.res.res_Emps.length > 0) {                
                //pausecomp(3000);
                var d = mt_data.res.res_Emps;
                var latlng = new google.maps.LatLng(d[4], d[5]);
                mapt = new google.maps.Map(document.getElementById("map_canvas2"), {
                    zoom: 0,
                    mapTypeId: google.maps.MapTypeId.ROADMAP
                });
                var count = 0;
                for (var i = 0; i < d.length; i++) {
                    if (d[i][4] != "" && d[i][5] != "") {
                        var latlng = new google.maps.LatLng(d[i][4], d[i][5]);
                        new google.maps.Marker({
                            position: latlng,
                            map: mapt,
                            title: d[i][0]
                        });
                        count++;
                    }
                }
                if (count > 0) {
                    $("#map_canvas2_nomapa").hide();
                    $("#map_canvas2").show();
                    var latlngbounds = new google.maps.LatLngBounds();
                    for (var i = 0; i < d.length; i++) {
                        if (d[i][4] != "" && d[i][5] != "") {
                            var latlng = new google.maps.LatLng(d[i][4], d[i][5]);
                            latlngbounds.extend(latlng);
                        }
                    }
                    mapt.fitBounds(latlngbounds);
                    new google.maps.Rectangle({
                        bounds: latlngbounds,
                        map: mapt,
                        fillColor: "#000000",
                        fillOpacity: 0.2,
                        strokeWeight: 0
                    });
                }
            /*} else {
                $("#map_canvas").hide().wrap("<div id=nohay />");
                $("#nohay").html("No disponible para esta búsqueda...");*/
            }            
        }
        //google.maps.event.addDomListener(window, 'load', map1_initialize);
    </script>
    <%//: Html.Partial("IndiceTags", ViewBag.Tags as IQueryable<MT_TAG>) %>
    
    <script src="/Scripts/jquery.tinyscrollbar.min.js"></script>
    <script src="/Scripts/jquery.foundation.forms.js"></script>
    <script src="/Scripts/jquery.foundation.tabs.js"></script>
    <script src="/Scripts/jquery.dataTables.min.js"></script>


    <%: Html.Partial("Html_QuienesSomos") %>

</div>
