
//-----------------------------
//****** MAIN FUNCTIONS *******
//-----------------------------


$(function () {

    $("body").on("hidden.bs.modal", ".modal", function () {
        $(this).not(".static").removeData("bs.modal");
        $(".modal input:not(.static), .modal textarea:not(.static)").val("");
        $(".modal .btn-disabled").disable(true);
    });

    $("#sideWrap a:first").tab("show");

    //---------------------------------------

    $(".btn-submit").disable(false);

    $("body").on("submit", "form", function () {
        $(this).find(".btn-submit").button("loading");
        $(".btn-single").disable(true);
    });


    $("body").on("click", "#header .hdrMenu", function () {
        $("#header .navbar-toggle").trigger("click");
    });

    $("body").on("click", ".btnPrint", function () {
        window.print();
    });

    $("body").on("click", "[data-hover='tooltip']", function () {
        $(".tooltip").remove();
    });

    //---------------------------------------

    setSelected();
    setChosen();
    //setDatePicker();
    dateTimeToDate();
    formatPhone();

});

// Disable function
jQuery.fn.extend({
    disable: function (state) {
        return this.each(function () {
            this.disabled = state;
        });
    }
});

//--------------------------------
//****** STARTUP FUNCTIONS *******
//--------------------------------


function setChosen() {

    //$(".chosen").chosen();
    //$(".chosen-limit").chosen({ max_selected_options: 3 });
}

function setSelected() {
    $("select[data-selected]").each(function () {

        var def = $(this).attr("data-selected");

        if ($(this).is('[multiple]')) {
            def = def.split(",");
        }
        $(this).val(def);

    });
}

function setDatePicker() {
    $('.datepicker').datetimepicker({
        viewMode: 'years',
        format: 'MM/DD/YYYY'
    });

    $('.datepicker-alt').datetimepicker({
        maxDate: new Date(),
        format: 'MM/DD/YYYY'
    });

    $('.datepicker-future').datetimepicker({
        format: 'MM/DD/YYYY'
    });
}

function dateTimeToDate() {
    $(".DateTimeToDate").each(function () {
        if ($(this).html() != "") {
            $(this).html($(this).html().split(" ")[0]);
        } else {
            $(this).val($(this).val().split(" ")[0]);
        }
    });
}

function formatNumbers(obj) {

    obj.find(".toCurrency").each(function () {
        var num = parseFloat($(this).html());
        if (isNaN(num)) return;

        $(this).html(currency_tag + num.formatMoney(2));
    });

    obj.find(".toNumber").each(function () {
        var n = $(this).html().replace(/[^\d.-]/g, '') * 1;
        var parts = n.toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        $(this).html(parts.join("."));
    });
}

Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

function formatPhone() {
    $('.FormatPhone').each(function () {
        var num = $(this).html().replace(/[^\d]/g, "");;
        var phone = "";
        if (num.length == 10) {
            phone = "(" + num.substr(0, 3) + ")" + " " + num.substr(3, 3) + "-" + num.substr(6, 4);
        } else if (num.length == 11) {
            phone = "+" + num.substr(0, 1) + " (" + num.substr(1, 3) + ")" + " " + num.substr(4, 3) + "-" + num.substr(7, 4);
        } else if (num.length == 12) {
            phone = "+" + num.substr(0, 2) + " " + num.substr(2, 2) + ")" + " " + num.substr(4, 4) + " " + num.substr(8, 4);
        } else if (num.length == 13) {
            phone = "+" + num.substr(0, 3) + " " + num.substr(3, 2) + ")" + " " + num.substr(5, 4) + " " + num.substr(9, 4);
        } else {
            phone = num;
        }

        $(this).html(phone);
    });
}

function LoadSpinners() {

    var spinner = new Spinner(opts).spin();
    $(".spin").append(spinner.el);

}




//-------------------------------
//****** TOGGLE FUNCTIONS *******
//-------------------------------

$(function () {

    $(".toggle-show:checked, .toggle-show>input:checked").closest(".wrap").find(".toggle-hidden").show();
    $(".toggle-hide:checked, .toggle-hide>input:checked").closest(".wrap").find(".toggle-hidden").hide();

    $(".toggle-show-first>input:checked").closest(".wrap").find(".toggle-hidden").first().show();
    $(".toggle-show-last>input:checked").closest(".wrap").find(".toggle-hidden").last().show();

    $(".toggle-toggle").closest(".wrap").find("li.active").closest(".toggle-hidden").slideDown("fast");

    $("body").on("click", ".toggle-toggle", function () {

        $(this).removeClass("active");
        if ($(this).closest(".wrap").find(".toggle-hidden").is(":visible")) {
            $(this).closest(".wrap").find(".toggle-hidden").slideUp("fast");
        } else {
            $(this).closest(".wrap").find(".toggle-hidden").slideDown("fast");
        }

    });

    $("body").on("click", ".toggle-show", function () {
        $(this).closest(".wrap").find(".toggle-hidden").slideDown("fast");
    });

    $("body").on("click", ".toggle-hide", function () {
        $(this).closest(".wrap").find(".toggle-hidden").slideUp("fast");
    });

    $("body").on("click", ".toggle-show-first", function () {
        $(this).closest(".wrap").find(".toggle-hidden").first().slideDown("fast");
    });

    $("body").on("click", ".toggle-show-last", function () {
        $(this).closest(".wrap").find(".toggle-hidden").last().slideDown("fast");
    });

});




// ------------------------
// ****** AJAX FORMS ******
// ------------------------


$(function () {

    $("body").on("click", ".btn-confirm", function () {

        var cur = $(this);
        var msg = cur.attr("data-msg");

        var r = confirm(msg);
        if (r == true) {
            cur.closest("form").submit();
        }
    });
});

function BeginFilter(obj) {

    $("#updating").slideDown("fast");
    //Mask search results?
}

function CompleteFilter(xhr) {
    $("#searchWrap").html(xhr.responseText);
    $("button").button("reset");
    $("#updating").slideUp("fast");
}


// ------------------------
// ****** MODAL FORMS ******
// ------------------------

$(function () {

    $(document).on('change', '#Sector', function () {
        var selection = this.value;

        $("#sectorAddon").html(selection);
    });

    $(document).on('change', '#Prices', function () {
        var selection = this.value;

        hideShowOtherPrice(selection);
    });

    function hideShowOtherPrice(selection) {
        if (selection === 99999) {
            $('#Price').val('');
            $('.other-price').css('display', '');
        }
        else {
            $(".other-price").css('display', 'none');
            $('#Price').val(selection);
        }
    }

    $(document).on('change', '#IntendedRecipientsDropDown', function () {
        var selection = this.value;

        hideShowOtherIntendedRecipients(selection);
    });

    function hideShowOtherIntendedRecipients(selection) {
        if (selection === 'Other') {
            $('#IntendedRecipients').val('');
            $('.other-intendedRecipients').css('display', '');
        }
        else {
            $('#IntendedRecipients').val(selection);
            $(".other-intendedRecipients").css('display', 'none');
        }
    }

    $(document).on('change', '#PreferredFormatDropDown', function () {
        var selection = this.value;

        hideShowOtherPreferredFormat(selection);
    });

    function hideShowOtherPreferredFormat(selection) {
        if (selection === 'Other') {
            $('#PreferredFormat').val('');
            $('.other-preferredFormat').css('display', '');
        }
        else {
            $('#PreferredFormat').val(selection);
            $(".other-preferredFormat").css('display', 'none');
        }
    }

    $(document).on('change', '#FrequencyOfUseDropDown', function () {
        var selection = this.value;

        hideShowOtherFrequencyOfUse(selection);
    });

    function hideShowOtherFrequencyOfUse(selection) {
        if (selection === 'Other') {
            $('#FrequencyOfUse').val('');
            $('.other-frequencyOfUse').css('display', '');
        }
        else {
            $('#FrequencyOfUse').val(selection);
            $(".other-frequencyOfUse").css('display', 'none');
        }
    }

    var defaultRangeValidator = $.validator.methods.range;
    $.validator.methods.range = function (value, element, param) {
        if (element.type === 'checkbox') {
            // if it's a checkbox return true if it is checked
            return element.checked;
        } else {
            // otherwise run the default validation function
            return defaultRangeValidator.call(this, value, element, param);
        }
    };
});