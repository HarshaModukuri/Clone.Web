// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // Navbar toggle
    const navButton = document.querySelector('[data-bs-toggle="collapse"]');
    if (navButton) {
        const targetId = navButton.getAttribute('data-bs-target');
        const targetElement = document.querySelector(targetId);

        if (targetElement) {
            navButton.addEventListener('click', function () {
                targetElement.classList.toggle('hidden');
            });
        }
    }
});