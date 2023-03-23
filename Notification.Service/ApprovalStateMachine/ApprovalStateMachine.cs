using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Service.ApprovalStateMachine
{
    public class ApprovalStateMachine : MassTransitStateMachine<ApprovalState>
    {
        public ApprovalStateMachine()
        {
            Event(() => NewComerSentForApproval, x => x.CorrelateById(m => m.Message.ApprovalId));

            InstanceState(x => x.CurrentState);
            Initially(
                When(NewComerSentForApproval)
                .TransitionTo(SentForApprovalState));
        }

        public State SentForApprovalState { get; private set; }

        public Event<INewComerApproval> NewComerSentForApproval { get; private set; }
    }

    public class ApprovalState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        public string UserId { get; set; }
        public int Version { get; set; }
        public DateTime? SubmitDate { get; internal set; }
        public DateTime? Updated { get; set; }
    }

}
