let hasPopulatedProvinceSelect = false;
let map = null;
// Variables to save between pages
var savedProvince = "";
var savedCanton = "";
var savedLocation = null;
// Variables to use for page logic
var selectedProvince = null;
var selectedCanton = null;
var selectedLocation = null;
var selectedMarker = null;

$(document).ready(function () {
    const currentPageUrl = window.location.href;
    // Show the popup when the button is clicked
    $("#showPopupButton").on("click", function () {
        $("#mapPopup").show();
       /* $("mapPopUp").style.display = "block";*/
        initializeMap();
    });

    // Close the popup when clicking outside of it
    $(document).on("click", function (e) {
        if (!$(e.target).closest("#mapPopup").length && !$(e.target).closest("#showPopupButton").length && $('#mapPopup').css('display') !== 'none') {
            $("#mapPopup").hide();
        }
    });
    $("#closePopup-button").on("click", function () {
        $("#mapPopup").hide();
    });

    $("#saveLocation-button").on("click", function () {
        var text = getLocationButtonText(selectedProvince, selectedCanton);
        $("#buttonSpan").text(text);
        saveLocation();
        $("#mapPopup").hide();
    });

    // Call the function to populate provinceSelect only once
    populateProvinceSelect();

    $("#province").on("change", function () {
        selectedProvince = $("#province").val();
        if (selectedProvince) {
            if (!currentPageUrl.includes("/AddProductPage") == null) document.getElementById("chosenProvince").textContent = selectedProvince;
            // Make a fetch request to get the cantons of the selected province
            fetch(`/Index?handler=Cantones&provincia=${selectedProvince}`)
                .then(response => response.json()) // Convert the response to JSON
                .then(data => {
                    $("#canton").html(""); // Clear the canton select
                    data.forEach(canton => {
                        // Create options for each canton and add them to the select
                        const option = document.createElement("option");
                        option.value = canton.value;
                        option.text = canton.text;
                        if (canton.value == "") option.hidden = true;
                        $("#canton").append(option);
                    });
                    $("#canton").removeAttr("disabled"); // Enable the canton select

                })
                .catch(error => {
                    console.error("Error getting cantons:", error); // Error handling
                });
        } else {
            $("#canton").attr("disabled", "disabled"); // Disable the canton select if no province is selected
            $("#canton").html('<option value="" disabled selected hidden>Select a canton</option>'); // Restore the default value
        }
    });

    $("#canton").on("change", function () {
        selectedCanton = $("#canton").val();
        if (selectedCanton) {
            if (!currentPageUrl.includes("/AddProductPage")) {
                $("#chosenCanton").text(selectedCanton);
                getCoordinatesFromName();
            }
        }
    });
});

// Google Maps initialization function
function initializeMap() {
    const startingLocation = (savedLocation !== null)? savedLocation : { lat: 9.9280694, lng: -84.0907246 };

    if (map !== null) return;

    var costaRicaBounds = new google.maps.LatLngBounds(
        new google.maps.LatLng(8.0364, -85.7733),  // Southwestern boundary
        new google.maps.LatLng(11.2188, -82.6312)  // Northeastern boundary
    );

    var mapOptions = {
        center: { lat: startingLocation.lat, lng: startingLocation.lng },
        zoom: 15,
        maxZoom: 20, // Set a maximum zoom level
        disableDefaultUI: true,
        restriction: {
            latLngBounds: costaRicaBounds,
            strictBounds: true
        }
    };

    // Get map from api and select the locations.
    map = new google.maps.Map(document.getElementById('map'), mapOptions);
    map.fitBounds(costaRicaBounds);

    // Add a click event listener to the map
    map.addListener('click', function (event) {
        // Function to handle the user's click on the map
        handleMapClick(event.latLng);
    });
    // Add your map functionality here, such as allowing users to select a point.
}

// Function to handle the user's click on the map
function handleMapClick(location) {
    // You can do something with the selected location here
    console.log('User clicked at latitude: ' + location.lat() + ', longitude: ' + location.lng());
    console.log(location);

    updateMarkerPosition(location);
    reverseGeocodeLocation(location);
   // 
}

// Function to update the marker's position
function updateMarkerPosition(location) {
    // Set the marker's position to the clicked location
    if (selectedMarker == null) {
        selectedMarker = new google.maps.Marker({
            position: location,
            map: map,
            title: 'Selected Location'
        });
    }
    selectedLocation = location;
    selectedMarker.setPosition(location);
}

// Function to perform reverse geocoding when a marker is selected
function reverseGeocodeLocation(location) {
    var geocoder = new google.maps.Geocoder();

    geocoder.geocode({ 'location': location }, function (results, status) {
        if (status === 'OK') {
            if (results[0]) {
                // Analyze componentes to get pertinent data.
                var provinceName = "";
                var cantonName = "";
                for (var i = 0; i < results[0].address_components.length; i++) {
                    const addressComponent = results[0].address_components[i];
                    const types = addressComponent.types;
                    /*const longName = addressComponent.long_name;*/
                    const shortName = addressComponent.short_name;
                    if (types[0] == 'administrative_area_level_1') {
                        /*console.log('Marker is in the province of ' + shortName);*/
                        provinceName = shortName.replace(/\s*Provinc\w*\s*(?:de)?\s*/g, '');
                    } else if (types[0] == 'administrative_area_level_2' && types[1] == 'political') {
                        cantonName = shortName;
                    }
                    //console.log('Address Component Types:', types);
                    //console.log('Long Name:', longName);
                    //console.log('Short Name:', shortName);
                    //console.log('---');
                }
                
                $("#province").val(provinceName);
                $('#province').trigger('change');

                $("#canton").val(cantonName);
                $('#canton').trigger('change');
            } else {
                console.log('No results found');
            }
        } else {
            console.log('Geocoder failed due to: ' + status);
        }
    });
}

function saveLocation() {
    var text = getLocationButtonText(selectedProvince, selectedCanton);
    $("#buttonSpan").text(text);
    $("#chosenProvince").text(selectedProvince);
    $("#chosenCanton").text(selectedCanton);
    $("#latitude").val(selectedLocation.lat);
    $("#longitude").val(selectedLocation.lng);
    if (window.location.href.includes("/AddProductPage")) {
        $("#locationInfo").text(`Ubicaci\u00F3n elegida: ${selectedProvince}, ${selectedCanton}`);
        $("#locationInfo").show();
        document.getElementById("selectedProvince").value = selectedProvince;
        document.getElementById("selectedCanton").value = selectedCanton;
    }
}

function getLocationButtonText(province, canton) {
    if (province == null || province === "") return "Ubicaci\u00F3n";

    var text = province;
    if (canton && canton !== "") {
        text = text + ", " + canton;
    }
    return text;
}

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
                    $("#province").append(option);
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

function getCoordinatesFromName() {
    // Ask for the Location
    $.ajax({
        url: "/Index?handler=LocationFromCanton",
        data: { provinceName: selectedProvince, cantonName: selectedCanton },
        dataType: 'json',
        success: function (data) {
            const geoJson = JSON.parse(data.point);
            //console.log(geoJson);

            const latitude = geoJson["coordinates"][0];
            const longitude = geoJson["coordinates"][1]
            const latlng = new google.maps.LatLng(latitude, longitude);
            updateMarkerPosition(latlng);
            map.setCenter(latlng);
        },
        error: function (error) {
            console.error('Error: ', error);
        }
    });
}

//// Add a beforeunload event listener to check if the user is navigating away
//let isRefresh = false;

//// Listen for the 'beforeunload' event
//window.addEventListener('beforeunload', function (e) {
//    if (!isRefresh) {
//        // User is navigating away from the page, you can clear data here.
//        localStorage.clear();
//    }
//});

//// Listen for the 'unload' event to detect page refresh
//window.addEventListener('unload', function () {
//    isRefresh = true;
//});

//// Listen for the 'load' event to reset the flag
//window.addEventListener('load', function () {
//    isRefresh = false;
//});