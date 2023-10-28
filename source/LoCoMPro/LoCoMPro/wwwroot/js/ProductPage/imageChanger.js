
// JavaScript para cambiar la imagen principal al poner el mouse sobre las imágenes pequeñas
const smallImages = document.querySelectorAll('.small-image');
const mainImage = document.getElementById('mainImage');

smallImages.forEach(smallImage => {
    smallImage.addEventListener('mouseover', () => {
        mainImage.src = smallImage.src;
    });
});
