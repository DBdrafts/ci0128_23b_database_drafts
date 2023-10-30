/// <summary>
/// Call the function that add the producto to the lsit
/// </summary>
function callAddProductToList(addProductButton) {
    // Gets the data associated with the button
    var productData = addProductButton.getAttribute('data-product-data');

    $.ajax({
        type: "POST",
        url: "/ProductPage/1?handler=AddToProductList",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { productData: productData },
        success: function (data) {
            console.log('Product added successfully' + data);
            showFeedbackMessage('El producto ha sido agregado a su lista!', 'feedbackMessage');
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al agregar el producto a la lista!', 'feedbackMessage');
        }
    });
}