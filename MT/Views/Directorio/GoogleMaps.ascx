<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false"></script>
<script type="text/javascript">
    function initialize() {
        mapaEmpresa();
    }
    function mapaEmpresa() {
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

                // codigo: http://powerhut.co.uk/googlemaps/custom_markers.php
                // tack: http://www.iconarchive.com/show/vista-map-markers-icons-by-icons-land/Map-Marker-Push-Pin-1-Left-Chartreuse-icon.html
                var image = new google.maps.MarkerImage(
	                '/Content/img/tack.png',
		            new google.maps.Size(32, 32),
		            new google.maps.Point(0, 0),
		            new google.maps.Point(32, 32)
	            );
                /*var shadow = new google.maps.MarkerImage(
	              'marker-images/shadow.png',
		            new google.maps.Size(62, 35),
		            new google.maps.Point(0, 0),
		            new google.maps.Point(0, 35)
	            );*/
                var shape = {
                    coord: [21, 10, 25, 11, 26, 12, 28, 13, 29, 14, 30, 15, 30, 16, 31, 17, 31, 18, 31, 19, 31, 20, 31, 21, 31, 22, 30, 23, 30, 24, 30, 25, 31, 26, 31, 27, 31, 28, 30, 29, 28, 30, 18, 31, 15, 31, 15, 30, 15, 29, 15, 28, 14, 27, 14, 26, 15, 25, 15, 24, 14, 23, 13, 22, 13, 21, 12, 20, 12, 19, 12, 18, 12, 17, 12, 16, 12, 15, 13, 14, 14, 13, 14, 12, 16, 11, 19, 10, 21, 10],
                    type: 'poly'
                };
                marker = new google.maps.Marker({
                    draggable: false,
                    raiseOnDrag: false,
                    icon: image,
                    /*shadow: shadow,*/
                    shape: shape,
                    map: map,
                    position: results[0].geometry.location
                });
                myInfoWindowOptions = {
                    content: '<div class="info-window-content"><h5><%: Model.XDE_RAZONSOCIAL %><img src="/Recursos/Avatar?eid=<%: Model.NRO_EMPRESA.ToString() %>" style="float:right;height:25px;width:auto"></h5><!--<p><%: ViewBag.Direccion %></p>--></div>',
                    maxWidth: 275
                };
                infoWindow = new google.maps.InfoWindow(myInfoWindowOptions);
                
                
                
                google.maps.event.addListener(marker, 'dragstart', function () {
                    infoWindow.close();
                });
                google.maps.event.addListener(marker, 'click', function () {
                    infoWindow.open(map, marker);
                });
                infoWindow.open(map, marker);
                /*
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });*/
            }
        });
        $("#map_canvas").show();
    }
    google.maps.event.addDomListener(window, 'load', initialize);
</script>
