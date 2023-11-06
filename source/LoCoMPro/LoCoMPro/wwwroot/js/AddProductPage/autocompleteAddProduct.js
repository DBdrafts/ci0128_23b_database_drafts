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
        var province = selectedProvince;
        var canton = selectedCanton;
        var store = $("#store").val();
        if (fields.includes(currentField)) {
            jq(currentField).autocomplete({
                // Ask the database for information to fill the autocomplete
                source: function (request, response) {
                    $.ajax({
                        url: "/AddProductPage?handler=AutocompleteSuggestions",
                        data: { field: currentField, term: request.term, provinceName: province, cantonName: canton, storeName: store },
                        success: function (data) {
                            response(data);
                        }
                    });
                },
                // Selection of a suggestion
                select: function (event, ui) {
                    // Handle selection of a suggestion
                    var selectedValue = ui.item.value;
                    $(currentField).val(selectedValue);
                    if (currentField == "#productName") {
                        //$("#model").val("alguno");
                        var product = selectedValue;
                        $.ajax({
                            url: "/AddProductPage?handler=ProductAutofillData",
                            data: { productName: product },
                            success: function (data) {
                                for (const field in data) {
                                    if (data.hasOwnProperty(field)) {
                                        $(field).val(data[field]);
                                    }
                                }
                            },
                            error: function (error) {
                                console.error('Error: ', error);
                            }
                        });
                    }
                },
                // Minimum characters to trigger autocomplete
                minLength: 2
            });
        }
    });
});