
document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('exportButton').addEventListener('click', function () {
        exportFile();
    });
});

function exportFile() {
    const canvas = document.getElementById('canvas');
    const exportType = document.querySelector('input[name="exportType"]:checked').value;

    if (exportType === 'jpeg') {
        const jpegDataURL = canvas.toDataURL('image/jpeg', 0.8);
        downloadFile(jpegDataURL, 'canvas.jpeg');
    }
    else if (exportType === 'png') {
        const pngDataURL = canvas.toDataURL('image/png');
        downloadFile(pngDataURL, 'canvas.png');
    }

    const fileExportModal = bootstrap.Modal.getInstance(document.getElementById('fileExportModal'));
    if (fileExportModal) {
        fileExportModal.hide();
    }
}

function downloadFile(dataURL, filename) {
    const link = document.createElement('a');
    link.href = dataURL;
    link.download = filename;
    link.click();
}

