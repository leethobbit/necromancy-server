using Arrowgene.Services.Buffers;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;

namespace Necromancy.Server.Packet.Area
{
    public class send_party_change_leader : ClientHandler
    {
        public send_party_change_leader(NecServer server) : base(server)
        {
        }

        public override ushort Id => (ushort) AreaPacketId.send_party_change_leader;

        public override void Handle(NecClient client, NecPacket packet)
        {
            int target = packet.Data.ReadInt32();

            IBuffer res = BufferProvider.Provide();

            res.WriteInt32(target);

            Router.Send(client, (ushort) AreaPacketId.recv_party_change_leader_r, res, ServerType.Area);
        }
    }
}