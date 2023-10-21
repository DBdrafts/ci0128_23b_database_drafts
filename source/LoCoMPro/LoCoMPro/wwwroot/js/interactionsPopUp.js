function openInteractionsPopup(openButton) {
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    registerKeys = openButton.getAttribute('data-register-id');

    var [submitionDate, userID, productName, storeName, price, date, userName, comment] = registerKeys.split(String.fromCharCode(31));

    document.getElementById('popup-submitionDate').textContent = date;
    document.getElementById('popup-price').textContent = '₡' + price;
    document.getElementById('popup-userName').textContent = userName;
    document.getElementById('popup-comment').textContent = comment;

}

function closeInteractionsPopup() {
    var popup = document.querySelector('.interactions-popup');
    popup.style.display = 'none';
}

function toggleReport() {
    var reportButton = document.getElementById('reportIcon');
    if (reportIcon.src.endsWith('DesactiveReportIcon.svg')) {
        reportIcon.src = '/img/ActiveReportIcon.svg';
        reportActivated = true;
    } else {
        reportIcon.src = '/img/DesactiveReportIcon.svg';
        reportActivated = false;
    }
}

function saveInteractions() {
    if (reportActivated) {
        $.ajax({
            url: '/ProductPage/1?handler=HandleInteraction',
            data: { registerKeys: registerKeys },
            success: function () {
                console.log('Report saved successfully');
            },
            error: function (error) {
                console.error('Error saving report: ' + error);
            }
        });
    }
    closeInteractionsPopup();
}