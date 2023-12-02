// Disable automatic Dropzone discovery
Dropzone.autoDiscover = false;

// Declare variables
let myDropzone;
const maxImages = 5;

// Initialize Dropzone when the DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Create a new instance of Dropzone
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
            // Event handlers for Dropzone
            this.on("success", function (file, response) {
                console.log('Success:', response);
            });

            this.on("addedfile", function (file) {

                let validFiles = myDropzone.files.filter(function (f) {
                    return f.status !== 'error';
                });

                if (validFiles.length > maxImages) {
                    showFeedbackMessage('El límite de imágenes permitidas es ' + maxImages + ' máximo.', 'feedbackMessage');
                    this.removeFile(file);
                }
            });

            this.on("sending", function (file, xhr, formData) {
                // Set XSRF token for the request
                const csrfToken = $('input:hidden[name="__RequestVerificationToken"]').val();
                xhr.setRequestHeader("XSRF-TOKEN", csrfToken);
            });
        }
    });

    // Event listener for the form submission button
    document.getElementById('form-submit-button').addEventListener('click', function (e) {
        e.preventDefault();

        // Validate the form before proceeding
        if (!validateForm()) {
            return;
        }

        // Process the Dropzone queue
        myDropzone.processQueue();

        // Create a FormData object from the form
        let formData = new FormData(document.getElementById('addProductForm'));

        // Append uploaded files to the FormData object
        if (myDropzone.files.length > 0) {
            myDropzone.files.forEach(function (file, index) {
                formData.append('productImages', file);
            });
        }

        // Create a new XMLHttpRequest for form submission
        let xhr = new XMLHttpRequest();
        xhr.open('POST', '/AddProductPage?handler=HandleFormSubmission', true);
        xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());

        // Define the onload callback for the XMLHttpRequest
        xhr.onload = function () {
            if (xhr.status === 200) {
                console.log('Success:', xhr.responseText);
                window.location.href = '/Index';
            } else {
                console.error('Error:', xhr.statusText);
            }
        };

        // Send the FormData with the XMLHttpRequest
        xhr.send(formData);
    });

    // Function to validate the form fields
    function validateForm() {
        const requiredFields = [
            { id: 'province', message: 'Por favor, seleccione una ubicación.' },
            { id: 'store', message: 'Por favor, ingrese el nombre del establecimiento.' },
            { id: 'productName', message: 'Por favor, ingrese el nombre del producto.' },
            { id: 'price', message: 'Por favor, ingrese el precio del producto.' }
        ];

        let isValid = true;

        for (let i = 0; i < requiredFields.length; i++) {
            let field = requiredFields[i];
            let fieldElement = document.getElementById(field.id);
            let fieldValue = fieldElement.value.trim();

            if (fieldValue === '') {
                isValid = false;
                showFeedbackMessage(field.message, 'feedbackMessage');
                break;
            }
        }

        let nonImageFiles = myDropzone.files.filter(function (file) {
            return !file.type.startsWith('image/');
        });

        if (nonImageFiles.length > 0) {
            isValid = false;
            showFeedbackMessage('Por favor, seleccione solo archivos de imagen', 'feedbackMessage');
        }

        let duplicateFileNames = findDuplicateFileNames(myDropzone.files);

        if (duplicateFileNames.length > 0) {
            isValid = false;
            let message = 'No se permite enviar imágenes con el mismo nombre: ' + duplicateFileNames.join(', ');
            showFeedbackMessage(message, 'feedbackMessage');
        }

        return isValid;
    }

    // Find duplicate image names
    function findDuplicateFileNames(files) {
        let fileNames = {};
        let duplicateFileNames = [];

        files.forEach(function (file) {
            let fileName = file.name;
            if (fileNames[fileName]) {
                duplicateFileNames.push(fileName);
            } else {
                fileNames[fileName] = true;
            }
        });

        return duplicateFileNames;
    }
});
