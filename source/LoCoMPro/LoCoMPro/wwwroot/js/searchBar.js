document.addEventListener("DOMContentLoaded", function () {
    // Get location button and location popup
    const span = document.getElementById("buttonSpan");
    // Get current URL
    const currentPageUrl = window.location.href;
    // Create a URLSearchParams object from the current URL
    const urlParams = new URLSearchParams(window.location.search);
    const searchType = urlParams.get('searchType');
    const searchString = urlParams.get('searchString');
    const selectedProvince = urlParams.get('province');
    const selectedCanton = urlParams.get('canton');

    // Get the value of a specific parameter (e.g., 'param1')
    // Get the current page's URL
    if (currentPageUrl.includes("/SearchPage")) {
        $("#type-search-selector").val(searchType);
        $("#search-input").val(searchString);
        if (selectedProvince && selectedProvince !== "") {
            var text = selectedProvince;
            if (selectedCanton && selectedCanton !== "") {
                text = text + ", " + selectedCanton;
            }
            span.textContent = text;
        }
    }
});