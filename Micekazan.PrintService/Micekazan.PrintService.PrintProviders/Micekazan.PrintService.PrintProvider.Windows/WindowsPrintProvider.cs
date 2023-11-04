namespace Micekazan.PrintService.PrintProvider.Windows;

public class WindowsPrintProvider : IPrintProvider
{
    public Task PrintDocument(FileStream document, PrintSettings? settings)
    {
        throw new NotImplementedException();
        
    }
}