document.addEventListener("DOMContentLoaded", function () {
    const moderatorButton = document.getElementById("moderator-button");

    const moderatorPopup = document.getElementById("popup-moderator");
    // Add listener to the button
    moderatorButton.addEventListener("click", function (event) {
        event.stopPropagation();
        moderatorPopup.style.display = "block";
    });

    // Closes the popup when clicking outside of it
    document.addEventListener("click", function (event) {
        if (event.target !== moderatorButton && event.target !== moderatorPopup) {
            moderatorPopup.style.display = "none";
        }
    });

    // Prevents the pop up from closing when clicking inside of it
    moderatorPopup.addEventListener("click", function (event) {
        event.stopPropagation();
    });
});