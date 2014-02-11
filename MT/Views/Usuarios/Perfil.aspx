<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.DatosdeUsuario>" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Perfil</title>
    <link rel="stylesheet" type="text/css" href="/css/uploadify.css" />
    <script src="/Scripts/jquery.uploadify.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns">
        <dl class="tabs contained tabsempresa">
            <dd>
                <a href="/Mensajes">Mensajes y Notificaciones</a></dd>
            <dd class="active">
                <a href="#simpleContained1">Mi Perfil</a></dd>
            <dd>
                <a href="/Usuarios/Empresas">Mis Empresas</a></dd>
        </dl>
        <ul class="tabs-content contained">
            <li class="active" id="simpleContained1Tab">
                <h4>
                    Perfil de Usuario</h4>
                <h5>
                    Avatar</h5>
                <%          
                    MTrepository mtdb = new MTrepository();
                    MT_RECURSO a = new MT_RECURSO();
                    a.NRO_TIPO = 7;
                    a.NRO_USUARIO = Model.nro_usuario;
                    a.NRO_ORDEN = 0;
                    a.NRO_GALERIA = 0;
                    a.NRO_EMPRESA = 0;
                    a.XDE_ARCHIVO = String.Empty;

                    if (Model.avatar != null)
                    {
                        a = Model.avatar;
                    }%>
                <fieldset>
                    <div class="row">
                        <span class="two columns"><a href="#" class="th">
                            <img src="/Recursos/Avatar?uid=<%: a.NRO_USUARIO.ToString() %>" style="height: 70px;
                                width: auto" /></a> </span><span class="four columns">
                                    <input type="file" id="archivo" name="archivo" />                                                                
                                </span><span class="two columns"></span>
                        <span class="four columns"></span>
                        <p>
                            &nbsp;</p>
                        <p>
                            &nbsp;</p>
                    </div>
                </fieldset>                
                <script>
                    $(function () {
                        $('#archivo').uploadify({
                            'swf': '/Scripts/uploadify.swf',
                            'uploader': '/Varios/Archivo/?uid=<%: a.NRO_USUARIO.ToString() %>&irc=<%: a.NRO_RECURSO.ToString() %>&itc=<%: "7" %>',
                            'auto': true,
                            'buttonClass': 'button icon-paper-clip',
                            'buttonText' : '',
                            'method': 'post',                            
                            'onUploadComplete': function (event, ID, fileObj, response, data) {
                                $('img[src*="Avatar"]').imgrefresh();
                            }
                        });
                    });                    
                </script>
                <%
                    MT_USUARIO_PERFIL u = Model.perfil;
                    using (Html.BeginForm("Perfil", "Usuarios", FormMethod.Post, new { @class = "custom" }))
                    { %>
                <%: Html.ValidationSummary(true, "")%>
                <h5>
                    Sus datos de perfil</h5>
                <fieldset>
                    <%: Html.HiddenFor(m => Model.perfil.NRO_USUARIO)%>
                    <%: Html.HiddenFor(m => Model.perfil.NRO_USUARIO_PERFIL)%>                    
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Nombre:</label></div>
                        <div class="four columns">
                            <%: Html.TextBoxFor(m => Model.perfil.XDE_NOMBRES)%>
                            <%: Html.ValidationMessageFor(m => Model.perfil.XDE_NOMBRES)%></div>
                        <div class="two columns">
                            <label class="inline">
                                Apellido:</label></div>
                        <div class="four columns">
                            <%: Html.TextBoxFor(m => Model.perfil.XDE_APELLIDOS)%>
                            <%: Html.ValidationMessageFor(m => Model.perfil.XDE_APELLIDOS)%></div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Nacimiento</label></div>
                        <div class="four columns">
                            <%: Html.HiddenFor(m => Model.perfil.FEC_NACIMIENTO)%>
                            <div class="row">
                                <div class="three columns">
                                    <%: Html.TextBox("perfil.FEC_NACIMIENTO_DIA", null, new { @placeholder = "dd", @maxlength = 2 })%></div>
                                <div class="six columns">
                                    <select name="perfil.FEC_NACIMIENTO_MES" id="perfil_FEC_NACIMIENTO_MES" style="display: none">
                                        <option value="1">Enero</option>
                                        <option value="2">Febrero</option>
                                        <option value="3">Marzo</option>
                                        <option value="4">Abril</option>
                                        <option value="5">Mayo</option>
                                        <option value="6">Junio</option>
                                        <option value="7">Julio</option>
                                        <option value="8">Agosto</option>
                                        <option value="9">Septiembre</option>
                                        <option value="10">Octubre</option>
                                        <option value="11">Noviembre</option>
                                        <option value="12">Diciembre</option>
                                    </select>
                                </div>
                                <div class="three columns">
                                    <%: Html.TextBox("perfil.FEC_NACIMIENTO_ANO", null, new { @placeholder = "yyyy", @maxlength = 4 })%></div>
                            </div>
                        </div>
                        <div class="two columns">
                            <label class="inline">
                                Domicilio</label></div>
                        <div class="four columns">
                            <%: Html.TextBoxFor(m => Model.perfil.OBS_DOMICILIO)%>
                            <%: Html.ValidationMessageFor(m => Model.perfil.OBS_DOMICILIO)%></div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                CP</label></div>
                        <div class="four columns">
                            <%: Html.TextBoxFor(m => Model.perfil.XDE_CP)%>
                            <%: Html.ValidationMessageFor(m => Model.perfil.XDE_CP)%></div>
                        <div class="two columns">
                            <label class="inline">
                                Ciudad</label></div>
                        <div class="four columns">
                            <%: Html.TextBoxFor(m => Model.perfil.XDE_CIUDAD)%>
                            <%: Html.ValidationMessageFor(m => Model.perfil.XDE_CIUDAD)%></div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Provincia</label></div>
                        <div class="four columns">
                            <%: Html.DropDownList("perfil.NRO_PROVINCIA", ViewBag.ComboProvincias as IEnumerable<SelectListItem>)%>
                            <%//: Html.ValidationMessageFor(m => Model.perfil.MT_PROVINCIA.XDE_PROVINCIA)%></div>
                        <div class="two columns">
                            <label class="inline">
                                País</label></div>
                        <div class="four columns">
                            <%: Html.DropDownList("perfil.NRO_PAIS", ViewBag.ComboPaises as IEnumerable<SelectListItem>)%>
                            <%//: Html.ValidationMessageFor(m => Model.perfil.MT_PAISE.XDE_PAIS)%>
                            <%//: Html.AFDropDown(ViewBag.ComboPaises as IEnumerable<SelectListItem>, "PrV")%>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Web</label></div>
                        <div class="ten columns">
                            <%: Html.TextBoxFor(m => Model.perfil.XDE_WEB)%>
                            <%: Html.ValidationMessageFor(m => Model.perfil.XDE_WEB)%></div>
                    </div>
                    <p>
                        <input type="submit" value="Actualizar" id="Actualizar" class="button" />
                    </p>
                    <script type="text/javascript">
                        var nac = new Date();
                        var nac_d, nac_m, nac_a = null;
                        <%      
                        Varias v = new Varias();                                           
                        if (!String.IsNullOrEmpty(Model.perfil.FEC_NACIMIENTO.ToString()) && v.IsDate(Model.perfil.FEC_NACIMIENTO.ToString())) {
                        DateTime nc = DateTime.Parse(Model.perfil.FEC_NACIMIENTO.ToString());
                        %>
                        nac = new Date("<%: Html.Raw(Model.perfil.FEC_NACIMIENTO.ToString()) %>");
                        nac_d = "<%: Html.Raw(nc.Day.ToString()) %>";
                        nac_m = "<%: Html.Raw(nc.Month.ToString()) %>";
                        nac_a = "<%: Html.Raw(nc.Year.ToString()) %>";
                        <%} %>
                        
                        $(document).ready(function () {
                            $("#perfil_NRO_PAIS").change(function () {
                                //$("#perfil_NRO_PAIS").val($("#ComboPaises option:selected").val());
                                $("#perfil_NRO_PROVINCIA").attr("disabled", true);
                                $.ajax({
                                    url: "/Varios/ProvinciasxPais?id=" + $(this).val(),
                                    type: "GET"
                                }).done(function (data) {
                                    //alert(data.success);
                                    if (data.success) {
                                        $("#perfil_NRO_PROVINCIA").empty();
                                        $.each(data.res, function (key, val) {
                                            $('#perfil_NRO_PROVINCIA').append($('<option>', {
                                                value: val["NRO_PROVINCIA"],
                                                text: val["XDE_PROVINCIA"]
                                            }));
                                        });
                                        $("#perfil_NRO_PROVINCIA").attr("disabled", false).trigger("change");
                                    }
                                });
                            }); /*
                            $("#ComboProvincias").change(function () {
                                $("#perfil_NRO_PROVINCIA").val("");
                            });*/
                            $("dl.tabsempresa dd:eq(0)").removeClass("active");

                            $("#perfil_FEC_NACIMIENTO_DIA").change(function () {
                                nac_d = $(this).val();                                
                                setBirthDate();
                            });
                            $("#perfil_FEC_NACIMIENTO_MES").change(function () {                                
                                nac_m = $("#perfil_FEC_NACIMIENTO_MES option:selected").val();
                                setBirthDate();                                
                            });
                            $("#perfil_FEC_NACIMIENTO_ANO").change(function () {
                                nac_a = $(this).val();
                                setBirthDate();
                            });
                            $("#perfil_FEC_NACIMIENTO_DIA").val( !isNaN(nac_d) ? nac_d : "" );                                
                            $("#perfil_FEC_NACIMIENTO_ANO").val( !isNaN(nac_a) ? nac_a : "" );                                                            
                            $("#perfil_FEC_NACIMIENTO_MES").val( !isNaN(nac_m) ? nac_m : "" ).trigger("change");
                        }); 
                        function setBirthDate() {
                            nac = new Date(pad(nac_m) +"-"+ pad(nac_d) +"-"+ nac_a);                                                        
                            $("#perfil_FEC_NACIMIENTO").attr("value", ISODateString(nac));
                        }
                        function pad(n){
                            return n < 10 ? '0'+n : n;
                        }
                        function ISODateString(d){                                                    
                          var o = d.getUTCFullYear()+'-'
                              + pad(d.getUTCMonth()+1)+'-'
                              + pad(d.getUTCDate())/*+'T'
                              + pad(d.getUTCHours())+':'
                              + pad(d.getUTCMinutes())+':'
                              + d.getUTCSeconds()+'Z'*/;                              
                          return o;
                        }                                               
                    </script>
                </fieldset>
                <%}

                    if (Model.emails.Count() > 0)
                    {
                        using (Html.BeginForm("Perfil_Emails", "Usuarios", FormMethod.Post))
                        {        
                %>
                <h5>
                    Mis E-mail de contacto:</h5>
                <fieldset>
                    <%: Html.ValidationSummary(true, "") %>
                    <div class="row">
                        <ol class="six columns">
                            <% for (var i = 0; i < Model.emails.Count(); i++)
                               {
                            %>
                            <li>
                                <%:Html.EditorFor(x => Model.emails[i].XDE_EMAIL)%>
                                <%:Html.HiddenFor(x => Model.emails[i].NRO_EMAIL)%>
                                <%:Html.HiddenFor(x => Model.emails[i].NRO_EMPRESA)%>
                                <%:Html.HiddenFor(x => Model.emails[i].NRO_USUARIO)%>
                            </li>
                            <%
                               }%>
                            <li><a href="#" onclick="$('#nuevoemail').show(); $(this).hide(); return false;">Agregar</a><input
                                type="text" name="nuevoemail" value="" id="nuevoemail" /></li>
                        </ol>
                        <input type="submit" name="envema" id="envema" value="Actualizar" class="button" />
                    </div>
                </fieldset>
                <%
                        }
                    }
                    if (Model.tels.Count() > 0)
                    {%>
                <h5>
                    Mis Teléfonos:</h5>
                <fieldset>
                    <%
                        foreach (MT_TELEFONO t in Model.tels)
                        {%>
                    <%: Html.TextBox("Telefono", t.XDE_TELEFONO) %>
                    <%} %>
                </fieldset>
                <%}%>
            </li>
        </ul>
    </div>
    <script src="/Scripts/jquery.imgrefresh-1.0.min.js"></script>
</asp:Content>
