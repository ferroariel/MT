<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%
    string u = Server.UrlEncode(Request.Url.AbsoluteUri);
%>
<ul class="nav-bar" >
    <li class="has-flyout"><a href="#"><span class="icon icon-bookmark-empty"></span></a>
        <a href="#" class="flyout-toggle"><span></span></a>
        <div class="flyout">
            <h5>Comparte</h5>
            <ul class="sharing">
                <li><a href="http://www.facebook.com/sharer.php?u=<%: u %>" target="_blank" class="facebook">
                    Facebook</a></li>
                <li><a href="http://twitter.com/home?status=<%: u %>" target="_blank" class="twitter">
                    Twitter</a></li>
                <li><a href="http://www.linkedin.com/shareArticle?mini=true&amp;url=<%: u %>" target="_blank"
                    class="linkedin">LinkedIn</a></li>
                <li><a href="https://skydrive.live.com/sharefavorite.aspx%2f.SharedFavorites?url=<%: u %>"
                    target="_blank" class="live">Windows Live</a></li>

                <li><a href="http://delicious.com/save?url=<%: u %>" target="_blank" class="delicious">
                    del.icio.us</a></li>
                <li><a href="http://digg.com/submit?phase=2&amp;url=<%: u %>" target="_blank" class="digg">
                    Digg</a></li>
                
                <li><a href="http://www.stumbleupon.com/submit?url=<%: u %>" target="_blank" class="stumbleupon">
                    Stumbleupon</a></li>
                <li><a href="http://furl.net/storeIt.jsp?u=<%: u %>" target="_blank" class="furl">Furl</a></li>
                <li><a href="http://blinklist.com/blink?u=<%: u %>" target="_blank" class="blinklist">
                    Blinklist</a></li>
                <li><a href="http://www.google.com/bookmarks/mark?op=edit&amp;bkmk=<%: u %>" target="_blank"
                    class="google">G Bookmarks</a></li>
                <li><a href="http://secure.diigo.com/post?url=<%: u %>" target="_blank" class="diigo">
                    Diigo</a></li>
                <li><a href="http://www.myspace.com/Modules/PostTo/Pages/?l=3&amp;u=<%: u %>" target="_blank"
                    class="myspace">My Space</a></li>
                
                <li><a href="http://www.xanga.com/private/editorx.aspx?u=<%: u %>" target="_blank"
                    class="xanga">Xanga</a></li>
                <li><a href="http://www.bebo.com/c/share?Url=<%: u %>" target="_blank" class="bebo">
                    Bebo</a></li>
                <li><a href="http://www.twine.com/bookmark/basic?u=<%: u %>" target="_blank" class="twine">
                    Twine</a></li>
                <li><a href="http://blogmarks.net/my/new.php?mini=1&amp;url=<%: u %>" target="_blank"
                    class="blogmarks">Blogmarks</a></li>
                <li><a href="http://faves.com/Authoring.aspx?u=<%: u %>" target="_blank" class="faves">
                    Faves</a></li>
                <li><a href="http://share.aim.com/share/?url=<%: u %>" target="_blank" class="aim">AIM</a></li>
                <li><a href="http://www.technorati.com/faves?add=<%: u %>" target="_blank" class="technorati">
                    Technorati</a></li>
                
                <li><a href="http://bookmarks.yahoo.com/toolbar/savebm?opener=tb&amp;u=<%: u %>"
                    target="_blank" class="ybookmarks">Y! Bookmarks</a></li>
                <li><a href="http://www.newsvine.com/_tools/seed&amp;save?popoff=0&amp;u=<%: u %>"
                    target="_blank" class="newsvine">Newsvine</a></li>
            </ul>
        </div>
    </li>
</ul>
