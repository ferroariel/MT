<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ OutputCache Duration="1" VaryByParam=”*” %>

<ul class="nav-bar global" id="NavBar">
    <li><a href="#">Acerca</a></li>
    <li><a href="#">Corporativo</a></li>
    <li><a href="#">Ayuda</a></li>
    <%if (Request.IsAuthenticated)
      {                  
    %>
    <li class="has-flyout"><a href="#">Mi Cuenta</a><b class="mCount">0</b><a href="#" class="flyout-toggle"><span></span>
    </a>
        <ul class="flyout small">
            <li><a href="/Usuarios/Perfil" class="row collapse"><span class="two columns">
                <img src="/Recursos/Avatar" /></span> <span class="one columns"></span><span class="nine columns">
                    <b id="nm" class="nm"></b></span></a></li>
            <li>
                <%: Html.ActionLink("Mensajes", "Index", "Mensajes", null, new { id = "cntMensajes" })%></li>
            <li>
                <%: Html.ActionLink("Salir", "Salir", "Usuarios") %></li>
        </ul>
    </li>
    <script>
        //var chk = null
        var audioElement = null;
        // aca guardamos los ids de los mensajes sin chequear, persistente entre las paginas
        var unread_msg = [];    
        var bp = false;
        var audioElement = null;
        var unread_msg = [];
        var bp = false;
        // variable global de actividad nula
        var uActive = true;
        var msgs = null;

        $(document).ready(function () {                                
            $(".mCount").hide();        
            audioElement = document.createElement('audio');
            audioElement.setAttribute('src', '/P/A/beep.ogg');            
            audioElement.load()
            if ($('.nm').text().length < 2 ) {
                $('.nm').load('/Usuarios/AvatarU', function() {                    
                    if (localStorage.getItem('MT_MsgLst')!=null) {            
                        unread_msg = JSON.parse(localStorage.getItem('MT_MsgLst'));                    
                        $("#cntMensajes").text(unread_msg.length + " Mensajes").css("font-weight","bold");                            
                        $("#AvatarU .mCount").text(unread_msg.length).show();                                                                                             
                    }   
                    $.doTimeout('chk', <%: System.Configuration.ConfigurationManager.AppSettings["MT_delaychequeomensajes"].ToString() %>, function(){                
                        HayMensajes();
                        return true;             
                    });           
                });
            }
            $("#cntMensajes").click(function(e){
                e.preventDefault();
                $.getJSON(
                    "/Mensajes/AlertaMensajeOK",
                    null,
                    function (data) {
                        localStorage.removeItem('MT_MsgLst');
                        self.document.location.href = "/Mensajes/";                
                });            
            });        
            setAwayTimeout(<%: System.Configuration.ConfigurationManager.AppSettings["MT_delaychequeomensajes"].ToString() %>); // 10 seconds        
            //document.onIdle = function() { }
            document.onAway = function() { uActive = false /*$.doTimeout('chk')*/; }
            document.onBack = function(isIdle, isAway) {
                if (isAway) {             
                    /*$.doTimeout('chk', <%: System.Configuration.ConfigurationManager.AppSettings["MT_delaychequeomensajes"].ToString() %>, function(){                
                        HayMensajes();
                        return true;             
                    });*/
                    uActive = true;
                }      
            };
        });
        function HayMensajes() {                        
            if (uActive) {
                var d = null;        
                msgs = $.getJSON(
                    "/Mensajes/HayMensajes",
                    null,
                    function (data) {                       
                        d = data;                
                        if ( d.cnt > 0 ) {                                                                        
                            $.each( d.c, function (key, val) {                
                                if($.inArray(val, unread_msg)==-1){
                                    unread_msg.push(val);                                                            
                                    bp = true;
                                }
                            });                       
                            localStorage.setItem('MT_MsgLst', JSON.stringify(unread_msg));                                   
                            if (bp) beep();
                            $("#cntMensajes").text(unread_msg.length + " Mensajes").css("font-weight","bold");                
                            $(".mCount").text(unread_msg.length).show();                                                    
                            //AppendNuevosMensajes(d);                                                
                            bp = false;
                        }                
                        //alert(d.cnt);
                }); 
            }
        }
        $(window).unload( function () { msgs.abort(); } );
        function beep() {audioElement.play();}
    </script>
    <%}
      else
      { %>
    <li class="has-flyout"><a href="#">Entrar</a><a href="#" class="flyout-toggle"><span></span>
    </a>
        <ul class="flyout small">
            <li>
                <%: Html.ActionLink("Entrar", "Ingreso", "Usuarios") %></li>
            <li>
                <%: Html.ActionLink("Registrar", "Registro", "Usuarios") %></li>
        </ul>
    </li>
    <%} %>
    <li><a href="#"><span id="fancyClock"></span></a></li>
</ul>
<script>
    $(document).ready(function () {
        $('#fancyClock').tzineClock();
    });        
</script>

