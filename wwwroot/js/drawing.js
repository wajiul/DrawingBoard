
const canvas = new fabric.Canvas('canvas', {
    width: window.innerWidth,
    height: window.innerHeight,
    backgroundColor: '#fff'
});

canvas.isDrawingMode = true;
canvas.freeDrawingBrush.width = 3;

document.body.style.overflow = 'hidden';

window.addEventListener('resize', () => {
    canvas.setWidth(window.innerWidth);
    canvas.setHeight(window.innerHeight);
    canvas.renderAll();
});


let pointerMode = false,
    paintingMode = true,
    eraserMode = false,
    currentColor = 'black',
    currentShape = 'rectangle';


var pen = document.getElementById('main-pen'),
    line = document.getElementById('line'),
    eraser = document.getElementById('eraser'),
    shape = document.getElementById('main-shape'),
    currentLineWidth = document.getElementById('line-width'),
    currentEraserWidth = document.getElementById('eraser-width'),
    allShapes = document.querySelectorAll('.shape'),
    color = document.getElementById('main-color'),
    selectionPointer = document.getElementById('selectionPointer');




function initializeObserver(elementSelector, onfig = { attributes: true }, callback) {
    const element = document.querySelector(elementSelector);

    if (!element) {
        console.error('Element not found:', elementSelector);
        return;
    }

    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.type === 'attributes' && config.attributeFilter.includes(mutation.attributeName)) {
                callback(mutation.target, mutation.attributeName);
            }
        });
    });
    observer.observe(element, config);
}

const config = {
    attributes: true,
    attributeFilter: ['class', 'value']
};

initializeObserver('#main-color > *', config, (target, attributeName) => {
    let className = target.getAttribute(attributeName);
    if (className.includes('black'))
        currentColor = 'black';
    else if (className.includes('red'))
        currentColor = 'red';
    else if (className.includes('blue'))
        currentColor = 'blue';
    else if (className.includes('green'))
        currentColor = 'green';

    canvas.freeDrawingBrush.color = currentColor;
});


initializeObserver('#main-shape > *', config, (target, attributeName) => {
    let shapeClass = target.getAttribute(attributeName);
    if (shapeClass.includes('triangle')) {
        currentShape = 'triangle';
    }
    else if (shapeClass.includes('circle')) {
        currentShape = 'circle';
    }
    else if (shapeClass.includes('square')) {
        currentShape = 'rectangle';
    }
    else if (shapeClass.includes('pentagon')) {
        currentShape = 'pentagon';
    }
    console.log(currentShape);
    drawShape(currentShape);
});


pen.addEventListener('click', function () {
    canvas.freeDrawingBrush = new fabric.PencilBrush(canvas);
    canvas.freeDrawingBrush.width = Math.floor(currentLineWidth.value);
    canvas.freeDrawingBrush.color = currentColor;
    canvas.isDrawingMode = true;
});


currentLineWidth.addEventListener('input', function () {
    canvas.freeDrawingBrush.width = Math.floor(currentLineWidth.value);
    canvas.isDrawingMode = true;
});


eraser.addEventListener('click', function () {
    canvas.freeDrawingBrush = new fabric.EraserBrush(canvas);
    canvas.freeDrawingBrush.width = currentEraserWidth.value * 5;
    canvas.isDrawingMode = true;
});

currentEraserWidth.addEventListener('input', function () {
    canvas.freeDrawingBrush = new fabric.EraserBrush(canvas);
    canvas.isDrawingMode = true;
    canvas.freeDrawingBrush.width = currentEraserWidth.value * 5;
});


shape.addEventListener('click', function () {
    canvas.isDrawingMode = false;
    drawShape(currentShape);
});

function drawShape(s) {
    canvas.isDrawingMode = false;
    if (s === 'circle') {
        drawCircle();
    }
    else if (s === 'rectangle') {
        drawRectangle();
    }
    else if (s == 'triangle') {
        drawTriangle();
    }
    else if (s === 'pentagon') {
        drawPentagon();
    }
}



function drawPentagon() {
    const pentagonPoints = [
        { x: 50, y: 0 },
        { x: 100, y: 38 },
        { x: 82, y: 100 },
        { x: 18, y: 100 },
        { x: 0, y: 38 }
    ];
    const pentagon = new fabric.Polygon(pentagonPoints, {
        left: 205,
        top: 115,
        fill: '',
        stroke: currentColor,
        strokeWidth: 2
    });
    canvas.add(pentagon);
}

function drawTriangle() {
    const triangle = new fabric.Triangle({
        width: 100,
        height: 100,
        fill: '',
        stroke: currentColor,
        strokeWidth: 2,
        left: 190,
        top: 100
    });
    canvas.add(triangle);
}

function drawCircle() {
    var circle = new fabric.Circle({
        left: 195,
        top: 105,
        radius: 45,
        fill: '',
        stroke: currentColor,
        strokeWidth: 2
    });
    canvas.add(circle);
}

function drawRectangle() {
    var rect = new fabric.Rect({
        left: 200,
        top: 110,
        width: 150,
        height: 80,
        fill: '',
        stroke: currentColor,
        strokeWidth: 2
    });
    canvas.add(rect);
}

selectionPointer.addEventListener('click', function () {
    canvas.isDrawingMode = false;
    console.log('selection');
});

/* Object change section */
const changeMenu = document.getElementById('changeMenu');

function showChangeMenu(e) {
    const activeObject = canvas.getActiveObject();
    const boundingRect = activeObject.getBoundingRect();
    if (activeObject) {
        changeMenu.style.left = `${boundingRect.left - 10}px`;
        changeMenu.style.top = `${boundingRect.top - 40}px`;
        changeMenu.style.display = 'block';
    } else {
        menu.style.display = 'none';
    }
}

canvas.on('selection:created', showChangeMenu);
canvas.on('selection:updated', showChangeMenu);
canvas.on('object:modified', showChangeMenu);
canvas.on('selection:cleared', () => {
    changeMenu.style.display = 'none';
});
canvas.on('object:moving', () => {
    changeMenu.style.display = 'none';
});

let objectRemove = document.getElementById('objectRemove'),
    objectOpacity = document.getElementById('objectOpacity'),
    objectLineWidth = document.getElementById('objectLineWidth'),
    objectColor = document.getElementById('objectColor');

objectRemove.addEventListener('click', function () {
    const activeObjects = canvas.getActiveObjects();

    if (activeObjects.length) {
        activeObjects.forEach(function (object) {
            canvas.remove(object);
        });
        canvas.discardActiveObject();
        canvas.renderAll();
        changeMenu.style.display = 'none';
    }
});

objectOpacity.addEventListener('input', function () {
    const activeObjects = canvas.getActiveObjects();
    if (activeObjects.length) {
        activeObjects.forEach(function (object) {
            object.set('opacity', parseFloat(objectOpacity.value));
        });
        canvas.renderAll();
    }
});

objectLineWidth.addEventListener('input', function () {
    const activeObjects = canvas.getActiveObjects();
    if (activeObjects.length) {
        activeObjects.forEach(function (object) {
            object.set('strokeWidth', parseInt(objectLineWidth.value, 10));
        });
        canvas.renderAll();
    }
});

objectColor.addEventListener('input', function () {
    const activeObjects = canvas.getActiveObjects();
    if (activeObjects.length) {
        activeObjects.forEach(function (object) {
            object.set('stroke', objectColor.value);
            if (object.text != null) {
                object.set('fill', objectColor.value);
            }
        });
        canvas.renderAll();
    }
});

let isTextModeEnabled = false;
document.getElementById("enableText").addEventListener("click", function () {
    isTextModeEnabled = true;
});

canvas.on("mouse:down", function (event) {
    if (isTextModeEnabled) {
        const pointer = canvas.getPointer(event.e);
        const text = new fabric.Textbox("Text Here", {
            left: pointer.x,
            top: pointer.y,
            width: 150,
            fontSize: 18,
            fill: currentColor
        });
        canvas.add(text).setActiveObject(text);
        isTextModeEnabled = false;
        canvas.isDrawingMode = false;
    }
});


let isImageModeEnabled = false;
const enableImageButton = document.getElementById("enableImage");
const imageInput = document.getElementById("imageInput");
enableImageButton.addEventListener("click", function () {
    isImageModeEnabled = true;
    imageInput.click();
});

imageInput.addEventListener("change", function (event) {
    const file = event.target.files[0];
    if (file && isImageModeEnabled) {
        const reader = new FileReader();
        reader.onload = function (e) {
            fabric.Image.fromURL(e.target.result, function (img) {
                img.set({
                    left: 100,
                    top: 100
                });
                canvas.add(img);
            });
        };
        reader.readAsDataURL(file);
    }
    isImageModeEnabled = false;
});








