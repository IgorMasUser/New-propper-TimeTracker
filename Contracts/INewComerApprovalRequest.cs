﻿
namespace Contracts
{
    public interface NewComerApprovalRequest
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserEmail { get; }
        string reasonOfRejection { get; }
    }
}
