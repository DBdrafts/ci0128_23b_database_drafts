$(document).ready(function () {
    // Selector para todas las casillas de verificación
    $('input[type="checkbox"]').change(function () {
        // Obtén todas las casillas de verificación seleccionadas
        var selectedCategories = $('input[type="checkbox"]:checked').map(function () {
            return $(this).val();
        }).get();

        // Obtén los valores de otros campos ocultos
        var searchType = $('#searchType').val();
        var searchString = $('#searchString').val();
        var sortOrder = $('#sortOrder').val();

        // Construye la URL con los parámetros
        var url = "/SearchPage/1?SelectedCategories=" + selectedCategories.join('&SelectedCategories=') +
                    "&searchType=" + searchType +
                    "&searchString=" + searchString +
                    "&sortOrder=" + sortOrder;

        // Redirige al usuario a la nueva URL
        window.location.href = url;
    });
});

