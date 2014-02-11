<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<style type="text/css">
    .shadow-wrap
    {
        position: relative;
        height: 200px;
        width: 300px;
        margin: 0 20px;
        zoom: 1;
    }
    .shadow-wrap .shadow-inner-1, .shadow-wrap .shadow-inner-2
    {
        position: absolute;
        bottom: 6px;
        width: 48%;
        height: 50px;
        z-index: 1;
        -moz-box-shadow: rgba(0,0,0,0.30) 0 6px 20px 0;
        -webkit-box-shadow: rgba(0,0,0,0.30) 0 6px 20px;
        box-shadow: rgba(0,0,0,0.30) 0 6px 20px 0;
    }
    .shadow-wrap .shadow-inner-3, .shadow-wrap .shadow-inner-4
    {
        position: absolute;
        top: 6px;
        width: 48%;
        height: 50px;
        z-index: 1;
        -moz-box-shadow: rgba(0,0,0,0.30) 0 -6px 20px 0;
        -webkit-box-shadow: rgba(0,0,0,0.30) 0 -6px 20px;
        box-shadow: rgba(0,0,0,0.30) 0 -6px 20px 0;
    }
    .shadow-wrap .shadow-inner-1
    {
        left: 6px;
        /*background-color: yellow;*/
        -webkit-transform: rotate(3deg);
        -moz-transform: rotate(3deg);
    }
    .shadow-wrap .shadow-inner-2
    {
        left: auto;
        right: 6px;
        /*background-color: orange;*/
        -webkit-transform: rotate(-3deg);
        -moz-transform: rotate(-3deg);
    }
    .shadow-wrap .shadow-inner-3
    {
        left: auto;
        right: 6px;
        /*background-color: Lime;*/
        -webkit-transform: rotate(3deg);
        -moz-transform: rotate(3deg);
    }
    .shadow-wrap .shadow-inner-4
    {
        left: 6px;
        right: auto;
        /*background-color: red;*/
        -webkit-transform: rotate(-3deg);
        -moz-transform: rotate(-3deg);
    }
    shadow-wrap .content
    {
        display:block;
        width: 400px;
        height: 200px;
        z-index: 2000;
        position: relative;
        background: #f00 url(/Content/img/loading.gif) no-repeat 50% 50% !important;
        border: solid 6px #fff;
        -moz-box-shadow: rgba(0,0,0,0.30) 0 0 6px 0;
        -webkit-box-shadow: rgba(0,0,0,0.30) 0 0 6px;
        box-shadow: rgba(0,0,0,0.30) 0 0 6px 0;
    }
    
</style>
<!--[if IE]>
<style type="text/css">
    .shadow-wrap .shadow-inner-1,
    .shadow-wrap .shadow-inner-2{
        background:#000;bottom:30px;
    }
    .shadow-wrap{padding:0 0 10px 0;}
    .shadow-wrap .content{width:96%;margin:0 2%;}
    .shadow-wrap .shadow-inner-1{
        left:0px;
        -ms-filter: "progid:DXImageTransform.Microsoft.Matrix(M11=0.99619470, M12=0.08715574, M21=-0.08715574, M22=0.99619470,sizingMethod='auto expand')";
        filter:
            progid:DXImageTransform.Microsoft.Matrix(M11=0.99619470, M12=0.08715574, M21=-0.08715574, M22=0.99619470,sizingMethod='auto expand')
            progid:DXImageTransform.Microsoft.Blur(PixelRadius='15', MakeShadow='true', ShadowOpacity='0.40');
    }
    .shadow-wrap .shadow-inner-2{
        right:0px;
        -ms-filter: "progid:DXImageTransform.Microsoft.Matrix(M11=0.99619470, M12=-0.08715574, M21=0.08715574, M22=0.99619470,sizingMethod='auto expand')";
        filter:
            progid:DXImageTransform.Microsoft.Matrix(M11=0.99619470, M12=-0.08715574, M21=0.08715574, M22=0.99619470,sizingMethod='auto expand')
            progid:DXImageTransform.Microsoft.Blur(PixelRadius='15', MakeShadow='true', ShadowOpacity='0.40');
    }
</style>
<![endif]-->
<!--[if IE 8]>
<style type="text/css">
    .shadow-wrap .shadow-inner-2{right:30px;}
</style>
<![endif]-->
