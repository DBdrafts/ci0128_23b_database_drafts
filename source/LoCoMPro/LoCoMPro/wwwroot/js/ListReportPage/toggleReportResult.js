document.addEventListener("DOMContentLoaded", function () {
    // Gets all the report blocks
    var reportItems = document.querySelectorAll('.report-list-block');

    reportItems.forEach(function (item, index) {
        var arrowIcons = item.querySelector('.block-arrow');

        // When the block is clicked
        item.addEventListener('click', function () {
            // Gets the list of blocks
            var productList = item.querySelector('.report-product-list');

            // Close all the other product list blocks
            reportItems.forEach(function (otherItem) {
                if (otherItem !== item) {
                    var otherProductList = otherItem.querySelector('.report-product-list');
                    otherProductList.hidden = true;

                    var otherArrowIcons = otherItem.querySelector('.block-arrow');
                    otherArrowIcons.querySelector('.fa-angle-up').hidden = true;
                    otherArrowIcons.querySelector('.fa-angle-down').hidden = false;
                }
            });

            // Open the product list
            productList.hidden = !productList.hidden;

            arrowIcons.querySelector('.fa-angle-up').hidden = productList.hidden;
            arrowIcons.querySelector('.fa-angle-down').hidden = !productList.hidden;
        });
    });
});
