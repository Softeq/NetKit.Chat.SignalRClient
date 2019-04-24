// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Request
{
    public class CreateChannelRequest : BaseRequest
    {
        public string Name { get; set; }
        public ChannelType Type { get; set; }
        public string Topic { get; set; }
        public bool Closed { get; set; }
        public string WelcomeMessage { get; set; }
        public string PhotoUrl { get; set; }
    }
}
