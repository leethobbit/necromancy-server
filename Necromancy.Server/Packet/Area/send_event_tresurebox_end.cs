﻿using Arrowgene.Services.Buffers;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;
using System;

namespace Necromancy.Server.Packet.Area
{
    public class send_event_tresurebox_end : Handler
    {
        public send_event_tresurebox_end(NecServer server) : base(server)
        {
        }


        public override ushort Id => (ushort)AreaPacketId.send_event_tresurebox_end;

        public override void Handle(NecClient client, NecPacket packet)
        {

            IBuffer res = BufferProvider.Provide();
            res.WriteByte(0);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_event_end, res);
        }


    }
}