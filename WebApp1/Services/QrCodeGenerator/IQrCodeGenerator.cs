namespace WebApp1.Services.QrCodeGenerator;

public interface IQrCodeGenerator
{
    public Stream GenerateQrCode(string data);
}