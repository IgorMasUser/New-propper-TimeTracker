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
            Event(() => NewComerRequestApproved, x => x.CorrelateById(m => m.Message.ApprovalId));
            Event(() => NewComerRequestRejected, x => x.CorrelateById(m => m.Message.ApprovalId));

            InstanceState(x => x.CurrentState);
            Initially(
                When(NewComerApprovalRequested)
                .Then(context =>
                {
                    context.Instance.SubmitDate = context.Data.TimeStamp;
                    context.Instance.UserEmail = context.Data.UserEmail;
                    context.Instance.Updated = DateTime.UtcNow;
                })
                .TransitionTo(RequestedForApproval));

            During(RequestedForApproval, Ignore(NewComerApprovalRequested));

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
                    context.Instance.UserEmail ??= context.Data.UserEmail;
                }));

            During(RequestedForApproval,
                When(NewComerRequestApproved)
                .TransitionTo(RequestApproved)
                .RespondAsync(x => x.Init<NewComerRequestApproved>(new
                {
                    ApprovalId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState,
                    UserEmail = x.Instance.UserEmail
                })));

            During(RequestedForApproval,
                When(NewComerRequestRejected)
                .TransitionTo(RequestRejected)
                .RespondAsync(x => x.Init<NewComerRequestRejected>(new
                {
                    ApprovalId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState,
                    UserEmail = x.Instance.UserEmail
                })));
        }

        public State RequestedForApproval { get; private set; }
        public State RequestApproved { get; private set; }
        public State RequestRejected { get; private set; }

        public Event<NewComerApprovalRequested> NewComerApprovalRequested { get; private set; }
        public Event<CheckApprovalStatus> ApprovalStatusRequested { get; private set; }
        public Event<NewComerRequestApproved> NewComerRequestApproved { get; private set; }
        public Event<NewComerRequestRejected> NewComerRequestRejected { get; private set; }
    }
}
