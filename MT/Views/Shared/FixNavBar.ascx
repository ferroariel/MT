<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="container navigation" id="FixNavBar">
    <div class="centered">
        <div class="row">
            <div class="three columns">
                <a href="/"><img src="/Content/img/mt_ui_logo_chico.png" /></a></div>            
            <div class="five columns">
                <dl class="sub-nav">                    
                    <dd class="active">
                        <a href="/">Inicio</a></dd>
                    <dd>
                        <a href="/Directorio">Directorio</a></dd>
                    <dd>
                        <a href="/Empleo">Empleo</a></dd>
                    <dd>
                        <a href="/Organizaciones.html">Organizaciones</a></dd>
                    <dd>
                        <a href="/Medios.html">Medios</a></dd>
                </dl>
            </div>            
            <div class="four columns">
                <dl class="sub-nav">                    
                    <%if (Request.IsAuthenticated)
                      {                  
                    %>
                    <dd><img src="/Recursos/Avatar" style="margin-top:-4px;height:22px;width:auto;" /></dd>
                    <dd><b class="nm"></b></dd>
                    <dd><a href="/Usuarios/Perfil">Perfil</a></dd>
                    <dd><a href="/Usuarios/Mensajes">Perfil</a></dd>
                    <dd><a href="/Usuarios/Salir">Salir</a></dd>
                    <%}
                      else { %>
                    <dd><a href="#">&nbsp;</a></dd>
                    <dd>&nbsp;</dd>
                    <dd>&nbsp;</dd>
                    <dd>&nbsp;</dd>
                    <dd><a href="#">Entrar</a></dd>
                      <%} %>
                </dl>
            </div>            
        </div>
    </div>
</div>
<script>
    function sticky_relocate() {
        var window_top = $(window).scrollTop();
        var div_top = $('#FixNavBar-anchor').offset().top;
        if (window_top > div_top)
            $('#FixNavBar').show();
        else
            $('#FixNavBar').hide();
    }
    $(window).load(function () {
        $(window).scroll(sticky_relocate);
        sticky_relocate();
    });
</script>