using Arrowgene.Services.Buffers;
using Arrowgene.Services.Logging;
using Arrowgene.Services.Tasks;
using Necromancy.Server.Common;
using Necromancy.Server.Common.Instance;
using Necromancy.Server.Logging;
using Necromancy.Server.Model;
using Necromancy.Server.Model.Skills;
using Necromancy.Server.Packet;
using Necromancy.Server.Packet.Id;
using Necromancy.Server.Packet.Receive;
using Necromancy.Server.Packet.Response;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Necromancy.Server.Tasks
{
    public class CharacterTask : PeriodicTask
    {
        private readonly object LogoutLock = new object();
        private readonly NecLogger _logger;
        private NecServer _server;
        private NecClient _client;
        private int tickTime;
        private DateTime _logoutTime;
        private byte _logoutType;
        private bool playerDied;
        public CharacterTask(NecServer server, NecClient client)
        {
            _logger = LogProvider.Logger<NecLogger>(server);
            _server = server;
            _client = client;
            tickTime = 500;
            _logoutTime = DateTime.MinValue;
            playerDied = false;
        }

        public override string Name { get; }
        public override TimeSpan TimeSpan { get; }
        protected override bool RunAtStart { get; }
        protected override void Execute()
        {
            while (_client.Character.characterActive)
            {
                if (_logoutTime != DateTime.MinValue)
                {
                    if (DateTime.Now >= _logoutTime)
                    {
                        LogOutRequest();
                    }
                }
                if (_client.Character.playerDead && !playerDied)
                    PlayerDead();
                else if (!_client.Character.playerDead && playerDied)
                    playerDied = false;

                Thread.Sleep(tickTime);
            }
            this.Stop();
        }
        private void PlayerDead()
        {
            playerDied = true;
            List<PacketResponse> brList = new List<PacketResponse>();
            RecvBattleReportStartNotify brStart = new RecvBattleReportStartNotify(_client.Character.killerInstanceId);
            RecvBattleReportNoactDead cDead1 = new RecvBattleReportNoactDead(_client.Character.InstanceId, 1);
            RecvBattleReportNoactDead cDead2 = new RecvBattleReportNoactDead(_client.Character.InstanceId, 2);
            RecvBattleReportEndNotify brEnd = new RecvBattleReportEndNotify();

            brList.Add(brStart);
            brList.Add(cDead1); //animate the death of your living body
            brList.Add(brEnd);
            _server.Router.Send(_client.Map, brList, _client); // send death animation to other players


            brList[1] = cDead2;
            _server.Router.Send(_client, brList); // send death animaton to player 1

            DeadBody deadBody = _server.Instances.GetInstance((uint)_client.Character.DeadBodyInstanceId) as DeadBody;

            deadBody.X = _client.Character.X;
            deadBody.Y = _client.Character.Y;
            deadBody.Z = _client.Character.Z;
            deadBody.Heading = _client.Character.Heading;
            _client.Character.movementId = _client.Character.DeadBodyInstanceId;

            Thread.Sleep(5000);
            _client.Character.hadDied = false; // quick switch to living state so your dead body loads with your gear
                                               //load your dead body on to the map for you to see in soul form. 
            RecvDataNotifyCharaBodyData cBodyData = new RecvDataNotifyCharaBodyData(deadBody, _client);
            _server.Router.Send(_client, cBodyData.ToPacket());

            _client.Character.hadDied = true; // back to dead so your soul appears with-out gear.

            Thread.Sleep(100);

            //reload your living body with no gear
            RecvDataNotifyCharaData cData = new RecvDataNotifyCharaData(_client.Character, _client.Soul.Name);
            _server.Router.Send(_client.Map, cData.ToPacket());
        }

        public void Logout(DateTime logoutTime, byte logoutType)
        {
            lock (LogoutLock)
            {
                _logoutTime = logoutTime;
                _logoutType = logoutType;
            }
            _logger.Debug($"logoutTime [{logoutTime}] _logoutType [{_logoutType}]");
        }

        private void LogOutRequest()
        {
            _logoutTime = DateTime.MinValue;
            IBuffer res = BufferProvider.Provide();
            IBuffer res2 = BufferProvider.Provide();
            IBuffer res3 = BufferProvider.Provide();
            IBuffer res4 = BufferProvider.Provide();
            IBuffer res5 = BufferProvider.Provide();
            _logger.Debug($"_logoutType [{_logoutType}]");
            if (_logoutType == 0x00)   // Return to Title   also   Exit Game
            {
                //res.WriteInt32(0);

                _server.Router.Send(_client, (ushort)AreaPacketId.recv_escape_start, res, ServerType.Area);
            }

            if (_logoutType == 0x01) // Return to Character Select
            {

                res.WriteInt32(0);
                _server.Router.Send(_client, (ushort)MsgPacketId.recv_chara_select_back_soul_select_r, res, ServerType.Msg);
                
                Thread.Sleep(4100);

                res = null;
                res = BufferProvider.Provide();
                res.WriteInt32(0);
                res.WriteByte(0);
                _server.Router.Send(_client, (ushort)MsgPacketId.recv_soul_authenticate_passwd_r, res, ServerType.Msg);
            }

            if (_logoutType == 0x02)
            {
                res.WriteInt32(0);
                _server.Router.Send(_client, (ushort)MsgPacketId.recv_chara_select_back_r, res, ServerType.Msg);
            }
        }
    }
}
