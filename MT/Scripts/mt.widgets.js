﻿
/*
* Widget de Actividad
*
* Copyright 2013 IndexAr, para MercadoTextil.com
*
* Algunos derechos reservados.
*/

(function ($) {

    /***********************************************************
    *          Base Mercado Textil Widget - 2013
    ***********************************************************
    */
    $.widget('nt.mtact', {
        options: {
            width: 150,
            height: 300,
            mcount: 4,
            empkey: 0,
            server: "http://localhost:5002"
        },
        widgetEventPrefix: 'mtact:',
        _create: function () {
            this.element.empty().addClass('mt-actividad');
            this._container = $('<div class="actividad-contenedor"></div>')
            .appendTo(this.element);
            this._setOptions({
                'width': this.options.width,
                'height': this.options.height,
                'mcount': this.options.mcount,
                'empkey': this.options.empkey,
                'server': this.options.server
            });
            crearListaMensajes(this);
        },
        _destroy: function () {
            this.element.removeClass('mt-actividad');
            this.element.empty();
            this._super();
        },
        _setOption: function (key, value) {
            var self = this,
            prev = this.options[key],
            fnMap = {
                'width': function () {
                    self.element.find('.actividad-contenedor')
                    .css('width', value + 'px');
                },
                'height': function () {
                    self.element.find('.actividad-contenedor')
                    .css('height', value + 'px');
                },
                'mcount': function () { value },
                'empkey': function () { value },
                'server': function () { value }
            };
            this._super(key, value);
            if (key in fnMap) {
                fnMap[key]();
                this._triggerOptionChanged(key, prev, value);
            }
        },
        _triggerOptionChanged: function (optionKey, previousValue, currentValue) {
            this._trigger('setOption', { type: 'setOption' }, {
                option: optionKey,
                previous: previousValue,
                current: currentValue
            });
        }
    });

    // Funciones privadas
    function crearListaMensajes(w) {
        w._container.append('<div class="mtact-head"></div>');
        cargarMuro(w);        
    }
    function cargarMuro(w) {
        w._container.find('.mtact-bar').remove();
        var msgBar = $('<ul class="mtact-bar"></ul>');
        $.getJSON(
            w.options.server +"/Recursos/CargarMuro?_e=" + w.options.empkey + "&_c=" + w.options.mcount +"&_d="+ (new Date()).getTime(),
            null,
            function (data) {
                d = data;
                if (data.ok && data.d.length > 0) {
                    $.each(data.d, function (idx, msg) {
                        var m = $('<li><abbr class=timeago title="' + msg["x"] + '">' + msg["x"] + '</abbr>' + msg["m"] + '</li>');
                        m.find('br').remove();
                        m.appendTo(msgBar);
                        w._container.append(msgBar);
                    });
                    /*$('.mtact-bar li img').each(function () {
                        if (!this.complete || typeof this.naturalWidth == "undefined" || this.naturalWidth == 0) {
                            this.remove();
                        }
                    });*/
                    $(".timeago").timeago();
                    NuevosMensajes(w);
                }
            });
    }
    function NuevosMensajes(w) {
        $.getJSON(
            w.options.server +"/Recursos/ChequearMuro?_e=" + w.options.empkey + "&_d=" + (new Date()).getTime(),
            null,
            function (data) {
                if (data.ok) {
                    cargarMuro(w);
                }
            });
    }
})(jQuery);

/**
* Timeago is a jQuery plugin that makes it easy to support automatically
* updating fuzzy timestamps (e.g. "4 minutes ago" or "about 1 day ago").
*
* @name timeago
* @version 0.11.4
* @requires jQuery v1.2.3+
* @author Ryan McGeary
* @license MIT License - http://www.opensource.org/licenses/mit-license.php
*
* For usage and examples, visit:
* http://timeago.yarp.com/
*
* Copyright (c) 2008-2012, Ryan McGeary (ryan -[at]- mcgeary [*dot*] org)
*/
(function ($) {
    $.timeago = function (timestamp) {
        if (timestamp instanceof Date) {
            return inWords(timestamp);
        } else if (typeof timestamp === "string") {
            return inWords($.timeago.parse(timestamp));
        } else if (typeof timestamp === "number") {
            return inWords(new Date(timestamp));
        } else {
            return inWords($.timeago.datetime(timestamp));
        }
    };
    var $t = $.timeago;

    $.extend($.timeago, {
        settings: {
            refreshMillis: 60000,
            allowFuture: false,
            strings: {
                prefixAgo: null,
                prefixFromNow: null,
                suffixAgo: "",
                suffixFromNow: "",
                seconds: "Recién",
                minute: "Un minuto",
                minutes: "%d minutos",
                hour: "Una hora atrás",
                hours: "%d horas atrás",
                day: "Ayer",
                days: "%d días atrás",
                month: "el mes pasado",
                months: "Hace %d meses",
                year: "Un año atrás",
                years: "%d años atrás",
                wordSeparator: " ",
                numbers: []
            }
        },
        inWords: function (distanceMillis) {
            var $l = this.settings.strings;
            var prefix = $l.prefixAgo;
            var suffix = $l.suffixAgo;
            if (this.settings.allowFuture) {
                if (distanceMillis < 0) {
                    prefix = $l.prefixFromNow;
                    suffix = $l.suffixFromNow;
                }
            }

            var seconds = Math.abs(distanceMillis) / 1000;
            var minutes = seconds / 60;
            var hours = minutes / 60;
            var days = hours / 24;
            var years = days / 365;

            function substitute(stringOrFunction, number) {
                var string = $.isFunction(stringOrFunction) ? stringOrFunction(number, distanceMillis) : stringOrFunction;
                var value = ($l.numbers && $l.numbers[number]) || number;
                return string.replace(/%d/i, value);
            }

            var words = seconds < 45 && substitute($l.seconds, Math.round(seconds)) ||
        seconds < 90 && substitute($l.minute, 1) ||
        minutes < 45 && substitute($l.minutes, Math.round(minutes)) ||
        minutes < 90 && substitute($l.hour, 1) ||
        hours < 24 && substitute($l.hours, Math.round(hours)) ||
        hours < 42 && substitute($l.day, 1) ||
        days < 30 && substitute($l.days, Math.round(days)) ||
        days < 45 && substitute($l.month, 1) ||
        days < 365 && substitute($l.months, Math.round(days / 30)) ||
        years < 1.5 && substitute($l.year, 1) ||
        substitute($l.years, Math.round(years));

            var separator = $l.wordSeparator === undefined ? " " : $l.wordSeparator;
            return $.trim([prefix, words, suffix].join(separator));
        },
        parse: function (iso8601) {
            var s = $.trim(iso8601);
            s = s.replace(/\.\d+/, ""); // remove milliseconds
            s = s.replace(/-/, "/").replace(/-/, "/");
            s = s.replace(/T/, " ").replace(/Z/, " UTC");
            s = s.replace(/([\+\-]\d\d)\:?(\d\d)/, " $1$2"); // -04:00 -> -0400
            return new Date(s);
        },
        datetime: function (elem) {
            var iso8601 = $t.isTime(elem) ? $(elem).attr("datetime") : $(elem).attr("title");
            return $t.parse(iso8601);
        },
        isTime: function (elem) {
            // jQuery's `is()` doesn't play well with HTML5 in IE
            return $(elem).get(0).tagName.toLowerCase() === "time"; // $(elem).is("time");
        }
    });

    $.fn.timeago = function () {
        var self = this;
        self.each(refresh);

        var $s = $t.settings;
        if ($s.refreshMillis > 0) {
            setInterval(function () { self.each(refresh); }, $s.refreshMillis);
        }
        return self;
    };

    function refresh() {
        var data = prepareData(this);
        if (!isNaN(data.datetime)) {
            $(this).text(inWords(data.datetime));
        }
        return this;
    }

    function prepareData(element) {
        element = $(element);
        if (!element.data("timeago")) {
            element.data("timeago", { datetime: $t.datetime(element) });
            var text = $.trim(element.text());
            if (text.length > 0 && !($t.isTime(element) && element.attr("title"))) {
                element.attr("title", text);
            }
        }
        return element.data("timeago");
    }

    function inWords(date) {
        return $t.inWords(distance(date));
    }

    function distance(date) {
        return (new Date().getTime() - date.getTime());
    }

    // fix for IE6 suckage
    document.createElement("abbr");
    document.createElement("time");
} (jQuery));