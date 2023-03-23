using MassTransit;
using Contracts;

namespace Notification.Service.StateMachines
{
    public class ApprovalStateMachine : MassTransitStateMachine<ApprovalState>
    {
        public ApprovalStateMachine()
        {
            Event(() => NewComerApprovalRequested, x => x.CorrelateById(m => m.Message.ApprovalId));
            Event(() => ApprovalStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.ApprovalId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                  {
                      if (context.RequestId.HasValue)
                      {
                          await context.RespondAsync<NewComerNotFound>(new { context.Message.ApprovalId });
                      }
                  }));
            });

            InstanceState(x => x.CurrentState);
            Initially(
                When(NewComerApprovalRequested)
                .Then(context =>
                {
                    context.Instance.SubmitDate = context.Data.TimeStamp;
                    context.Instance.UserId = context.Data.UserId;
                    context.Instance.Updated = DateTime.UtcNow;
                })
                .TransitionTo(Requested));

            During(Requested, Ignore(NewComerApprovalRequested));

            DuringAny(When(ApprovalStatusRequested)
                .RespondAsync(x => x.Init<ApprovalStatus>(new
                {
                    ApprovalId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState
                }))
            );
            
            DuringAny(
                When(NewComerApprovalRequested)
                .Then(context =>
                {
                    context.Instance.SubmitDate ??= context.Data.TimeStamp;
                    context.Instance.UserId ??= context.Data.UserId;

                }));
        }

        public State Requested { get; private set; }

        public Event<NewComerApprovalRequested> NewComerApprovalRequested { get; private set; }
        public Event<CheckApprovalStatus> ApprovalStatusRequested { get; private set; }
    }
}
