/* Toolbar controll section */

document.querySelectorAll('.position-control').forEach(e => {
    // console.log(e);
    e.addEventListener('click', function (button) {
        console.log(e.id);
        moveToolbar(e.id);
    });
});


document.querySelectorAll('.main-btn').forEach(e => {
    e.addEventListener('mouseover', function (button) {
        let ctn = e.nextElementSibling;
        ctn.classList.add('show-tool');
        ctn.addEventListener('mouseover', function () {
            ctn.classList.add('show-tool');
            // console.log('mouse on sub section');
        });

    });
    e.addEventListener('mouseout', function (button) {
        let ctn = e.nextElementSibling;
        ctn.classList.remove('show-tool');
    });
});
document.querySelectorAll('.tool-options').forEach(e => {
    e.addEventListener('mouseout', function (button) {
        let ctn = e;
        ctn.classList.remove('show-tool');
    });
});


document.querySelectorAll('.tool-btn').forEach(e => {
    // console.log(e);
    e.addEventListener('mouseover', function () {
        e.firstElementChild.style.fontSize = '22px';
        e.firstElementChild.style.transition = 'all 0.3s ease-out';
        // e.firstElementChild.style.color = 'cyan';
    });
    e.addEventListener('mouseout', function () {
        e.firstElementChild.style.fontSize = '16px';
        // e.firstElementChild.style.color = 'black';

    });
});


let lineWidth = document.getElementById('line-width');
let lineValue = document.getElementById('line-value');
lineValue.innerHTML = lineWidth.value;
lineValue.value = lineWidth.value

lineWidth.addEventListener('input', function () {
    lineValue.value = lineWidth.value
    lineValue.innerHTML = lineWidth.value;
});

let eraserWidth = document.getElementById('eraser-width');
let eraserValue = document.getElementById('eraser-value');
eraserValue.value = eraserWidth.value
eraserValue.innerHTML = eraserWidth.value;

eraserWidth.addEventListener('input', function () {
    eraserValue.value = eraserWidth.value
    eraserValue.innerHTML = eraserWidth.value;
});


function removeClass(element, clist) {
    clist.forEach(c => {
        element.classList.remove(c);
    });
}

function addClass(element, c) {
    element.classList.add(c);
}

function moveToolbar(buttonId) {
    let toolbar = document.getElementById('toolbar');
    let toolOptions = document.getElementsByClassName('tool-options');
    let drawingArea = document.getElementById('drawing-area');

    removeClass(toolbar, ['toolbar__left', 'toolbar__right', 'toolbar__top', 'toolbar__bottom']);

    let toolbarClass = '', toolOptionsClass = '', drawingAreaClass = '';

    if (buttonId == 'left') {
        toolbarClass = 'toolbar__left';
        toolOptionsClass = 'tool-options__left';
        drawingAreaClass = 'drawing-area-left';
    }
    else if (buttonId == 'right') {
        toolbarClass = 'toolbar__right';
        toolOptionsClass = 'tool-options__right';
        drawingAreaClass = 'drawing-area-right';
    }
    else if (buttonId === 'top') {
        toolbarClass = 'toolbar__top';
        toolOptionsClass = 'tool-options__top';
        drawingAreaClass = 'drawing-area-top';
    }
    else {
        toolbarClass = 'toolbar__bottom';
        toolOptionsClass = 'tool-options__bottom';
        drawingAreaClass = 'drawing-area-bottom';

    }

    removeClass(drawingArea, ['drawing-area-left', 'drawing-area-right', 'drawing-area-top', 'drawing-area-bottom'])
    addClass(drawingArea, drawingAreaClass);

    addClass(toolbar, toolbarClass);

    for (let i = 0; i < toolOptions.length; i++) {
        removeClass(toolOptions[i], ['tool-options__left', 'tool-options__right', 'tool-options__top', 'tool-options__bottom']);
        addClass(toolOptions[i], toolOptionsClass);
    }
}


/*Tool Selection*/
function markItemSelected(e, selector) {
    document.querySelectorAll(selector).forEach(btn => {
        btn.classList.remove('tool-selected');
    })
    e.classList.add('tool-selected');
}

document.querySelectorAll('.d-tools').forEach(e => {
    e.addEventListener('click', function () {
        markItemSelected(e, '.d-tools');
    });
});





// change classes array, common selector, main selector
function optionChanger(changeClasses, optionSelector, primaryOptionSelector) {
    let options = document.querySelectorAll(optionSelector);
    let primaryOptionFirstChild = document.querySelector(primaryOptionSelector).firstElementChild;

    options.forEach(e => {
        e.addEventListener('click', function () {
            let currentClass = Array.from(primaryOptionFirstChild.classList).find(c => changeClasses.includes(c));
            let newClass = Array.from(e.firstElementChild.classList).find(c => changeClasses.includes(c));
            if (currentClass && newClass) {
                primaryOptionFirstChild.classList.replace(currentClass, newClass);
            }
        });
    });
}


let colorClasses = ['icon-black', 'icon-red', 'icon-blue', 'icon-green'];
optionChanger(colorClasses, '.color', '#main-color');

let shapeClasses = ['bi-triangle', 'bi-circle', 'bi-square', 'bi-pentagon'];
optionChanger(shapeClasses, '.shape', '#main-shape');










