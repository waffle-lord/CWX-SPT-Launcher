// document.addEventListener('wheel', function (event) {
//     if (event.ctrlKey === true) {
//         event.preventDefault();
//     }
// }, { passive: false });

function stopScroll() {
    document.getElementById("main").classList.toggle("stopscroll");
}