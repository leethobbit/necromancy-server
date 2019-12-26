using Arrowgene.Services.Buffers;
using Necromancy.Server.Chat;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;
using System.Numerics;

namespace Necromancy.Server.Packet.Receive
{
    public class RecvTest : PacketResponse
    {
        private readonly int _instanceId;
        private readonly int _targetInstanceId;
        public RecvTest(int instanceId, int targetInstanceId)
            : base((ushort) AreaPacketId.recv_0x1489, ServerType.Area)
        {
            _instanceId = instanceId;
            _targetInstanceId = targetInstanceId;
        }

        protected override IBuffer ToBuffer()
        {
            IBuffer res = BufferProvider.Provide();
            //No String - push wizardryonline_patched.4D6044
            res.WriteInt32(_instanceId);
            res.WriteInt32(_targetInstanceId);

            return res;
        }
    }
}
