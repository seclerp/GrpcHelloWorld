using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcHelloWorld.Protobuf;
using Microsoft.Extensions.Configuration;

namespace GrpcHelloWorld.Client
{
    class Program
    {
        private static void WriteMessage(string nickName, string message, DateTime receivedOn)
        {
            Console.WriteLine($"[{receivedOn:s}] {nickName}: {message}");
        }

        private static async Task MessageFeedConsumer(IAsyncStreamReader<MessageFeedResponse> streamReader, CancellationToken token)
        {
            while (!token.IsCancellationRequested && await streamReader.MoveNext())
            {
                var current = streamReader.Current;

                // Note: `Console` class is not thread-safe, so `WriteLine` will potentially interrupt user input in main thread
                var receivedOnDateTime = DateTimeOffset.FromUnixTimeSeconds(current.TimeReceived).DateTime;
                WriteMessage(current.NickName, current.Message, receivedOnDateTime);
            }
        }

        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var grpcEndpoint = configuration["GrpcEndpoint"];
            if (string.IsNullOrWhiteSpace(grpcEndpoint))
            {
                throw new ArgumentNullException(nameof(grpcEndpoint),
                    "`GrpcEndpoint` value should be set using appsettings.json," +
                    "user secrets, environment variables or command line.");
            }

            var nickName = configuration["NickName"];
            if (string.IsNullOrWhiteSpace(grpcEndpoint))
            {
                throw new ArgumentNullException(nameof(grpcEndpoint),
                    "`NickName` value should be set using appsettings.json," +
                    "user secrets, environment variables or command line.");
            }

            var channel = GrpcChannel.ForAddress(grpcEndpoint, new GrpcChannelOptions());
            var client = new ChatService.ChatServiceClient(channel);

            // Setup message feed consumer before first message being sent to see this message in our own client's console
            var tokenSource = new CancellationTokenSource();
            using var call = client.MessageFeed(new MessageFeedRequest());
            var streamReader = call.ResponseStream;
            var _ = Task.Run(() => MessageFeedConsumer(streamReader, tokenSource.Token), tokenSource.Token);

            WriteMessage("System", "Connection is ready. To close console, send `!exit` message.", DateTime.UtcNow);

            while (true)
            {
                var message = Console.ReadLine();
                if (message is "!exit")
                {
                    break;
                }

                var messageRequest = new MessageRequest
                {
                    NickName = nickName,
                    Message = message
                };

                await client.SendMessageAsync(messageRequest);
            }

            tokenSource.Cancel();
        }
    }
}