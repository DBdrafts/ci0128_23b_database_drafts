// .ready means that the function is executed when the document is fully loaded and ready to be manipulated
$(document).ready(function () {

    // Event handler for when a checkbox for a category is selected or deselected
    $('input[type="checkbox"]').change(function () {
        // This function is triggered when the state of a checkbox changes

        // Get an array of values from checked checkboxes (selected categories)
        var selectedCategories = $('input[type="checkbox"]:checked').map(function () {
            return $(this).val();
        }).get();

        // Get values from the different HTML elements on the page
        var searchType = $('#searchType').val(); // Get the value of searchType
        var searchString = $('#searchString').val(); // Get the value of searchString
        var sortOrder = $('#sortOrder').val(); // Get the value of sortOrder

        // Build a URL based on selected categories, search type, search string, and sort order
        var url = "/SearchPage/1?SelectedCategories=" + selectedCategories.join(',') +
            "&searchType=" + searchType +
            "&searchString=" + searchString +
            "&sortOrder=" + sortOrder;

        // Redirect the browser to the constructed URL
        window.location.href = url;
    });
});
