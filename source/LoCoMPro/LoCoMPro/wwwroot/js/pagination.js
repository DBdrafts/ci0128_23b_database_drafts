function getOppositeOrder(order) {
    return order === "asc" ? "desc" : "asc";
}
document.addEventListener("DOMContentLoaded", function () {
    const pageSize = 5;
    const resultBlocks = Array.from(document.querySelectorAll('.result-block'));
    const pageButtonsContainer = document.getElementById("pageButtonsContainer");
    const previousButton = document.getElementById('pagination-button-left');
    const nextButton = document.getElementById('pagination-button-right');
    const total = document.getElementById('total');

    let totalPages = Math.ceil(resultBlocks.length / pageSize);
    let currentPage = 1;

    // Sort the result blocks based on the selected sorting criteria
    function sortResultBlocks(sortOrder, field = "#result-price") {
        // Implement your sorting logic here
        // Example: Sort by price
        resultBlocks.sort((a, b) => {
            const priceA = parseFloat(a.querySelector(field).getAttribute("value"));
            const priceB = parseFloat(b.querySelector(field).getAttribute("value"));
            return sortOrder === 'asc' ? priceA - priceB : priceB - priceA;
        });
        const a = resultBlocks;
    }

    // Show current results page
    function showPage(page) {
        const startIndex = (page - 1) * pageSize;
        const endIndex = startIndex + pageSize;
        resultBlocks.forEach((block, index) => {
            block.style.display = index >= startIndex && index < endIndex ? 'block' : 'none';
        });
        total.textContent = `Página ${currentPage} de ${totalPages}`;

    }

    // Hides left and right navigation buttons.
    function updateNavigationButtons() {
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
            const sortOrder = getOppositeOrder(button.getAttribute("data-sort-order"));
            const field = button.getAttribute("target");
            sortResultBlocks(sortOrder, field);
            totalPages = Math.ceil(resultBlocks.length / pageSize);
            currentPage = 1;
            showPage(currentPage, sortOrder);
            updateNavigationButtons();
            button.setAttribute("data-sort-order", sortOrder);
        });
    });

    // Initialize pagination
    initialize();
});