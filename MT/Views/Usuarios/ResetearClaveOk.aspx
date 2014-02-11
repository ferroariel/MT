<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Reseteo de Clave</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="six columns">
        <h4>
            Reseteo de clave</h4>
        <p>
            Hemos enviado instrucciones a la dirección de e-mail proporcionada
            a fin de que puedas reestablecer tus credenciales de acceso.</p>
        <p><b>Por favor, revisa tu correo para continuar</b></p>
        <%: Html.ActionLink("Volver a página de ingreso", "Ingreso", "Usuarios", null, new { @class = "button small" })%>
    </div>
</asp:Content>
