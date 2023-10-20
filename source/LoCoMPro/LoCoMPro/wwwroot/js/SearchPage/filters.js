// .ready means that the function is executed when the document is fully loaded and ready to be manipulated
document.addEventListener("DOMContentLoaded", function () {
    const resultSection = document.getElementById("search-results");
    // Function to update the URL and trigger a GET request
    function filterResults() {
        const selectedCategories = getSelectedCheckboxes('SelectedCategories');
        const selectedProvinces = getSelectedCheckboxes('SelectedProvinces');
        const selectedCantons = getSelectedCheckboxes('SelectedCantons');
        const notfound = document.getElementById("product-not-found-result");

        // Filter the resultBlocks based on the selected criteria
        resultBlocks = queryResults.filter(block => {
            const productType = block.querySelector('#result-product-name').getAttribute("value");
            const province = block.querySelector('#result-province-name').getAttribute("value");
            const canton = block.querySelector('#result-canton-name').getAttribute("value");

            const meetsCategoryFilter = selectedCategories.length === 0 || selectedCategories.includes(productType);
            const meetsProvinceFilter = selectedProvinces.length === 0 || selectedProvinces.includes(province);
            const meetsCantonFilter = selectedCantons.length === 0 || selectedCantons.includes(canton);

            return meetsCategoryFilter && meetsProvinceFilter && meetsCantonFilter;
        });

        if (resultBlocks.length <= 0) {
            notfound.hidden = false;
            resultSection.hidden = true;
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
