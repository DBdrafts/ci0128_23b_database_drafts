document.addEventListener("DOMContentLoaded", function () {
    // Get location button and location popup
    const popupProvince = document.getElementById("province");
    const popupCanton = document.getElementById("canton");
    const selectButtonProvince = document.getElementById("chosenProvince");
    const selectButtonCanton = document.getElementById("chosenCanton");
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
        document.getElementById("type-search-selector").value = searchType;
        document.getElementById("search-input").value = searchString;
        if (selectedProvince && selectedProvince !== "") {
            span.textContent = selectedProvince;
            popupProvince.textContent = selectedProvince;
            popupProvince.value = selectedProvince;
            selectButtonProvince.textContent = selectedProvince;
            var text = span.textContent;
            if (selectedCanton && selectedCanton !== "") {
                span.textContent = span.textContent + ", " + selectedCanton;
                locationPopup.lastChild.value = selectedCanton;
                popupCanton.textContent = selectedCanton;
                popupCanton.value = selectedCanton;
                selectButtonCanton.textContent = selectedCanton;
            }
        }
    }
});