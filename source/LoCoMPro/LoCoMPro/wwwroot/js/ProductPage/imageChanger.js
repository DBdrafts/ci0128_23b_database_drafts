
// Change the main imagen for every small image in the page
const smallImages = document.querySelectorAll('.small-image');
const imageContainer = document.querySelector('.img-carousel-pop-up');
const mainImage = document.getElementById('mainImage');

// Simple changer in the page of product
smallImages.forEach(smallImage => {
    smallImage.addEventListener('mouseover', () => {
        mainImage.src = smallImage.src;
    });
});

// Add an event listener to the image container using event delegation
imageContainer.addEventListener('mouseover', (event) => {
    if (event.target.classList.contains('small-image')) {
        mainImage.src = event.target.src;
    }
});