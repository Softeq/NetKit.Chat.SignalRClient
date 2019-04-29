// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Channel;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Request;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Response;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Message;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Client.Response;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Member.Request;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Member.Response;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Message.Request;

namespace Softeq.NetKit.Chat.SignalRClient.Abstract
{
    public interface ISignalRClient
    {
        #region Events

        event Action AccessTokenExpired;
        event Action Disconnected;

        event Action<ChannelSummaryResponse> ChannelUpdated;
        event Action<ChannelSummaryResponse> ChannelAdded;
        event Action<ChannelSummaryResponse> ChannelClosed;

        event Action<MessageResponse> MessageAdded;
        event Action<Guid, ChannelSummaryResponse> MessageDeleted;
        event Action<MessageResponse> MessageUpdated;
        event Action<Guid> LastReadMessageUpdated;

        event Action<MemberSummaryResponse, ChannelSummaryResponse> MemberJoined;
        event Action<MemberSummaryResponse, Guid> MemberLeft;
        event Action<MemberSummaryResponse, Guid> MemberDeleted;
        event Action<MemberSummaryResponse, Guid> YouAreDeleted;

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
