function openInteractionsPopupOldRegister(openButton) {
    var interactionsPopup = document.querySelector('.interactions-popup');
    interactionsPopup.style.display = 'block';
    reportData = openButton.getAttribute('report-data');
    reportNumber = openButton.getAttribute('report-number');

    // Gets the register data
    var [productName, storeName, dateLimit, numOldRegisters] = reportData.split(String.fromCharCode(31));
    // Sets the register data
    document.getElementById('popup-productName').textContent = productName;
    document.getElementById('popup-storeName').textContent = storeName;
    document.getElementById('popup-dateLimit').textContent = dateLimit;
    document.getElementById('popup-old-registers-amount').textContent = numOldRegisters;
}

function closeInteractionsPopupMod() {
    var popup = document.querySelector('.interactions-popup');
    popup.style.display = 'none';
}

/// <summary>
/// The moderator accepts the report, hiding the report and setting its ReportState to 2
/// </summary>
function hideOldRegisters() {

    $.ajax({
        type: 'POST',
        url: '/ModerateOldRegisters?handler=HideRegisters',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { registerData: reportData },
        success: function (data) {
            hideReport(reportNumber);
            console.log('Report updated successfully' + data);
            showFeedbackMessage('Los registros han sido ocultados exitosamente', 'feedbackMessage');
            updateReportList();
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al ocultar los registros', 'feedbackMessage');
        }
    });
    closeInteractionsPopupMod();
}

/// <summary>
/// The moderator rejects the report, hiding the report and setting its ReportState to 0
/// </summary>
function keepOldRegisters() {

    $.ajax({
        type: 'POST',
        url: '/ModerateOldRegisters?handler=KeepRegisters',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: { registerData: reportData },
        success: function (data) {
            hideReport(reportNumber);
            console.log('Report updated successfully' + data);
            showFeedbackMessage('Se han conservado los registros', 'feedbackMessage');
            updateReportList();
        },
        error: function (error) {
            console.error('Error saving report: ' + error);
            showFeedbackMessage('Error al conservar los registros', 'feedbackMessage');
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