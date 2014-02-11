<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.MT_EMPRESA_PERFIL>" %>
<%@ Import Namespace="MT.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <style type="text/css">
        #map_canvas {display:none;}
    </style>
    <title>
        <%: Model.XDE_RAZONSOCIAL %></title>
    <% if (Model.XDE_DOMICILIO != null)
       {%>
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false"></script>
    <script type="text/javascript">
        function initialize() {
            var address = "<%: ViewBag.Mapa %>";
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
                    $("#map_canvas").show();
                }
            });
        }
        google.maps.event.addDomListener(window, 'load', initialize);
    </script>
    <% } %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <fieldset>
    
    
    
    <legend><%: Model.XDE_RAZONSOCIAL%></legend>
    
    <blockquote>
        <%: Html.DisplayFor(model => model.XDE_CORTA) %>
    <p><%: Html.Raw(Model.MEM_LARGA) %></p>
    <% IQueryable<MT_RECURSO> r = ViewBag.EmpresaFotos as IQueryable<MT_RECURSO>;
       if (r != null) { 
           foreach (MT_RECURSO i in r) {
               %>
               <img src="/P/E/<%: Html.Raw(Model.NRO_EMPRESA.ToString()) %>/<%: Html.Raw(i.XDE_ARCHIVO) %>" />
           <%}
       }
       %>
    </blockquote>
    <h4>
        Datos de Contacto</h4>
    <p>
        <%: ViewBag.Direccion%>
    </p>
    <% if (!String.IsNullOrEmpty(Model.XDE_WEB))
       {%>
    <p>
        <img src="http://open.thumbshots.org/image.pxf?url=<%: HttpUtility.UrlEncode(Model.XDE_WEB) %>" /><br />
        <a href="<%: Html.DisplayFor(model => model.XDE_WEB) %>" target="_blank">
            <%: Html.DisplayFor(model => model.XDE_WEB)%></a>
    </p>

    <%} %>
    <div id="map_canvas" style="width: 300px; height: 200px">
    </div>
    </fieldset>
</asp:Content>
