
namespace Contracts
{
    public class NewComerApprovalRequestAccepted
    {
       public Guid ApprovalId { get; set; }
       public DateTime Timestamp { get; set; }
       public string UserEmail { get; set; }
    }
}
