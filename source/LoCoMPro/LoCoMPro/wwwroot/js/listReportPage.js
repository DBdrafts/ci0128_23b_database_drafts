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


//document.addEventListener("DOMContentLoaded", function () {
//    var reportItems = document.querySelectorAll('.report-list-block');

//    reportItems.forEach(function (item, index) {
//        var arrowIcons = item.querySelector('.block-arrow');
//        var productList = item.querySelector('.report-product-list');

//        // Establecer el estado inicial de las flechas
//        arrowIcons.querySelector('.fa-angle-up').hidden = true;
//        arrowIcons.querySelector('.fa-angle-down').hidden = false;

//        item.addEventListener('click', function () {
//            // Cerrar todos los demás elementos y reiniciar los íconos
//            reportItems.forEach(function (otherItem) {
//                if (otherItem !== item) {
//                    var otherProductList = otherItem.querySelector('.report-product-list');
//                    otherProductList.hidden = true;

//                    var otherArrowIcons = otherItem.querySelector('.block-arrow');
//                    otherArrowIcons.querySelector('.fa-angle-up').hidden = true;
//                    otherArrowIcons.querySelector('.fa-angle-down').hidden = false;
//                }
//            });

//            // Alternar la visibilidad del elemento actual
//            productList.hidden = !productList.hidden;

//            // Alternar la visibilidad de los íconos de flecha
//            arrowIcons.querySelector('.fa-angle-up').hidden = !productList.hidden;
//            arrowIcons.querySelector('.fa-angle-down').hidden = productList.hidden;
//        });
//    });
//});
