var jq = jQuery.noConflict()
jq(document).ready(function () {
    jq("#location").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/AddProductPage?handler=AutocompleteSuggestions", // Use the correct URL for your page
                data: { field: "storeName", term: request.term },
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 2 // Minimum characters to trigger autocomplete
    });
    jq("#productName").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/AddProductPage?handler=AutocompleteSuggestions", // Use the correct URL for your page
                data: { field: "productName", term: request.term },
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 2 // Minimum characters to trigger autocomplete
    });
    jq("brand").autocomplete({
        source: availableTags
    });
    jq("#model").autocomplete({
        source: availableTags
    });
});
