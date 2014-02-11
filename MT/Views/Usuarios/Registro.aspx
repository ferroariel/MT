<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.RegisterModel>" %>

<%@ Import Namespace="CaptchaMvc.HtmlHelpers" %>
<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Nuevo usuario</title>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns">
        <div id="form_signup">
            <% using (Html.BeginForm("Registro", "Usuarios", FormMethod.Post, new { @class = "custom" }))
               { %>
            <%: Html.AntiForgeryToken()%>
            <%: Html.ValidationSummary(true, "")%>
            <fieldset>
                <legend>Crear una Cuenta</legend>
                
                
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.UserName)%>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName)%>
                    <%: Html.ValidationMessageFor(m => m.UserName)%>
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password)%>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password)%>
                    <%: Html.ValidationMessageFor(m => m.Password)%>
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword)%>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword)%>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword)%>
                </div>
                <%: Html.Captcha("Recargar", "Código de seguridad", 4)%>
                <p>
                    <input type="checkbox" value="acepto" id="acepto" name="acepto" style="display: none;" />
                    <span class="custom checkbox left"></span>
                    <label for="acepto">
                        &nbsp;&nbsp;Acepto los <a href="#" class="ugh">términos y condiciones</a> de uso
                        del sitio.</label>
                </p>
                <p>
                    <input type="submit" id="env" value="Registrar" class="button" disabled="disabled" />
                    o
                    <%: Html.ActionLink("Cancelar", "Index", "Directorio")%>
                </p>
            </fieldset>
            <% } %>
        </div>
    </div>
    
    <script>
        $(document).ready(function () {
            $("#acepto, span.checkbox").click(function () {
                var b = ($(this).is(":checked") || !$(this).hasClass("checked")) ? false : true;
                $("#env").attr("disabled", b);
            });
            $("#form_signup form").validate({
                rules: {
                    UserName: { required: true, email: true },
                    Password: { required: true },
                    ConfirmPassword: { required: true, equalTo: "#Password" },
                    CaptchaInputText: { required: true }
                },
                messages: {
                    UserName: { required: "Ingrese su Usuario / E-mail", email: "E-mail invalido" },
                    Password: { required: "Ingrese su clave" },
                    ConfirmPassword: { required: "Confirme su clave", equalTo: "Las claves no coinciden" },
                    CaptchaInputText: { required: "Ingrese código de seguridad" }
                }
            });
            $("#form_signup form").submit(function (e) {
                if ($(this).valid()) {
                    return true;
                } else {
                    return false;
                }
            });
        });
    </script>
</asp:Content>
