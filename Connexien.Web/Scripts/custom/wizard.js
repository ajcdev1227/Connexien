$(function () {


    // Prepare the preview for profile picture
    $("body").on("change", "#wizard-picture", function () {

        if (this.files && this.files[0]) {
            var cur = $(this);

            var size = this.files[0].size;
            var ext = $(this).val().split('.').pop().toLowerCase();

            if ($.inArray(ext, ['gif', 'png', 'jpg', 'jpeg']) == -1 || size > 5000000) {

                cur.closest(".picture-container").find("h6 span").html(size > 5000000 ? "Too Large" : "Wrong Format");
                cur.closest(".picture-container").removeClass("active");
                cur.closest(".picture-container").addClass("error");

                cur.closest(".picture").css('background-image', "url('" + avatarUrl + "')").fadeIn('slow');

                var control = $(this);
                control.replaceWith(control.val('').clone(true));

            } else {

                cur.closest(".picture").css("background-image", "url('" + loadingGif + "')");
                cur.closest(".picture-container").find("h6 span").html("Looks Good");
                cur.closest(".picture-container").removeClass("error");
                cur.closest(".picture-container").addClass("active");

                var reader = new window.FileReader();
                reader.onload = function (e) {
                    $("#darkroom-image").attr("src", e.target.result);
                    LoadEditor();
                    
                    //$("body").css('background-image', "url('" + e.target.result + "')");
                    cur.closest(".picture").css("background-image", "url('" + e.target.result + "')").fadeIn("slow");
                    cur.closest(".picture-container").find(".picture-edit").show("fast");
                };
                reader.readAsDataURL(this.files[0]);

                cur.closest(".wrap").find(".btn-wrap").show("fast");
            }

        }

    });


    // Check profile resume
    $("body").on("change", "#wizard-resume", function () {

        var size = this.files[0].size;
        var ext = $(this).val().split('.').pop().toLowerCase();

        if ($.inArray(ext, ['doc', 'docx', 'pdf', 'txt']) == -1 || size > 5000000) {
            $(this).closest(".picture-container").find("h6 span").html(size > 5000000 ? "Too Large" : "Wrong Format");
            $(this).closest(".picture-container").removeClass("active");
            $(this).closest(".picture-container").addClass("error");

            var control = $(this);
            control.replaceWith(control.val('').clone(true));
        } else {
            $(this).closest(".picture-container").find("h6 span").html(this.files[0].name);
            $(this).closest(".picture-container").removeClass("error");
            $(this).closest(".picture-container").addClass("active");
        }
    });

    // Check profile vid
    $("body").on("change", "#wizard-vid", function () {

        var size = this.files[0].size;
        var ext = $(this).val().split('.').pop().toLowerCase();

        var formats = ['avi', 'mpeg', 'flv', 'wmv', 'wma', 'ogg', 'oga', 'ogv', 'ogx', '3gp', '3gp2', '3g2', '3gpp', '3gpp2', 'mp4', 'm4a', 'm4v', 'f4v', 'f4a', 'm4b', 'm4r', 'f4b', 'mov'];

        if ($.inArray(ext, formats) == -1 || size > 100000000) {
            $(this).closest(".picture-container").find("h6 span").html(size > 100000000 ? "Too Large" : "Wrong Format");
            $(this).closest(".picture-container").removeClass("active");
            $(this).closest(".picture-container").addClass("error");

            $(this).closest(".picture").find(".glyphicon").removeClass("glyphicon-ok").addClass("glyphicon-play");

            var control = $(this);
            control.replaceWith(control.val('').clone(true));
        } else {
            $(this).closest(".picture-container").find("h6 span").html(this.files[0].name);
            $(this).closest(".picture-container").removeClass("error");
            $(this).closest(".picture-container").addClass("active");

            $(this).closest(".picture").css("background-image", "none");
            $(this).closest(".picture").find(".glyphicon").removeClass("glyphicon-play").addClass("glyphicon-ok");
            $(this).closest(".wrap").find(".btn-wrap").show("fast");
        }
    });





});



//-------------------------------------------------------------



$(function () {
    /*
    $("body").on("click", ".btn-next", function () {

    var submitOpt = {
    beforeSend: function () {
    //$(".btn-wrap").hide("fast");
    $(".btn-finish").button("loading");
    $(".welcomeProgress .progress-bar").width(0);
    $(".welcomeProgress .progress-bar").html("Uploading 0%");
    $(".welcomeProgress").show();
    },
    uploadProgress: function (event, position, total, percentComplete) {
    var percentValue = ((position / total) * 100).toFixed(0) + '%';
    $(".welcomeProgress .progress-bar").css("width", percentValue);
    $(".welcomeProgress .progress-bar").html("Uploading " + percentValue);

    if (percentValue == "100%")
    $(".welcomeProgress .progress-bar").html("Processing...");
    },
    success: function () {
    $(".btn-finish").button("reset");
    var percentValue = '100%';
    $(".welcomeProgress .progress-bar").width(percentValue);
    $(".welcomeProgress .progress-bar").html("Processing...");
    },
    complete: function (xhr) {
    $(".welcomeProgress").delay(500).hide(0);

    if (xhr.responseText == "true") {
    if (partial == "all") {
    LoadPartials();
    } else {
    partial.split(",").forEach(function(v) {
    LoadPartial(v);
    });
                        
    }
    } else {
    $(".btn-next").button("reset");
    toastr.error("File could not be uploaded.<br>Please try again later.");
    }
    }
    };

    var i = $(this).attr("data-tab") * 1;

    if (i == 0) {
    //$("#wzForm1").ajaxSubmit(submitOpt);
    } else if (i == 1) {
    //$("#wzForm2").ajaxSubmit(submitOpt);
    } else {
    $(this).button("loading");
    partial = $(this).closest(".wrap").find("form").attr("data-reload");
    $(this).closest(".wrap").find("form").ajaxSubmit(submitOpt);
    }

    $(this).attr("data-tab", i + 1);

    });*/
    /*
    $("body").on("click", ".btn-previous", function () {
    var nxt = $(this).closest(".wizard-footer").find(".btn-next");
    var i = nxt.attr("data-tab") * 1;
    nxt.attr("data-tab", i - 1);
    });*/

    $(".btn-finish").click(function () {

        var submitOpt = {
            beforeSend: function () {
                $(".wizard-body").slideUp("fast");
                $(".wizard-footer").slideUp("fast");
                $(".wizard-wait").slideDown("fast");

                $(".welcomeProgress .progress-bar").width(0);
                $(".welcomeProgress .progress-bar").html("Uploading 0%");
                $(".welcomeProgress").show();
            },
            uploadProgress: function (event, position, total) {
                var percentValue = ((position / total) * 100).toFixed(0) + '%';
                $(".welcomeProgress .progress-bar").css("width", percentValue);
                $(".welcomeProgress .progress-bar").html("Uploading " + percentValue);

                if (percentValue == "100%")
                    $(".welcomeProgress .progress-bar").html("Processing...");
            },
            success: function () {
                $(".btn-finish").button("reset");
                var percentValue = '100%';
                $(".welcomeProgress .progress-bar").width(percentValue);
                $(".welcomeProgress .progress-bar").html("Processing...");
            },
            complete: function (xhr) {
                $(".welcomeProgress").delay(500).hide(0);

                if (xhr.responseText == "true") {
                    if (partial == "all") {
                        $(".wizard-close-hide").slideUp("fast");
                        $(".wizard-close").slideDown("fast");
                    }
                } else {
                    $(".wizard-body").slideDown("fast");
                    $(".wizard-footer").slideDown("fast");
                    $(".wizard-wait").slideUp("fast");

                    if ($("#Password").val().length < 6)
                        toastr["error"]("Please go back to step #1 and enter a longer password.", "Error");
                    
                    if ($("#Password").val() != $("#ConfirmPassword").val())
                        toastr["error"]("Please go back to step #1 and confirm your password.", "Error");

                    toastr["error"]("Yikes! We could not setup your profile.", "Error");

                }
            }
        };

        $("#wzForm").ajaxSubmit(submitOpt);

    });

    $(".wizard-close").click(function () {
        LoadPartials();
        $("#welcome").removeClass("active");
    });

});



//-------------------------------------------------------------

searchVisible = 0;
transparent = true;

$(document).ready(function () {
    /*  Activate the tooltips      */
    $('[rel="tooltip"]').tooltip();


    $('#wizard').bootstrapWizard({
        'tabClass': 'nav nav-pills',
        'nextSelector': '.btn-next',
        'previousSelector': '.btn-previous',
        onInit: function (tab, navigation, index) {

            //check number of tabs and fill the entire row
            var $total = navigation.find('li').length;
            $width = 100 / $total;

            $display_width = $(document).width();

            if ($display_width < 400 && $total > 3) {
                $width = 50;
            }
            navigation.find('li').css('width', $width + '%');
        },
        onTabClick: function (tab, navigation, index) {
            // Disable the posibility to click on tabs
            return false;
        },
        onTabShow: function (tab, navigation, index) {
            var $total = navigation.find('li').length;
            var $current = index + 1;

            var wizard = navigation.closest('.wizard-card');

            // If it's the last tab then hide the last button and show the finish instead
            if ($current >= $total) {
                $(wizard).find('.btn-next').hide();
                $(wizard).find('.btn-finish').show();
            } else {
                $(wizard).find('.btn-next').show();
                $(wizard).find('.btn-finish').hide();
            }
        }
    });
    

    $('[data-toggle="wizard-radio"]').click(function (event) {
        wizard = $(this).closest('.wizard-card');
        wizard.find('[data-toggle="wizard-radio"]').removeClass('active');
        $(this).addClass('active');
        $(wizard).find('[type="radio"]').removeAttr('checked');
        $(this).find('[type="radio"]').attr('checked', 'true');
    });

    $height = $(document).height();
    $('.set-full-height').css('height', $height);




});












