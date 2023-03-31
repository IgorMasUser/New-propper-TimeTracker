
namespace Contracts
{
    public interface NewComerRequestApproved
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserEmail { get; }
        string State { get; }
    }
}
