namespace Micekazan.PrintDispatcher.Domain.Contracts;

public class Update
{
    public long Id { get; set; }
    public UpdateKind Kind { get; set; }
    public Document? Document { get; set; }
}