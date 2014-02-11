<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MT.Models.MT_EMPRESA_PERFIL>" %>
<%@ Import Namespace="MT.Models" %>
<% 
 
    
    %>
<div id="mInsetarMuro" class="reveal-modal">
    <h4>
        Inserte este Muro en su Web!</h4>
    <div class="row">
    
    <div class="twelve columns">
    
    
    
        <h5>Paso 1: Cargar librerías requeridas</h5>
        <p>El muro funciona con las librerías <a href="http://jquery.com/" target=_blank>JQuery</a> y 
        <a href="http://jqueryui.com/" target=_blank>JQuery UI</a>. Si su página no tiene
        estas librerías, puede copiar y pegar el siguiente código en su código HTML.</p>
        <blockquote>
            <b>Nota:</b> si ya dispone de estas librerías en su página, pase al siguiente paso</blockquote>
            <pre class="html">&lt;script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js"&gt;&lt;/script&gt;
&lt;script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.0/jquery-ui.min.js"&gt;&lt;/script&gt;</pre>
        
        <h5>Paso 2: Cargar las librerías de MercadoTextil</h5>
        <p>
            Estas librerías corresponden a la hoja de estilos CSS que define los estilos visuales
            del muro, y el código necesario para ejecutar nuestro widget:</p>
            <pre class="html">&lt;link href="<%: Html.Label(ConfigurationManager.AppSettings["MT_sitiourl"]) %>/css/mt.widgets.css" rel="Stylesheet" media="all" type="text/css" /&gt;
&lt;script src="<%: Html.Label(ConfigurationManager.AppSettings["MT_sitiourl"]) %>/Scripts/mt.widgets.js"&gt;&lt;/script&gt;</pre>
        
        <h5>Último Paso!</h5>
        <p>
            Copie y pegue el siguiente código en el lugar en el que desee que aparezca el muro.</p>
        <pre class="html">&lt;div id="actividadenMT">&lt;/div&gt;
&lt;script&gt;
    $('#actividadenMT').mtact({
        width: 200,
        height: 200,
        mcount: 4,
        empkey: <%: StringMethodExtensions.EncodeId(Model.NRO_EMPRESA)%>,
        server: "http://localhost:7256"
    });
&lt;/script&gt;</pre>
    
    <script>
        $(document).ready(function () {
            $("pre.html").snippet("html", { menu: true, style: "acid", transparent: false, showNum: true });
            // <%: Html.DisplayFor(model => model.NRO_EMPRESA) %>
        });
    </script>
    <a class="close-reveal-modal">&#215;</a>
</div>
</div>
    
    </div>

<script type="text/javascript" src="/Scripts/jquery.snippet.min.js"></script>
<link rel="stylesheet" type="text/css" href="/css/jquery.snippet.min.css" />
