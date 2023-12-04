document.addEventListener('DOMContentLoaded', function () {
    var blockInformationElements = document.querySelectorAll('.report-list-block-information');

    blockInformationElements.forEach(function (blockInformation) {
        blockInformation.addEventListener('click', function () {
            var listItem = this.closest('.report-list-block');

            var hiddenSection = listItem.querySelector('.report-list-products-section');

            closeAllSectionsExcept(hiddenSection);

            hiddenSection.hidden = !hiddenSection.hidden;

            var icon = this.querySelector('.fa-pencil-square-o');
            if (!hiddenSection.hidden) {
                icon.style.color = "var(--locompro-yellow)";
            }
            else {
                icon.style.color = "var(--locompro-dark-gray)";
            }

        });
    });

    function closeAllSectionsExcept(exceptSection) {
        var allHiddenSections = document.querySelectorAll('.report-list-products-section');
        allHiddenSections.forEach(function (section) {
            if (section !== exceptSection) {
                section.hidden = true;

                var icon = section.previousElementSibling.querySelector('.fa-pencil-square-o');
                icon.style.color = "var(--locompro-dark-gray)";
            }
        });
    }
});