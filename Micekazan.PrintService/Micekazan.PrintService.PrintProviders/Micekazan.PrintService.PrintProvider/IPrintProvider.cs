namespace Micekazan.PrintService.PrintProvider;

public interface IPrintProvider
{
    public Task PrintDocument(FileStream document, PrintSettings? settings);
}