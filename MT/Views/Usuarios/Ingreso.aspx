<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Entrar</title>
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
    <script type="text/javascript" src="/Scripts/openid-jquery.js"></script>
    <script type="text/javascript" src="/Scripts/openid-en.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            openid.init('openid_identifier');
        });
    </script>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="ten columns">
        <h4>
            Ingresar</h4>
        <div class="row">
            <div class="six columns">
                <h5>
                    Ingrese con su usuario y clave</h5>
                <fieldset id="form_login">
                    <% using (Html.BeginForm())
                       { %>
                    <%: Html.AntiForgeryToken()%>
                    <%: Html.ValidationSummary(true, "") %>
                    <div class="row">
                        <div class="two columns">
                            <label class="right inline">
                                <%: Html.LabelFor(m => m.UserName) %></label>
                        </div>
                        <div class="ten columns">
                            <%: Html.TextBoxFor(m => m.UserName, new { @class = "eight" })%>
                            <%: Html.ValidationMessageFor(m => m.UserName) %>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two columns">
                            <label class="right inline">
                                <%: Html.LabelFor(m => m.Password) %></label>
                        </div>
                        <div class="ten columns">
                            <%: Html.PasswordFor(m => m.Password, new { @class = "eight" })%>
                            <%: Html.ValidationMessageFor(m => m.Password) %>
                        </div>
                    </div>
                    <div class="row">
                        <div class="two mobile-one columns">
                        </div>
                        <div class="four mobile-one columns">
                            <input type="submit" value="Entrar" class="button" />
                        </div>
                        <div class="six mobile-two columns">
                            <%: Html.CheckBoxFor(m => m.RememberMe, new { @class = "left inline" })%>
                            <%: Html.LabelFor(m => m.RememberMe) %>
                            <p>
                                <%= Html.ActionLink("Olvidé mi clave", "ResetearClave", "Usuarios", null, new { @id = "olvida" })%></p>
                        </div>
                    </div>
                    <% } %>
                    <div class="row">
                        <div class="twelve columns">
                            <p>
                                No es usuario?
                                <%: Html.ActionLink("Regístrese", "Registro", "Usuarios") %></p>
                        </div>
                    </div>
                </fieldset>
            </div>
            <div class="six columns">
                <h5>
                    ...O con su proveedor favorito</h5>
                <form action="Authenticate?ReturnUrl=<%=HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]) %>"
                method="post" id="openid_form">
                <input type="hidden" name="action" value="verify" />
                <%//= Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
                <fieldset>
                    <div class="openid_choice">
                        <div id="openid_btns">
                        </div>
                    </div>
                    <div id="openid_input_area">
                            <%= Html.TextBox("openid_identifier") %>
                            <input type="submit"  value="Log On" />
                        </div>
                </fieldset>
                </form>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#form_login form").validate({
                rules: {
                    UserName: { required: true, email: true },
                    Password: { required: true }
                },
                messages: {
                    UserName: { required: "Ingrese su Usuario / E-mail", email: "E-mail invalido" },
                    Password: { required: "Ingrese su clave" }
                }
            });
            $("#form_login form").submit(function (e) {
                if ($(this).valid()) {
                    return true;
                } else {
                    return false;
                }
            });
        });
    </script>
</asp:Content>
