
/*
* twitter-text-js 1.4.10
*
* Copyright 2011 Twitter, Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this work except in compliance with the License.
* You may obtain a copy of the License at:
*
*    http://www.apache.org/licenses/LICENSE-2.0
*/

var jQueryScriptOutputted = false;
var jQload = null;

function initJQuery() {
    if (typeof jQuery == 'undefined' && typeof jQuery.ui == 'undefined') {
        if (!jQueryScriptOutputted) {
            jQueryScriptOutputted = true;
            document.write("<scr" + "ipt src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js\"></scr" + "ipt>");
            document.write("<scr" + "ipt src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js\"></scr" + "ipt>");
        }
        jQload = setTimeout("initJQuery()", 50);
    } else {
        $(function () {
            clearTimeout(jQload);
        });
    }
}
initJQuery();


