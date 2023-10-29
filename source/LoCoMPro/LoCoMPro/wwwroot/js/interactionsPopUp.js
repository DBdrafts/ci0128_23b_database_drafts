/// <summary>
/// Obtain the data needed to operate the Pop Up
/// </summary>
function openInteractionsPopup(openButton) {
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    registerKeys = openButton.getAttribute('data-register-id');

    // Gets the register data
    var [submitionDate, userID, productName, storeName, price, date,
        userName, comment, lastReviewValue, lastReportState] = registerKeys.split(String.fromCharCode(31));

    // Sets the register data
    document.getElementById('popup-submitionDate').textContent = date;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-userName').textContent = userName;
    document.getElementById('popup-comment').textContent = comment;

    // Set the report button off as default
    document.getElementById('reportIcon').src = '/img/DesactiveReportIcon.svg';

    // Sets the information for the review function and report
    setReviewedValue(lastReviewValue);   
    setReportedValue(lastReportState);   
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
    reportChanged = !reportChanged;

}



/// <summary>
/// Save the interaction made by the user
/// </summary>
function saveInteractions() {
    // If the user made a change
    if (reportChanged || registerReviewed) {
        $.ajax({
            type: 'POST',
            url: '/ProductPage/1?handler=HandleInteraction', // Specify the handler
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: { registerKeys: registerKeys, reportChanged: reportChanged, reviewedValue: reviewedValue },
            success: function (data) {
                console.log('Report saved successfully' + data);
                showFeedbackMessage('Su reporte se ha realizado correctamente!');
            },
            error: function (error) {
                console.error('Error saving report: ' + error);
                showFeedbackMessage('Error al realizar el reporte!');

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
/// Establish the initial state of the values for review
/// </summary>
function setReviewedValue(lastReviewValue) {
    // If the user has already made a review
    reviewedValue = lastReviewValue == null ? 0 : lastReviewValue;
    highlight_star(reviewedValue);
    registerReviewed = false;
}

/// <summary>
/// Establish the initial state of the values for report
/// </summary>
function setReportedValue(lastReportedValue) {
    // If the user has already made a report
    reportActivated = false;
    if (lastReportedValue != -1) {
        reportActivated = true;
        reportIcon.src = '/img/ActiveReportIcon.svg';
    }
    reportChanged = false;
}