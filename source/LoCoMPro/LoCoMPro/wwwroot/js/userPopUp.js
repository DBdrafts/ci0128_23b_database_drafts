document.addEventListener("DOMContentLoaded", function () {
    const notLoggedButton = document.getElementById("usr-not-logged");
    const popupNotLogged = document.getElementById("popup-not-logged");

    const LoggedButton = document.getElementById("usr-logged");
    const popupLogged = document.getElementById("popup-logged");

    if (notLoggedButton) {
        // Add listener to the button
        notLoggedButton.addEventListener("click", function (event) {
            event.stopPropagation();
            popupNotLogged.style.display = "block";
        });

        // Closes the popup when clicking outside of it
        document.addEventListener("click", function (event) {
            if (event.target !== notLoggedButton && event.target !== popupNotLogged) {
                popupNotLogged.style.display = "none";
            }
        });

        // Prevents the pop up from closing when clicking inside of it
        popupNotLogged.addEventListener("click", function (event) {
            event.stopPropagation();
        });

    }

    if (LoggedButton) {
        // Add listener to the button
        LoggedButton.addEventListener("click", function (event) {
            event.stopPropagation();
            popupLogged.style.display = "block";
        });

        // Closes the popup when clicking outside of it
        document.addEventListener("click", function (event) {
            if (event.target !== LoggedButton && event.target !== popupLogged) {
                popupLogged.style.display = "none";
            }
        });

        // Prevents the pop up from closing when clicking inside of it
        popupLogged.addEventListener("click", function (event) {
            event.stopPropagation();
        });
    }
});