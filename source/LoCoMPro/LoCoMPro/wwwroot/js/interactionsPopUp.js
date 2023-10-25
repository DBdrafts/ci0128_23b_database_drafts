function openInteractionsPopup(openButton) {
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    registerKeys = openButton.getAttribute('data-register-id');

    var [submitionDate, userID, productName, storeName, price, date, userName, comment] = registerKeys.split(String.fromCharCode(31));

    document.getElementById('popup-submitionDate').textContent = date;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-userName').textContent = userName;
    document.getElementById('popup-comment').textContent = comment;

    document.getElementById('reportIcon').src = '/img/DesactiveReportIcon.svg';
    reportActivated = false;

    highlight_star(0);
    registerReviewed = false;
    reviewedValue = 0;
}

function closeInteractionsPopup() {
    var popup = document.querySelector('.interactions-popup');
    popup.style.display = 'none';
}

function toggleReport() {
    if (reportIcon.src.endsWith('DesactiveReportIcon.svg')) {
        reportIcon.src = '/img/ActiveReportIcon.svg';
        reportActivated = true;
    } else {
        reportIcon.src = '/img/DesactiveReportIcon.svg';
        reportActivated = false;
    }
}

function saveInteractions() {
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

jQuery(document).ready(function ($) {
    $('.rating_stars span.r').hover(function () {
        // get hovered value
        var rating = $(this).data('rating');
        var value = $(this).data('value');
        $(this).parent().attr('class', '').addClass('rating_stars').addClass('rating_' + rating);
        highlight_star(value);
    }, function () {
        // get hidden field value
        var rating = $("#rating").val();
        var value = $("#rating_val").val();
        $(this).parent().attr('class', '').addClass('rating_stars').addClass('rating_' + rating);
        highlight_star(value);
    }).click(function () {
        // Set hidden field value
        var value = $(this).data('value');
        $("#rating_val").val(value);

        var rating = $(this).data('rating');
        $("#rating").val(rating);

        save_reviewed_state(value)
    });
});

function highlight_star(rating) {
    $('.rating_stars span.s').each(function () {
        var low = $(this).data('low');
        var high = $(this).data('high');
        $(this).removeClass('active-high').removeClass('active-low');
        if (rating >= high) $(this).addClass('active-high');
        else if (rating == low) $(this).addClass('active-low');
    });
}

function save_reviewed_state(value) {
    highlight_star(value);
    registerReviewed = true;
    reviewedValue = value;
}