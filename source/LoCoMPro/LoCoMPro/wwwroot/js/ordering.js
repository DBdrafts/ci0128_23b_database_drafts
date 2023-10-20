document.addEventListener("DOMContentLoaded", function () {
    const resultBlocksContainer = document.querySelector(".result-section");
    const resultBlocks = Array.from(document.querySelectorAll('.result-block'));

    document.getElementById('sort-lowest-price').addEventListener('click', function () {
        // Sort the items by lowest price
        resultBlocks.sort((a, b) => {
            const priceA = parseFloat(a.querySelector("#result-price").getAttribute("value"));
            const priceB = parseFloat(b.querySelector("#result-price").getAttribute("value"));
            return priceA - priceB;
        });

        // Reorder the result blocks in the container
        resultBlocks.forEach(block => resultBlocksContainer.appendChild(block));
    });

    document.getElementById('sort-highest-price').addEventListener('click', function () {
        // Sort the items by highest price
        resultBlocks.sort((a, b) => {
            const priceA = parseFloat(a.querySelector("#result-price").getAttribute("value"));
            const priceB = parseFloat(b.querySelector("#result-price").getAttribute("value"));
            return priceB - priceA;
        });

        // Reorder the result blocks in the container
        resultBlocks.forEach(block => resultBlocksContainer.appendChild(block));
    });
});
