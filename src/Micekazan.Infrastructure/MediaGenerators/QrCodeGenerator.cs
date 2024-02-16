using QRCoder;

namespace Micekazan.Infrastructure.MediaGenerators;

public static class QrCodeGenerator
{
    public static Stream GenerateQrCode(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(16, false);
        var stream = new MemoryStream(qrCodeImage);

        return stream;
    }
}