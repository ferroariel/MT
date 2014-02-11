/*
ESTOS ESTILOS SON EXCLUSIVOS PARA PATCHEAR LAS DISTINTAS VERSIONES DE IE PREVIAS A LA 9
INCLUYENDO IE 7 y 8 - EL SOPORTE PARA IE 6 Y VERSIONES PREVIAS ES INCIERTO
*/
/*
$(document).ready(function () {
    if ($("#home_intro").length > 0) {
        
        
    }    
});
*/

/* código para mostrar los placeholder en IE 8  - http://www.cssnewbie.com/cross-browser-support-for-html5-placeholder-text-in-forms/*/
$(function () {
    if (!$.support.placeholder) {
        var active = document.activeElement;
        $(':text').focus(function () {
            if ($(this).attr('placeholder')!= null && $(this).val() == $(this).attr('placeholder')) {
                $(this).val('').removeClass('hasPlaceholder');
            }
        }).blur(function () {
            if ($(this).attr('placeholder')!= null && ($(this).val() == '' || $(this).val() == $(this).attr('placeholder'))) {
                $(this).val($(this).attr('placeholder')).addClass('hasPlaceholder');
            }
        });
        $(':text').blur();
        $(active).focus();
        $('form').submit(function () {
            $(this).find('.hasPlaceholder').each(function () { $(this).val(''); });
        });
    }
});