namespace Notification.Service.ApprovalStateMachine
{
    public interface INewComerApproval
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserId { get; }
    }
}