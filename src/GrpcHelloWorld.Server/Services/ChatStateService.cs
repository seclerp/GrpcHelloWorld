using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GrpcHelloWorld.Server.Services
{
    public class ChatStateService<TMessage> : IChatStateService<TMessage>
    {
        private readonly ConcurrentDictionary<string, Channel<TMessage>> _channelsPerUser;

        public ChatStateService()
        {
            _channelsPerUser = new ConcurrentDictionary<string, Channel<TMessage>>();
        }

        public void ConnectUser(string nickName)
        {
            _channelsPerUser[nickName] = Channel.CreateUnbounded<TMessage>();
        }

        public async Task SendToAllAsync(TMessage messageFeedResponse)
        {
            var sendTasks =
                _channelsPerUser.Values.Select(channel => channel.Writer.WriteAsync(messageFeedResponse).AsTask());

            await Task.WhenAll(sendTasks);
        }

        public ChannelReader<TMessage> GetReader(string nickName)
        {
            return _channelsPerUser[nickName].Reader;
        }

        public ChannelWriter<TMessage> GetWriter(string nickName)
        {
            return _channelsPerUser[nickName].Writer;
        }
    }
}