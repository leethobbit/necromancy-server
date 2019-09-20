using Arrowgene.Services.Buffers;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;
using System;

namespace Necromancy.Server.Packet.Area
{
    public class send_create_package : Handler
    {
        public send_create_package(NecServer server) : base(server)
        {
        }

        public override ushort Id => (ushort) AreaPacketId.send_create_package;

        public override void Handle(NecClient client, NecPacket packet)
        {
            string recipient = packet.Data.ReadCString();
            string title = packet.Data.ReadCString();
            string content = packet.Data.ReadCString();
            byte unknownByte = packet.Data.ReadByte();
            int unknownInt = packet.Data.ReadInt32();
            long money = packet.Data.ReadInt64();

            IBuffer res = BufferProvider.Provide();

            res.WriteInt32(0);//Failed to send message error if not 0

            Router.Send(client, (ushort) AreaPacketId.recv_create_package_r, res);

            SendPackageNotifyAdd(client, recipient, title, content, unknownByte, unknownInt, money);
        }
        int i = 0;
        private void SendPackageNotifyAdd(NecClient client, string recipient, string title, string content,
                                         byte unknownByte, int unknownInt, long money)
        {
            IBuffer res = BufferProvider.Provide();
            //recv_package_notify_add = 0x556E,

            res.WriteInt32(0);//Failed to send message error if not 0
            res.WriteInt32(69);//Object ID
            res.WriteFixedString("unknown", 0x31);//Soul name
            res.WriteFixedString("master", 0x5B);//Character name sender?
            res.WriteFixedString($"{title}", 0x5B);//Title
            res.WriteFixedString($"{content}", 0x259);//Content
            res.WriteInt32(0);
            res.WriteInt16(1);//This number needs to be odd otherwise it causes a "colored" mail and causes inf loop of send_select_package_update
            res.WriteInt64(10200101);
            res.WriteInt32(10200101);//Responsible for icon
            res.WriteFixedString("help", 0x49);//
            res.WriteFixedString($"me {i} ", 0x49);//Item Title
            res.WriteInt32(-1);//Odd numbers here make the item have the title and correct icon
            res.WriteInt32(1);
            res.WriteInt32(1);
            res.WriteInt32(1);
            res.WriteFixedString("pls", 0x10);
            res.WriteByte(1);
            res.WriteInt32(1);
            res.WriteInt32(1);

            res.WriteByte(1);//bool
            res.WriteInt32(1);
            res.WriteInt32(1);
            res.WriteInt32(1);

            res.WriteByte(1);//bool
            res.WriteInt32(1);
            res.WriteInt32(1);
            res.WriteInt32(1);

            res.WriteByte(1);//bool
            res.WriteInt32(1);
            res.WriteInt32(1);
            res.WriteInt32(1);

            res.WriteInt64(money);//Transfered money

            i++;

            Router.Send(client, (ushort)AreaPacketId.recv_package_notify_add, res);
        }
    }
}