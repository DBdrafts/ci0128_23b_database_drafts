// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Define a key to store the tab count
//const tabCountKey = 'tabCount';

//// Function to decrement the tab count
//function decrementTabCount() {
//    const tabCount = parseInt(localStorage.getItem(tabCountKey) || '0', 10);
//    if (tabCount > 0) {
//        localStorage.setItem(tabCountKey, (tabCount - 1).toString());
//    }
//}

//// Function to increment the tab count
//function incrementTabCount() {
//    const tabCount = parseInt(localStorage.getItem(tabCountKey) || '0', 10);
//    localStorage.setItem(tabCountKey, (tabCount + 1).toString());
//}

//// Check if this is the first tab and set the tab count
//if (localStorage.getItem(tabCountKey) === null) {
//    localStorage.setItem(tabCountKey, '1');
//} else {
//    incrementTabCount();
//}

//window.addEventListener('beforeunload', function () {
//    decrementTabCount();
//    // If all tabs are closed (tabCount becomes 0), clear the localStorage
//    if (parseInt(localStorage.getItem(tabCountKey) || '0', 10) === 0) {
//        localStorage.clear();
//    }
//});