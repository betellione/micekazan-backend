namespace Micekazan.PrintDispatcher.Domain.Contracts;

public class Document
{
    public long Id { get; set; }
    public string DocumentPath { get; set; } = null!;
    public string DocumentUri { get; set; } = null!;
}