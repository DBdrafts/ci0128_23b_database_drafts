$(document).ready(function () {
    // Define the order of required fields in an array
    const requiredFields = [
        "#store",
        "#productName",
        "#price"
    ];

    // Initially, disable all fields
    $(".required-field").prop("disabled", true);
   // $(requiredFields[0]).prop("disabled", false);

    // Enable the next field in the sequence when the previous field is filled
    $(".required-field").on("input", function () {
        let $this = $(this); // Capture the current field as a jQuery object
        if ($this.val().trim() !== "") { // Check if the current field has content
            let currentIndex = requiredFields.indexOf("#" + $this.attr("id")); // Get the index of the current field in the array
            if (currentIndex !== -1 && currentIndex < requiredFields.length - 1) {
                let nextField = requiredFields[currentIndex + 1]; // Get the next field in the sequence
                $(nextField).prop("disabled", false); // Enable the next field
            }
        } else {
            // If the current field is cleared, disable the fields following in the sequence
            let currentIndex = requiredFields.indexOf("#" + $this.attr("id")); // Get the index of the current field in the array
            if (currentIndex !== -1) {
                for (let i = currentIndex + 1; i < requiredFields.length; i++) {
                    $(requiredFields[i]).prop("disabled", true); // Disable the following fields
                    // $(requiredFields[i]).prop("disabled", true).val(""); -> If we want to clean the next fields
                }
            }
        }
    });

    // When a location is saved, enable store field
    $("#saveLocationMap-button").on("click", function () {
        $("#store").prop("disabled", false); 
    });
});
