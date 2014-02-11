<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.MT_EMPRESA_PERFIL>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>CrearEmpresaPerfil</title>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
    <script type="text/javascript" src="//cdn.aloha-editor.org/latest/lib/require.js"></script>
    <script src="//cdn.aloha-editor.org/latest/lib/aloha.js" data-aloha-plugins="common/ui, common/format, common/list, common/link, common/highlighteditables"></script>
    <link href="//cdn.aloha-editor.org/latest/css/aloha.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="EmpNueva">
        <% using (Html.BeginForm())
           { %>
        <%: Html.ValidationSummary(true) %>
        <div class="nine columns">
            <h4>
                Agregar Nueva Empresa</h4>
            <p>
                Razon Social:
                <%: Html.EditorFor(model => model.XDE_RAZONSOCIAL) %>
                <%: Html.ValidationMessageFor(model => model.XDE_RAZONSOCIAL) %>
            </p>        
            <p>
                <input type="submit" value="Continuar" class="button" />
                <!--o
                <%: Html.ActionLink("Cancelar", "Perfil", "Usuarios") %>-->
            </p>
            <% } %>
        </div>
    </div>
    <script type="text/javascript">
        Aloha.ready(function () {
            var $ = Aloha.jQuery;
            $('.editable').aloha();
        });
        $(document).ready(function () {
            $("#EmpNueva form").validate({
                rules: {
                    XDE_RAZONSOCIAL: { required: true }
                },
                messages: {
                    XDE_RAZONSOCIAL: { required: "Ingrese la razón social de su Empresa" }                    
                }
            });
            $("#EmpNueva form").submit(function (e) {
                if ($(this).valid()) {
                    return true;
                } else {
                    return false;
                }
            });
            $("#ComboPaises").change(function () {
                $("#NRO_PAIS").val($("#ComboPaises option:selected").val());
                $("#ComboProvincias").attr("disabled", true);
                $.ajax({
                    url: "/Varios/ProvinciasxPais?id=" + $("#NRO_PAIS").val(),
                    type: "GET"
                }).done(function (data) {
                    if (data.success) {
                        $("#ComboProvincias").empty();
                        $.each(data.res, function (key, val) {
                            $('#ComboProvincias').append($('<option>', {
                                value: val["NRO_PROVINCIA"],
                                text: val["XDE_PROVINCIA"]
                            }));
                        });
                        $("#ComboProvincias").attr("disabled", false);
                    }
                });

            });
            $("#ComboProvincias").change(function () {
                $("#NRO_PROVINCIA").val($("#ComboProvincias option:selected").val());
            });
        });
    </script>
</asp:Content>
