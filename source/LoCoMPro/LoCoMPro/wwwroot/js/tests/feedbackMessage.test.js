const { showFeedbackMessage } = require('../feedbackMessage');

describe('showFeedbackMessage', () => {

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should log an error when the feedback message is null', () => {
        // Arrange
        const elementId = 'element';
        const message = null;

        // Spy on the console.error function to track its calls.
        const consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => { });

        // Mock document.getElementById to simulate an element with empty textContent.
        document.getElementById = jest.fn().mockReturnValueOnce({
            textContent: '',
            classList: {
                add: jest.fn(),
                remove: jest.fn(),
            },
        });

        // Act
        showFeedbackMessage(message, elementId);

        // Assert
        expect(consoleErrorSpy).toHaveBeenCalledWith('Null or empty message.');

        // Restore the original console.error function.
        consoleErrorSpy.mockRestore();
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should log an error when the feedback message is empty', () => {
        // Arrange
        const elementId = 'element';
        const message = '';

        // Spy on the console.error function to track its calls.
        const consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => { });

        // Mock document.getElementById to simulate an element with empty textContent.
        document.getElementById = jest.fn().mockReturnValueOnce({
            textContent: '',
            classList: {
                add: jest.fn(),
                remove: jest.fn(),
            },
        });

        // Act
        showFeedbackMessage(message, elementId);

        // Assert
        expect(consoleErrorSpy).toHaveBeenCalledWith('Null or empty message.');

        // Restore the original console.error function.
        consoleErrorSpy.mockRestore();
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should log an error when the elementId is null', () => {
        // Arrange
        const elementId = 'element';
        const message = 'This is a test message';

        // Spy on the console.error function to track its calls.
        const consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => { });

        // The mock is configured so that the first time document.getElementById is called in the showMessage method,
        // it should return null, no matter what the actual value of elementId is.
        document.getElementById = jest.fn().mockReturnValueOnce(null);

        // Act
        showFeedbackMessage(message, elementId);

        // Assert
        expect(consoleErrorSpy).toHaveBeenCalledWith(`Element with ID ${elementId} not found.`);

        // Restore the original console.error function.
        consoleErrorSpy.mockRestore();
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should display the message in the existing element', () => {
        // Arrange
        const elementId = 'element';
        const message = 'This is a test message';

        // Mock document.getElementById to simulate an existing element.
        const mockElement = {
            textContent: '',
            classList: {
                add: jest.fn(),
                remove: jest.fn(),
            },
        };

        // get a simulated valid element when document.getElementById is called in the showMessage method
        document.getElementById = jest.fn().mockReturnValueOnce(mockElement);

        // Act
        showFeedbackMessage(message, elementId);

        // Asserts
        expect(document.getElementById).toHaveBeenCalledWith(elementId);
        expect(mockElement.textContent).toBe(message);
    });
});
