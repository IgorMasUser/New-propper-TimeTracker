﻿
namespace Contracts
{
    public interface NewComerApprovalRequested
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserEmail { get; }
    }
}
