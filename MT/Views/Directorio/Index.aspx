<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MT.Models.MT_EMPRESA_PERFIL>>" %>
<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">    
    <title>Directorio</title>   
    <link rel=stylesheet type="text/css" href="/css/jquery.dataTables.css" /> 
    <script src="/Scripts/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false"></script>
    <script type="text/javascript">        
        /*function initialize() {
            var address = "Av. Nazca 10 - C.A.B.A. - Buenos Aires - Argentina";            
            var mapDiv = document.getElementById('map_canvas2');
            var geocoder = new google.maps.Geocoder();
            geocoder.geocode( {'address': address }, function (results, status) {
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
                }
            });
            //map1_initialize();           
        }*/
        //google.maps.event.addDomListener(window, 'load', initialize);
        /*
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
                }
            });
        }*/
        function verenmapa(lat, lng) {
            $("#map_canvas").show();
            var latlng = new google.maps.LatLng(lat, lng);
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
        }
    </script>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
    <%: Html.Partial("Buscar") %>    
</asp:Content>
