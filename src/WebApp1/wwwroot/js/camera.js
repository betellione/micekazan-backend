function startCamera() {
    const videoElement = document.getElementById('videoElement');
    const resultElement = document.getElementById('result');

    navigator.mediaDevices.getUserMedia({video: {facingMode: 'user'}})
        .then(function (mediaStream) {
            videoElement.srcObject = mediaStream;

            const canvasElement = document.createElement('canvas');
            const canvasContext = canvasElement.getContext('2d');

            setInterval(function () {
                canvasElement.width = videoElement.videoWidth;
                canvasElement.height = videoElement.videoHeight;
                canvasContext.drawImage(videoElement, 0, 0, canvasElement.width, canvasElement.height);

                const imageData = canvasContext.getImageData(0, 0, canvasElement.width, canvasElement.height);
                const code = jsQR(imageData.data, imageData.width, imageData.height);

                if (code) {
                    resultElement.innerHTML = 'QR-код: ' + code.data;
                } else {
                    Quagga.decodeSingle({
                        decoder: {
                            readers: ['ean_reader', 'ean_8_reader', 'code_128_reader', 'code_39_reader', 'code_93_reader'],
                        },
                        locate: true,
                        src: canvasElement.toDataURL(),
                    }, function (result) {
                        if (result && result.codeResult) {
                            resultElement.innerHTML = 'Штрих-код: ' + result.codeResult.code;
                        }
                    });
                }
            }, 1000);
        })
        .catch(function (error) {
            console.error('Ошибка при получении доступа к камере:', error);
        });
}

startCamera();