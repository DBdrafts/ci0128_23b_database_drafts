// Usage: elementId is de id of the html div that will show the message,
// Example: showFeedbackMessage('saved successfully!', 'addProductFeedbackMessage');
function showFeedbackMessage(message, elementId) {
    var feedbackMessage = document.getElementById(elementId);
    if (feedbackMessage) {
        if (!message || message.trim() === '') {
            console.error('Null or empty message.');
        } else {
            feedbackMessage.textContent = message;
            feedbackMessage.classList.add('active');
            setTimeout(function () {
                feedbackMessage.classList.remove('active');
            }, 2500); // shows the message for 2.5 sg
        }
    } else {
        console.error('Element with ID ' + elementId + ' not found.');
    }
}


document.addEventListener("DOMContentLoaded", function () {
    if (feedbackMessageTemp && feedbackMessageTemp.trim() !== "") {
        showFeedbackMessage(feedbackMessageTemp, 'feedbackMessage');
    }
});

module.exports = { showFeedbackMessage };