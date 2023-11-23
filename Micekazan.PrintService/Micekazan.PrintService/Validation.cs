namespace Micekazan.PrintService;

public static class Validation
{
    public static bool ValidateToken(string token)
    {
        if (token.Length is < 32 or > 256) return false;

        foreach (var c in token)
        {
            if (!char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_') return false;
        }

        return true;
    }
}