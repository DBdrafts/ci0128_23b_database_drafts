const { toggleReport, setElementDisplay } = require('../interactionsPopUp');

describe('toggleReport', () => {
    let originalReportActivated;
    let originalReportLabel;
    let originalReportCommentInput;
    let originalLastReportLabel;
    let originalLastReportCommentInput;
    let originalUndoReportLabel;
    let originalHasReported;
    let originalReportChanged;

    // Mock config / Default Arrange
    beforeEach(() => {
        global.document.getElementById = jest.fn();

        global.document.getElementById.mockReturnValue({
            src: '/img/DesactiveReportIcon.svg',
        });

        global.reportIcon = {
            src: '/img/DesactiveReportIcon.svg',
        };

        originalReportActivated = global.reportActivated;
        global.reportActivated = false;

        originalReportLabel = global.reportLabel;
        global.reportLabel = {
            style: {},
        };

        originalReportCommentInput = global.reportCommentInput;
        global.reportCommentInput = {
            style: {},
        };

        originalLastReportLabel = global.lastReportLabel;
        global.lastReportLabel = {
            style: {},
        };

        originalLastReportCommentInput = global.lastReportCommentInput;
        global.lastReportCommentInput = {
            style: {},
        };

        originalUndoReportLabel = global.undoReportLabel;
        global.undoReportLabel = {
            style: {},
        };

        originalHasReported = global.hasReported;
        global.hasReported = false;

        originalReportChanged = global.reportChanged;
        global.reportChanged = false;
    });

    // Restore mock config and default values after each test
    afterEach(() => {
        global.document.getElementById.mockRestore();
        delete global.reportIcon;
        global.reportActivated = originalReportActivated;
        global.reportLabel = originalReportLabel;
        global.reportCommentInput = originalReportCommentInput;
        global.lastReportLabel = originalLastReportLabel;
        global.lastReportCommentInput = originalLastReportCommentInput;
        global.undoReportLabel = originalUndoReportLabel;
        global.hasReported = originalHasReported;
        global.reportChanged = originalReportChanged;
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should activate the report and change the icon to active', () => {
        // Using default arrange config

        // Act
        toggleReport();

        // Assertions
        expect(global.reportIcon.src).toBe('/img/ActiveReportIcon.svg');
        expect(global.reportActivated).toBe(true);
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should deactivate the report and change the icon to deactive', () => {
        // Arrange
        global.reportIcon.src = '/img/ActiveReportIcon.svg';
        global.reportActivated = true;

        // Act
        toggleReport();

        // Assertions
        expect(global.reportIcon.src).toBe('/img/DesactiveReportIcon.svg');
        expect(global.reportActivated).toBe(false);
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should display lastReportLabel and lastReportComment field', () => {
        // Using default arrange config

        // Act
        toggleReport();

        // Assertions
        expect(global.reportLabel.style.display).toBe('block');
        expect(global.reportCommentInput.style.display).toBe('block');
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should hide lastReportLabel and lastReportComment field', () => {
        // Arrange
        global.hasReported = true;
        global.reportIcon.src = '/img/ActiveReportIcon.svg';
        global.reportActivated = true;

        // Act
        toggleReport();

        // Assertions
        expect(global.lastReportLabel.style.display).toBe('none');
        expect(global.lastReportCommentInput.style.display).toBe('none');
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should set true to reportChanged', () => {
        // Using default arrange config

        // Act
        toggleReport();

        // Assertions
        expect(global.reportChanged).toBe(true);
    });
});
