function openInteractionsPopup(openButton) {
    // Muestra el pop-up
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    registerKeys = openButton.getAttribute('data-register-id');

    var [SubmitionDate, ContributorId, ProductName, StoreName, Price] = registerKeys.split(String.fromCharCode(31));
    document.getElementById('popup-submitionDate').textContent = SubmitionDate;
    document.getElementById('popup-price').textContent = '₡' + Price;
    document.getElementById('popup-userName').textContent = ContributorId;
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
    closeInteractionsPopup();
}