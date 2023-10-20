function openInteractionsPopup() {
    var popup = document.querySelector('.interactions-popup');
    popup.style.display = 'block';
}

function closeInteractionsPopup() {
    var popup = document.querySelector('.interactions-popup');
    popup.style.display = 'none';
}

function toggleReport() {
    var reportButton = document.getElementById('reportIcon');
    if (reportIcon.src.endsWith('DesactiveReportIcon.svg')) {
        reportIcon.src = '/img/ActiveReportIcon.svg';
    } else {
        reportIcon.src = '/img/DesactiveReportIcon.svg';
    }
}

function saveInteractions() {
    closeInteractionsPopup(); // Cierra el pop-up después de guardar.
}
