//-------------------------------------------
//****** SLIDE FUNCTIONS /STAR RATING *******
//-------------------------------------------

$(function () {


    $("body").on("click", "[data-goto]", function () {
        var goto = $(this).attr("data-goto");
        $(this).closest(".slide").hide();
        $("#" + goto).show();
    });

    //---------------------------------------------------------

    $("body").on("mouseover", ".star-rating", function () {
        var cur = $(this).attr("data-rating");
        $(this).closest(".star-wrap").find(".star-rating").each(function (i) {
            var rating = $(this).attr("data-rating");

            if (rating <= cur) {
                $(this).addClass("glyphicon-star");
                $(this).removeClass("glyphicon-star-empty");
            } else {
                $(this).addClass("glyphicon-star-empty");
                $(this).removeClass("glyphicon-star");
            }

        });
    });

    $("body").on("mouseleave", ".star-wrap", function () {
        $(this).find(".star-rating").addClass("glyphicon-star-empty");
        $(this).find(".star-rating").removeClass("glyphicon-star");
    });

    $("body").on("click", ".star-rating", function () {

        var url = $(this).closest(".star-wrap").attr("data-posturl");
        var rating = $(this).attr("data-rating");

        $.post(url, { rating: rating }, function (xhr) {

        });
        
    });


});