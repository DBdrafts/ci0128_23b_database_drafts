function stringContainsSubstringIgnoreCase(inputString, substringsArray) {
    // Convert the input string to lowercase for case-insensitive comparison
    const lowerCaseInput = inputString.toLowerCase();

    // Check if any substring in the array is found in the lowercase input string
    return substringsArray.some(substring => lowerCaseInput.includes(substring.toLowerCase()));
}
$(document).ready(function () {
    // Function to update the URL and trigger a GET request
    const resultSection = document.getElementById("search-results");

    function clearFilters() {
        // Uncheck all boxes in the filter section
        $("#filter-section input[type=checkbox]").prop("checked", false);

        filterResults();
    }

    // Event listener for the botton of clean
    $("#clear-filters").click(clearFilters);

    function filterResults() {
        const selectedCategories = getSelectedCheckboxes('SelectedCategories');
        const selectedProvinces = getSelectedCheckboxes('SelectedProvinces');
        const selectedCantons = getSelectedCheckboxes('SelectedCantons');
        notfound = document.getElementById("product-not-found");
        filterSection = document.getElementById("filter-section");

        // Filter the resultBlocks based on the selected criteria
        resultBlocks = queryResults.filter(block => {
            const productName = block.querySelector('#register-product-name').getAttribute("value");
            const province = block.querySelector('#register-province-name').getAttribute("value");
            const canton = block.querySelector('#register-canton-name').getAttribute("value");
            const categories = block.querySelector('#register-categories').getAttribute("value");

            const meetsCategoryFilter = selectedCategories.length === 0 || stringContainsSubstringIgnoreCase(categories, selectedCategories);
            const meetsProvinceFilter = selectedProvinces.length === 0 || selectedProvinces.includes(province);
            const meetsCantonFilter = selectedCantons.length === 0 || selectedCantons.includes(canton);

            return meetsCategoryFilter && meetsProvinceFilter && meetsCantonFilter;
        });

        if (resultBlocks.length <= 0) {
            notfound.hidden = false;
            resultSection.hidden = true;
            filterSection.hidden = true;
            document.getElementById("results-count").textContent = "0";
        } else {

            // Sort the filtered resultBlocks
            sortResultBlocks("desc");

            // Update the totalPages based on the filtered resultBlocks
            totalPages = Math.ceil(resultBlocks.length / pageSize);

            if (resultBlocks.length !== queryResults.length) {
                queryResults.forEach((block) => {
                    block.style.display = 'none';
                });
            }
            // Show the first page
            currentPage = 1;
            showPage(currentPage);

            // Update navigation buttons and page buttons
            updateNavigationButtons();

            notfound.hidden = true;
            resultSection.hidden = false;
            filterSection.hidden = false;
        }
    }

    // Helper function to get an array of values from checked checkboxes
    function getSelectedCheckboxes(checkboxName) {
        return Array.from(document.querySelectorAll(`input[name="${checkboxName}"]:checked`)).map(checkbox => checkbox.value);
    }

    // Event handlers for when checkboxes are selected or deselected
    document.querySelectorAll('input[name="SelectedCategories"]').forEach(checkbox => {
        checkbox.addEventListener('change', filterResults);
    });

    document.querySelectorAll('input[name="SelectedProvinces"]').forEach(checkbox => {
        checkbox.addEventListener('change', filterResults);
    });

    document.querySelectorAll('input[name="SelectedCantons"]').forEach(checkbox => {
        checkbox.addEventListener('change', filterResults);
    });

    // Initialize the filter on page load
    filterResults();
});

