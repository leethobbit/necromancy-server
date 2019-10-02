using Arrowgene.Services.Buffers;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;

namespace Necromancy.Server.Packet.Area
{
    public class send_logout_start_request : Handler
    {
        public send_logout_start_request(NecServer server) : base(server)
        {
        }

        public override ushort Id => (ushort) AreaPacketId.send_logout_start_request;

        public override void Handle(NecClient client, NecPacket packet)
        {
            IBuffer res = BufferProvider.Provide();
            res.WriteInt32(0);//0 = nothing happens, 1 = logout failed:1
            Router.Send(client, (ushort) AreaPacketId.recv_logout_start_request_r, res);

            IBuffer res2 = BufferProvider.Provide();
            res2.WriteInt32(10);//
            Router.Send(client, (ushort)AreaPacketId.recv_logout_start, res2);

            IBuffer res3 = BufferProvider.Provide();

            Router.Send(client, (ushort)AreaPacketId.recv_base_exit_r, res3);

            IBuffer res4 = BufferProvider.Provide();
            res4.WriteInt32(client.Character.Id);
            Router.Send(client, (ushort)AreaPacketId.recv_thread_exit_r, res4);
        }
    }
}