$(document).ready(function () {
    // Manejar cambios en los checkboxes de categorías
    $(".category-checkbox").change(function () {
        // Recopilar todas las categorías seleccionadas
        var selectedCategories = [];
        $(".category-checkbox:checked").each(function () {
            selectedCategories.push($(this).data("category-name"));
        });

        // Realizar una solicitud AJAX al servidor para obtener los resultados filtrados
        $.ajax({
            url: "/SearchPage", 
            type: "GET",
            data: { selectedCategories: selectedCategories },
            success: function (data) {
                // Actualizar el contenido de la sección de resultados con los nuevos resultados
                $(".search-section").html(data);
            },
            error: function (error) {
                console.error(error);
            }
        });
    });
});
