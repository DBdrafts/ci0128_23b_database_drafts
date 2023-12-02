var jq = jQuery.noConflict()
jq(document).ready(function () {
    $(".product-name-bar").on("input", function () {
        var $this = $(this); // Capture the current field as a jQuery object
        var currentField = "#" + $this.attr("id");
        jq(currentField).autocomplete({
            // Ask the database for information to fill the autocomplete
            source: function (request, response) {
                $.ajax({
                    url: "/AddProductPage?handler=AutocompleteSuggestions",
                    data: { field: "#productName", term: request.term, provinceName: "", cantonName: ""},
                    success: function (data) {
                        response(data);
                    },
                    error: function (error) {
                        console.error('Errro getting autocomplete results: ' + error);
                    }
                });
            },
            // Minimum characters to trigger autocomplete
            minLength: 2
        });
    });
});