using Arrowgene.Services.Buffers;
using Necromancy.Server.Chat;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;
using System;

namespace Necromancy.Server.Packet.Receive
{
    public class RecvEventEnd : PacketResponse
    {
        private readonly byte _unknown;
        public RecvEventEnd(byte unknown)
            : base((ushort) AreaPacketId.recv_event_end, ServerType.Area)
        {
            _unknown = unknown;
        }

        protected override IBuffer ToBuffer()
        {
            IBuffer res = BufferProvider.Provide();
            res.WriteByte(_unknown);

            return res;
        }
    }
}
