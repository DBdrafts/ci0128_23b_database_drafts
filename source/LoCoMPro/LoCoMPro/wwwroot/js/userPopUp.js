document.addEventListener("DOMContentLoaded", function () {
    const userButton = document.getElementById("usr-options");
    const popup = document.getElementById("popup");

    // Agregar un manejador de clic al botón de Usuario para mostrar el pop-up
    userButton.addEventListener("click", function (event) {
        event.stopPropagation(); // Evita que el clic llegue al documento
        popup.style.display = "block";
    });

    // Agregar un manejador de clic al documento para cerrar el pop-up
    document.addEventListener("click", function (event) {
        if (event.target !== userButton && event.target !== popup) {
            // Si el clic se produce fuera del botón de Usuario y el pop-up
            popup.style.display = "none";
        }
    });

    // Evitar que los clics dentro del pop-up cierren el pop-up
    popup.addEventListener("click", function (event) {
        event.stopPropagation(); // Evita que el clic llegue al documento
    });
});