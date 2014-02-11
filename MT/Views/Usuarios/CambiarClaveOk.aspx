<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <title>Clave Actualizada</title>
</asp:Content>
<asp:Content ID="changePasswordSuccessContent" ContentPlaceHolderID="MainContent"
    runat="server">
    <div class="ten columns">
        <h4>
            Cambiar Clave</h4>
        <p>
            La clave fue actualizada correctamente.
        </p>
    </div>
</asp:Content>
