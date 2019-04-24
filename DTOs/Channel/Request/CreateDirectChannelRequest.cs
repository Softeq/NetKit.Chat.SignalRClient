// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Request
{
    public class CreateDirectChannelRequest : BaseRequest
    {
        public Guid MemberId { get; set; }
        public ChannelType Type { get; } = ChannelType.Direct;
    }
}