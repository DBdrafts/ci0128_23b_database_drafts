// Usage: elementId is de id of the html div that will show the message,
// Example: showFeedbackMessage('saved successfully!', 'addProductFeedbackMessage');
function showFeedbackMessage(message, elementId) {
    var feedbackMessage = document.getElementById(elementId);
    if (feedbackMessage) {
        feedbackMessage.textContent = message;
        feedbackMessage.classList.add('active');
        setTimeout(function () {
            feedbackMessage.classList.remove('active');
        }, 2500); // shows the message for 2.5 sg
    } else {
        console.error('Element with ID ' + elementId + ' not found.');
    }
}