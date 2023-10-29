


function callAddProductToList() {
    $.ajax({
        type: "POST",
        url: "/ProductPage/1?handler=AddToProductList",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
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