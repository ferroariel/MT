<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MT.Models.BuscarEmpleoModel>" %>
<%@ Import Namespace="MT.Models" %>

<% using (Html.BeginForm("Index", "Empleo", FormMethod.Get, new { @id = "EmplBs" }))
   { %>
<%: Html.ValidationSummary(true)%>
<div class="row collapse">
    <div class="nine mobile-three columns editor-field">
        <input type=text name="Buscar" id="Buscar" placeholder="Ej: diseño" value="" />
    </div>
    <div class="three columns">
        <!--<input type="button" value="   " class="button small expand postfix icon-search" />-->
        <a id="bsEmp" href="#" class="button expand postfix icon-search"></a>
    </div>    
</div>
<script>
    $(document).ready(function () {
        if (localStorage.getItem("MT_Empleo") != null) {
            $("#Buscar").val(localStorage.getItem("MT_Empleo"));
        }
        $("#bsEmp").click(function (e) {
            e.preventDefault();
            $("#EmplBs").submit();
        });
        $("#EmplBs").submit(function () {
            localStorage.setItem("MT_Empleo", $("#Buscar").val());
            return true;
        });
    });
    </script>
<% } %>
