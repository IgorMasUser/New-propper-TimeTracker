
namespace Contracts
{
    public interface INewComerApprovalRequest
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserId { get; }
    }
}
