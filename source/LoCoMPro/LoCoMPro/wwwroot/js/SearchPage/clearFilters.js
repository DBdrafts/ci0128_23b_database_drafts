$(document).ready(function () {
    // manage the click in the button
    $("#clear-filters").click(function () {
        // Uncheck all boxes in the filter section
        $("#filter-section input[type=checkbox]").prop("checked", false);
        
        // Trigger the filter function to reset the results
        document.querySelectorAll('input[name="SelectedCantons"]').forEach(checkbox => {
            checkbox.addEventListener('change', filterResults);
        });
    });
});