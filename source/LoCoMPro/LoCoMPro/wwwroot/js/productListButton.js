


function callAddProductToList(addProductButton) {
    productData = addProductButton.getAttribute('data-product-data');

    $.ajax({
        type: "POST",
        url: "/ProductPage/1?handler=AddToProductList",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { productData: productData },
        success: function (data) {
            console.log('Report saved successfully' + data);
            showFeedbackMessage('Su reporte se ha realizado correctamente!');
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al realizar el reporte!');

        }
    });
}