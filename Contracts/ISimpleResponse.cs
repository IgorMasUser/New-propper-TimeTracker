﻿
namespace Contracts
{
    public interface ISimpleResponse
    {
        Guid ApprovalId { get; }
        DateTime TimeStamp { get; }
        string UserId { get; }
        public string ResponseMessage { get; }
    }
}
