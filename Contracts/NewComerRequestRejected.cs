
namespace Contracts
{
    public interface NewComerRequestRejected: NewComerApprovalRequest
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserEmail { get; }
        string State { get; }
    }
}
