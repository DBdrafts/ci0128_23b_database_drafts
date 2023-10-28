let pageSize = 5;
let queryResults = Array.from(document.querySelectorAll('.register-block-pagination'));
let resultBlocks = Array.from(document.querySelectorAll('.register-block-pagination'));
let filteredResultBlocks = null;
let pageButtonsContainer = document.getElementById("pageButtonsContainer");
let previousButton = document.getElementById('pagination-button-left');
let nextButton = document.getElementById('pagination-button-right');
let firstButton = document.getElementById('pagination-button-first');
let lastButton = document.getElementById('pagination-button-last');
let total = document.getElementById('total');

let totalPages = Math.ceil(resultBlocks.length / pageSize);
let queryTotalResults = resultBlocks.length;
let currentPage = 1;

let activeButton = null;

function getOppositeOrder(order) {
    return order === "desc" ? "asc" : "desc";
}
document.addEventListener("DOMContentLoaded", function () {
    // Sort the result blocks based on the selected sorting criteria
    window.sortResultBlocks = function (order, field = "#register-price") {
        // Implement your sorting logic here
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

        // Range of results of the current page
        const firstResultCurrentPage = (currentPage - 1) * pageSize + 1;
        const lastResultCurrentPage = Math.min(currentPage * pageSize, resultBlocks.length);


        let resultsRangeMsg = `${firstResultCurrentPage}-${lastResultCurrentPage} de ${resultBlocks.length} resultados encontrados`;

        if (firstResultCurrentPage === resultBlocks.length) {
            resultsRangeMsg = `Resultado ${lastResultCurrentPage} de ${resultBlocks.length} encontrados`
        }

        document.getElementById("results-count").textContent = resultsRangeMsg;
        total.textContent = `Página ${currentPage} de ${totalPages}`;
    }
 

    // Hides left and right navigation buttons.
    window.updateNavigationButtons = function () {
        firstButton.hidden = previousButton.hidden = currentPage === 1;
        lastButton.hidden = nextButton.hidden = currentPage === totalPages;

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

    // First Page Button
    firstButton.addEventListener('click', () => {
        currentPage = 1;
        showPage(currentPage);
        updateNavigationButtons();
    });

    // Last Page Button
    lastButton.addEventListener('click', () => {
        currentPage = totalPages;
        showPage(currentPage);
        updateNavigationButtons();
    });

    // Sort buttons (e.g., "Menores precios" and "Mayores precios")
    const sortButtons = document.querySelectorAll(".sort-button");

    sortButtons.forEach((button) => {
        button.addEventListener('click', () => {

            // Hides all the images of the order
            sortButtons.forEach((btn) => {
                const images = btn.querySelectorAll("img");
                images.forEach((image) => {
                    image.classList.add("hidden-image");
                });
            });

            const sortOrder = getOppositeOrder(button.getAttribute("data-sort-order"));
            const field = button.getAttribute("target");
            sortResultBlocks(sortOrder, field);
            totalPages = Math.ceil(resultBlocks.length / pageSize);
            currentPage = 1;
            showPage(currentPage, sortOrder);
            updateNavigationButtons();
            button.setAttribute("data-sort-order", sortOrder);

            // Show the order button image
            const currentImage = button.querySelector("." + sortOrder);
            if (currentImage) {
                currentImage.classList.remove("hidden-image");
            }
        });
    });

    // Initialize pagination
    initialize();
});