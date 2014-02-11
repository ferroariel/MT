<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.MT_EMPRESA_PERFIL>" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Perfil de Empresa</title>
    <script src="http://maps.google.com/maps/api/js?sensor=false"></script>
    <style type="text/css">
        #mMensaje, #mMensaje-aloha
        {
            width: 100% !important;
            height: 150px !important;
        }
    </style>
    <%if (!String.IsNullOrEmpty(Model.XDE_LAT) && !String.IsNullOrEmpty(Model.XDE_LON))
      {%>
    <script type="text/javascript">
        function initialize() {            
            var latlng = new google.maps.LatLng(<%: Html.Raw(Model.XDE_LAT) %>, <%: Html.Raw(Model.XDE_LON) %>);
            var myOptions = {
              zoom: 15,
              center: latlng,
              mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            var map = new google.maps.Map(document.getElementById("map_canvas"),myOptions);
            var marker = new google.maps.Marker({
                map: map,
                position: latlng
            });
            //$("#map_canvas").slideDown();         
            $("#cargarlatylon").trigger("click");
        }        
        google.maps.event.addDomListener(window, 'load', initialize);
    </script>
    <%}%>
    <script type="text/javascript">
        var editor1, editor2 = null;
        function verenmapa(address) {
            var mapDiv = document.getElementById('map_canvas');
            var geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    var map = new google.maps.Map(mapDiv, {
                        center: results[0].geometry.location,
                        zoom: 15,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    });
                    //map.setCenter(results[0].geometry.location);
                    var marker = new google.maps.Marker({
                        map: map,
                        position: results[0].geometry.location
                    });
                    $("#XDE_LAT").attr("value", new String(results[0].geometry.location.lat()).substr(0, 15));
                    $("#XDE_LON").attr("value", new String(results[0].geometry.location.lng()).substr(0, 15));
                }
            });
        }
    </script>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
    <script src="/Scripts/CKEditor/ckeditor.js"></script>
    <script src="/Scripts/CKEditor/assets/uilanguages/languages.js"></script>
    <link rel="stylesheet" type="text/css" href="/css/uploadify.css" />
    <script src="/Scripts/jquery.uploadify.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns">
        <div class="row">
            <div class="nine columns">
                <h4>
                    <%: Model.XDE_RAZONSOCIAL%></h4>
            </div>
            <div class="three columns">
                <%: Html.ActionLink("Mis Empresas","Empresas","Usuarios",null,new{ @class="button small" }) %></div>
        </div>
        <dl class="tabs contained tabsempresa five-up">
            <dd class="active">
                <a href="#simpleContained1">Datos de Empresa</a></dd>
            <dd>
                <a href="#simpleContained2" onclick="cargarMuro();">Muro Público</a></dd>
            <dd>
                <%: Html.ActionLink("Ofertas de Empleo", "Agregar", "Empleo", new { id = Model.NRO_EMPRESA, nombre = Model.XDE_RAZONSOCIAL }, null)%></dd>
        </dl>
        <ul class="tabs-content contained">
            <li class="active" id="simpleContained1Tab">
                <ul class="block-grid two-up">
                    <li>
                        <h5>
                            Logo</h5>
                        <fieldset>
                            <% 
                                MT_RECURSO a = ViewBag.Logo as MT_RECURSO;                        
                            %>
                            <div class="row">
                                <div class="five columns">
                                    <a href="#" class="th">
                                        <img src="/Recursos/Avatar?eid=<%: a.NRO_EMPRESA.ToString() %>" /></a>
                                </div>
                                <div class="seven columns">
                                    <input type="file" name="archivo" id="archivo" />
                                </div>
                            </div>
                        </fieldset>
                        <script>
                            $(document).ready(function () {
                                $('#archivo').uploadify({
                                    'swf': '/Scripts/uploadify.swf',
                                    'uploader': '/Varios/Archivo/?uid=<%: a.NRO_USUARIO.ToString() %>&eid=<%: a.NRO_EMPRESA.ToString() %>&irc=<%: a.NRO_RECURSO.ToString() %>&itc=<%: "10" %>',
                                    'auto': true,
                                    'buttonClass': 'button icon-paper-clip',
                                    'buttonText': '',
                                    'method': 'post',
                                    'onUploadSuccess': function (file, data, response) {
                                        $('img[src*="vatar?eid"]').imgrefresh();
                                        //CargarMuro();
                                    }
                                });
                            });                    
                        </script>
                    </li>
                    <li>
                        <h5>
                            Imagen de Fondo</h5>
                        <fieldset>
                            <% 
                                MT_RECURSO f = ViewBag.FotoGrande as MT_RECURSO;                                
                            %>
                            <div class="row">
                                <div class="ten columns">
                                    <a href="#" class="th">
                                        <img id="iFondo" src="/Recursos/Fondo?uid=<%: f.NRO_USUARIO.ToString() %>&eid=<%: f.NRO_EMPRESA.ToString() %>" /></a>
                                </div>
                                <div class="two columns">
                                    <input type="file" name="archivo" id="File2" />
                                </div>
                            </div>
                        </fieldset>
                        <script>
                            $(document).ready(function () {
                                $('#File2').uploadify({
                                    'swf': '/Scripts/uploadify.swf',
                                    'uploader': '/Varios/Archivo/?uid=<%: f.NRO_USUARIO.ToString() %>&eid=<%: f.NRO_EMPRESA.ToString() %>&irc=<%: f.NRO_RECURSO.ToString() %>&itc=<%: "8" %>',
                                    'auto': true,
                                    'buttonClass': 'button icon-paper-clip',
                                    'buttonText': '',
                                    'method': 'post',
                                    'onUploadSuccess': function (file, data, response) {
                                        $('#iFondo').imgrefresh();
                                        //CargarMuro();
                                    }
                                });
                            });                    
                        </script>
                    </li>
                </ul>
                <% using (Html.BeginForm("Empresa", "Usuarios", FormMethod.Post, new { @id = "fPfEd", @class = "custom" }))
                   { %>
                <h5>
                    Datos de la Empresa</h5>
                <fieldset id="perfil">
                    <%: Html.ValidationSummary(true)%>
                    <%: Html.HiddenFor(model => model.NRO_EMPRESA_PERFIL)%>
                    <%: Html.HiddenFor(model => model.NRO_EMPRESA)%>
                    <%: Html.HiddenFor(model => model.XDE_LAT)%>
                    <%: Html.HiddenFor(model => model.XDE_LON)%>
                    <p>
                        Razón Social:
                        <%: Html.EditorFor(model => model.XDE_RAZONSOCIAL)%>
                        <%: Html.ValidationMessageFor(model => model.XDE_RAZONSOCIAL)%>
                    </p>
                    <p>
                        Resumen:
                        <%: Html.EditorFor(model => model.XDE_CORTA)%>
                        <%: Html.ValidationMessageFor(model => model.XDE_CORTA)%>
                    </p>
                    <p>
                        Descripción extendida:
                        <%: Html.TextAreaFor(model => model.OBS_MEDIA)%>
                        <%: Html.ValidationMessageFor(model => model.OBS_MEDIA)%>
                    </p>
                    <%: Html.HiddenFor(model => model.MEM_LARGA)%>
                    <p>
                        Productos y Servicios (texto enrriquecido):
                        <%: Html.TextArea("MEM_LARGA_EDITOR", Model.MEM_LARGA)%>
                        <%//: Html.ValidationMessageFor(model => model.MEM_LARGA)%>
                    </p>
                </fieldset>
                <h5>
                    Ubicación</h5>
                <fieldset>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Domicilio:</label>
                        </div>
                        <div class="ten columns">
                            <p>
                                <%: Html.EditorFor(model => model.XDE_DOMICILIO)%>
                                <%: Html.ValidationMessageFor(model => model.XDE_DOMICILIO)%></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Código Postal:</label>
                        </div>
                        <div class="four columns">
                            <p>
                                <%: Html.EditorFor(model => model.XDE_CP)%>
                                <%: Html.ValidationMessageFor(model => model.XDE_CP)%></p>
                        </div>
                        <div class="two columns">
                            <label class="inline">
                                Ciudad:</label>
                        </div>
                        <div class="four columns">
                            <p>
                                <%: Html.EditorFor(model => model.XDE_CIUDAD)%>
                                <%: Html.ValidationMessageFor(model => model.XDE_CIUDAD)%></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                País:</label>
                        </div>
                        <div class="four columns">
                            <p>
                                <%: Html.DropDownList("NRO_PAIS", ViewBag.ComboPaises as IEnumerable<SelectListItem>)%>
                                <%//: Html.ValidationMessageFor(m => Model.MT_PAISE.XDE_PAIS)%></p>
                        </div>
                        <div class="two columns">
                            <label class="inline">
                                Provincia:</label>
                        </div>
                        <div class="four columns">
                            <p>
                                <%: Html.DropDownList("NRO_PROVINCIA", ViewBag.ComboProvincias as IEnumerable<SelectListItem>) %>
                                <%//: Html.ValidationMessageFor(m => Model.MT_PROVINCIA.XDE_PROVINCIA)%></p>
                        </div>
                        
                    </div>
                    <div class="row collapse">
                        <div class="one columns">
                            <input type="checkbox" value="1" onchange="getCoords(this)" id="cargarlatylon" name="cargarlatylon" 
                            <% 
                            if (!String.IsNullOrEmpty(Model.XDE_LON + Model.XDE_LAT)) {%> checked="checked"  <%}%>/></div>
                        <div class="eleven columns">
                            Mostrar dirección en <a href="http://maps.google.com.ar/" target="_blank">Google Maps</a>.</div>
                    </div>
                    <p>
                        &nbsp;</p>
                    <div id="map_canvas" class="map_canvas" style="display: block; width: 300px; height: 200px">
                    </div>                    
                </fieldset>
                <h5>
                    Redes Sociales:</h5>
                <fieldset>
                    <div class="row collapse">
                        <div class="two columns">
                            <label class="inline">
                                Página web:</label>
                        </div>
                        <div class="one mobile-one columns">
                            <span class="prefix">http://</span>
                        </div>                        
                        <div class="nine columns">
                            <p>
                                <%: Html.EditorFor(model => model.XDE_WEB)%>
                                <%: Html.ValidationMessageFor(model => model.XDE_WEB)%></p>
                        </div>
                    </div>
                    <div class="row collapse">
                        <div class="two columns">
                            <label class="inline">
                                Facebook:</label>
                        </div>
                        <div class="four columns">
                            <p>
                                <%: Html.EditorFor(model => model.XDE_FACEBOOK)%>
                                <%: Html.ValidationMessageFor(model => model.XDE_FACEBOOK)%></p>
                        </div>
                        <div class="one columns">
                        </div>
                        <div class="two columns">
                            <label class="inline">
                                Twitter ID:</label>
                        </div>
                        <div class="one columns">
                            <span class="prefix">@</span></div>
                        <div class="two columns">
                            <p>
                                <%: Html.EditorFor(model => model.XDE_TWITTER)%>
                                <%: Html.ValidationMessageFor(model => model.XDE_TWITTER)%></p>
                        </div>
                    </div>
                    <p>
                        <input type="button" id="bActEmp" value="Publicar" class="button" />
                        <input type="button" value="Vista Previa" id="vistaprevia" class="button" />
                        <input type="button" value="Eliminar" onclick="if (confirm('Está seguro?')) {self.document.location.href='/Usuarios/BorrarEmpresa?id=<%: Model.NRO_EMPRESA.ToString() %>';}"
                            class="button" />
                    </p>
                    <% } %>
                </fieldset>
                <hr />
                <h5>
                    Teléfonos de Contacto</h5>
                <fieldset>
                    <%: Html.Partial("EmpresaTelefonos", ViewBag.Telefonos as IQueryable<MT_TELEFONO>) %>
                    <%                
                        using (Html.BeginForm("AgregarTelefono", "Usuarios", FormMethod.Post))
                        { %>
                    <%: Html.Hidden("NRO_USUARIO", String.Empty)%>
                    <%: Html.Hidden("NRO_EMPRESA", Model.NRO_EMPRESA.ToString())%>
                    <div class="row">
                        <div class="one columns">
                            <label class="inline">
                                Teléfono:</label></div>
                        <div class="three columns">
                            <%: Html.Editor("XDE_TELEFONO")%></div>
                        <%: Html.Editor("XDE_TELEFONO")%></div>
                    <div class="eight columns">
                        <input type="submit" id="bTel" value="Agregar" class="button" /></div>
                </fieldset>
                <%} %>
                <hr />
                <h5>
                    Categorías ("tags"):</h5>
                <fieldset>
                    <% 
                        var tg = ViewBag.TagsdeEmpresa as IList<MT_TAG>;
                        if (tg.Count > 0)
                        {
                            using (Html.BeginForm("BorrarTagDeEmpresa", "Usuarios", FormMethod.Post, new { @id = "BTAG" }))
                            { %>
                    <%: Html.HiddenFor(m => Model.NRO_EMPRESA)%>
                    <%: Html.Hidden("NRO_RUBRO_ABORRAR", "", new { @id = "NRO_RUBRO_ABORRAR" })%>
                    <ol>
                        <%foreach (MT_TAG m in tg)
                          {%>
                        <li><b><a href="/Directorio/Tag/<%: Html.Raw(m.NRO_RUBRO.ToString()) %>" target="_blank">
                            <%: Html.Raw(m.OBS_NOMBRE)%></a></b> - <a href="#" onclick="$('#NRO_RUBRO_ABORRAR').val('<%: Html.Raw(m.NRO_RUBRO.ToString()) %>'); $('#BTAG').submit();">
                                X</a></li>
                        <%}%>
                    </ol>
                    <%  }
                    }
                    else
                    { %>
                    No se ha asociado a ninguna categoría
                    <% } %>
                    <% using (Html.BeginForm("AgregarTagDeEmpresa", "Usuarios", FormMethod.Post, new { @class = "____________________________custom" }))
                       { %>
                    <%: Html.Hidden("NRO_RUBRO", "", new { @id = "NRO_RUBRO_NUEVO" })%>
                    <%: Html.HiddenFor(m => Model.NRO_EMPRESA)%>
                    <div class="row">
                        <div class="two columns">
                            Asociar a esta categoría:</div>
                        <div class="four columns">
                            <select id="ArbolDeTags" size="1">
                            </select></div>
                        <div class="two columns">
                            <input type="submit" id="nuevo" value="Asociar" class="button" />
                        </div>
                    </div>
                    <%} %>
                </fieldset>
                <% Html.RenderPartial("DataVistaPrevia", Model as MT_EMPRESA_PERFIL); %>
                <h5>
                    Archivos descargables
                </h5>
                <fieldset>
                    <ul id="filegal" class="block-grid one-up mobile-two-up">
                        <%         
                            int emp_id = Model.NRO_EMPRESA;
                            int usr_id = ViewBag.NroUsuario;
                            var files = ViewBag.EmpresaDescargas as IQueryable<MT_RECURSO>;

                            if (files != null)
                            {
                                foreach (var item in files)
                                { %>
                        <li><a class="th" target="_blank" style="float: left;" href="/P/E/<%: Html.Raw(item.NRO_EMPRESA.ToString()) %>/<%: Html.Raw(item.XDE_ARCHIVO) %>">
                            <img src="/Content/img/document.png" style="float: left;" />&nbsp;&nbsp;<%: Html.Raw(item.XDE_ARCHIVO) %></a>&nbsp;&nbsp;
                            <b><a href="#" class="remFile button small icon-ban-circle" style="width: 35px;"
                                data-idfile="<%: item.NRO_RECURSO.ToString() %>"></a></b></li>
                        <%}
                        }
                        %>
                    </ul>
                    <% 
                        int file_max = Convert.ToInt32(ConfigurationManager.AppSettings["MT_maxrecxprod"]);
                    %>
                    <p>
                        <b id="fileCount">
                            <%:files.Count().ToString() %></b> de <b>
                                <%: file_max.ToString()%></b> archivos descargables cargados. <span id="AddNewFile"
                                    style="float: left; width: 50px">
                                    <input type="file" name="XDE_ARCHIVO" id="File1" value /></span></p>
                </fieldset>
                <h5>
                    Galería de Fotos</h5>
                <fieldset>
                    <ul id="picgal" class="block-grid six-up mobile-two-up">
                        <%         
                            var pics = ViewBag.EmpresaFotos as IQueryable<MT_RECURSO>;

                            if (pics != null)
                            {
                                foreach (var item in pics)
                                { %>
                        <li style="position: relative;"><a class="th" target="_blank" href="/P/E/<%: Html.Raw(item.NRO_EMPRESA.ToString()) %>/<%: Html.Raw(item.XDE_ARCHIVO) %>">
                            <img src="/P/E/<%: Html.Raw(item.NRO_EMPRESA.ToString()) %>/<%: Html.Raw(item.XDE_ARCHIVO) %>" /></a>
                            <b><a href="#" class="remPic button small icon-ban-circle" style="width: 35px; position: absolute;
                                top: 1px; right: 1px;" data-idpic="<%: item.NRO_RECURSO.ToString() %>"></a></b>
                        </li>
                        <%}
                        }
                        %>
                    </ul>
                    <% 
                        int pics_max = Convert.ToInt32(ConfigurationManager.AppSettings["MT_maxrecxprod"]);
                    %>
                    <p>
                        <b id="picCount">
                            <%:pics.Count().ToString() %></b> de <b>
                                <%: pics_max.ToString()%></b> fotos cargadas. <span id="AddNewPic" style="float: left;
                                    width: 50px">
                                    <input type="file" name="XDE_ARCHIVO" id="XDE_ARCHIVO" value /></span></p>
                    <script>
                        var pics_count, file_count
                        pics_count = <%: pics.Count().ToString() %>;
                        file_count = <%: files.Count().ToString() %>;
                        $(document).ready(function () {
                            $('#XDE_ARCHIVO').uploadify({
                                'swf': '/Scripts/uploadify.swf',
                                'uploader': '/Varios/Archivo/?uid=<%: a.NRO_USUARIO.ToString() %>&eid=<%: a.NRO_EMPRESA.ToString() %>&itc=<%: "19" %>',
                                'auto': true,
                                'buttonClass': 'button icon-paper-clip',
                                'buttonText': '',
                                'onUploadSuccess': function (file, data, response) {                                    
                                    data = JSON.parse(data);
                                    if (!data.ok) {
                                        alert(data.d);
                                    } else {
                                        var pc = $('<li style="position:relative;"><a class="th" target="_blank"' +
                                        ' href="/P/E/<%: a.NRO_EMPRESA.ToString() %>/' + file.name + '">' +
                                        '<img src="/P/E/<%: a.NRO_EMPRESA.ToString() %>/' + file.name + '" /></a>' +
                                        ' <a href="#" class="remPic button small icon-ban-circle" style="width:35px;position:absolute;top:1px;right:1px;" data-idpic="' + data.d + '"></a></b></li>').hide();
                                        $("#picgal").append(pc);
                                        pc.show("slow");
                                        pics_count++;
                                        if (pics_count >= <%: pics_max.ToString() %>) { $("#AddNewPic").hide("slow"); }
                                        $("#picCount").text(pics_count);
                                        //CargarMuro();
                                    }
                                }
                            });
                            $('#File1').uploadify({
                                'swf': '/Scripts/uploadify.swf',
                                'uploader': '/Varios/Archivo/?uid=<%: a.NRO_USUARIO.ToString() %>&eid=<%: a.NRO_EMPRESA.ToString() %>&itc=<%: "23" %>',
                                'auto': true,
                                'buttonClass': 'button icon-paper-clip',
                                'buttonText': '',                                
                                'onUploadSuccess': function (file, data, response) {                                    
                                    data = JSON.parse(data);
                                    if (!data.ok) {
                                        alert(data.d);
                                    } else {
                                        var pc = $('<li><a class="th" target="_blank" style="float:left;"' +
                                        ' href="/P/E/<%: a.NRO_EMPRESA.ToString() %>/' + file.name + '">' +
                                        '<img src="/Content/img/document.png" style="float:left;" />&nbsp;&nbsp;'+ file.name +'</a>&nbsp;&nbsp;' +
                                        ' <a href="#" class="remFile button small icon-ban-circle" style="width:35px;" data-idfile="' + data.d + '"></a></b></li>').hide();                                        
                                        $("#filegal").append(pc);
                                        pc.show("slow");
                                        file_count++;
                                        if (file_count >= <%: file_max.ToString() %>) { $("#AddNewFile").hide("slow"); }
                                        $("#fileCount").text(file_count);
                                        //CargarMuro();
                                    }
                                }
                            });
                            $(document).on( "click", "a.remPic", function(e) { 
                                var a = $(this);
                                e.preventDefault();                                
                                $.ajax({
                                    url: "/Usuarios/BorrarFoto/"+ a.data("idpic"),
                                    type: "GET",
                                    async: false                                    
                                }).done(function (data) {                                                                                                
                                    if (data.ok) {                                        
                                        a.closest("li").hide('slow', function(){ $target.remove(); });
                                        pics_count--;
                                        $("#picCount").text(pics_count);
                                        if (pics_count >= <%: pics_max.ToString() %>) { $("#AddNewPic").show("slow"); }
                                        //CargarMuro();
                                    }
                                }); 
                            });                            
                            $(document).on( "click", "a.remFile", function(e) { 
                                var a = $(this);
                                e.preventDefault();                                
                                $.ajax({
                                    url: "/Usuarios/BorrarFoto/"+ a.data("idfile"),
                                    type: "GET",
                                    async: false                                    
                                }).done(function (data) {                                                                                                
                                    if (data.ok) {                                        
                                        a.closest("li").hide('slow', function(){ $target.remove(); });
                                        file_count--;
                                        $("#fileCount").text(file_count);
                                        if (file_count >= <%: file_max.ToString() %>) { $("#AddNewFile").show("slow"); }
                                        //CargarMuro();
                                    }
                                }); 
                            });                            
                            <% if (pics.Count() >= pics_max){ %>
                                $("#AddNewPic").hide();
                            <%} %>
                        });                            
                    </script>
                </fieldset>
            </li>
            <li id="simpleContained2Tab">
                <div class="row" style="border: solid 1px #e0e0e0; height: 390px; margin: 9px 0 19px">
                    <div id="scrollbar1" class="twelve columns">
                        <div class="scrollbar">
                            <div class="track">
                                <div class="thumb">
                                    <div class="end">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="viewport">
                            <div class="overview">
                                <div id="pMuro">
                                    <%
                                        List<MsgMuro> mu = ViewBag.Muro as List<MsgMuro>;
                                        if (mu != null && mu.Count() > 0)
                                        {
                                            foreach (MsgMuro m in mu)
                                            {%>
                                    <div class="row">
                                        <div class="two columns">
                                            <img src="/Recursos/Avatar?eid=<%: Model.NRO_EMPRESA.ToString() %>" />
                                        </div>
                                        <div class="nine columns">
                                            <% if (!String.IsNullOrEmpty(m.t))
                                               { %>
                                            <h5>
                                                <%: Html.Raw(m.t) %></h5>
                                            <%}%>
                                            <%: Html.Raw(m.m) %>
                                            <div class="msg_sep">
                                                <abbr class="timeago" title="<%: m.x%>">
                                                    <%: m.x%></abbr>
                                            </div>
                                        </div>
                                        <div class="one columns">
                                            <a href="#" class="button small icon-ban-circle ocultarmsg" style="width: 37px; float: right;"
                                                data-m="<%: m.i.ToString() %>"></a>
                                        </div>
                                    </div>
                                    <%}
                                        }
                                        else
                                        { %>
                                    <p>
                                        <i>Su muro está vacío.</i></p>
                                    <%}
                                    %>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <h5>
                    Publicar Actualización</h5>
                <fieldset>
                    <form id="fPublicacion" class="custom">
                    <div class="row">
                        <div class="two columns">
                            <a href="#" class="th">
                                <img src="/Recursos/Avatar?eid=<%: Model.NRO_EMPRESA.ToString() %>" style="width: 60px;
                                    height: 60px" /></a>
                        </div>
                        <div class="ten columns">
                            <div class="row">
                                <div class="two columns">
                                    <label for="mAsunto" class="inline">
                                        Título:</label></div>
                                <div class="ten columns">
                                    <input type="text" id="mAsunto" name="mAsunto" /></div>
                            </div>
                            <div class="row">
                                <div class="two columns">
                                    <label for="mMensaje">
                                        Desarrollo:</label></div>
                                <div class="ten columns">
                                    <textarea name="mMensaje" id="mMensaje" class="qNop"></textarea></div>
                            </div>
                            <div class="row">
                                <div class="twelve columns">
                                    &nbsp;</div>
                            </div>
                            <div class="row">
                                <div class="two columns">
                                    <label for="mModo" class="inline">
                                        Destinatarios:</label></div>
                                <div class="eight columns">
                                    <select size="1" id="mModo" style="display: none">
                                        <option value="0">Muro público</option>
                                        <option value="1">Solo a mis Seguidores</option>
                                    </select></div>
                                <div class="two columns">
                                    <input type="button" value="Enviar" id="bEnviarMensaje" class="button" />
                                </div>
                            </div>
                            <input type="hidden" id="mEmpresa" value="<%: Model.NRO_EMPRESA.ToString() %>" />
                            <% MTrepository mtdb = new MTrepository();  %>
                            <input type="hidden" id="mUsuario" value="<%: mtdb.AutorDeEmpresa(Model.NRO_EMPRESA).ToString() %>" />
                        </div>
                    </div>
                    </form>
                </fieldset>
                <p>
                    &nbsp;</p>
            </li>
        </ul>
    </div>
    <script src="/Scripts/jquery.tinyscrollbar.min.js"></script>
    <script src="/Scripts/jquery.imgrefresh-1.0.min.js"></script>
    <script src="/Scripts/jquery.timeago.js"></script>
    <script>
        var oScrollbar;
        function CargarMuro() {
            $.ajax({
                url: "/Mensajes/CargarMuro?eid=<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>",
                type: "GET"
            }).done(function (data) {
                ActualizarMuro(data);
            });
        }
        function ActualizarMuro(data) {
            var h = "";
            $.each(data.d, function (key, val) {
                h += '<div class="row">' +
                '<div class="two columns">' +
                '<img src="/Recursos/Avatar?eid=<%: Model.NRO_EMPRESA.ToString() %>" />' +
                '</div>' +
                '<div class="nine columns">';
                h += (val["t"] != null) ? "<h5>" + val["t"] + "</h5>" : "";
                h += val["m"] + "<div class=msg_sep><abbr class=timeago title='" + val["x"] + "'>" + val["x"] + "</abbr></div>" +
                '</div>' +
                '<div class="one columns">' +
                '<a href=# class="button small icon-ban-circle ocultarmsg" style="width:37px;float:right;" data-m="' + val["i"] + '"></a>' +
                '</div>' +
                '</div>'
            });
            h = (h != "") ? h : "<p><i>Su muro está vacío.</i></p>";
            $('#pMuro').empty().append(h);
            $("abbr.timeago").timeago();
            oScrollbar.tinyscrollbar_update("top");
        }
        editor1 = CKEDITOR.replace("MEM_LARGA_EDITOR", {
            language: "es",
            uiColor: "#ffffff",
            toolbar: [
				['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
				['FontSize', 'TextColor', 'BGColor'],
				['UIColor']
			]
        });
        editor2 = CKEDITOR.replace("mMensaje", {
            language: "es",
            uiColor: "#ffffff",
            toolbar: [
				['Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
				['FontSize', 'TextColor', 'BGColor'],
				['UIColor']
			]
        });
        function getCoords(c) {
            if (c.checked) {
                if ($("#XDE_DOMICILIO").val() == "") {
                    alert("Debe agregar la dirección primero");
                    $(c).attr("checked", false);
                    $("#XDE_DOMICILIO").focus();
                } else if ($("#XDE_CIUDAD").val() == "") {
                    alert("Debe agregar la ciudad");
                    $(c).attr("checked", false);
                    $("#XDE_CIUDAD").focus();
                } else {
                    var a = $("#XDE_DOMICILIO").val() + ", " +
                            $("#XDE_CP").val() + ", " +
                            $("#XDE_CIUDAD").val() + ", " +
                            $("#NRO_PROVINCIA option:selected").text() + ", " +
                            $("#NRO_PAIS options:selected").text();
                    verenmapa(a);
                    $("#map_canvas").slideDown();
                }
            } else {
                $("#map_canvas").slideUp();
                $("#XDE_LAT").attr("value", "");
                $("#XDE_LAT").attr("value", "");
            }
        }
        $(document).ready(function () {
            $("abbr.timeago").timeago();
            $('body').on('click', 'a.ocultarmsg', function (e) {
                e.preventDefault();
                $.ajax({
                    url: "/Mensajes/MuroOcultar",
                    contentType: "application/json; charset=utf-8",
                    type: "POST",
                    data: JSON.stringify({ i: $(this).data("m") })
                }).done(function (data) {
                    ActualizarMuro(data);
                });
            });
            $("#map_canvas").hide();
            $("#NRO_PAIS").change(function () {
                $.ajax({
                    url: "/Varios/ProvinciasxPais?id=" + $(this).val(),
                    type: "GET"
                }).done(function (data) {
                    if (data.success) {
                        $("#NRO_PROVINCIA").empty();
                        $.each(data.res, function (key, val) {
                            $('#NRO_PROVINCIA').append($('<option>', {
                                value: val["NRO_PROVINCIA"],
                                text: val["XDE_PROVINCIA"]
                            }));
                        });
                        $("#NRO_PROVINCIA").trigger("change");
                    }
                });
            });
            $("#vistaprevia").click(function () {
                $("#vp_NRO_EMPRESA").val($("#NRO_EMPRESA").val());
                //$("#vp_NRO_EMPRESA").val($("input[name='NRO_EMPRESA']")[0]);                
                $("#vp_XDE_RAZONSOCIAL").val($("#XDE_RAZONSOCIAL").val());
                $("#vp_XDE_CORTA").val($("#XDE_CORTA").val());
                $("#vp_OBS_MEDIA").val($("#OBS_MEDIA").val());
                $("#vp_MEM_LARGA").val(editor1.getData());
                $("#vp_XDE_CIUDAD").val($("#XDE_CIUDAD").val());
                $("#vp_XDE_DOMICILIO").val($("#XDE_DOMICILIO").val());
                $("#vp_XDE_CP").val($("#XDE_CP").val());
                $("#vp_XDE_WEB").val($("#XDE_WEB").val());
                $("#vp_XDE_FACEBOOK").val($("#XDE_FACEBOOK").val());
                $("#vp_XDE_TWITTER").val($("#XDE_TWITTER").val());
                $("#vp_NRO_PAIS").val($("#NRO_PAIS").val());
                $("#vp_NRO_PROVINCIA").val($("#NRO_PROVINCIA").val());
                $("#vp_XDE_LAT").val($("#XDE_LAT").val());
                $("#vp_XDE_LON").val($("#XDE_LON").val());
                $("#VP").submit();
            });
            $.ajax({
                url: "/Usuarios/Tags/?modo=all&emp_id=<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>",
                type: "GET"
            }).done(function (data) {
                if (data.success) {
                    //localStorage["MT.TAGS"] = JSON.parse(data.res);
                    tags = data.res;
                    $('#ArbolDeTags').empty();
                    $.each(tags, function (key, val) {
                        $('#ArbolDeTags').append($('<option>', {
                            value: val["NRO_RUBRO"],
                            text: val["OBS_NOMBRE"]
                        }));
                    });
                    $('#ArbolDeTags').trigger("change");
                }
            });
            $("#ArbolDeTags").change(function () {
                $("#NRO_RUBRO_NUEVO").val($("#ArbolDeTags option:selected").val());
            });
            $("#bActEmp").click(function (e) {
                $("#MEM_LARGA").val(editor1.getData());
                $("#fPfEd").submit();
            });
            //$("#cargarlatylon").attr("checked",true).trigger("click");
            //alert(typeof ($("#cargarlatylon").attr("value")))
            

            //alert($("#XDE_LAT").attr("value"));
            //$("#XDE_LAT").attr("value", "");

            oScrollbar = $('#scrollbar1');
            oScrollbar.tinyscrollbar();
            oScrollbar.tinyscrollbar_update("top");
            $("#fPublicacion").validate({
                rules: {
                    mAsunto: { required: true },
                    mMensaje: { required: true }
                },
                messages: {
                    mAsunto: { required: "Ingresa un título para tu publicación" },
                    mMensaje: { required: "Ingresa el contenido de tu publicación" }
                }
            });
            $("#bEnviarMensaje").click(function (e) {
                e.preventDefault();
                editor2.updateElement();
                $("#mMensaje").css({ visibility: "hidden", display: "" }).val(editor2.getData());
                if ($("#fPublicacion").valid()) {
                    $.ajax({
                        url: "/Mensajes/Muro",
                        contentType: "application/json; charset=utf-8",
                        type: "POST",
                        data: JSON.stringify({ a: $("#mAsunto").val(), m: $("#mMensaje").val(), e: $("#mEmpresa").val(), u: $("#mUsuario").val(), s: $("#mModo option:selected").val() })
                    }).done(function (data) {
                        ActualizarMuro(data);
                    });
                } else {
                    return false;
                }
            });
        });   
        <% if (!String.IsNullOrEmpty(Model.XDE_LON + Model.XDE_LAT)) {%>                    
        $(document).ready(function () {
            $("#map_canvas").show();            
        });                    
    <%}%>         
    </script>
</asp:Content>
