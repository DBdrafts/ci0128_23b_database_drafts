var jq = jQuery.noConflict()
jq(document).ready(function () {
    var fields = [
        "#store",
        "#productName",
        "#price",
        "#brand",
        "#model"
    ];
    $(".required-field").on("input", function () {
        var $this = $(this); // Capture the current field as a jQuery object
        var currentField = "#" + $this.attr("id");
        var province = document.getElementById("selectedProvince").value;
        var canton = document.getElementById("selectedCanton").value;
        var store = document.getElementById("store").value;
        if (fields.includes(currentField)) {
            jq(currentField).autocomplete({
                // Ask the database for information to fill the autocomplete
                source: function (request, response) {
                    $.ajax({
                        url: "/AddProductPage?handler=AutocompleteSuggestions",
                        data: { field: currentField, term: request.term, provinceName: province , cantonName: canton, storeName: store},
                        success: function (data) {
                            response(data);
                        }
                    });
                },
                // Minimum characters to trigger autocomplete
                minLength: 2
            });
        }
    });
});

