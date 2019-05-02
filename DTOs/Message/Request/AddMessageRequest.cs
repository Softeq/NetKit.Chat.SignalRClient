// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalRClient.DTOs.Message.Request
{
    public class AddMessageRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }
        public MessageType Type { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
    }
}
