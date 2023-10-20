let pageSize = 5;
let queryResults = Array.from(document.querySelectorAll('.result-block'));
let resultBlocks = Array.from(document.querySelectorAll('.result-block'));
let filteredResultBlocks = null;
let pageButtonsContainer = document.getElementById("pageButtonsContainer");
let previousButton = document.getElementById('pagination-button-left');
let nextButton = document.getElementById('pagination-button-right');
let total = document.getElementById('total');

let totalPages = Math.ceil(resultBlocks.length / pageSize);
let queryTotalResults = resultBlocks.length;
let currentPage = 1;

let sortOrder = "";
let field = "";

function getOppositeOrder(order) {
    return order === "desc" ? "asc" : "desc";
}
document.addEventListener("DOMContentLoaded", function () {
    // Sort the result blocks based on the selected sorting criteria
    window.sortResultBlocks = function (order, field = "#result-product-name") {
        // Implement your sorting logic here
        // Example: Sort by price
        resultBlocks.sort((a, b) => {
            const valueA = parseFloat(a.querySelector(field).getAttribute("value"));
            const valueB = parseFloat(b.querySelector(field).getAttribute("value"));
            return order === 'desc' ? valueA - valueB : valueB - valueA;
        });

        const parentContainer = resultBlocks[0].parentElement;
        resultBlocks.forEach(block => {
            parentContainer.appendChild(block); // Reorder the elements in the parent container
        });

    }

    // Show current results page
    window.showPage = function (page) {
        const startIndex = (page - 1) * pageSize;
        const endIndex = startIndex + pageSize;
        resultBlocks.forEach((block, index) => {
            block.style.display = index >= startIndex && index < endIndex ? 'block' : 'none';
        });

        total.textContent = `Página ${currentPage} de ${totalPages}`;

    }

    // Hides left and right navigation buttons.
    window.updateNavigationButtons = function () {
        previousButton.hidden = currentPage === 1;
        nextButton.hidden = currentPage === totalPages;

        pageButtonsContainer.innerHTML = ""; // Clear the container

        for (let i = Math.max(1, currentPage - 1); i <= Math.min(totalPages, currentPage + 3); i++) {
            pageButtonsContainer.appendChild(createPageButton(i));
        }
    }

    // Initialize pagination and sorting (initially unsorted)
    function initialize() {
        sortResultBlocks('unsorted'); // Initialize with an unsorted order
        totalPages = Math.ceil(resultBlocks.length / pageSize);
        updateNavigationButtons();
        showPage(currentPage);
    }

    // Function to create a page button
    function createPageButton(index) {
        const button = document.createElement("button");
        button.className = index === currentPage ? "pagination-square-active" : "";
        button.innerText = index;
        button.addEventListener("click", function () {
            currentPage = index;
            showPage(currentPage);
            updateNavigationButtons();
        });
        return button;
    }

    // Next Page Button
    nextButton.addEventListener('click', () => {
        if (currentPage < totalPages) {
            currentPage++;
            showPage(currentPage);
            updateNavigationButtons();
        }
    });

    // Previous Page Button
    previousButton.addEventListener('click', () => {
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
            updateNavigationButtons();
        }
    });

    // Sort buttons (e.g., "Menores precios" and "Mayores precios")
    const sortButtons = document.querySelectorAll(".sort-button");
    sortButtons.forEach((button) => {
        button.addEventListener('click', () => {

            // Reset all other buttons
            sortButtons.forEach((btn) => {
                if (btn !== button) {
                    btn.removeAttribute("data-sort-order");
                    btn.classList.remove("active");
                }
            });

            // Toggle "up" and "down" classes for the arrow image
            const arrow = button.querySelector(".arrow");
            if (button.hasAttribute("data-sort-order")) {
                sortOrder = getOppositeOrder(button.getAttribute("data-sort-order"));
                arrow.classList.toggle("up", sortOrder === 'asc');
                arrow.classList.toggle("down", sortOrder === 'desc');
            } else {
                arrow.classList.remove("up");
                arrow.classList.add("down");
            }

            // Update the sorting order
            sortOrder = getOppositeOrder(button.getAttribute("data-sort-order"));
            field = button.getAttribute("target");
            sortResultBlocks(sortOrder, field);
            totalPages = Math.ceil(resultBlocks.length / pageSize);
            currentPage = 1;
            showPage(currentPage, sortOrder);
            updateNavigationButtons();
            button.setAttribute("data-sort-order", sortOrder);
            button.classList.add("active");
        });
    });

    // Initialize pagination
    initialize();
});