Dropzone.autoDiscover = false;

var myDropzone = new Dropzone("#dragDropZone", {
    url: "/AddProductPage/?handler=UploadImage",
    paramName: "productImages",
    maxFiles: 5,
    acceptedFiles: "image/*",
    addRemoveLinks: true,
    dictRemoveFile: "Eliminar",
    autoProcessQueue: true,
    init: function () {
        this.on("success", function (file, response) {
        });

        this.on("sending", function (file, xhr, formData) {
            var csrfToken = $('input:hidden[name="__RequestVerificationToken"]').val();

            xhr.setRequestHeader("XSRF-TOKEN", csrfToken);
        });
    }
});
