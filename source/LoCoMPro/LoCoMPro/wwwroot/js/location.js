let changeFromMap = false;
let markers = [];
// Google services
let map = null;
let geocoder = null;
let placesService = null;
// Variables to use for page logic
var selectedProvince = null;
var selectedCanton = null;
var selectedLocation = null;
var selectedMarker = null;
var selectedStore = null;

let cantonMapping = {}

$(document).ready(function () {
    let hasPopulatedProvinceSelect = false;
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
    $("#closePopupMap-button").on("click", function () {
        $("#mapPopup").hide();
    });

    $("#saveLocationMap-button").on("click", function () {
        saveLocation();
        $("#mapPopup").hide();
    });

    // Call the function to populate provinceSelect only once
    populateProvinceSelect(hasPopulatedProvinceSelect);

    $("#province").on("click", function () {
        changeFromMap = false;
    });

    $("#province").on("change", function () {
        selectedProvince = $("#province").val();
        if (selectedProvince) {
            if (!currentPageUrl.includes("/AddProductPage") == null) $("#chosenProvince").text(selectedProvince);
            // Make a fetch request to get the cantons of the selected province
            populateCantonSelect(selectedProvince);
            
        } else {
            $("#canton").attr("disabled", "disabled"); // Disable the canton select if no province is selected
            $("#canton").html('<option value="" disabled selected hidden>Select a canton</option>'); // Restore the default value
        }
    });

    $("#canton").on("change", function () {
        selectedCanton = $("#canton").val();
        if (selectedCanton) {
            $("#chosenCanton").text(selectedCanton);
            if (changeFromMap == false) { getCoordinatesFromName(); }
        }
    });

    fillCantonMapping();
});

// Google Maps initialization function
function initializeMap() {
    const startingLocation = { lat: 9.9280694, lng: -84.0907246 };

    if (map !== null) return;


    var costaRicaBounds = new google.maps.LatLngBounds(
        //new google.maps.LatLng(8.0364, -85.7733),  // Southwestern boundary
        new google.maps.LatLng(7.96, -85.70),  // Southwestern boundary
        //new google.maps.LatLng(11.2188, -82.6312)  // Northeastern boundary
        new google.maps.LatLng(11.2188, -82.6312)  // Northeastern boundary
    );

    var mapOptions = {
        center: { lat: startingLocation.lat, lng: startingLocation.lng },
        zoom: 15,
        maxZoom: 500, // Set a maximum zoom level
        disableDefaultUI: true,
        restriction: {
            latLngBounds: costaRicaBounds,
            strictBounds: true
        },
        mapTypeId: "roadmap"
    };

    // Get map from api and select the locations.
    map = new google.maps.Map(document.getElementById('map'), mapOptions);
    map.fitBounds(costaRicaBounds);

    // Add a click event listener to the map
    map.addListener('click', function (event) {
        // Function to handle the user's click on the map
        handleMapClick(event.latLng);
    });

    // Initialize Search bar if in product page.
    if (window.location.href.includes("/AddProductPage")) {
        initializeSearchBar();
    }
}

// Function to handle the user's click on the map
function handleMapClick(location) {
    changeFromMap = true;

    updateMarkerPosition(location);
    reverseGeocodeLocation(location);
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
    clearMarkers();
}

// Initializes SearchBar only if in addProductPage
function initializeSearchBar() {
    // Create a search input and link it to the map.
    const input = document.createElement('input');
    input.id = 'search-input';
    input.type = 'text';
    input.placeholder = 'Search for places';

    // Create the search box and link it to the UI element.
    const searchBox = new google.maps.places.SearchBox(input);

    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
    // Bias the SearchBox results towards current map's viewport.
    map.addListener("bounds_changed", () => {
        searchBox.setBounds(map.getBounds());
    });

    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    searchBox.addListener("places_changed", () => {
        const places = searchBox.getPlaces();

        if (places.length == 0) {
            return;
        }

        // Clear out the old markers.
        markers.forEach((marker) => {
            marker.setMap(null);
        });
        markers = [];

        // For each place, get the icon, name and location.
        const bounds = new google.maps.LatLngBounds();

        places.forEach((place) => {
            if (!place.geometry || !place.geometry.location) {
                console.log("Returned place contains no geometry");
                return;
            }

            const icon = {
                url: place.icon,
                size: new google.maps.Size(85, 85),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(17, 34),
                scaledSize: new google.maps.Size(25, 25),
            };

            // Create a marker for each place.
            markers.push(
                new google.maps.Marker({
                    map,
                    icon,
                    title: place.name,
                    position: place.geometry.location,
                }),
            );
            if (place.geometry.viewport) {
                // Only geocodes have viewport.
                bounds.union(place.geometry.viewport);
            } else {
                bounds.extend(place.geometry.location);
            }
        });
        changeFromMap = true;
        /*var firstplace = places[0];*/
        selectedStore = places[0].name;
        selectedLocation = places[0].geometry.location;
        reverseGeocodeLocation(selectedLocation);
        map.fitBounds(bounds);
        selectedMarker.setMap(null);
        selectedMarker = null;
    });
}

// Function to perform reverse geocoding when a marker is selected
function reverseGeocodeLocation(location) {

    if (geocoder == null) {
        geocoder = new google.maps.Geocoder();
    }
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
                    } else if ((types[0] == 'administrative_area_level_2' || types[0] == 'locality') && types[1] == 'political') {
                        cantonName = shortName;
                    }
                }

                selectedCanton = (provinceName in cantonMapping && cantonName in cantonMapping[provinceName]) ? cantonMapping[provinceName][cantonName] : cantonName;
                $("#province").val(provinceName).trigger('change');

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
        $("#locationInfo").text(`Ubicaci\u00F3n elegida: ${text}`);
        $("#locationInfo").show();
        document.getElementById("selectedProvince").value = selectedProvince;
        document.getElementById("selectedCanton").value = selectedCanton;
    } else if (window.location.href.includes("/UserInfoPage")) {
        $("#ubicacion-change").html(`<strong>${selectedProvince}, ${selectedCanton}<strong>`);
        //showFeedbackMessage('Su lugar de preferencia se ha guardado!', 'feedbackMessage');
        try {
            const response = updateProvinciaToUser(selectedProvince);
            console.log('Province Updated: ', response.message);
        } catch (error) {
            console.error('Fail in update: ', error);
        }
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

function populateProvinceSelect(hasPopulatedProvinceSelect) {
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

function populateCantonSelect(canton) {
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

            if (changeFromMap) {
                const found = $("#canton option[value='" + selectedCanton + "']").length > 0;
                const cantonName = found ? selectedCanton : "";
                $("#canton").val(cantonName);
                $("#chosenCanton").text(cantonName);
            }
        })
        .catch(error => {
            console.error("Error getting cantons:", error); // Error handling
        }
    );
}

// Gets coordinates from name.
function getCoordinatesFromName() {
    // Ask for the Location
    $.ajax({
        url: "/Index?handler=LocationFromCanton",
        data: { provinceName: selectedProvince, cantonName: selectedCanton },
        dataType: 'json',
        success: function (data) {
            const geoJson = JSON.parse(data.point);
            //console.log(geoJson);

            const latitude = geoJson["coordinates"][1];
            const longitude = geoJson["coordinates"][0]

            const latlng = new google.maps.LatLng(latitude, longitude);

            map.setCenter(latlng);
            updateMarkerPosition(latlng);
        },
        error: function (error) {
            console.error('Error: ', error);
        }
    });
}

function clearMarkers() {
    if (markers.length > 0) {
        // Clear out the old markers.
        markers.forEach((marker) => {
            marker.setMap(null);
        });
        markers = [];
    }
}

function fillCantonMapping() {
    cantonMapping['San Jos\u00E9'] = {};
    cantonMapping['San Jos\u00E9']['San Pedro'] = 'Montes de Oca';
    cantonMapping['San Jos\u00E9']['San Rafael de Escaz\u00FA'] = 'Escaz\u00FA';
    cantonMapping['San Jos\u00E9']['Escazu'] = 'Escaz\u00FA';
    cantonMapping['San Jos\u00E9']['San Rafael'] = 'Escaz\u00FA';
    cantonMapping['San Jos\u00E9']['Guadalupe'] = 'Goicoechea';
    cantonMapping['San Jos\u00E9']['Copey District'] = 'Dota';
    cantonMapping['San Jos\u00E9']['Santa María'] = 'Dota';
    cantonMapping['San Jos\u00E9']['V\u00E1squez de Coronado'] = 'V\u00E1zquez de Coronado';
    cantonMapping['San Jos\u00E9']['San Isidro'] = 'V\u00E1zquez de Coronado';
    cantonMapping['San Jos\u00E9']['Carara'] = 'Turrubares';
    cantonMapping['San Jos\u00E9']['San Lorenzo'] = 'Terraz\u00FA';
    cantonMapping['San Jos\u00E9']['Tarrazu'] = 'Terraz\u00FA';
    cantonMapping['San Jos\u00E9']['Rivas'] = 'P\u00E9rez Zeled\u00F3n';

    cantonMapping['Cartago'] = {};
    cantonMapping['Cartago']['Guadalupe'] = 'Cartago';
    cantonMapping['Cartago']['Tierra Blanca'] = 'Cartago';
    cantonMapping['Cartago']['Llano Grande'] = 'Cartago';
    cantonMapping['Cartago']['Orosi'] = 'Para\u00EDso';
    cantonMapping['Cartago']['Llanos de Santa Luc\u00EDa'] = 'Para\u00EDso';
    cantonMapping['Cartago']['San Rafael'] = 'Oreamuno';
    cantonMapping['Cartago']['San Diego'] = 'La Uni\u00F3n';
    cantonMapping['Cartago']['Cot'] = 'Oreamuno';
    cantonMapping['Cartago']['Tres Equis'] = 'Turrialba';

    cantonMapping['Lim\u00F3n'] = {};
    cantonMapping['Lim\u00F3n']['Gu\u00E1piles'] = 'Pococ\u00ED';
    cantonMapping['Lim\u00F3n']['Jim\u00E9nez'] = 'Pococ\u00ED';
    cantonMapping['Lim\u00F3n']['Telire'] = 'Talamanca';
    cantonMapping['Lim\u00F3n']['Puerto Viejo de Talamanca'] = 'Talamanca';
    cantonMapping['Lim\u00F3n']['Dikoguicha'] = 'Talamanca';
    cantonMapping['Lim\u00F3n']['Sibubeta'] = 'Talamanca';
    cantonMapping['Lim\u00F3n']['Florida'] = 'Siquirres';


    cantonMapping['Alajuela'] = {};
    cantonMapping['Alajuela']['La Tigra'] = 'San Carlos';
    cantonMapping['Alajuela']['Pocosol'] = 'San Carlos';
    cantonMapping['Alajuela']['Cd Quesada'] = 'San Carlos';
    cantonMapping['Alajuela']['Santa Rosa de Pocosol'] = 'San Carlos';
    cantonMapping['Alajuela']['Aguas Zarcas'] = 'San Carlos';
    cantonMapping['Alajuela']['Gloria'] = 'San Carlos';
    cantonMapping['Alajuela']['La Fortuna'] = 'San Carlos';
    cantonMapping['Alajuela']['Valverde Vega'] = 'Sarch\u00ED';
    cantonMapping['Alajuela']['San Ramon'] = 'San Ram\u00F3n';

    cantonMapping['Heredia'] = {};
    cantonMapping['Heredia']['Puerto Viejo'] = 'Sarapiqu\u00ED';
    cantonMapping['Heredia']['Sarapiqui'] = 'Sarapiqu\u00ED';

    cantonMapping['Puntarenas'] = {};
    cantonMapping['Puntarenas']['Pitahaya'] = 'Puntarenas'

}
    });
}

function updateProvinciaToUser(province) {
    $.ajax({
        type: 'POST',
        url: '/UserInfoPage?handler=UpdateProvince', // Specify the handler
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { longitude: selectedLocation.lng, latitude: selectedLocation.lat },

        success: function (data) {
            console.log('Province Updated: ', data.message);
            return data;
        },

        error: function (error) {
            console.error('Fail in update: ', error);
            return data;
        }
    });

}

function setNullToUser() {
    //showFeedbackMessage('Su lugar de preferencia se ha guardado!', 'feedbackMessage');
    $("#ubicacion-change").html(`<strong>No agregada</strong>`);
    $.ajax({
        type: 'POST',
        url: '/UserInfoPage?handler=ResetGeolocation', // Specify the handler
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { },
        success: function (data) {
            console.log('Province Updated: ', data);
            return data;
        },

        error: function (error) {
            console.error('Fail in update: ', error);
            return data;
        }
    });

}

