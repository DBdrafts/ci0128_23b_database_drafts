let map = null;

$(document).ready(function () {
    // Show the popup when the button is clicked
    $("#showPopupButton").on("click", function () {
        $("#mapPopup").show();
       /* $("mapPopUp").style.display = "block";*/
        initializeMap();
    });

    // Close the popup when clicking outside of it
    //$(document).on("click", function (e) {
    //    if (!$(e.target).closest("#mapPopup").length) {
    //        $("#mapPopup").hide();
    //       /* $("mapPopUp").style.display = "none";*/
    //    }
    //});
});

// Google Maps initialization function
function initializeMap() {

    if (map !== null) return;

    var costaRicaBounds = new google.maps.LatLngBounds(
        new google.maps.LatLng(8.0364, -85.7733),  // Southwestern boundary
        new google.maps.LatLng(11.2188, -82.6312)  // Northeastern boundary
    );

    var mapOptions = {
        center: { lat: 9.9280694, lng: -84.0907246 },
        zoom: 15,
        maxZoom: 20, // Set a maximum zoom level
        disableDefaultUI: true,
        restriction: {
            latLngBounds: costaRicaBounds,
            strictBounds: true
        }
    };

    if (map === null) {
        map = new google.maps.Map(document.getElementById('map'), mapOptions);
        map.fitBounds(costaRicaBounds);


    // Add your map functionality here, such as allowing users to select a point.
    // Fit the map to the initial bounds
}

/*window.initializeMap();*/