using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcHelloWorld.Protobuf;

namespace GrpcHelloWorld.Server.Services
{
    public class ChatService : Protobuf.ChatService.ChatServiceBase
    {
        private readonly IChatStateService<MessageFeedResponse> _chatStateService;

        public ChatService(IChatStateService<MessageFeedResponse> chatStateService)
        {
            _chatStateService = chatStateService;
        }

        public override async Task MessageFeed(
            MessageFeedRequest request,
            IServerStreamWriter<MessageFeedResponse> responseStream,
            ServerCallContext context)
        {
            _chatStateService.ConnectUser(request.NickName);
            var reader = _chatStateService.GetReader(request.NickName);
            while (!reader.Completion.IsCompleted && !context.CancellationToken.IsCancellationRequested)
            {
                var message = await reader.ReadAsync();
                await responseStream.WriteAsync(message);
            }
        }

        public override async Task<MessageReply> SendMessage(MessageRequest request, ServerCallContext context)
        {
            var utcNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var feedResponse = new MessageFeedResponse
            {
                NickName = request.NickName,
                Message = request.Message,
                TimeReceived = utcNow
            };

            await _chatStateService.SendToAllAsync(feedResponse);

            return new MessageReply();
        }
    }
}