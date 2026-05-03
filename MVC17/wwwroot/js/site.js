
document.addEventListener('DOMContentLoaded', function () {
    const dropdown = document.querySelector('.category-dropdown');
    const toggle = document.querySelector('.category-dropdown-toggle');
    const menu = document.querySelector('.category-dropdown .dropdown-menu');

    if (dropdown && toggle && menu) {
        dropdown.addEventListener('mouseenter', function () {
            this.classList.add('show');
            menu.style.display = 'block';
        });

        dropdown.addEventListener('mouseleave', function () {
            this.classList.remove('show');
            menu.style.display = 'none';
        });
    }
});