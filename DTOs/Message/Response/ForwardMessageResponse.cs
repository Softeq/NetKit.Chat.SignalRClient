// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Response;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Member.Response;

namespace Softeq.NetKit.Chat.SignalRClient.DTOs.Message
{
    public class ForwardMessageResponse
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public Guid ChannelId { get; set; }
        public Guid? OwnerId { get; set; }
        public ChannelSummaryResponse Channel { get; set; }
        public MemberSummaryResponse Owner { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}