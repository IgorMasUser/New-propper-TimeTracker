
namespace Contracts
{
    public interface NewComerRequestApproved: NewComerApprovalRequest
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserEmail { get; }
        string State { get; }
    }
}
