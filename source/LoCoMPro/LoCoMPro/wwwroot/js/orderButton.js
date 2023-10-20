
// Gets the content of the order button
$(document).ready(function () {
    $('.order-button').on('click', function () {
        // Gets the button
        var button = $(this);
        // Gets the text
        var buttonText = button.text();

        $.get('/_Order_Button/SetContent', { buttonText: buttonText }, function (data) {
            button.text(data);
        });
    });
});
