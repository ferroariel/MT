<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MT.Models.ForgotPasswordModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleContent" runat="server">
    <script src="//ajax.aspnetcdn.com/ajax/jquery.validate/1.9/jquery.validate.min.js"
        type="text/javascript"></script>
    <title>Resetear Clave</title>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="three columns">
        <h4>
                Reseteo de clave</h4>
        <div id="form_reset_pass">
            
            <%= Html.ValidationSummary(true)%>
            <% using (Html.BeginForm())
               { %>
            <% if (Session["ValidateWithUsername"] == null)
               {%>
            <%= Html.LabelFor(m => m.Email)%>
            <%= Html.TextBoxFor(m => m.Email)%>
            <%= Html.ValidationMessageFor(m => m.Email)%>
            <%}
               else
               { %>
            <%= Html.HiddenFor(m => m.Email)%>
            Tu usuario
            <input type="text" name="_MUsername" id="_MUsername" value="" />
            <%} %>
            <input type="submit" id="resetFormSubmit" value="Resetear" class="button" />
            <% } %>
        </div>
        <script>
            $("#form_reset_pass form").validate({
                rules: {
                    Email: { required: true, email: true }
                },
                messages: {
                    Email: { required: "Ingrese su E-mail registrado", email: "E-mail invalido" }
                }
            });
            $("#form_reset_pass form").submit(function (e) {
                if ($(this).valid()) {
                    return true;
                } else {
                    return false;
                }
            });
        </script>
    </div>
</asp:Content>
