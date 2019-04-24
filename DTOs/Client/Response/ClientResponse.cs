// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalRClient.DTOs.Client.Response
{
    public class ClientResponse
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string ConnectionClientId { get; set; }
        public string UserName { get; set; }
    }
}
