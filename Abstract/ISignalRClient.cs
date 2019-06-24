// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels.Client;

namespace Softeq.NetKit.Chat.SignalRClient.Abstract
{
    public interface ISignalRClient
    {
        #region Events

        event Action AccessTokenExpired;
        event Action Disconnected;

        event Action<ChannelSummaryResponse> ChannelUpdated;
        event Action<Guid> ChannelClosed;

        event Action<MessageResponse> MessageAdded;
        event Action<Guid, Guid> MessageDeleted;
        event Action<MessageResponse> MessageUpdated;
        event Action<Guid> LastReadMessageUpdated;

        event Action<ChannelSummaryResponse> MemberJoined;
        event Action<Guid> MemberLeft;
        event Action<MemberSummaryResponse, Guid> MemberDeleted;

        #endregion

        Task<ClientResponse> ConnectAsync();
        Task DisconnectAsync();

        Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request);
        Task<ChannelSummaryResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request);
        Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request);
        Task PinChannelAsync(PinChannelRequest request);
        Task MuteChannelAsync(MuteChannelRequest request);
        Task CloseChannelAsync(ChannelRequest request);
        Task JoinToChannelAsync(ChannelRequest request);
        Task LeaveChannelAsync(ChannelRequest request);

        Task<MessageResponse> AddMessageAsync(AddMessageRequest request);
        Task DeleteMessageAsync(DeleteMessageRequest request);
        Task UpdateMessageAsync(UpdateMessageRequest request);
        Task MarkAsReadMessageAsync(SetLastReadMessageRequest request);

        Task<ClientResponse> GetClientAsync();
        Task InviteMemberAsync(InviteMemberRequest request);
        Task DeleteMemberAsync(DeleteMemberRequest request);
        Task InviteMultipleMembersAsync(InviteMultipleMembersRequest request);
    }
}
