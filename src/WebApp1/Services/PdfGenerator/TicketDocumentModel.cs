namespace WebApp1.Services.PdfGenerator;

public sealed class TicketDocumentModel : IDisposable, IAsyncDisposable
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string FontColor { get; set; } = null!;
    public string? BackgroundPath { get; set; }
    public string? LogoPath { get; set; }
    public Stream? QrStream { get; set; }
    public bool IsHorizontal { get; set; }

    public void Dispose()
    {
        QrStream?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (QrStream != null) await QrStream.DisposeAsync();
    }
}