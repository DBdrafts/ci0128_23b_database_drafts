Dropzone.autoDiscover = false;
var myDropzone;
document.addEventListener('DOMContentLoaded', function () {
    myDropzone = new Dropzone("#dragDropZone", {
        url: "/AddProductPage/?handler=HandleFormSubmission",
        paramName: "productImages",
        maxFiles: 5,
        acceptedFiles: "image/*",
        addRemoveLinks: true,
        dictRemoveFile: "Eliminar",
        autoProcessQueue: false,
        init: function () {
            this.on("success", function (file, response) {
                console.log('Success:', response);
            });

            this.on("sending", function (file, xhr, formData) {
                var csrfToken = $('input:hidden[name="__RequestVerificationToken"]').val();
                xhr.setRequestHeader("XSRF-TOKEN", csrfToken);
            });
        }
    });

    document.getElementById('form-submit-button').addEventListener('click', function (e) {
        e.preventDefault();
        myDropzone.processQueue();
        var formData = new FormData(document.getElementById('addProductForm'));

        if (myDropzone.files.length > 0) {
            myDropzone.files.forEach(function (file, index) {
                formData.append('productImages', file);
            });
        }

        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/AddProductPage?handler=HandleFormSubmission', true);
        xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());

        xhr.onload = function () {
            if (xhr.status === 200) {
                console.log('Success:', xhr.responseText);
                window.location.href = '/Index';
            } else {
                console.error('Error:', xhr.statusText);
            }
        };

        xhr.send(formData);
    });

});
