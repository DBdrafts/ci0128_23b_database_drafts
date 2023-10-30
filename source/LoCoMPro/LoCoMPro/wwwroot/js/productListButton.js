/// <summary>
/// Call the function that add the product to the list
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
            toggleButtonToRemove()
            showFeedbackMessage('El producto ha sido agregado a su lista!', 'feedbackMessage');
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al agregar el producto a la lista!', 'feedbackMessage');
        }
    });
}

/// <summary>
/// Call the function that remove the product to the list
/// </summary>
function callRemoveProductFromList(removeProductButton) {
    // Gets the data associated with the button
    var productData = removeProductButton.getAttribute('data-product-data');

    $.ajax({
        type: "POST",
        url: "/ProductPage/1?handler=RemoveFromProductList",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { productData: productData },
        success: function (data) {
            console.log('Product added successfully' + data);
            showFeedbackMessage('El producto ha sido eliminado de su lista!');
            toggleButtonToAdd()
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al eliminar el producto a la lista!');

        }
    });

}

/// <summary>
/// Toggle the button to the add product button
/// </summary>
function toggleButtonToAdd() {
    document.getElementById("add-to-list").style.display = "inline-block";
    document.getElementById("remove-from-list").style.display = "none";
}

/// <summary>
/// Toggle the button to the remove product button
/// </summary>
function toggleButtonToRemove() {
    document.getElementById("add-to-list").style.display = "none";
    document.getElementById("remove-from-list").style.display = "inline-block";
}