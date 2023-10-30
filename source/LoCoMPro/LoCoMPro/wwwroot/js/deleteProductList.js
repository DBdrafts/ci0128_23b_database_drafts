function hideProductItem(index) {
    var productItem = document.getElementById("product-item-" + index);
    if (productItem) {
        productItem.style.display = "none";
    }
}