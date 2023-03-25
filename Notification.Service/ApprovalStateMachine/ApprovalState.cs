using MassTransit;

namespace Notification.Service.StateMachines
{
    public class ApprovalState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string UserId { get; set; }
        public int Version { get; set; }
        public DateTime? SubmitDate { get; internal set; }
        public DateTime? Updated { get; set; }
        public string ApprovalRejected { get; set; }
    }
}
