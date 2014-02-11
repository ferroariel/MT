<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.MT_EMPLEO_OFERTA>" %>

<%@ Import Namespace="MT.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Nueva Oferta de Trabajo</title>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
    <script src="/Scripts/jquery-ui-1.9.1.custom.min.js"></script>
    <link rel=stylesheet type="text/css" href="/css/jquery-ui-1.9.2.custom.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% 
        string[] e = ViewBag.Empresa as string[]; 
    %>
    <div class="ten columns">
        <h4>
            <%: e[1] %></h4>
        <dl class="tabs contained tabsempresa five-up">
            <dd>
                <a href="/Usuarios/Empresa/<%: e[0] %>">Datos de Empresa</a>
            </dd>
            <dd>
                <a href="/Usuarios/Empresa/<%: e[0] %>#simpleContained2">Muro Público</a></dd>
            <dd class="active">
                <a href="#simpleContained1">Ofertas de Empleo</a></dd>
        </dl>
        <ul class="tabs-content contained">
            <li class="active" id="simpleContained1Tab">
                <h5>
                    Nueva Oferta</h5>
                <% using (Html.BeginForm("Agregar", "Empleo", FormMethod.Post, new { @class = "custom" }))
                   { %>
                <%: Html.ValidationSummary(true)%>
                <%: Html.Hidden("NRO_EMPRESA", e[0])%>
                <fieldset>
                    <legend>Datos de Oferta</legend>
                    <div class="row" style="height: 30px">
                        <div class="two columns">
                            Empresa:</div>
                        <div class="ten columns">
                            <b>
                                <%: e[1]%></b></div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Título</label>
                        </div>
                        <div class="ten columns">
                            <%: Html.EditorFor(model => model.XDE_TITULO)%>
                            <%: Html.ValidationMessageFor(model => model.XDE_TITULO)%>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Descripción</label>
                        </div>
                        <div class="ten columns">
                            <%: Html.TextAreaFor(model => model.MEM_LARGA)%>
                            <%: Html.ValidationMessageFor(model => model.MEM_LARGA)%>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="inline">
                                Desde:</label>
                        </div>
                        <div class="four columns">
                            <%: Html.EditorFor(model => model.FEC_DESDE)%>
                            <%: Html.ValidationMessageFor(model => model.FEC_DESDE)%>
                        </div>
                        <div class="two columns">
                            <label class="inline">
                                Hasta:</label>
                        </div>
                        <div class="four columns">
                            <%: Html.EditorFor(model => model.FEC_HASTA)%>
                            <%: Html.ValidationMessageFor(model => model.FEC_HASTA)%>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            Sectores:</div>
                        <div class="four columns">
                            <%
                       List<MT_EMPLEO_CATEGORIA> c = ViewBag.Categorias as List<MT_EMPLEO_CATEGORIA>;
                       foreach (MT_EMPLEO_CATEGORIA ct in c)
                       {
                            %>
                            <label for="ch<%: Html.Raw(ct.NRO_CATEGORIA.ToString()) %>">
                                <input id="ch<%: Html.Raw(ct.NRO_CATEGORIA.ToString()) %>" type="checkbox" name="_MT_CATEGORIA"
                                    value="<%: Html.Raw(ct.NRO_CATEGORIA.ToString())%>" /><span class="custom checkbox"></span>
                                <%: Html.Raw(ct.XDE_NOMBRE)%></label>
                            <%}%>
                        </div>
                        <div class="two columns">
                            Modalidad:</div>
                        <div class="four columns">
                            <%
                       List<MT_EMPLEO_TIPO> t = ViewBag.Tipos as List<MT_EMPLEO_TIPO>;
                       foreach (MT_EMPLEO_TIPO tp in t)
                       {
                            %>
                            <label for="ch<%: Html.Raw(tp.NRO_TIPO.ToString()) %>">
                                <input id="ch<%: Html.Raw(tp.NRO_TIPO.ToString()) %>" type="checkbox" name="_MT_TIPO"
                                    value="<%: Html.Raw(tp.NRO_TIPO.ToString())%>" /><span class="custom checkbox"></span>
                                <%: Html.Raw(tp.XDE_NOMBRE)%></label>
                            <%}%></div>
                    </div>
                    <p>
                        <input type="submit" value="Crear Oferta" class="button" />
                        o
                        <%: Html.ActionLink("Cancelar", "Perfil", "Usuarios")%>
                    </p>
                </fieldset>
                <% } %>
            </li>
        </ul>
    </div>
    <script>
        $(document).ready(function () {
            $("#FEC_DESDE, #FEC_HASTA").datepicker();
            $.datepicker.setDefaults(
              $.extend(
                { 'dateFormat': 'dd-mm-yy' },
                $.datepicker.regional['es']
              )
            );
            $("form.custom input[type=checkbox]").css({display:"",visibility:"hidden",width:0,padding:0,margin:0,overflow:"hidden"});
            $("form.custom").validate({
                rules: {
                    XDE_TITULO: { required: true },
                    MEM_LARGA: { required: true },
                    FEC_DESDE: { required: true, date: true },
                    FEC_HASTA: { required: true, date: true },
                    "_MT_CATEGORIA": { required: true, minlength: 1 },
                    "_MT_TIPO": { required: true, minlength: 1 }
                },
                messages: {
                    XDE_TITULO: { required: "Ingrese un título para la OFerta de Empleo" },
                    MEM_LARGA: { required: "Ingrese en detalle la Oferta de Empleo" },
                    FEC_DESDE: { required: "Ingrese la fecha de inicio de la Oferta", date: "El valor ingresado no es una fecha válida" },
                    FEC_HASTA: { required: "Ingrese la fecha de fin de la Oferta", date: "El valor ingresado no es una fecha válida" },
                    "_MT_CATEGORIA": { required: "Seleccione el o los sectores a los que aplique" },
                    "_MT_TIPO": { required: "Seleccione la o las modalidades a los que aplica" }
                }
            });
            $("form.custom").submit(function (e) {
                if ($(this).valid()) {
                    return true;
                } else {
                    return false;
                }
            });
        });
    </script>
</asp:Content>
