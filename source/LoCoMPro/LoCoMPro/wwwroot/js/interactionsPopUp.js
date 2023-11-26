/// <summary>
/// Obtain the data needed to operate the Pop Up
/// </summary>
function openInteractionsPopup(openButton) {
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    registerKeys = openButton.getAttribute('data-register-id');

    // Gets the register data
    var [submitionDate, userID, productName, storeName, price, date,
        userName, comment, lastReviewValue, lastReportState, registerNumber] = registerKeys.split(String.fromCharCode(31));

    // Sets the register data
    document.getElementById('popup-submitionDate').textContent = date;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-userName').textContent = userName;
    document.getElementById('popup-comment').textContent = comment;


    // Set the images data
    var imagesData = openButton.getAttribute('images-register-id').split(String.fromCharCode(31));

    // Load the images in the pop up
    loadRegisterImages(imagesData)

    // Copies the validation star of the register
    copyRegisterValidation(registerNumber);

    // Set the information of the report button
    document.getElementById('reportIcon').src = '/img/DesactiveReportIcon.svg';

    // Sets the information for the review function and report
    setReviewedValue(lastReviewValue);   
    setReportedValue(lastReportState);   
}

function openInteractionsPopupMod(openButton) {
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    reportData = openButton.getAttribute('report-data');
    reportNumber = openButton.getAttribute('report-number');

    // Gets the register data
    var [ReporterId, ContributorId, ProductName, StoreName, SubmitionDate, CantonName,
        ProvinceName, ReportDate, ReportState, reporterName, contributorName, price, registerNumber, comment] = reportData.split(String.fromCharCode(31));

    

    // Sets the register data
    document.getElementById('popup-contributorName').textContent = contributorName;
    document.getElementById('popup-submitionDate').textContent = SubmitionDate;
    document.getElementById('popup-prodName').textContent = ProductName;
    document.getElementById('popup-store').textContent = StoreName;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-comment').textContent = comment;
    document.getElementById('popup-reporterName').textContent = reporterName;

    // Set the images data
    var imagesData = openButton.getAttribute('images-register-id').split(String.fromCharCode(31));

    // Copies the validation star of the register
    copyRegisterValidation(registerNumber);

    // Load the images in the pop up
    loadRegisterImages(imagesData)
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
                if (reportChanged && registerReviewed) {
                    showFeedbackMessage('Su reporte y valoración se han realizado correctamente!', 'feedbackMessage');
                } else {
                    if (reportChanged) {
                        if (reportActivated) {
                            showFeedbackMessage('Su reporte se ha realizado correctamente!', 'feedbackMessage');
                        } else {
                            showFeedbackMessage('Su reporte ha sido revertido correctamente!', 'feedbackMessage');
                        }
                    } else {
                        showFeedbackMessage('Su valoración se ha realizado correctamente!', 'feedbackMessage');
                    }
                }
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
    closeInteractionsPopup();
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
    closeInteractionsPopup();
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

/// <summary>
/// Set the amount of stars highlighted 
/// </summary>
function highlight_star(rating) {
    // If the user has already made a review
    if (rating == 0 && reviewedValue != 0) {
        rating = reviewedValue;
    }
    $('.rating_stars span.s').each(function () {
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
    registerReviewed = false;
    highlight_star(reviewedValue);
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
        url: '/ModerateAnomaliesPage?handler=acceptReport',
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
    closeInteractionsPopup();
}

/// <summary>
/// The moderator rejects the report, hiding the report and setting its ReportState to 0
/// </summary>
function rejectRegisterAnormal() {
    $.ajax({
        type: 'POST',
        url: '/ModerateAnomaliesPage?handler=rejectReport',
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
    closeInteractionsPopup();
} 