document.addEventListener("DOMContentLoaded", function () {
    if (feedbackMessageTemp && feedbackMessageTemp.trim() !== "") {
        showFeedbackMessage(feedbackMessageTemp, 'feedbackMessage');
    }
});