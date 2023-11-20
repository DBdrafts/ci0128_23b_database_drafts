Dropzone.autoDiscover = false;
var myDropzone;
maxImages = 5;
document.addEventListener('DOMContentLoaded', function () {
    myDropzone = new Dropzone("#dragDropZone", {
        url: "/AddProductPage/?handler=HandleFormSubmission",
        paramName: "productImages",
        maxFiles: maxImages,
        acceptedFiles: "image/*",
        addRemoveLinks: true,
        dictRemoveFile: "Eliminar",
        dictDefaultMessage: "Haz clic o arrastra y suelta aquí las imágenes del producto",
        dictMaxFilesExceeded: "El límite de imágenes permitidas son " + maxImages + " máximo.",
        dictInvalidFileType: "Solo se permiten archivos de imagen.",
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

        if (!validateForm()) {
            return;
        }

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

    function validateForm() {
        var requiredFields = [
            { id: 'province', message: 'Por favor, seleccione una ubicación.' },
            { id: 'store', message: 'Por favor, ingrese el nombre del establecimiento.' },
            { id: 'productName', message: 'Por favor, ingrese el nombre del producto.' },
            { id: 'price', message: 'Por favor, ingrese el precio del producto.' }
        ];

        var isValid = true;

        for (var i = 0; i < requiredFields.length; i++) {
            var field = requiredFields[i];
            var fieldElement = document.getElementById(field.id);
            var fieldValue = fieldElement.value.trim();

            if (fieldValue === '') {
                isValid = false;
                displayErrorMessage(field.message);
                break;
            }
        }
        var nonImageFiles = myDropzone.files.filter(function (file) {
            return !file.type.startsWith('image/');
        });

        if (nonImageFiles.length > 0) {
            isValid = false;
            displayErrorMessage('Por favor, seleccione solo archivos de imagen');
        }

        return isValid;
    }

    function displayErrorMessage(message) {
        showFeedbackMessage(message, 'feedbackMessage');
    }

});
