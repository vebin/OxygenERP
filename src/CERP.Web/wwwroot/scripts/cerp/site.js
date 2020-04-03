﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(document).ready(function () {
    initSettings(); 
});

// Returns mathematic mod, not javascript mod
// e.g. gmod(-3, 2) returns 1, whereas -3%2 returns -1
function gmod(n, m) {
    return ((n % m) + m) % m;
}

/* @param {Date}   date   - optional, default is today
** @param {number} adjust - optiona, days to adjust date by
*/
function kuwaiticalendar(date, adjust) {
    var today = date ? new Date(+date) : new Date();
    if (adjust) {
        today.setDate(today.getDate() + +adjust);
    }

    var day = today.getDate();
    var month = today.getMonth();
    var year = today.getFullYear();
    var m = month + 1;
    var y = year;

    if (m < 3) {
        y -= 1;
        m += 12;
    }

    var a = Math.floor(y / 100);
    var b = 2 - a + Math.floor(a / 4);

    if (y < 1583) b = 0;
    if (y == 1582) {
        if (m > 10) b = -10;
        if (m == 10) {
            b = 0;
            if (day > 4) b = -10;
        }
    }

    var jd = Math.floor(365.25 * (y + 4716)) + Math.floor(30.6001 * (m + 1)) + day + b - 1524;

    b = 0;
    if (jd > 2299160) {
        a = Math.floor((jd - 1867216.25) / 36524.25);
        b = 1 + a - Math.floor(a / 4);
    }

    var bb = jd + b + 1524;
    var cc = Math.floor((bb - 122.1) / 365.25);
    var dd = Math.floor(365.25 * cc);
    var ee = Math.floor((bb - dd) / 30.6001);
    day = (bb - dd) - Math.floor(30.6001 * ee);
    month = ee - 1;

    if (ee > 13) {
        cc += 1;
        month = ee - 13;
    }
    year = cc - 4716;
    var wd = gmod(jd + 1, 7) + 1;

    var iyear = 10631. / 30.;
    var epochastro = 1948084;
    var epochcivil = 1948085;

    var shift1 = 8.01 / 60.;

    var z = jd - epochastro;
    var cyc = Math.floor(z / 10631.);
    z = z - 10631 * cyc;
    var j = Math.floor((z - shift1) / iyear);
    var iy = 30 * cyc + j;
    z = z - Math.floor(j * iyear + shift1);
    var im = Math.floor((z + 28.5001) / 29.5);

    if (im == 13) im = 12;
    var id = z - Math.floor(29.5001 * im - 29);

    return [
        day,       //calculated day (CE)
        month - 1, //calculated month (CE)
        year,      //calculated year (CE)
        jd - 1,    //julian day number
        wd - 1,    //weekday number
        id,        //islamic date
        im - 1,    //islamic month
        iy         //islamic year
    ];
}

function dataBoundResponsive() {
    var visCount = 0;
    for (var i = 0; i < this.columns.length; i++) {
        if (this.columns[i].visible) visCount++;
    }
    if (visCount == this.columns.length) {
        for (var i = 0; i < this.columns.length; i++) {
            var col = this.columns[i];
            if (typeof col.customAttributes != 'undefined' && typeof col.customAttributes.id != 'undefined' && col.customAttributes.id == 'detailed')
                this.showHider.hide(col.headerText, 'headerText');
            else
                this.showHider.show(col.headerText, 'headerText');
        }
    }
    if (screen.width <= 600)
        this.autoFitColumns();
}

function writeIslamicDate(date, adjustment) {
    var wdNames = ["Ahad", "Ithnin", "Thulatha", "Arbaa", "Khams", "Jumuah", "Sabt"];
    var iMonthNames = ["Muharram", "Safar", "Rabi'ul Awwal", "Rabi'ul Akhir", "Jumadal Ula", "Jumadal Akhira",
        "Rajab", "Sha'ban", "Ramadan", "Shawwal", "Dhul Qa'ada", "Dhul Hijja"];
    var iDate = kuwaiticalendar(date, adjustment);
    var outputIslamicDate = wdNames[iDate[4]] + ", " + iDate[5] + " " +
        iMonthNames[iDate[6]] + " " + iDate[7] + " AH";
    return outputIslamicDate;
}

function initSettings() {
    $('.navbar-toggler').on('click', function () {
        let hB = $('#header-brand');
        if (hB.attr('src') != '/img/logos/OxygenERPLogo.png') {
            hB.attr('src', '/img/logos/OxygenERPLogo.png');

            hB.animate({ height: '113%' }, 500);
        }
        else {
            hB.animate({ height: '100%' }, 300);
            hB.attr('src', '/img/logos/OxygenERP 2.3.png');
        }
    });
    Date.prototype.getWeekOfMonth = function (exact) {
        var month = this.getMonth()
            , year = this.getFullYear()
            , firstWeekday = new Date(year, month, 1).getDay()
            , lastDateOfMonth = new Date(year, month + 1, 0).getDate()
            , offsetDate = this.getDate() + firstWeekday - 1
            , index = 1 // start index at 0 or 1, your choice
            , weeksInMonth = index + Math.ceil((lastDateOfMonth + firstWeekday - 7) / 7)
            , week = index + Math.floor(offsetDate / 7)
            ;
        if (exact || week < 2 + index) return week;
        return week === weeksInMonth ? index + 5 : week;
    };
    Date.prototype.getWeek = function () {
        return $.datepicker.iso8601Week(this);
    }
    $(".DatePicker").on('change', function () {
        $(this).parent().parent().siblings().find(".DatePickerHijri").val(writeIslamicDate(new Date($(this).val()), 0));
    });
    initDatePicker();


    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    //toastr.options.positionClass = 'toast-bottom-right';
    toastr.options.closeButton = true;
        
    toastr.options.showEasing = 'swing';
    toastr.options.hideEasing = 'linear';
    toastr.options.closeEasing = 'linear';

    toastr.options.showMethod = 'slideDown';
    toastr.options.hideMethod = 'slideUp';
    toastr.options.closeMethod = 'slideUp';

    toastr.options.progressBar = true;
}

function initDatePicker() {
    $('.DatePicker').datepicker();
    //$('.DatePicker').attr('readonly', 'readonly');
    $('.DatePicker').css('cursor', 'default');

    $('.DatePicker').calendarsPicker({
        yearRange: 'c-40:c+6',
        dateFormat: 'dd-M-yyyy',
    });


    $('.DatePicker,.DatePickerHijri').attr('autocomplete', 'off');

    $('.DatePicker').on("keypress paste", function (e) {
        e.preventDefault();
    })

    $('.DatePickerHijri').calendarsPicker($.extend({
        calendar: $.calendars.instance('Islamic', 'ar'),
        yearRange: 'c-40:c+6',
        dateFormat: 'dd-m-yyyy',
    }, $.calendarsPicker.regionalOptions['ar']));
}

function objectifyForm(formArray) {//serialize data function
    var returnArray = {};
    for (var i = 0; i < formArray.length; i++) {
        var curName = formArray[i]['name'];
        var newObjIndex = curName.indexOf('.');
        if (newObjIndex != -1) {
            var objName = curName.substring(0, newObjIndex);
            var propName = curName.substring(newObjIndex + 1);
            if (returnArray[objName] == null)
                returnArray[objName] = {};
            returnArray[objName][propName] = formArray[i]['value'];
        }
        else
            returnArray[curName] = formArray[i]['value'];
    }
    return returnArray;
}
