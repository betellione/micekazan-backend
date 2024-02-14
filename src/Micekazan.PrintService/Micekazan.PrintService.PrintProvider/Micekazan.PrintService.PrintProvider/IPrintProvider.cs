namespace Micekazan.PrintService.PrintProvider;

public interface IPrintProvider
{
    Task PrintDocument(Stream document, PrintSettings? settings);
}