const { closeInteractionsPopup } = require('../interactionsPopUp');

describe('closeInteractionsPopup', () => {
    let originalReportCommentInput;
    let originalReportLabel;
    let originalUndoReportLabel;

    beforeEach(() => {
        // Arrange simulate elements (mock)
        global.document.querySelector = jest.fn();

        global.document.querySelector.mockReturnValue({
            style: {},
        });

        global.reportCommentInput = {
            style: {},
        };

        global.reportLabel = {
            style: {},
        };

        global.undoReportLabel = {
            style: {},
        };

        global.reviewedValue = 1; // Simulate review
    });

    // Reset mock config
    afterEach(() => {
        global.document.querySelector.mockRestore();
        global.reportCommentInput = originalReportCommentInput;
        global.reportLabel = originalReportLabel;
        global.undoReportLabel = originalUndoReportLabel;
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should close interactions popup and reset values', () => {
        // Using default arrange

        // Act
        closeInteractionsPopup();

        // Assertions
        expect(global.reportCommentInput.style.display).toBe('none');
        expect(global.reportLabel.style.display).toBe('none');
        expect(global.undoReportLabel.style.display).toBe('none');
        expect(global.document.querySelector).toHaveBeenCalledWith('.interactions-popup');
    });
});