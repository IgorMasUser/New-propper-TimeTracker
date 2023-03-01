
namespace Contracts
{
    public interface ISimpleRequest
    {
        DateTime Timestamp { get; }
        string SentMessage { get; }
    }
}
