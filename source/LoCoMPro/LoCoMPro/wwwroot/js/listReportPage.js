
// TODO(Julio): Discutir esto de aquí
/*document.addEventListener("DOMContentLoaded", function () {
    var reportItems = document.querySelectorAll('.report-list-block');

    reportItems.forEach(function (item, index) {
        item.addEventListener('click', function () {
            var productList = item.querySelector('.report-product-list');
            productList.hidden = !productList.hidden;
        });
    });
});*/


document.addEventListener("DOMContentLoaded", function () {
    // Gets all the report blocks
    var reportItems = document.querySelectorAll('.report-list-block');

    reportItems.forEach(function (item, index) {
        // When the block is clicked
        item.addEventListener('click', function () {
            // Gets the list of blocks
            var productList = item.querySelector('.report-product-list');

            // Close all the other product list blocks
            reportItems.forEach(function (otherItem) {
                if (otherItem !== item) {
                    var otherProductList = otherItem.querySelector('.report-product-list');
                    otherProductList.hidden = true;
                }
            });

            // Open the product list
            productList.hidden = !productList.hidden;
        });
    });
});