namespace VideoGameApi.Models
{
    public class PurchaseRequest
    {
        public Guid UserId { get; set; }
        public int VideoGameId { get; set; }
        public int Quantity { get; set; } 
    }
}
