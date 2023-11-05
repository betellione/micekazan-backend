namespace Micekazan.PrintService.PrintProvider;

public interface IPrintProvider
{
    public Task PrintDocument(Stream document, PrintSettings? settings);
}