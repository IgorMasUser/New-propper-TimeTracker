namespace Contracts
{
    public class NewComerApprovalRequestRejected
    {
        public Guid ApprovalId { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserEmail { get; set; }
        public string Reason { get; set; }
    }
}