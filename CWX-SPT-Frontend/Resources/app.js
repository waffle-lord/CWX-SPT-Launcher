// window.startDragging = function (startX, startY) {
//
//     function onMouseMove(event) {
//         const offsetX = event.clientX - startX;
//         const offsetY = event.clientY - startY;
//        
//         DotNet.invokeMethodAsync('CWX-SPT-Frontend', 'MoveWindow', offsetX, offsetY);
//     }
//
//     function onMouseUp() {
//         document.removeEventListener('mousemove', onMouseMove);
//         document.removeEventListener('mouseup', onMouseUp);
//     }
//
//     document.addEventListener('mousemove', onMouseMove);
//     document.addEventListener('mouseup', onMouseUp);
//    
// };