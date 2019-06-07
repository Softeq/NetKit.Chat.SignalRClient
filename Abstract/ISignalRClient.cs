// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels.Client;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Member.Request;
using AddMessageRequest = Softeq.NetKit.Chat.SignalRClient.DTOs.Message.Request.AddMessageRequest;
using CreateDirectChannelRequest = Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Request.CreateDirectChannelRequest;
using MuteChannelRequest = Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Request.MuteChannelRequest;
using PinChannelRequest = Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.Request.PinChannelRequest;
using SetLastReadMessageRequest = Softeq.NetKit.Chat.SignalRClient.DTOs.Message.Request.SetLastReadMessageRequest;
using UpdateChannelRequest = Softeq.NetKit.Chat.SignalRClient.DTOs.Channel.UpdateChannelRequest;

namespace Softeq.NetKit.Chat.SignalRClient.Abstract
{
    public interface ISignalRClient
    {
        #region Events

        event Action AccessTokenExpired;
        event Action Disconnected;

        event Action<ChannelSummaryResponse> ChannelUpdated;
        event Action<ChannelSummaryResponse> ChannelAdded;
        event Action<Guid> ChannelClosed;

        event Action<MessageResponse> MessageAdded;
        event Action<Guid> MessageDeleted;
        event Action<MessageResponse> MessageUpdated;
        event Action<Guid> LastReadMessageUpdated;

        event Action<ChannelSummaryResponse> MemberJoined;
        event Action<Guid> MemberLeft;
        event Action<MemberSummaryResponse, Guid> MemberDeleted;
        event Action<Guid> YouAreDeleted;

        #endregion

        Task<ClientResponse> ConnectAsync();
        Task DisconnectAsync();

        Task<ChannelSummaryResponse> CreateChannelAsync(SignalRRequest<CreateChannelRequest> request);
        Task<ChannelSummaryResponse> CreateDirectChannelAsync(SignalRRequest<CreateDirectChannelRequest> request);
        Task<ChannelSummaryResponse> UpdateChannelAsync(SignalRRequest<UpdateChannelRequest> request);
        Task PinChannelAsync(SignalRRequest<PinChannelRequest> request);
        Task MuteChannelAsync(SignalRRequest<MuteChannelRequest> request);
        Task CloseChannelAsync(SignalRRequest<ChannelRequest> request);
        Task JoinToChannelAsync(SignalRRequest<ChannelRequest> request);
        Task LeaveChannelAsync(SignalRRequest<ChannelRequest> request);

        Task<MessageResponse> AddMessageAsync(SignalRRequest<AddMessageRequest> request);
        Task DeleteMessageAsync(SignalRRequest<DeleteMessageRequest> request);
        Task UpdateMessageAsync(SignalRRequest<UpdateMessageRequest> request);
        Task MarkAsReadMessageAsync(SignalRRequest<SetLastReadMessageRequest> request);

        Task<ClientResponse> GetClientAsync();
        Task InviteMemberAsync(SignalRRequest<InviteMemberRequest> request);
        Task DeleteMemberAsync(SignalRRequest<DeleteMemberRequest> request);
        Task InviteMultipleMembersAsync(SignalRRequest<InviteMultipleMembersRequest> request);
    }
}
