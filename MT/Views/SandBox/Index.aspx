<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>
<html>
<head>
    <title>Prueba Widget MT13</title>            
</head>
<body>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js"></script>
    <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.0/jquery-ui.min.js"></script>
    <link href="http://localhost:7256/css/mt.widgets.css" rel="Stylesheet" media="all" type="text/css" />
    <script src="http://localhost:7256/Scripts/mt.widgets.js"></script>
    <div id="actividadenMT"></div>
    <script>
        $('#actividadenMT').mtact({
            width: 200,
            height: 200,
            mcount: 4,
            empkey: 311052632,
            server: "http://localhost:7256"
        });
</script>
</body>
</html>
