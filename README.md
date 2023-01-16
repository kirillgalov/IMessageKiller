# IMessageKiller Chat

It's a chat application which have to kill Apple IMessage. In the future. May be.

---

## How to run

### Way 1

1. install .net7
2. clone repository
3. run server

```bash
dotnet run -c Release --project IMessageKiller.WebAPI --urls http://*:8080
```
4. run cli client
```bash
dotnet run -c Release --project IMessageKiller.CLI_Client -- --nickname Kirill --address 192.168.0.103:8080
```
If you have problem with running second client, you would to try to run second client with Debug configuration (change Release to Debug)

### Way 2

1. download client and server with built-in runtime on [releases page](https://github.com/kirillgalov/IMessageKiller/releases/tag/Publish)
2. run with args
```bash
./IMessageKiller.WebAPI --urls http://*:8080
./IMessageKiller.CLI_Client -- --nickname Kirill --address 192.168.0.103:8080
```

**Note:** if you have problem with running on local network(like me), you would to set concrete ip address on server process.
**For example:**
run WebAPI like ``./IMessageKiller.WebAPI --urls http://192.168.0.103:8080`` (change 192.168.0.103 to ip of yours server)

## About project
Chat was implemented with websockets.
