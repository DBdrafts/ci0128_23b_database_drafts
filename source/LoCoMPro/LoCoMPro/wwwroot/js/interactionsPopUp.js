/// <summary>
/// Obtain the data needed to operate the Pop Up
/// </summary>
function openInteractionsPopup(openButton) {
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    registerKeys = openButton.getAttribute('data-register-id');

    // Gets the register data
    var [submitionDate, userID, productName, storeName, price, date, userName, comment, lastReviewValue] = registerKeys.split(String.fromCharCode(31));

    // Sets the register data
    document.getElementById('popup-submitionDate').textContent = date;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-userName').textContent = userName;
    document.getElementById('popup-comment').textContent = comment;

    // Set the images data
    var imagesData = openButton.getAttribute('images-register-id').split(String.fromCharCode(31));

    // Load the images in the pop up
    loadRegisterImages(imagesData)

    // Set the information of the report button
    document.getElementById('reportIcon').src = '/img/DesactiveReportIcon.svg';
    reportActivated = false;

    // Sets the information for the review function
    setReviewedValue(lastReviewValue);
}

/// <summary>
/// Add the images to the Pop Up
/// </summary>
function loadRegisterImages(imagesData) {
    // container for pop up images
    var imageContainer = document.querySelector('.img-carousel-pop-up');

    // Clear previous images in the carousel
    imageContainer.innerHTML = '';

    // Create and display image elements in the carousel
    for (var i = 1; i < imagesData.length; i++) { // Start from 1 to skip the first empty string
        // Split every imagen
        var imageData = imagesData[i].split(":");
        // Use the js Image element
        var imageElement = new Image();
        // Set the sourse and the Data of image
        imageElement.src = 'data:image/jpeg;base64,' + imageData[1];
        // Alternative text
        imageElement.alt = 'Imagen ' + i;

        // Add the styles of the image
        imageElement.style.width = '48.5px';
        imageElement.style.height = '48.5px';
        imageElement.style.borderRadius = '5px';
        imageElement.style.objectFit = 'fill';
        imageElement.className = 'small-image'

        // Add the html div for every image
        var smallImageSpace = document.createElement('div');
        // Set the space to add the image
        smallImageSpace.classList.add('small-img-space-pop-up');
        // Add the space and the element
        smallImageSpace.appendChild(imageElement);
        imageContainer.appendChild(smallImageSpace);
    }
}

/// <summary>
/// Close the Pop Up
/// </summary>
function closeInteractionsPopup() {
    // Returns the values to it´s original form
    reviewedValue = 0;
    var popup = document.querySelector('.interactions-popup');
    popup.style.display = 'none';
}

/// <summary>
/// Toggle the report button between active and deactivate
/// </summary>
function toggleReport() {
    if (reportIcon.src.endsWith('DesactiveReportIcon.svg')) {
        // Active
        reportIcon.src = '/img/ActiveReportIcon.svg';
        reportActivated = true;
    } else {
        // Deactivate
        reportIcon.src = '/img/DesactiveReportIcon.svg';
        reportActivated = false;
    }
}

/// <summary>
/// Save the interaction made by the user
/// </summary>
function saveInteractions() {
    // If the user made a change
    if (reportActivated || registerReviewed) {
        $.ajax({
            type: 'POST',
            url: '/ProductPage/1?handler=HandleInteraction', // Specify the handler
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: { registerKeys: registerKeys, reportActivated: reportActivated, reviewedValue: reviewedValue },
            success: function (data) {
                console.log('Report saved successfully' + data);
                showFeedbackMessage('Su reporte se ha realizado correctamente!', 'feedbackMessage');
            },
            error: function (error) {
                console.error('Error saving report: ' + error);
                showFeedbackMessage('Error al realizar el reporte!', 'feedbackMessage');

            }
        });
    }
    closeInteractionsPopup();
}

/// <summary>
/// Show the feedback message that confirms the action
/// </summary>
function showFeedbackMessage(message) {
    var feedbackMessage = document.getElementById('feedbackMessage');
    feedbackMessage.textContent = message;
    feedbackMessage.classList.add('active');

    setTimeout(function () {
        feedbackMessage.classList.remove('active');
    }, 2500); // shows the message for 2.5 sg
}

//Note: This code was adapted from the page "codepen.io" to satisfies the needs of the project. All the credit go to this page.
//    The specific link of the page where the code was take from is: https://codepen.io/ashdurham/pen/AVVGvP

/// <summary>
/// Stabling the default values of the page
/// </summary>
jQuery(document).ready(function ($) {
    //  Review section when hover
    $('.rating_stars span.r').hover(function () {
        // get hovered value
        var rating = $(this).data('rating');
        var value = $(this).data('value');
        $(this).parent().attr('class', '').addClass('rating_stars').addClass('rating_' + rating);
        highlight_star(value);
    }, function () {
        // Review section when not hover
        // get hidden field value
        var rating = $("#rating").val();
        var value = $("#rating_val").val();
        $(this).parent().attr('class', '').addClass('rating_stars').addClass('rating_' + rating);
        highlight_star(value);
    }).click(function () {
        // Review section when clicked
        // Set hidden field value
        var value = $(this).data('value');
        $("#rating_val").val(value);

        var rating = $(this).data('rating');
        $("#rating").val(rating);

        save_reviewed_state(value)
    });
});

/// <summary>
/// Set the amount of stars highlighted 
/// </summary>
function highlight_star(rating) {
    $('.rating_stars span.s').each(function () {
        // If the user has already made a review
        if (rating == 0 && reviewedValue != 0) {
            rating = reviewedValue;
        }
        var low = $(this).data('low');
        var high = $(this).data('high');
        $(this).removeClass('active-high').removeClass('active-low');
        if (rating >= high) $(this).addClass('active-high');
        else if (rating == low) $(this).addClass('active-low');
    });
}

/// <summary>
/// Saves the actual review value choose
/// </summary>
function save_reviewed_state(value) {
    // Sets the values of the review
    highlight_star(value);
    registerReviewed = true;
    reviewedValue = value;
}

/// <summary>
/// Establish the initial state of the values
/// </summary>
function setReviewedValue(lastReviewValue) {
    // If the user has already made a review
    reviewedValue = lastReviewValue == null ? 0 : lastReviewValue;
    highlight_star(reviewedValue);
    registerReviewed = false;
}