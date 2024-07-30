let undoStack = [];
let redoStack = [];

let isUndoRedoOperation = false;

function saveState() {
    if (isUndoRedoOperation) return;
    redoStack = [];
    undoStack.push(JSON.stringify(canvas));
}

function undo() {
    if (undoStack.length > 0) {
        isUndoRedoOperation = true;
        redoStack.push(JSON.stringify(undoStack.pop()));
        let state = "";
        if (undoStack.length > 0) {
            state = JSON.parse(undoStack[undoStack.length - 1]);
        }
        else {
            state = JSON.stringify('');
        }
        console.log('undo ',undoStack.length);
        canvas.loadFromJSON(state, () => {
            canvas.renderAll();
            isUndoRedoOperation = false;
        });
    }
}

function redo() {
    if (redoStack.length > 0) {
        isUndoRedoOperation = true;

        undoStack.push(JSON.stringify(canvas));

        let state = JSON.parse(redoStack.pop());

        canvas.loadFromJSON(state, () => {
            canvas.renderAll();
            isUndoRedoOperation = false;
        });
    }
}

function handleEvent() {
    if (isUndoRedoOperation) return;
    setTimeout(saveState, 200);
}

canvas.on('object:added', handleEvent);
canvas.on('object:removed', handleEvent);
canvas.on('object:modified', handleEvent);
