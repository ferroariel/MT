<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<MT.Models.MT_EMPRESA_SEGUIDORE>>" %>
<%@ Import Namespace="MT.Models" %>
<%@ Import Namespace="MT.Controllers" %>
<ul class="nav-bar">
    <li class="has-flyout"><a href="#"><span class="icon icon-group"></span>
        <!--<%: Model.Count().ToString() %>
        seguidores--></a> <a href="#" class="flyout-toggle"><span></span></a>
        <div class="flyout">
            <h5>
                Seguidores</h5>
            <% 
           MTrepository mtdb = new MTrepository();
           int k = ViewBag.MiID;
           foreach (var item in Model)
           { 
            %>
            <div class="row">
                <div class="one columns">
                </div>
                <div class="two columns">
                    <a href="#" class="th"><img src="/Recursos/Avatar?uid=<%: Html.Raw(item.NRO_USUARIO.ToString()) %>" style="height: 30px;
                        width: 30px" /></a>
                </div>
                <div class="one columns">
                </div>
                <div class="seven columns">
                    <p>
                        <b>
                            <%: (k != item.NRO_USUARIO) ? Html.Raw(mtdb.NombreyApellidoUsuario(item.NRO_USUARIO)) : Html.Raw("Tú") %></b></p>
                </div>
                <div class="one columns">
                </div>
            </div>
            <% } %>
            <% if (!String.IsNullOrEmpty(ViewBag.Autor))
               { %>
            <div class="row">
                <div class="twelve columns">
                    <a href="#" id="bMensajeSeguidores" class="button small" data-reveal-id="pMensaje">Mensaje
                        a Seguidores</a>
                </div>
            </div>
            <%} %>
        </div>
    </li>    
</ul>
