using QRCoder;

namespace WebApp1.Services.QrCodeGenerator;

public class QrCodeGenerator : IQrCodeGenerator
{
    public Stream GenerateQrCode(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(16, false);
        var stream = new MemoryStream(qrCodeImage);

        return stream;
    }
}