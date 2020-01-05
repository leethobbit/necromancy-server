using Arrowgene.Services.Buffers;
using Necromancy.Server.Chat;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;
using System;

namespace Necromancy.Server.Packet.Receive
{
    public class RecvEventSystemMessageTimer : PacketResponse
    {
        private readonly string _message;
        public RecvEventSystemMessageTimer(string message)
            : base((ushort) AreaPacketId.recv_event_system_message_timer, ServerType.Area)
        {
            _message = message;
        }

        protected override IBuffer ToBuffer()
        {
            IBuffer res = BufferProvider.Provide();
            res.WriteCString(_message);

            return res;
        }
    }
}
