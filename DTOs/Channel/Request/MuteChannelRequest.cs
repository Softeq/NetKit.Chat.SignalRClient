// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Request
{
    public class MuteChannelRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }
        public bool IsMuted { get; set; }
    }
}