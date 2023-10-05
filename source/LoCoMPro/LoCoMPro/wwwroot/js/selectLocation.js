// Wait for the DOM to be fully loaded before executing the code

var js = 
document.addEventListener("DOMContentLoaded", function () {
    // Get references to HTML elements by their IDs
    const locationButton = document.getElementById("locationButton"); // Button to open the popup
    const locationPopup = document.getElementById("locationPopup"); // The popup itself
    const provinceSelect = document.getElementById("province"); // Province select
    const cantonSelect = document.getElementById("canton"); // Canton select
    const saveLocationButton = document.getElementById("saveLocation-button"); // Button to save location
    const addProductForm = document.getElementById("addProductForm"); // Form
    const closePopupButton = document.getElementById("closePopup-button"); // Button to close the popup
    var span = document.getElementById("buttonSpan");

    // Add a click event to the button to open the popup
    locationButton.addEventListener("click", function () {
        locationPopup.style.display = "block"; // Display the popup when clicking the button
    });

    // Add a click event to the button to close the popup
    closePopupButton.addEventListener("click", function () {
        locationPopup.style.display = "none"; // Hide the popup when clicking the close button
    });

    // Ask for the Province list
    $.ajax({
        url: "/AddProductPage?handler=Provinces",
        success: function (provinceList) {
            // Populate the select options
            provinceList.forEach(p => {
                const option = document.createElement("option");
                option.value = p.value;
                option.text = p.text;
                provinceSelect.appendChild(option);
            })
        }
    });

    // Add a change event to the province select
    provinceSelect.addEventListener("change", function () {
        const selectedProvince = provinceSelect.value;
        if (selectedProvince) {
            span.textContent = selectedProvince;
            // Make a fetch request to get the cantons of the selected province
            fetch(`/AddProductPage?handler=Cantones&provincia=${selectedProvince}`)
                .then(response => response.json()) // Convert the response to JSON
                .then(data => {
                    cantonSelect.innerHTML = ""; // Clear the canton select
                    data.forEach(canton => {
                        // Create options for each canton and add them to the select
                        const option = document.createElement("option");
                        option.value = canton.value;
                        option.text = canton.text;
                        cantonSelect.appendChild(option);
                    });
                    cantonSelect.removeAttribute("disabled"); // Enable the canton select
                    
                })
                .catch(error => {
                    console.error("Error getting cantons:", error); // Error handling
                });
        } else {
            cantonSelect.setAttribute("disabled", "disabled"); // Disable the canton select if no province is selected
            cantonSelect.innerHTML = '<option value="" disabled selected hidden>Select a canton</option>'; // Restore the default value
        }
    });

    cantonSelect.addEventListener("change", function () {
        const selectedCanton = cantonSelect.value;
        if (selectedCanton) {
            var text = span.textContent;
            text = text + ", " + selectedCanton;
            span.textContent = text;
        }
    });

    // Add a submit event to the form
    addProductForm.addEventListener("submit", function (event) {
        const selectedProvince = provinceSelect.value;
        const selectedCanton = cantonSelect.value;

        if (!selectedProvince || selectedProvince === "" || !selectedCanton || selectedCanton === "") {
            event.preventDefault(); // Prevent the form from being submitted if information is missing
            alert("Debe seleccionar una ubicaci\u00F3n antes de enviar el formulario."); // Display an alert to the user
        }
    });

    // Add a click event to the button to save the location
    saveLocationButton.addEventListener("click", function () {
        const selectedProvince = provinceSelect.value;
        const selectedCanton = cantonSelect.value;

        if (selectedProvince && selectedProvince !== "" && selectedCanton && selectedCanton !== "") {
            // Update the location information in the "locationInfo" element
            const locationInfo = document.getElementById("locationInfo");
            locationInfo.textContent = `Ubicaci\u00F3n elegida: ${selectedProvince}, ${selectedCanton}`;
            locationInfo.style.display = "block"; // Display the location information
            document.getElementById("selectedProvince").value = selectedProvince; // Update hidden values in the form
            document.getElementById("selectedCanton").value = selectedCanton;
            locationPopup.style.display = "none"; // Hide the popup after saving the location
        }
    });
});
