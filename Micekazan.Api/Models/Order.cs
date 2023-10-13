namespace Micekazan.Api.Models;

public class Order
{
    public long Id { get; set; }
    public long ForeignId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPayed { get; set; }
    public DateTime PayedAt { get; set; }

}