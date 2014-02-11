<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Cambiar clave</title>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns nuevaclave">
        <div class="row">
            <div class="six columns">
                
                <h4>
                    Antes de continuar</h4>
                
                <p>Te recomendamos que, antes de continuar, modifiques tu contraseña
                para reestablecer la seguridad de tu cuenta.</p>
                <h5>Crea una nueva clave de acceso</h5>
                <%= Html.ValidationSummary(true, "No se realizó el cambio de contraseña. Corrija los errores e inténtelo de nuevo.") %>
                <% using (Html.BeginForm())
                   { %>
                <div class="row">
                    <div class="three columns">
                        <%= Html.LabelFor(m => m.NewPassword) %></div>
                    <div class="nine columns">
                        <%= Html.PasswordFor(m => m.NewPassword) %>
                        <%= Html.ValidationMessageFor(m => m.NewPassword) %>
                    </div>
                </div>
                <div class="row">
                    <div class="three columns">
                        <%= Html.LabelFor(m => m.ConfirmPassword) %></div>
                    <div class="nine columns">
                        <%= Html.PasswordFor(m => m.ConfirmPassword) %>
                        <%= Html.ValidationMessageFor(m => m.ConfirmPassword) %></div>
                </div>
                <div class="row">
                    <div class="three columns">
                    </div>
                    <div class="nine columns">
                        <input type="submit" id="changeFormSubmit" value="Crear y Continuar" class="button" /></div>
                </div>
                <% } %>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            jQuery.validator.addMethod(
                'ContainsAtLeastOneDigit',
                function (value) {
                    return /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/.test(value);
                },
                'La clave debe tener entre 6 y 20 caracteres, incluyendo números y letras minúsculas y mayúsculas.<br /><i>(Ej: juanFernadez12)</i>'
            );  
            var Validator = $(".nuevaclave form").validate({
                rules: {
                    NewPassword: { required: true, ContainsAtLeastOneDigit: true },
                    ConfirmPassword: { required: true, equalTo: "#NewPassword" }                   
                },
                messages: {
                    NewPassword: { required: "Ingrese su nueva clave" },
                    ConfirmPassword: { required: "Confirme su nueva clave", equalTo: "Por favor, confirme la clave" }
                }
            });
            $(".nuevaclave form").submit(function (e) {
                if ($(this).valid()) {
                    return true;
                } else {
                    return false;
                }
            });
        });
    </script>
</asp:Content>
