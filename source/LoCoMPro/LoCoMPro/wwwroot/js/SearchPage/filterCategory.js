// .ready means that the function is executed when the document is fully loaded and ready to be manipulated
$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const searchType = urlParams.get('searchType');
    const searchString = urlParams.get('searchString');
    const province = urlParams.get('province');
    const canton = urlParams.get('canton');
    const sortOrder = urlParams.get('sortOrder');

    // Function to update the URL and trigger a GET request
    function updateURLAndFetch() {
        // Get an array of values from checked checkboxes
        var selectedCategories = $('input[name="SelectedCategories"]:checked').map(function () {
            return $(this).val();
        }).get();

        var selectedProvinces = $('input[name="SelectedProvinces"]:checked').map(function () {
            return $(this).val();
        }).get();

        var selectedCantons = $('input[name="SelectedCantons"]:checked').map(function () {
            return $(this).val();
        }).get();

        // Build the URL with updated values

        var url = `/SearchPage/1?searchType=${searchType}&searchString=${searchString}${sortOrder? ("&sortOrder=" + sortOrder) : ""}`;
        if (province && province !== "") url += `&province=${province}`;
        if (canton && canton !== "") url += `&canton=${canton}` 
        if (selectedCategories && selectedCategories.length > 0) url += `&SelectedCategories=${selectedCategories.join(',')}`;
        if (selectedProvinces && selectedProvinces.length > 0) url += `&SelectedProvinces=${selectedProvinces.join(',')}`;
        if (selectedCantons && selectedCantons.length > 0) url += `&SelectedCantons=${selectedCantons.join(',')}`;

        // Redirect the browser to the constructed URL
        window.location.href = url;
    }

    // Event handlers for when checkboxes are selected or deselected
    $('input[name="SelectedCategories"]').change(updateURLAndFetch);
    $('input[name="SelectedProvinces"]').change(updateURLAndFetch);
    $('input[name="SelectedCantons"]').change(updateURLAndFetch)
});
