const { toggleReport } = require('../interactionsPopUp')

describe('toggleReport', () => { // describe sentence is for group related tests and config
    let originalReportActivated;
    let originalReportLabel;
    let originalReportCommentInput;
    let originalReportChanged;

    // Mock config
    beforeEach(() => {
        // Arrange simulate elements and set defaults values
        global.document.getElementById = jest.fn();

        global.document.getElementById.mockReturnValue({
            src: '/img/DesactiveReportIcon.svg', // Simulate .src property when getElementById is called in the method
        });
        
        global.reportIcon = {
            src: '/img/DesactiveReportIcon.svg', // Set .src desactive as default
        };

        originalReportActivated = global.reportActivated;  // This is beacuse in the method reportActivaded is a global variable
        global.reportActivated = false;  // Set false as default

        originalReportLabel = global.reportLabel;
        global.reportLabel = {
            style: {},  // Simulate css style property for 'none' and 'block' parameters
        };

        originalReportCommentInput = global.reportCommentInput;
        global.reportCommentInput = {
            style: {},
        };

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
        global.reportChanged = originalReportChanged;
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should activate the report and change de icon to active', () => {
        // Using default arrange config

        // Act
        toggleReport();

        // Assertions
        expect(global.reportIcon.src).toBe('/img/ActiveReportIcon.svg');
        expect(global.reportActivated).toBe(true);
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should deactivate the report ant change de icon to desactive', () => {
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
    test('should hide the label and reportCommentInputField', () => {
        // Arrange
        global.reportIcon.src = '/img/ActiveReportIcon.svg';
        global.reportActivated = true;

        // Act
        toggleReport();

        // Assertions
        expect(global.reportLabel.style.display).toBe('none');
        expect(global.reportCommentInput.style.display).toBe('none');
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should show the label and reportCommentInputField', () => {
        // Using default arrange config

        // Act
        toggleReport();

        // Assertions
        expect(global.reportLabel.style.display).toBe('block');
        expect(global.reportCommentInput.style.display).toBe('block');
    });

    // Test by Geancarlo Rivera Hernández C06516 | Sprint 3
    test('should set true to reportChanged', () => {
        // Using default arrange config

        // Act
        toggleReport();

        // Assertions
        expect(global.reportLabel.style.display).toBe('block');
        expect(global.reportCommentInput.style.display).toBe('block');
    });
});