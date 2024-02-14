namespace WebApp1.Services.QrCodeGenerator;

public interface IQrCodeGenerator
{
    Stream GenerateQrCode(string data);
}