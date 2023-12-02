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

            // Do something with the selected products and the entered product name
            // For example, send the data to a page handler using AJAX

            // TODO: Add your AJAX request or any other logic here to send data to the server
            $.ajax({
                type: 'POST',
                url: '/ModerateSimilarNamePage?handler=ChangeSelectedProductsName', // Specify the handler
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: { productName: productName, groupProductNames: selectedProductsForReport },
                success: function (data) {
                    console.log('Report saved successfully' + data);
                },
                error: function (error) {
                    console.error('Error saving report: ' + error);
                    //showFeedbackMessage('Error al realizar el reporte!', 'feedbackMessage');

                }
            });

            // Reset input value and selected products array for the report block
            input.value = '';
            selectedProducts[reportIndex] = {};

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
});
