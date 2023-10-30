/// <summary>
/// Controls the remove of the element from the list
/// </summary>
function removeProductFromList(removeProductButton, index) {
    // Gets the data associated with the button
    var productData = removeProductButton.getAttribute('data-product-data');

    // Hide the product selected
    hideProductItem(index);

    // Call the function that delete the product from the list
    callRemoveProductFromList(productData);

}

/// <summary>
/// Hides the entry of the visual list that have this index
/// </summary>
function hideProductItem(index) {
    // Hides the entry of the visual list that have this index
    var productItem = document.getElementById("product-item-" + index);
    if (productItem) {
        productItem.style.display = "none";
    }
}

/// <summary>
/// Call the function that remove the product to the list
/// </summary>
function callRemoveProductFromList(productData) {
    $.ajax({
        type: "POST",
        url: "/ProductListPage?handler=RemoveFromProductList",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { productData: productData },
        success: function (data) {
            console.log('Product added successfully' + data);
            showFeedbackMessage('El producto ha sido eliminado de su lista!', 'feedbackMessage');
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al eliminar el producto a la lista!', 'feedbackMessage');
        }
    });
}
