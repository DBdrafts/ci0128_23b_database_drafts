SelectLocation:
document.addEventListener("DOMContentLoaded", function () {
    const locationButton = document.getElementById("locationButton");
    const locationPopup = document.getElementById("locationPopup");
    const provinceSelect = document.getElementById("province");
    const cantonSelect = document.getElementById("canton");
    const saveLocationButton = document.getElementById("saveLocation-button");
    const addProductForm = document.getElementById("addProductForm");

    locationButton.addEventListener("click", function () {
        locationPopup.style.display = "block";
    });

    provinceSelect.addEventListener("change", function () {
        const selectedProvince = provinceSelect.value;
        if (selectedProvince) {
            fetch(`/AddProductPage?handler=Cantones&provincia=${selectedProvince}`)
                .then(response => response.json())
                .then(data => {
                    cantonSelect.innerHTML = "";
                    data.forEach(canton => {
                        const option = document.createElement("option");
                        option.value = canton.value;
                        option.text = canton.text;
                        cantonSelect.appendChild(option);
                    });
                    cantonSelect.removeAttribute("disabled");
                })
                .catch(error => {
                    console.error("Error getting cantones:", error);
                });
        } else {
            cantonSelect.setAttribute("disabled", "disabled");
            cantonSelect.innerHTML = '<option value="" disabled selected hidden>Seleccionar un cantón</option>';
        }
    });

    addProductForm.addEventListener("submit", function (event) {
        const selectedProvince = provinceSelect.value;
        const selectedCanton = cantonSelect.value;

        if (!selectedProvince || selectedProvince === "" || !selectedCanton || selectedCanton === "") {
            event.preventDefault();
            alert("Debe seleccionar una ubicaci\u00F3n antes de enviar el formulario.");
        }
    });

    saveLocationButton.addEventListener("click", function () {
        const selectedProvince = provinceSelect.value;
        const selectedCanton = cantonSelect.value;

        if (selectedProvince && selectedProvince !== "" && selectedCanton && selectedCanton !== "") {
            const locationInfo = document.getElementById("locationInfo");
            locationInfo.textContent = `Provincia: ${selectedProvince}, Cant\u00F3n: ${selectedCanton}`;
            locationInfo.style.display = "block";
            document.getElementById("selectedProvince").value = selectedProvince;
            document.getElementById("selectedCanton").value = selectedCanton;
            locationPopup.style.display = "none";
        }
    });
});