using System.Threading.Channels;
using System.Threading.Tasks;

namespace GrpcHelloWorld.Server.Services
{
    public interface IChatStateService<TMessage>
    {
        void ConnectUser(string nickName);
        Task SendToAllAsync(TMessage messageFeedResponse);
        ChannelReader<TMessage> GetReader(string nickName);
        ChannelWriter<TMessage> GetWriter(string nickName);
    }
}