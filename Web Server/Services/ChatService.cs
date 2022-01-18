using System.Threading.Tasks;
using Grpc.Core;

namespace Web_Server.Services
{
    public class ChatService: ChatRoom.ChatRoomBase
    {
        private readonly ChatRoom _chatroomService;
        
        public ChatService(ChatRoom chatRoomService)
        {
            _chatroomService = chatRoomService;
        }
        
        public override async Task join(IAsyncStreamReader<Message> requestStream,
            IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;
        
            do
            {
                _chatroomService.Join(requestStream.Current.User, responseStream);
                await _chatroomService.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());
        
            _chatroomService.Remove(context.Peer);
        }
    }
}