<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <link rel="stylesheet" type="text/css" href="/css/tinyscrollbar.css" />
    <title>Mensajes</title>    
    <script>        var MsgLst = 1;</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns">
        <dl class="tabs contained tabsempresa">
            <dd class="active">
                <a href="#simpleContained1">Mensajes y Notificaciones</a></dd>
            <dd class="hide-for-small">
                <a href="/Usuarios/Perfil">Mi Perfil</a></dd>
            <dd class="hide-for-small">
                <a href="/Usuarios/Empresas">Mis Empresas</a></dd>
        </dl>
        <ul class="tabs-content contained">
            <li class="active" id="simpleContained1Tab">
                <h4>
                    Notificaciones</h4>
                <div class="row">
                    <dl class="vertical tabs three columns" id="vAutores">
                    </dl>
                    <div class="nine columns">
                        <div class="row" id="pArbol" style="border: solid 1px #e0e0e0; background-color: #F2F2F2;
                            height: 390px; display: block; margin-bottom: 15px">
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
                                    <div class="overview" style="width: 100%">
                                        <ul class="accordion" id="vMensajes">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row collapse">
                            <div class="ten columns gris" id="nWr">
                                <span class="icon-edit"></span><span id="uWr"></span> está escribiendo...
                            </div>
                        </div>
                        <div class="row" id="pForm">
                            <div class="two columns">
                                <a href="/Usuarios/Perfil" class="th">
                                    <img src="/Recursos/Avatar" /></a>
                            </div>
                            <div class="ten columns">
                                <%: Html.Partial("RespuestaMensaje")%>
                            </div>
                        </div>
                    </div>
                </div>
                
            </li>
        </ul>
    </div>
    <!--
    <textarea id="dbg" rows="10"></textarea>
    
    
    <div class="circle"></div>
    <div class="circle1"></div>
    -->
    <script src="/Scripts/jquery.foundation.tabs.js"></script>
    <script src="/Scripts/jquery.timeago.js"></script>
    <script src="/Scripts/jquery.tinyscrollbar.min.js"></script>
    <script type="text/javascript">        
        var d = null;
        //var vistaarbolactual = null;
        var yo = "<%: ViewBag.MiID.ToString() %>";
        var yon = "<%: ViewBag.MiNombre.ToString() %>";
        var yoe = "";
        var curr_e = 0;
        var curr_u = 0;
        var curr_un = "";
        var c_u = 0;
        var escribiendo = false;

        $(document).ready(function () {            
            localStorage.removeItem("MT_MsgLst");            
            //CargarMensajes();            
            $("#pForm, #nWr").hide();
            InicializarArbol();            
            localStorage.removeItem("MT_MsgLst");            
            //CargarMensajes();            
            $("#pForm, #nWr").hide();
            InicializarArbol();            
            // al responder un mensaje ----------------------------------------------
            $("#bResp").click(function (e) {
                e.preventDefault();
                if ($("#rep_mensaje").val() != "" && $("#msg_id").val() != "" && $("#msg_id").val() != "0") {
                    $.ajax({
                        url: "/Mensajes/Respuesta",
                        contentType: "application/json; charset=utf-8",
                        type: "POST",
                        async: false,
                        data: JSON.stringify(
                        {
                            rep_nro_usuario : yo, 
                            rep_nro_empresa : $("#rep_nro_empresa").val(), 
                            rep_nro_usuario_destino: $("#rep_nro_usuario_destino").val(),
                            rep_nro_empresa_destino: $("#rep_nro_empresa_destino").val(),
                            msg_id: $("#msg_id").val(),
                            rep_mensaje: $("#rep_mensaje").val()                            
                        })
                    }).done(function (data) {                                                                                                
                        ToyTipeando(yo,false);
                        var obj = {
                            "ok":true,
                            "cnt": 1,
                            "c": [
                                {"i" : (new Date()).getTime(),
                                "d" : (new Date()).toISOString(),
                                "m" : $("#rep_mensaje").val(),
                                "ei" : $("#rep_nro_empresa").val(),
                                "en" : yoe,
                                "ui" : yo,
                                "un" : yon,
                                "t" : 3}
                                ]
                        } 
                        AppendNuevosMensajes(obj);                           
                        $("#rep_mensaje").val("").blur();                        
                    });
                }
            });
            // tecla ENTER -------------------------------------------
            $('#rep_mensaje').keypress(function (e) {
                if (e.which == 13) {
                    $("#bResp").trigger("click");                    
                    return false;
                }                
                if ( !escribiendo ) { ToyTipeando(yo,true); }
            }).blur( function () {
                if ( escribiendo ) { ToyTipeando(yo,false); }
            });
            // ----------------------------------------------------------------
        }); 

        function ToyTipeando(i,b) {
            $.getJSON(
            "/Mensajes/Tipear?id="+i+"&b="+b,
            null,
            function (data) {escribiendo = b;});
        }
        function TaTipeando(i) {
            if (uActive) {
                $.getJSON(
                "/Mensajes/Tipea?id="+i,
                null,
                function (data) {                
                    // si está tipeando...
                    if (data.ok) {
                        $("#uWr").text(data.u);
                        $("#nWr").show();
                        $("span.mUsrAv"+i).removeClass("mCount0").addClass("mCount1");
                    // si no
                    }else{
                        $("#nWr").hide();                    
                        if ( data.u != "" ) {
                            // si está logueado                        
                            if (i==curr_u) {
                                $("#UsrConn").empty().append('<cite class="round success label"><span class="icon-ok"></span> '+ curr_un +' está conectado! Escríbele un mensaje.</cite>')
                            };
                            $("span.mUsrAv"+i).removeClass("mCount0").addClass("mCount1");
                        }else{
                            // si no esta logueado
                            if (/*$("#UsrConn").length > 0 &&*/ i==curr_u) {
                                if ($("#UsrConn").length > 0)
                                    $("#UsrConn").empty().append('<cite class="round secondary label"><span class="icon-warning-sign"></span> '+ curr_un +' está desconectado. Su mensaje será leído después.</cite>').show();                                            
                                /*else
                                    $("p.conversacion").last().parent(".row").after('<blockquote id="UsrConn"><cite class="round secondary label"><span class="icon-warning-sign"></span> '+ curr_un +' está desconectado. Su mensaje será leído después.</cite></blockquote>').show();                                            */
                            }
                            $("span.mUsrAv"+i).removeClass("mCount1").addClass("mCount0");
                        }
                    }
                    //$("#dbg").text(JSON.stringify(data));
                });
            }
        }        
        function AppendNuevosMensajes(data) {                        

            var i = curr_u;
            var li = "";   
            var c = 0; 
            var dt = null           
            if ($("#lstMsgDt").length>0) { $("#lstMsgDt").remove(); }
            $.each(data.c, function (key, val) {                
                if ($("#mg"+val["i"]).length == 0) {                                        
                    if (c_u != val["ui"]) {                        
                        c_u = val["ui"];
                        li += '<div class="row">' +
                            '  <div class="two columns"><a href=# class=th><img src="/Recursos/Avatar?uid=' + val["ui"] + '" style="width:45px;height:45px" /></a></div>' +

                            '  <div class="ten columns">'+
                            '    <div><b>' + val["un"];
                        if (val["ei"] != 0) {
                            li += ' de ' + val["en"]; 
                            yoe = val["en"];
                        }
                        li+='</b></div>'+
                            '<p class=conversacion><span id="mg'+ val["i"] +'" > '+ val["m"] +'</span></p>';
                        $("#vMensajes li.active .content").append(li);
                    }else{

                        if ($("#vMensajes li.active .content").length > 0) {
                            var b = (val["t"]==1) ? '<br /><hr /><span id="mg'+ val["i"] +'" > '+ val["m"] +'</span>' : ' <br /><span id="mg'+ val["i"] +'" > '+ val["m"] +'</span>';                            
                            $("#vMensajes li.active .content p.conversacion")
                            .last()                            
                            .append(b);                            
                        }                        
                    }   
                    dt = val["d"];                                         

                    if (val["ui"] != yo && val["t"]=="3" && val["ui"] != "<%: System.Configuration.ConfigurationManager.AppSettings["MT_admusr"].ToString() %>" ) 
                        $("#msg_id").val(val["i"]);
                    c++;
                }
            });     
            if ( dt != null ) {
                if( $("#UsrConn").length > 0 ) { $("#UsrConn").remove(); }
                var dlm = '<div id="lstMsgDt" class="msg_sep">' +
                        '   <abbr class="timeago" title="' + dt + '">' +
                            dt +
                        '   </abbr>' +
                        '</div>';
                $("#vMensajes li.active .content")
                .append(dlm);
                $("abbr.timeago").timeago();
                var oScrollbar = $('#scrollbar1');
                oScrollbar.tinyscrollbar();
                oScrollbar.tinyscrollbar_update("bottom");                
            }                        
        }                 

        function InicializarArbol() {
            $.doTimeout('utp');
            $.getJSON(
            "/Mensajes/ListadoArbol",
            null,
            function (data) {
                var dd = "";
                if (data.ok && data.r.length > 0) {                    
                    //$("#dbg").val(JSON.stringify(data.r));
                    $.each(data.r, function (key, val) {
                        if (val["ui"]!=yo) {
                            dd += '' +

                                '<dd >' +
                                '<a class="row collapse" onclick="ListarMensajes(this); return false" href="#" data-u="' + val["ui"] + '" data-un="'+ val["un"] +'" data-e="' + val["ei"] + '" >' +
                                '<span class="four columns th Avtr"><img src="/Recursos/Avatar?uid=' + val["ui"] + '" style="width:40px;height:40px" />' +
                                '<span class="mCount icon-comment ';
                            dd += (val["on"]) ? ' mCount1' : '-alt mCount0';
                            dd +=' mUsrAv'+ val["ui"] +'"></span>';
                            //dd += (val["mc"]!=0) ? '<!--<span class="mCount">'+ val["mc"] +'</span>-->' : '';                            
                            dd += '</span><span class="eight columns">' + val["un"];

                            dd += (val["ei"]!="0") ? ' de ' + val["en"] : "";
                            dd += '</span>' +
                            '' +
                            '</a></dd>';
                        }
                    });
                    $("#vAutores").empty().append(dd);
                    //$("#dbg").val(dd);
                }
            });            
        }

        function ListarMensajesNuevos(e,u) {            
            if (uActive) {
                $.getJSON(
                "/Mensajes/ListadoMensajes?u=" + curr_u + "&e=" + curr_e +"&n=1",
                null,
                function (data) {
                    if (data.ok && data.c.length > 0) {
                        //alert(data.c.length);
                        AppendNuevosMensajes(data);
                    }
                });
            }
        }

        function ListarMensajes(g) {                        
            var e = ($(g).data("e") != 0) ? $(g).data("e") : 0;
            var u = ($(g).data("u") != 0) ? $(g).data("u") : 0;
            var n = ($(g).data("u") != 0) ? $(g).data("un") : "";
            curr_e = e;
            curr_u = u;
            curr_un = n;
            ListadoMensajes(e,u);
        }
        function ListadoMensajes(e,u) {            
            $("body").css("cursor", "progress");
            var dt = null;
            var online = false;
            var msg_d = null;
            $("#msg_id").val("");
            $.getJSON(
                "/Mensajes/ListadoMensajes?u=" + u + "&e=" + e,
                null,
                function (data) {
                    d = data;
                    var li = "";
                    if (data.ok && data.c.length > 0) {                    
                        if ($("#lstMsgDt").length>0) { $("#lstMsgDt").remove(); }
                        if ($("#UsrConn").length>0) { $("#UsrConn").remove(); }
                        li += '<li class="active msgusuario emp' + u + '">' +
                            '<div class="content">';
                        var i = u;
                        var c = 0;                    
                        $.each(data.c, function (key, val) {
                            if (c_u != val["ui"] || c == 0) {
                                c_u = val["ui"];
                                if (c == 0) {                                
                                    li+='<div class="msg_sep">' +
                                        '   <abbr class="timeago" title="' + val["d"] + '">' +
                                            val["d"] +
                                        '   </abbr>' +
                                        '</div>';
                                }else 
                                    li += '</div></div>';                                                            
                                li += '<div class="row" style="margin-bottom:11px;">' +
                                    '  <div class="two columns"><a href=# class=th><img src="/Recursos/Avatar?uid=' + val["ui"] + '" style="width:45px;height:45px" /></a></div>' +
                                    '  <div class="ten columns">'+
                                    '    <div><b>' + val["un"];
                                if (val["ei"] != 0) {
                                    li += ' de ' + val["en"]; 
                                    yoe = ' '+ val["en"];
                                }
                                li+='</b></div>'+
                                    '<p class=conversacion><span id="mg'+ val["i"] +'" >';
                                li+= (val["t"]==1) ? '<hr />'+ val["m"] +'</span></p>' : val["m"] +'</span></p>';
                            }else{
                                var pos = li.lastIndexOf("</p>");
                                var a = li;
                                var b = ' <br /><span id="mg'+ val["i"] +'" >'+ val["m"] +'</span>';                            
                                li = [a.slice(0, pos), b, a.slice(pos)].join('');                            

                            }                        
                            if (val["ui"] != yo && val["t"]=="3" && val["ui"] != "<%: System.Configuration.ConfigurationManager.AppSettings["MT_admusr"].ToString() %>" ) {
                                $("#msg_id").val(val["i"]);
                                $.doTimeout( 'utp', <%: System.Configuration.ConfigurationManager.AppSettings["MT_delaychequeomensajes"].ToString() %>, function(){                
                                    TaTipeando(val["ui"]);                
                                    return true;
                                });                       
                            }    
                            dt = val["d"];                    
                            c++;
                        });
                        li += '</div></div></div>';                                   
                        online = (data.o) ? '<blockquote id="UsrConn"><cite class="round success label"><span class="icon-ok"></span> '+ curr_un +' está conectado! Escríbele un mensaje.</cite></blockquote>' : '<blockquote id="UsrConn"><cite class="round secondary label"><span class="icon-warning-sign"></span> '+ curr_un +' está desconectado. Su mensaje será leído después.</cite></blockquote>' ;                    
                    }
                    $("#pForm").show();                
                    $("#vMensajes").empty().append(li +"</li>");
                    if ( dt != null ) {
                        var dlm = '<div id="lstMsgDt" class="msg_sep">' +
                            '   <abbr class="timeago" title="' + dt + '">' +
                                dt +
                            '   </abbr>' +
                            '</div>'
                        $("#vMensajes li.active .content")                                        
                        .append(dlm)
                        .append(online); 
                    }                              
                    /*if ( dt != null ) {
                        $("#vMensajes li.active .content")                                        
                        .append( '<div id="lstMsgDt" class="msg_sep">' +
                                '   <abbr class="timeago" title="' + dt + '">' +
                                    dt +

                                '   </abbr>' +
                                '</div>')
                        .append(online);
                    }    */            
                    var oScrollbar = $('#scrollbar1');
                    oScrollbar.tinyscrollbar();
                    oScrollbar.tinyscrollbar_update("bottom");
                    $("abbr.timeago").timeago();
                    $("body").css("cursor", "auto");
            });     
            //$("#dbg").val($("#vMensajes").html());                                          
            $.doTimeout( 'umn', <%: System.Configuration.ConfigurationManager.AppSettings["MT_delaychequeomensajes"].ToString() %>, function(){                                
                ListarMensajesNuevos(e,u);                
                return true;
            });               
        }          
    </script>
</asp:Content>
