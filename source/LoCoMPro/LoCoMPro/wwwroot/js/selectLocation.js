// Wait for the DOM to be fully loaded before executing the code

// Define a flag to track if the population has already occurred
let hasPopulatedProvinceSelect = false;
document.addEventListener("DOMContentLoaded", function () {
    // Get references to HTML elements by their IDs
    const locationButton = document.getElementById("locationButton"); // Button to open the popup
    const locationInfo = document.getElementById("locationInfo");
    const locationPopup = document.getElementById("locationPopup"); // The popup itself
    const provinceSelect = document.getElementById("province"); // Province select
    const cantonSelect = document.getElementById("canton"); // Canton select
    const saveLocationButton = document.getElementById("saveLocation-button"); // Button to save location
    const addProductForm = document.getElementById("addProductForm"); // Form
    const closePopupButton = document.getElementById("closePopup-button"); // Button to close the popup
    var span = document.getElementById("buttonSpan");
    const currentPageUrl = window.location.href;
    // Add a click event to the button to open the popup
    locationButton.addEventListener("click", function () {
        locationPopup.style.display = "block"; // Display the popup when clicking the button
    });

    // Add a click event to the button to close the popup
    closePopupButton.addEventListener("click", function () {
        locationPopup.style.display = "none"; // Hide the popup when clicking the close button
    });

    // Function to populate the provinceSelect
    function populateProvinceSelect() {
        // Check if the population has already occurred
        if (hasPopulatedProvinceSelect) {
            return; // Exit the function if already populated
        }

        // Ask for the Province list
        $.ajax({
            url: "/Index?handler=Provinces",
            success: function (provinceList) {
                // Create a Set to store unique values
                const uniqueValues = new Set();

                // Iterate through the received provinceList
                provinceList.forEach(province => {
                    // Check if the value is not already in the Set
                    if (!uniqueValues.has(province.value)) {
                        // Add the value to the Set to mark it as seen
                        uniqueValues.add(province.value);

                        // Create a new option element
                        const option = document.createElement("option");
                        option.value = province.value;
                        option.text = province.text;
                        if (province.value == "") option.hidden = true;
                        // Append the option to the provinceSelect
                        provinceSelect.appendChild(option);
                    }
                });

                // Set the flag to indicate that population has occurred
                hasPopulatedProvinceSelect = true;
            },
            error: function (error) {
                console.error('Error: ', error);
            }
        });
    }

    // Call the function to populate provinceSelect only once
    populateProvinceSelect();

    // Add a change event to the province select
    provinceSelect.addEventListener("change", function () {
        const selectedProvince = provinceSelect.value;
        if (selectedProvince) {
            if (!currentPageUrl.includes("/AddProductPage") == null) document.getElementById("chosenProvince").textContent = selectedProvince;
            // Make a fetch request to get the cantons of the selected province
            fetch(`/Index?handler=Cantones&provincia=${selectedProvince}`)
                .then(response => response.json()) // Convert the response to JSON
                .then(data => {
                    cantonSelect.innerHTML = ""; // Clear the canton select
                    data.forEach(canton => {
                        // Create options for each canton and add them to the select
                        const option = document.createElement("option");
                        option.value = canton.value;
                        option.text = canton.text;
                        if (canton.value == "") option.hidden = true;
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
            if (!currentPageUrl.includes("/AddProductPage")) {
                var text = span.textContent;
                text = text + ", " + selectedCanton;
                span.textContent = text;
                document.getElementById("chosenCanton").textContent = selectedCanton;
            }
        }
    });

    if (addProductForm != null) {
        // Add a submit event to the form
        addProductForm.addEventListener("submit", function (event) {
            const selectedProvince = provinceSelect.value;
            const selectedCanton = cantonSelect.value;

            if (!selectedProvince || selectedProvince === "" || !selectedCanton || selectedCanton === "") {
                event.preventDefault(); // Prevent the form from being submitted if information is missing
                alert("Debe seleccionar una ubicaci\u00F3n antes de enviar el formulario."); // Display an alert to the user
            }
        });
    }

    // Add a click event to the button to save the location
    saveLocationButton.addEventListener("click", function () {
        const selectedProvince = provinceSelect.value;
        const selectedCanton = cantonSelect.value;

        if (selectedProvince && selectedProvince !== "") {
            // Update the location information in the "locationInfo" element
            if (currentPageUrl.includes("/AddProductPage") && selectedCanton && selectedCanton !== "") {
                locationInfo.textContent = `Ubicaci\u00F3n elegida: ${selectedProvince}, ${selectedCanton}`;
                locationInfo.style.display = "block"; // Display the location information
                document.getElementById("selectedProvince").value = selectedProvince; // Update hidden values in the form
                document.getElementById("selectedCanton").value = selectedCanton;
            } else {
                span.textContent = selectedProvince;
                var text = span.textContent;
                if (selectedCanton && selectedCanton !== "") {
                    text = text + ", " + selectedCanton;
                    document.getElementById("chosenCanton").textContent = selectedCanton;
                }
                span.textContent = text;
            }
            locationPopup.style.display = "none"; // Hide the popup after saving the location
        }
    });
});
