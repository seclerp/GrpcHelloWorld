# GrpcHelloWorld

!['main' Build status](../../actions/workflows/ci.yml/badge.svg?branch=main)


ASP.NET Core 5.0 gRPC simple chat implementation for people who want to get familiar with some gRPC features

## Description

This solution contains server and client for simplest chat app.
Console client connects to the server and sends messages. Server replicates messages to sender and other connected clients.
- **Client-to-server messages** (client sends messages) are done using [unary calls](https://grpc.io/docs/languages/csharp/basics/#simple-rpc).
- **Server-to-client messages** (server sends received message to clients) are done using [server-side streaming](https://grpc.io/docs/languages/csharp/basics/#server-side-streaming-rpc) and [Channels](https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/).

There are nothing about authentication. Client sends a `nickname` when connects to a feed and `nickname + message` on each chat message send.

More details in [chat.proto](src/Protos/chat.proto) file.

## How to run

### Server

#### Using Docker

1. Change your working directory to `/src`
1. Run `docker build .`
1. Run `docker run -p 80:80 <container_id_from_build>`

Don't miss the part with port forwarding using `-p`, otherwise client will stay without port to connect to.

#### Using dotnet CLI

1. Make sure you have at least **.NET 5.0** SDK installed
1. Change your working directory to `/src/GrpcHelloWorld.Server`
1. Run `dotnet run`

### Client

1. Make sure you have at least **.NET 5.0** SDK installed
1. Change your working directory to `/src/GrpcHelloWorld.Client`
1. *(optional)* Change your nickname in `appsettings.json`
1. Run `dotnet run`