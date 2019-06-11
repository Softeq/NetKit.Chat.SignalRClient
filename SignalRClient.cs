// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Softeq.NetKit.Chat.SignalRClient.Abstract;
using Softeq.NetKit.Chat.SignalRClient.DTOs.Validation;
using Softeq.NetKit.Chat.SignalRClient.Extensions;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels.Client;

namespace Softeq.NetKit.Chat.SignalRClient
{
    public class SignalRClient : ISignalRClient
    {
        private HubConnection _connection;
        private readonly string _chatHubUrl;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private IDisposable _accessTokenExpiredSubscription;
        private readonly Func<Task<string>> _accessToken;

        #region Events

        public event Action AccessTokenExpired;
        public event Action Disconnected;

        public event Action<ChannelSummaryResponse> ChannelUpdated;
        public event Action<ChannelSummaryResponse> ChannelAdded;
        public event Action<Guid> ChannelClosed;

        public event Action<MessageResponse> MessageAdded;
        public event Action<Guid> MessageDeleted;
        public event Action<MessageResponse> MessageUpdated;
        public event Action<Guid> LastReadMessageUpdated;

        public event Action<ChannelSummaryResponse> MemberJoined;
        public event Action<Guid> MemberLeft;
        public event Action<MemberSummaryResponse, Guid> MemberDeleted;
        public event Action<Guid> YouAreDeleted;

        #endregion

        public SignalRClient(string chatHubUrl, Func<Task<string>> accessToken)
        {
            _chatHubUrl = chatHubUrl;
            _accessToken = accessToken;
        }

        public virtual IHubConnectionBuilder SetupConnection()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"{_chatHubUrl}/chat", options =>
                {
                    options.AccessTokenProvider = () => _accessToken.Invoke();
                });

            return connection;
        }

        public async Task<ClientResponse> ConnectAsync()
        {
            Console.WriteLine("Connecting to {0}", _chatHubUrl);

            _connection = SetupConnection()
                .Build();

            _connection.Closed += e =>
            {
                Console.WriteLine("Connection closed...");
                Disconnected?.Invoke();
                return Task.CompletedTask;
            };

            await _connection.StartAsync().ConfigureAwait(false);
            Console.WriteLine("Connected to {0}", _chatHubUrl);
            _accessTokenExpiredSubscription?.Dispose();
            _accessTokenExpiredSubscription = _connection.On<string>(ClientEvents.AccessTokenExpired, requestId =>
            {
                AccessTokenExpired?.Invoke();
            });

            var client = await _connection.InvokeAsync<ClientResponse>(ServerMethods.AddClientAsync);

            SubscribeToEvents();
            
            return client;
        }

        #region Channel
        
        public Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            return SendAndHandleExceptionsAsync<ChannelSummaryResponse>(ServerMethods.CreateChannelAsync, request);
        }

        public Task<ChannelSummaryResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request)
        {
            return SendAndHandleExceptionsAsync<ChannelSummaryResponse>(ServerMethods.CreateDirectChannelAsync, request);
        }

        public Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            return SendAndHandleExceptionsAsync<ChannelSummaryResponse>(ServerMethods.UpdateChannelAsync, request);
        }

        public Task MuteChannelAsync(MuteChannelRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.MuteChannelAsync, request);
        }

        public Task PinChannelAsync(PinChannelRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.PinChannelAsync, request);
        }

        public Task CloseChannelAsync(ChannelRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.CloseChannelAsync, request);
        }

        public Task JoinToChannelAsync(ChannelRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.JoinToChannelAsync, request);
        }

        public Task LeaveChannelAsync(ChannelRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.LeaveChannelAsync, request);
        }

        #endregion

        #region Message

        public Task<MessageResponse> AddMessageAsync(AddMessageRequest request)
        {
            return SendAndHandleExceptionsAsync<MessageResponse>(ServerMethods.AddMessageAsync, request);
        }

        public Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.DeleteMessageAsync, request);
        }

        public Task UpdateMessageAsync(UpdateMessageRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.UpdateMessageAsync, request);

        }

        public Task MarkAsReadMessageAsync(SetLastReadMessageRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.MarkAsReadMessageAsync, request);
        }

        #endregion

        #region Members

        public async Task<ClientResponse> GetClientAsync()
        {
            return await _connection.InvokeAsync<ClientResponse>(ServerMethods.GetClientAsync).ConfigureAwait(false);
        }

        public Task InviteMemberAsync(InviteMemberRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.InviteMemberAsync, request);
        }

        public Task DeleteMemberAsync(DeleteMemberRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.DeleteMemberAsync, request);
        }

        public Task InviteMultipleMembersAsync(InviteMultipleMembersRequest request)
        {
            return SendAndHandleExceptionsAsync(ServerMethods.InviteMultipleMembersAsync, request);
        }

        #endregion

        private async Task SendAndHandleExceptionsAsync(string methodName, BaseRequest request)
        {
            var tcs = new TaskCompletionSource<bool>();
            var requestId = Guid.NewGuid().ToString();

            CreateExceptionSubscription(requestId, tcs);
            CreateValidationFailedSubscription(requestId, tcs);

            IDisposable successSubscription = null;
            successSubscription = _connection.On<string>(ClientEvents.RequestSuccess, id =>
            {
                if (id == requestId)
                {
                    successSubscription.Dispose();
                    tcs.SetResult(true);
                }
            });

            var signalRRequest = new SignalRRequest<BaseRequest>()
            {
                Request = request,
                RequestId = requestId
            };

            await _connection.InvokeAsync(methodName, signalRRequest).ConfigureAwait(false);
            await tcs.Task.ConfigureAwait(false);
        }

        private async Task<TR> SendAndHandleExceptionsAsync<TR>(string methodName, BaseRequest request)
        {
            var tcs = new TaskCompletionSource<TR>();
            var requestId = Guid.NewGuid().ToString();

            CreateExceptionSubscription(requestId, tcs);
            CreateValidationFailedSubscription(requestId, tcs);

            IDisposable successSubscription = null;
            var isCallEnded = false;
            TR result = default(TR);
            successSubscription = _connection.On<string>(ClientEvents.RequestSuccess, id =>
            {
                if (id == requestId)
                {
                    successSubscription.Dispose();
                    if (isCallEnded)
                    {
                        tcs.SetResult(result);
                    }
                    else
                    {
                        isCallEnded = true;
                    }
                }
            });

            var signalRRequest = new SignalRRequest<BaseRequest>
            {
                Request = request,
                RequestId = requestId
            };

            result = await _connection.InvokeAsync<TR>(methodName, signalRRequest).ConfigureAwait(false);

            if (isCallEnded)
            {
                return result;
            }
            isCallEnded = true;
            return await tcs.Task.ConfigureAwait(false);
        }

        private void CreateExceptionSubscription<T>(string requestId, TaskCompletionSource<T> tcs)
        {
            IDisposable exceptionSubscription = null;
            exceptionSubscription = _connection.On<Exception, string>(ClientEvents.ExceptionOccurred,
                (ex, id) =>
                {
                    if (id == requestId)
                    {
                        exceptionSubscription.Dispose();
                        tcs.SetException(ex);
                    }
                });
        }

        private void CreateValidationFailedSubscription<T>(string requestId, TaskCompletionSource<T> tcs)
        {
            IDisposable validationFailedSubscription = null;
            validationFailedSubscription = _connection.On<IEnumerable<ValidationErrorsResponse>, string>(
                ClientEvents.RequestValidationFailed,
                (errors, id) =>
                {
                    if (id == requestId)
                    {
                        validationFailedSubscription.Dispose();
                        tcs.SetException(new ChatValidationException(errors.Select(x => x.ErrorMessage).ToList()));
                    }
                });
        }

        public async Task DisconnectAsync()
        {
            await _connection.InvokeAsync(ServerMethods.DeleteClientAsync).ConfigureAwait(false);
            await _connection.StopAsync().ConfigureAwait(false);
        }

        private void SubscribeToEvents()
        {
            _subscriptions.Apply(x => x.Dispose());
            _subscriptions.Clear();

            #region Channel

            _subscriptions.Add(_connection.On<ChannelSummaryResponse>(ClientEvents.ChannelAdded,
                channel =>
                {
                    ChannelAdded?.Invoke(channel);
                }));

            _subscriptions.Add(_connection.On<ChannelSummaryResponse>(ClientEvents.ChannelUpdated, 
                channel =>
                {
                    ChannelUpdated?.Invoke(channel);
                }));

            _subscriptions.Add(_connection.On<Guid>(ClientEvents.ChannelClosed,
                channelId =>
                {
                    ChannelClosed?.Invoke(channelId);
                }));

            #endregion

            #region Message

            _subscriptions.Add(_connection.On<MessageResponse>(ClientEvents.MessageAdded,
                message =>
                {
                    MessageAdded?.Invoke(message);
                }));

            _subscriptions.Add(_connection.On<MessageResponse>(ClientEvents.MessageUpdated,
                message =>
                {
                    MessageUpdated?.Invoke(message);
                }));

            _subscriptions.Add(_connection.On<Guid>(ClientEvents.MessageDeleted,
                (deletedMessageId) =>
                {
                    MessageDeleted?.Invoke(deletedMessageId);
                }));

            _subscriptions.Add(_connection.On<Guid>(ClientEvents.LastReadMessageChanged,
                channelId =>
                {
                    LastReadMessageUpdated?.Invoke(channelId);
                }));

            #endregion

            #region Member

            _subscriptions.Add(_connection.On<ChannelSummaryResponse>(ClientEvents.MemberJoined,
                (channel) =>
                {
                    MemberJoined?.Invoke(channel);
                }));

            _subscriptions.Add(_connection.On<Guid>(ClientEvents.MemberLeft,
                (channelId) =>
                {
                    MemberLeft?.Invoke(channelId);
                }));
            
            _subscriptions.Add(_connection.On<MemberSummaryResponse, Guid>(ClientEvents.MemberDeleted,
                (member, channelId) =>
                {
                    MemberDeleted?.Invoke(member, channelId);
                }));
            
            _subscriptions.Add(_connection.On<Guid>(ClientEvents.YouAreDeleted,
                (channelId) =>
                {
                    YouAreDeleted?.Invoke(channelId);
                }));

            #endregion
        }
    }
}
