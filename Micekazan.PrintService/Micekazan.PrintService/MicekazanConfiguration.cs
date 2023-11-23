namespace Micekazan.PrintService;

public class MicekazanConfiguration
{
    public string Token { get; set; } = null!;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Token) || !Validation.ValidateToken(Token)) throw new Exception("Invalid configuration.");
    }
}