document.addEventListener('DOMContentLoaded', function () {
    // Function to handle checkbox selection
    function handleCheckboxSelection(reportIndex, productIndex) {
        const checkbox = document.getElementById(`report-product-${reportIndex}-${productIndex}`).querySelector('input[type="checkbox"]');
        checkbox.addEventListener('change', function () {
            const productName = checkbox.value;
            // Update the selected products array for the corresponding report block
            selectedProducts[reportIndex][productName] = checkbox.checked;
        });
    }

    // Function to handle product name button click
    function handleProductNameButtonClick(reportIndex) {
        const button = document.getElementById(`product-name-button-${reportIndex}`);
        button.addEventListener('click', function () {
            const input = document.getElementById(`product-name-bar-${reportIndex}`);
            const productName = input.value.trim();
            if (productName == null || productName == "") return;   // TODO: Message warning for empty input box.
            // Get selected products for the current report block
            const selectedProductsForReport = selectedProducts[reportIndex];

            $.ajax({
                type: 'POST',
                url: '/ModerateSimilarNamePage?handler=ChangeSelectedProductsName', // Specify the handler
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: { productName: productName, groupProductNames: selectedProductsForReport },
                success: function (data) {
                    console.log('Product names changed successfully' + data);
                    showFeedbackMessage('Los productos se cambiaron correctamente!', 'feedbackMessage');
                },
                error: function (error) {
                    console.error('Error changing product names: ' + error);
                    showFeedbackMessage('Error al intentar cambiar el nombre de los productos!', 'feedbackMessage');

                }
            });

            // Reset input value and selected products array for the report block
            removeResultBlock(reportIndex);
        });
    }


    // Initialize an array to store selected products for each report block
    const selectedProducts = [];

    // Loop through each report block to initialize the selected products array and add event listeners
    const reportBlocks = document.querySelectorAll('.report-list-block');
    reportBlocks.forEach((reportBlock, reportIndex) => {
        selectedProducts[reportIndex] = {};
        const products = reportBlock.querySelectorAll('.list-products');
        products.forEach((product, productIndex) => {
            handleCheckboxSelection(reportIndex, productIndex);
        });

        handleProductNameButtonClick(reportIndex);
    });

    function removeResultBlock(reportIndex) {
        const id = `report-block-${reportIndex}`;
        document.getElementById(id).style.display = 'none';
        resultBlocks = resultBlocks.filter((element) => {
            return !element.id.includes(id);
        });
        updateNavigationButtons();
        showPage(currentPage);

        if (resultBlocks.length <= 0) {
            showNoResultsImage();
            var elementsToHide = document.getElementsByClassName("group-pagination");
            document.getElementById('results-count').style.display = 'none';
            //// Loop through the selected elements and hide them
            for (let i = 0; i < elementsToHide.length; i++) {
                elementsToHide[i].style.display = 'none';
            }
        }
    }
    showNoResultsImage();
});

function showNoResultsImage() {
    if (resultBlocks.length <= 0) {
        document.getElementById("empty-list").hidden = false;
    }
}
