document.addEventListener("DOMContentLoaded", function () {
    const locationButton = document.getElementById("locationButton");
    const locationPopup = document.getElementById("locationPopup");
    const provinceSelect = document.getElementById("province");
    const cantonSelect = document.getElementById("canton");
    const saveLocationButton = document.getElementById("saveLocation");

    locationButton.addEventListener("click", function () {
        locationPopup.style.display = "block";
    });

    provinceSelect.addEventListener("change", function () {
        const selectedProvince = provinceSelect.value;
        if (selectedProvince) {
            cantonSelect.removeAttribute("disabled");

            // Verificamos si se seleccion� la "Provincia 2" y habilitamos las opciones correspondientes.
            if (selectedProvince === "provincia2") {
                const cantonOptions = cantonSelect.getElementsByTagName("option");
                for (let i = 0; i < cantonOptions.length; i++) {
                    const option = cantonOptions[i];
                    if (option.value === "canton3" || option.value === "canton4") {
                        option.removeAttribute("disabled");
                    } else {
                        option.setAttribute("disabled", "disabled");
                    }
                }
            } else {
                // Si se selecciona otra provincia, deshabilitamos todas las opciones de cant�n.
                const cantonOptions = cantonSelect.getElementsByTagName("option");
                for (let i = 0; i < cantonOptions.length; i++) {
                    cantonOptions[i].setAttribute("disabled", "disabled");
                }
            }
        } else {
            cantonSelect.setAttribute("disabled", "disabled");
        }
    });


    saveLocationButton.addEventListener("click", function () {
        const selectedProvince = provinceSelect.value;
        const selectedCanton = cantonSelect.value;

        if (selectedProvince && selectedCanton) {
            const cantonText = decodeURIComponent(selectedCanton);
            const provinceText = decodeURIComponent(selectedProvince);

            const locationText = `Ubicacion seleccionada: ${cantonText}, ${provinceText}`;
            const chosenLocation = document.getElementById("chosenLocation");
            chosenLocation.style.display = "block";
            chosenLocation.value = locationText;
        } else {
            // Si no se ha seleccionado una ubicaci�n v�lida, ocultamos la caja de texto.
            document.getElementById("chosenLocation").style.display = "none";
        }

        locationPopup.style.display = "none";
    })
;


});

