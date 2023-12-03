/// <summary>
/// Obtain the data needed to operate the Pop Up
/// </summary>
function openInteractionsPopup(openButton) {
    let interactionsPopup = document.querySelector('.interactions-popup');
    reportCommentInput = document.getElementById('report-comment');
    reportLabel = document.getElementById('report-label');
    undoReportLabel = document.getElementById('undo-report-label');

    lastReportLabel = document.getElementById('last-report-label');
    lastReportCommentInput = document.getElementById('last-report-comment');

    interactionsPopup.style.display = 'block';
    reportIcon = document.getElementById('reportIcon');
    registerKeys = openButton.getAttribute('data-register-id');

    // Gets the register data
    var [_, _, _, _, price, date, userName, comment, lastReviewValue, _,
        registerNumber] = registerKeys.split(String.fromCharCode(31));

    // Sets the register data
    document.getElementById('popup-submitionDate').textContent = date;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-userName').textContent = userName;
    document.getElementById('popup-comment').textContent = (comment !== null && comment !== '') ? comment : "N/A";
    document.getElementById('saveButton').value = registerNumber;
    
    // Set the images data
    let imagesData = openButton.getAttribute('images-register-id').split(String.fromCharCode(31));

    // Load the images in the pop up
    loadRegisterImages(imagesData);

    // Copies the validation star of the register
    copyRegisterValidation(registerNumber);

    // Set the information of the report button
    setReportedValue();

    // Sets the information for the review function and report
    setReviewedValue(lastReviewValue);
}


function openInteractionsPopupMod(openButton) {
    let interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    reportData = openButton.getAttribute('report-data');
    reportNumber = openButton.getAttribute('report-number');

    // Gets the register data
    var [_, _, ProductName, StoreName, SubmitionDate, _, _, _, _, reporterName, contributorName,
        price, registerNumber, comment, reportReason] = reportData.split(String.fromCharCode(31));


    // Sets the register data
    document.getElementById('popup-contributorName').textContent = contributorName;
    document.getElementById('popup-submitionDate').textContent = SubmitionDate;
    document.getElementById('popup-prodName').textContent = ProductName;
    document.getElementById('popup-store').textContent = StoreName;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-comment').textContent = comment;
    document.getElementById('popup-reporterName').textContent = reporterName;
    document.getElementById('popup-report-comment').innerHTML = reportReason !== '' ? reportReason : '-No especificado-';
    // Set the images data
    var imagesData = openButton.getAttribute('images-register-id').split(String.fromCharCode(31));

    // Copies the validation star of the register
    copyRegisterValidation(registerNumber);

    // Load the images in the pop up
    loadRegisterImages(imagesData)
}

function closeInteractionsPopupMod() {
    let popup = document.querySelector('.interactions-popup');
    popup.style.display = 'none';
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
    let popup = document.querySelector('.interactions-popup');
    if (reportCommentInput && reportLabel) {
        reportCommentInput.style.display = 'none';
        reportLabel.style.display = 'none';
    }
    if (undoReportLabel) {
        undoReportLabel.style.display = 'none';
    }
    popup.style.display = 'none';
}

/// <summary>
/// Toggle the report button between active and deactivate to show reason input field
/// </summary>
function toggleReport() {
    const isActivated = reportIcon.src.endsWith('DesactiveReportIcon.svg');
    reportIcon.src = isActivated ? '/img/ActiveReportIcon.svg' : '/img/DesactiveReportIcon.svg';
    reportActivated = isActivated;
    const displayStyle = isActivated ? 'block' : 'none';
    const undoDisplayStyle = isActivated ? 'none' : 'block';
    if (!hasReported) {
        setElementDisplay(reportLabel, displayStyle);
        setElementDisplay(reportCommentInput, displayStyle);
    } else {
        setElementDisplay(lastReportLabel, displayStyle);
        setElementDisplay(lastReportCommentInput, displayStyle);
        setElementDisplay(undoReportLabel, undoDisplayStyle);
    }
    reportChanged = !reportChanged;
}

function setElementDisplay(element, displayStyle) {
    element.style.display = displayStyle;
}


/// <summary>
/// Establish the initial state of the values for report
/// </summary>
function setReportedValue() {
    reportCommentInput.value = "";
    $.ajax({
        url: '/ProductPage/1?handler=CheckReportStatus',
        type: 'GET',
        data: { registerKeys: registerKeys },
        success: function (data) {
            hasReported = data.hasReported;
            if (hasReported) {
                reportIcon.src = '/img/ActiveReportIcon.svg';
                lastReportCommentInput.innerHTML = data.previousReportComment !== null ? data.previousReportComment : '-No especificado-';
                lastReportLabel.style.display = 'block';
                lastReportCommentInput.style.display = 'block';
            } else {
                reportIcon.src = '/img/DesactiveReportIcon.svg';
                lastReportLabel.style.display = 'none';
                lastReportCommentInput.style.display = 'none';
            }
            reportChanged = false;
        },
        error: function (error) {
            console.error('Error report status verification: ' + error);
        }
    });
}

/// <summary>
/// Save the interaction made by the user
/// </summary>
function saveInteractions() {

    // If the user made a change
    if (reportChanged || registerReviewed) {
        const reportComment = document.getElementById('report-comment').value;
        const url = '/ProductPage/1?handler=HandleInteraction';
        const beforeSendHandler = function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
        };

        const successHandler = function (data) {
            console.log('Report saved successfully' + data);
            const feedbackMessage = getFeedbackMessage();
            showFeedbackMessage(feedbackMessage, 'feedbackMessage');
            if (registerReviewed) {
                updateRegisterRating($("#saveButton").val(), registerKeys);
            }
        }

        const errorHandler = function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al realizar la interacción!', 'feedbackMessage');
        };

        $.ajax({
            type: 'POST',
            url: url,
            beforeSend: beforeSendHandler,
            data: {
                registerKeys: registerKeys, reportChanged: reportChanged,
                reviewedValue: reviewedValue, reportComment: reportComment
            },
            success: successHandler,
            error: errorHandler
        });
    }
    closeInteractionsPopup();
}

function getFeedbackMessage() {
    if (reportChanged && registerReviewed) {
        return 'Su reporte y valoración se han realizado correctamente!';
    } else if (reportChanged) {
        if (reportActivated) {
            return 'Su reporte se ha realizado correctamente!';
        } else {
            return 'Su reporte ha sido revertido correctamente!';
        }
    } else {
        return 'Su valoración se ha realizado correctamente!';
    }
}

///<summary>
/// This methods updates the register rating if the user just rated a register.
///</summary
function updateRegisterRating(registerNumber, registerKeys) {
    var register_veracity = $("#register_veracity_" + registerNumber).find('.veracity-stars-section');
    $.ajax({
        url: '/ProductPage/1?handler=AverageRegisterRating',
        type: 'GET',
        data: { registerKeys: registerKeys },
        success: function (data) {
            var rating = data.rating;
            var reviewCount = data.reviewCount;

            highlight_star(rating, register_veracity);
            $("#register_review_count_" + registerNumber).val(reviewCount).text(`(${reviewCount})`);
        },
        error: function (error) {
            console.error('Error report status verification: ' + error);
        }
    });
}

/// <summary>
/// The moderator accepts the report, hiding the report and setting its ReportState to 2
/// </summary>
function acceptReport() {

    $.ajax({
        type: 'POST',
        url: '/ModeratePage?handler=AcceptReport',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { reportData: reportData },
        success: function (data) {
            hideReport(reportNumber);
            console.log('Report updated successfully' + data);
            showFeedbackMessage('El reporte ha sido aprobado exitosamente', 'feedbackMessage');
            updateReportList();
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al aceptar el reporte ', 'feedbackMessage');
        }
    });
    closeInteractionsPopupMod();
}

/// <summary>
/// Hides the report visually from the moderator
/// </summary>
function hideReport(reportNumber) {
    // Hides the entry of the visual list that have this index
    var reportItem = document.getElementById("report-item-" + reportNumber);
    if (reportItem) {
        reportItem.style.display = "none";
    }
}

/// <summary>
/// The moderator rejects the report, hiding the report and setting its ReportState to 0
/// </summary>
function rejectReport() {

    $.ajax({
        type: 'POST',
        url: '/ModeratePage?handler=RejectReport',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { reportData: reportData },
        success: function (data) {
            hideReport(reportNumber);
            console.log('Report updated successfully' + data);
            showFeedbackMessage('El reporte ha sido rechazado exitosamente', 'feedbackMessage');
            updateReportList();
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al rechazar el reporte ', 'feedbackMessage');
        }
    });
    closeInteractionsPopupMod();
}

function updateReportList() {
    var reportCount = parseInt(document.getElementById("report-count").textContent);

    --reportCount;

    document.getElementById("report-count").textContent = reportCount;

    if (reportCount == 0) {
        document.getElementById("cant-reportes").style.display = 'none';
        document.getElementById("report-count").style.display = 'none';
        document.getElementById("empty-list").style.display = 'inline-block';
        document.getElementById("no-empty-list").style.display = 'none';
    }
}


//Note: This code was adapted from the page "codepen.io" to satisfies the needs of the project. All the credit go to this page.
//    The specific link of the page where the code was take from is: https://codepen.io/ashdurham/pen/AVVGvP

/// <summary>
/// Stabling the default values of the page
/// </summary>
if (typeof jQuery !== 'undefined') {
    jQuery(document).ready(function ($) {
        // Partes del código que dependen de jQuery
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
            highlight_star(0);
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
}


//function highlight_star(rating, rating_stars = '.rating_stars span.s') {
//    // If the user has already made a review
//    if (rating == 0 && reviewedValue != 0) {
//        rating = reviewedValue;
//    }
//    $(rating_stars).each(function () {
//        var low = $(this).data('low');
//        var high = $(this).data('high');
//        $(this).removeClass('active-high').removeClass('active-low');
//        if (rating >= high) $(this).addClass('active-high');
//        else if (rating == low) $(this).addClass('active-low');
//    });
//}

/// <summary>
/// Highlights the stars with corresponding rating.
/// </summary>
function highlight_star(rating, rating_stars = '.rating_stars span.s') {
    // If the user has already made a review
    if (rating == 0 && reviewedValue != 0) {
        rating = reviewedValue;
    }

    // Check if the rating_stars container has the class 'rating_stars'
    var isOldStructure = (rating_stars.valueOf() === '.rating_stars span.s');

    if (isOldStructure) {
        // Handle the original structure
        $(rating_stars).each(function () {
            var low = $(this).data('low');
            var high = $(this).data('high');
            $(this).removeClass('active-high').removeClass('active-low');
            if (rating >= high) $(this).addClass('active-high');
            else if (rating == low) $(this).addClass('active-low');
        });
    } else {
        // Handle the new structure
        // You may need to adjust this based on the specific structure of your new HTML
        // Iterate through each star icon
        rating_stars.find('i').each(function (index) {
            var currentStarIndex = index + 1; // Adjust index to start from 1

            // Determine the appropriate classes based on the rating
            if (currentStarIndex <= rating) {
                $(this).removeClass('fa-star-o fa-star-half-o').addClass('fa-star');
            } else if (currentStarIndex - 0.5 == rating) {
                $(this).removeClass('fa-star-o fa-star').addClass('fa-star-half-o');
            } else {
                $(this).removeClass('fa-star fa-star-half-o').addClass('fa-star-o');
            }
        });
    }
}

/// <summary>
/// Saves the actual review value choose
/// </summary>
function save_reviewed_state(value) {
    // Sets the values of the review
    registerReviewed = true;
    highlight_star(value);
    reviewedValue = value;
}

/// <summary>
/// Establish the initial state of the values for review
/// </summary>
function setReviewedValue(lastReviewValue) {
    // If the user has already made a review
    reviewedValue = lastReviewValue == null ? 0 : lastReviewValue;
    $.ajax({
        url: '/ProductPage/1?handler=CheckLastRaiting',
        type: 'GET',
        data: { registerKeys: registerKeys },
        success: function (data) {
            hasReviewd = data.hasReviewed;
            if (hasReviewd) {
                reviewedValue = data.previousReview;
              }
            registerReviewed = false;
            highlight_star(reviewedValue);
        },
        error: function (error) {
            console.error('Error report status verification: ' + error);
        }
    });
}


/// <summary>
/// Copies the representation of the register stars
/// </summary>
function copyRegisterValidation(registerNumber) {

    // Gets the representation of that register
    var veracityContent = $("#register_veracity_" + registerNumber).html();

    // Change the actual representation of the veracity
    $("#popup-veracity").html(veracityContent);
}

/// <summary>
/// The moderator accepts the report, hiding the report and setting its ReportState to 2
/// </summary>
function acceptRegisterAnormal() {

    $.ajax({
        type: 'POST',
        url: '/ModerateAnomaliesPage?handler=AcceptReport',

        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { reportData: reportData },
        success: function (data) {
            hideReport(reportNumber);
            console.log('Report updated successfully' + data);
            showFeedbackMessage('El reporte anómalo ha sido aprobado exitosamente', 'feedbackMessage');
            updateReportList();
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al aceptar el reporte ', 'feedbackMessage');
        }
    });
    closeInteractionsPopupMod();
}

/// <summary>
/// The moderator rejects the report, hiding the report and setting its ReportState to 0
/// </summary>
function rejectRegisterAnormal() {
    $.ajax({
        type: 'POST',
        url: '/ModerateAnomaliesPage?handler=RejectReport',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { reportData: reportData },
        success: function (data) {
            hideReport(reportNumber);
            console.log('Report updated successfully' + data);
            showFeedbackMessage('El reporte anómalo ha sido rechazado exitosamente', 'feedbackMessage');
            updateReportList();
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al rechazar el reporte ', 'feedbackMessage');
        }
    });
    closeInteractionsPopupMod();
} 

// export functions for tests
module.exports = {
    toggleReport,
    closeInteractionsPopup,
    setElementDisplay
};