namespace LineBotMessage.Hub
{
    public interface IMessageHub
    {
        Task UpdListConnections(string message);
        Task UpdSelfIDConnections(string message);
        Task UpdLineBotInfoConnections(string token, string id);
    }
}
