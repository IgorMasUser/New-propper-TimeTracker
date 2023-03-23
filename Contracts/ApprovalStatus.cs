
namespace Contracts
{
    public interface ApprovalStatus
    {
        Guid ApprovalId { get; }
        string State { get; }
    }
}