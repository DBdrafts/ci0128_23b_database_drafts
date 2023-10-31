/// <summary>
/// Controls the remove of the element from the list
/// </summary>
function removeProductFromList(removeProductButton, index) {
    // Gets the data associated with the button
    var productData = removeProductButton.getAttribute('data-product-data');

    // Gets the product data
    var [productName, productBrand, productModl, storeName, province,
        canton, avgPrice] = productData.split(String.fromCharCode(31));

    // Hide the product selected
    hideProductItem(index);

    // Call the function that delete the product from the list
    callRemoveProductFromList(productData);

    // Update the data of the page to be actualized
    updateListData(avgPrice);
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
            console.log('Product removed successfully' + data);
            showFeedbackMessage('El producto ha sido eliminado de su lista!', 'feedbackMessage');
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al eliminar el producto a la lista!', 'feedbackMessage');
        }
    });
}

/// <summary>
/// Update the visual information of the list
/// </summary>
function updateListData(avgPriceString) {
    // Gets the amount of products in the actual list
    var productCount = parseInt(document.getElementById("product-count").textContent);

    // Gets the total aprox price of all the product
    var totalPriceText = document.getElementById("total-price").textContent;

    var totalPrice = parseInt(totalPriceText.replace(/[^\d]/g, ''), 10);
    totalPrice = isNaN(totalPrice) ? 0 : totalPrice;

    avgPrice = parseInt(avgPriceString.replace(/[^\d]/g, ''), 10);
    avgPrice = isNaN(avgPrice) ? 0 : avgPrice;

    // Decrease the product count and the total price
    --productCount;
    totalPrice -= avgPrice;

    // Actualize the visual data
    document.getElementById("product-count").textContent = productCount;
    document.getElementById("total-price").textContent = totalPrice.toLocaleString();

    // If it was the last product, the show the empty version
    if (productCount == 0) {
        document.getElementById("empty-list").style.display = 'inline-block';
        document.getElementById("no-empty-list").style.display = 'none';
    }
}