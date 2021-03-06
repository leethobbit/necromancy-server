using Arrowgene.Services.Buffers;
using Necromancy.Server.Common;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;
using Necromancy.Server.Common.Instance;
using System;
using System.Collections.Generic;
using Necromancy.Server.Packet.Receive;
using Necromancy.Server.Packet.Response;

namespace Necromancy.Server.Packet.Area
{
    public class send_battle_attack_exec : ClientHandler
    {
        private readonly NecServer _server;
        public send_battle_attack_exec(NecServer server) : base(server)
        {
            _server = server;
        }

        public override ushort Id => (ushort)AreaPacketId.send_battle_attack_exec;

        public override void Handle(NecClient client, NecPacket packet)
        {
            int Unknown1 = packet.Data.ReadInt32();
            uint instanceId = packet.Data.ReadUInt32();
            int Unknown2 = packet.Data.ReadInt32();

            client.Character.eventSelectReadyCode = (uint)instanceId;
            Logger.Debug($"Just attacked Target {client.Character.eventSelectReadyCode}");


            if (instanceId == 0)
                return;

            int damage = 0;
            float perHp = 100.0f;
            int seed = Util.GetRandomNumber(0, 20);
            if (seed < 2)
                damage = Util.GetRandomNumber(1, 4);    // Light hit
            else if (seed < 19)
                damage = Util.GetRandomNumber(16, 24);  // Normal hit
            else
                damage = Util.GetRandomNumber(32, 48);  // Critical hit

            IInstance instance = Server.Instances.GetInstance(instanceId);
            SendBattleAttackExecR(client);
//            SendBattleReportStartNotify(client, instance);

            switch (instance)
            {
                case NpcSpawn npcSpawn:
                    client.Map.NpcSpawns.TryGetValue(npcSpawn.InstanceId, out npcSpawn);
                    {
                        double distanceToNPC = distance(npcSpawn.X, npcSpawn.Y, client.Character.X, client.Character.Y);
                        Logger.Debug($"NPC name [{npcSpawn.Name}] distanceToNPC [{distanceToNPC}] Radius [{npcSpawn.Radius}] {npcSpawn.Name}");
                        if (distanceToNPC > npcSpawn.Radius + 125)
                        {
                            //SendBattleReportEndNotify(client, instance);
                            return;
                        }
                        if (client.Character.criminalState < 1 ) 
                        { 
                            client.Character.criminalState = 1;
                            IBuffer res40 = BufferProvider.Provide();
                            res40.WriteInt32(client.Character.InstanceId);
                            res40.WriteByte(client.Character.criminalState);

                            Logger.Debug($"Setting crime level for Character {client.Character.Name} to {client.Character.criminalState}");
                            Router.Send(client, (ushort)AreaPacketId.recv_chara_update_notify_crime_lv, res40, ServerType.Area);
                            Router.Send(client.Map, (ushort)AreaPacketId.recv_charabody_notify_crime_lv, res40, ServerType.Area, client);
                        }

                    }
                    break;
                case MonsterSpawn monsterSpawn:
                    client.Map.MonsterSpawns.TryGetValue(instanceId, out monsterSpawn);
                    {
                        double distanceToMonster = distance(monsterSpawn.X, monsterSpawn.Y, client.Character.X, client.Character.Y);
                        Logger.Debug($"monster name [{monsterSpawn.Name}] distanceToMonster [{distanceToMonster}] Radius [{monsterSpawn.Radius}] {monsterSpawn.Name}");
                        if (distanceToMonster > monsterSpawn.Radius + 125)
                        {
                            //SendBattleReportEndNotify(client, instance);
                            return;
                        }
                        if (monsterSpawn.GetAgroCharacter(client.Character.InstanceId))
                        {
                            monsterSpawn.UpdateHP(-damage);
                        }
                        else
                        {
                            monsterSpawn.UpdateHP(-damage, _server, true, client.Character.InstanceId);
                        }
                        if (client.Character.IsStealthed())
                        {
                            uint newState = client.Character.ClearStateBit(0x8);
                            RecvCharaNotifyStateflag charState = new RecvCharaNotifyStateflag(client.Character.InstanceId, newState);
                            _server.Router.Send(client.Map, charState);
                        }
                        perHp = (((float)monsterSpawn.GetHP() / (float)monsterSpawn.MaxHp) * 100);
                        Logger.Debug($"CurrentHp [{monsterSpawn.GetHP()}] MaxHp[{ monsterSpawn.MaxHp}] perHp[{perHp}]");
                    }
                    break;
                case Character character:
                    NecClient targetClient = client.Map.ClientLookup.GetByCharacterInstanceId(instanceId);
                    double distanceToCharacter = distance(targetClient.Character.X, targetClient.Character.Y, client.Character.X, client.Character.Y);
                    Logger.Debug($"target Character name [{targetClient.Character.Name}] distanceToCharacter [{distanceToCharacter}] Radius {/*[{monsterSpawn.Radius}]*/"125"} {targetClient.Character.Name}");
                    if (distanceToCharacter > /*targetClient.Character.Radius +*/ 125)
                    {
                        //SendBattleReportEndNotify(client, instance);
                        return;
                    }
                    targetClient.Character.currentHp -= damage;
                    perHp = ((targetClient.Character.currentHp / targetClient.Character.maxHp) * 100);
                    Logger.Debug($"CurrentHp [{targetClient.Character.currentHp}] MaxHp[{targetClient.Character.maxHp}] perHp[{perHp}]");
                    RecvCharaUpdateHp cHpUpdate = new RecvCharaUpdateHp(targetClient.Character.currentHp);
                    _server.Router.Send(targetClient, cHpUpdate.ToPacket());

                    //logic to turn characters to criminals on criminal actions.  possibly should move to character task.
                    client.Character.criminalState += 1;
                    if(client.Character.criminalState == 1 | client.Character.criminalState == 2 | client.Character.criminalState ==3)
                    {
                        IBuffer res40 = BufferProvider.Provide();
                        res40.WriteInt32(client.Character.InstanceId);
                        res40.WriteByte(client.Character.criminalState);

                        Logger.Debug($"Setting crime level for Character {client.Character.Name} to {client.Character.criminalState}");
                        Router.Send(client, (ushort)AreaPacketId.recv_chara_update_notify_crime_lv, res40, ServerType.Area);
                        Router.Send(client.Map, (ushort)AreaPacketId.recv_charabody_notify_crime_lv, res40, ServerType.Area, client);
                    }
                    if (client.Character.criminalState > 255) { client.Character.criminalState = 255; }

                    break;

                default:
                    Logger.Error($"Instance with InstanceId: {instanceId} does not exist");
                    break;
            }

            List<PacketResponse> brTargetList = new List<PacketResponse>();
            RecvBattleReportStartNotify brStart = new RecvBattleReportStartNotify(client.Character.InstanceId);
            RecvBattleReportEndNotify brEnd = new RecvBattleReportEndNotify();
            RecvBattleReportActionAttackExec brAttack = new RecvBattleReportActionAttackExec((int)instance.InstanceId);
            RecvBattleReportNotifyHitEffect brHit = new RecvBattleReportNotifyHitEffect(instance.InstanceId);
            RecvBattleReportPhyDamageHp brPhyHp = new RecvBattleReportPhyDamageHp(instance.InstanceId, damage);
            RecvBattleReportDamageHp brHp = new RecvBattleReportDamageHp(instance.InstanceId, damage);
            RecvObjectHpPerUpdateNotify oHpUpdate = new RecvObjectHpPerUpdateNotify(instance.InstanceId, perHp);


            brTargetList.Add(brStart);
            brTargetList.Add(brAttack);
            brTargetList.Add(brHit);
            brTargetList.Add(brPhyHp);
            //brTargetList.Add(brHp);
            brTargetList.Add(oHpUpdate);
            brTargetList.Add(brEnd);
            Router.Send(client.Map, brTargetList);



            //SendReportAcctionAtackExec(client, instance);
            //SendReportNotifyHitEffect(client, instance);
            //SendReportDamageHP(client, instance, damage, perHp);
            //SendBattleReportEndNotify(client, instance);
        }

        private void SendBattleReportStartNotify(NecClient client, IInstance instance)
        {
            IBuffer res4 = BufferProvider.Provide();
            res4.WriteInt32(client.Character.InstanceId);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_start_notify, res4, ServerType.Area);
        }   

        private void CheckMonsterStats(NecClient client, IInstance instance)
        {
            MonsterSpawn monster = (MonsterSpawn)instance;


        }
        private void SendReportNotifyHitEffect(NecClient client, IInstance instance)
        {
            IBuffer res4 = BufferProvider.Provide();
            res4.WriteInt32(instance.InstanceId);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_notify_hit_effect, res4, ServerType.Area);
        }

        private void SendReportDamageHP(NecClient client, IInstance instance, int damage, float perHp)
        {      
            IBuffer res = BufferProvider.Provide();
            res.WriteInt32(instance.InstanceId);
            res.WriteInt32(damage);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_notify_phy_damage_hp, res, ServerType.Area);            

            if (perHp < 0) { perHp = 0; }

            IBuffer res4 = BufferProvider.Provide();
            res4.WriteInt32(instance.InstanceId);
            res4.WriteByte((byte)perHp); // % hp remaining of target.  need to store current NPC HP and OD as variables to "attack" them
            Router.Send(client.Map, (ushort)AreaPacketId.recv_object_hp_per_update_notify, res4, ServerType.Area);
        }

        private void SendBattleAttackExecR(NecClient client)
        {
            IBuffer res = BufferProvider.Provide();
            res.WriteInt32(0);  // If not zero next attack is allowed before first complete
            Router.Send(client, (ushort)AreaPacketId.recv_battle_attack_exec_r, res, ServerType.Area);
        }

        static double distance(double targetX, double targetY, double objectX, double objectY)
        {
            // Calculating distance 
            return Math.Sqrt(Math.Pow(objectX - targetX, 2) +
                          Math.Pow(objectY - targetY, 2) * 1.0);
        }
        private void SendBattleReportEndNotify(NecClient client, IInstance instance)
        {
            IBuffer res4 = BufferProvider.Provide();
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_end_notify, res4, ServerType.Area);
        }
        private void SendBattleReportKnockBack(NecClient client, IInstance instance)
        {
            MonsterSpawn monster = (MonsterSpawn)instance;
            IBuffer res = BufferProvider.Provide();
            res.WriteInt32(monster.InstanceId);
            res.WriteFloat(0);
            res.WriteFloat(2);   // delay in seconds
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_noact_notify_knockback, res, ServerType.Area);
        }

        private void SendBattleAteckExecDirect(NecClient client, IInstance instance)
        {
            IBuffer res4 = BufferProvider.Provide();
            res4.WriteInt32(instance.InstanceId);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_attack_exec_direct_r, res4, ServerType.Area);            
        }
        private void SendReportAcctionAtackExec(NecClient client, IInstance instance)
        {
            IBuffer res4 = BufferProvider.Provide();
            res4.WriteInt32(instance.InstanceId);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_action_attack_exec, res4, ServerType.Area);


        }
        private void SendDataNotifyEoData(NecClient client, IInstance instance)
        {
            MonsterSpawn monster = (MonsterSpawn)instance;
            IBuffer res2 = BufferProvider.Provide();
            uint skillInstanceID = Server.Instances.CreateInstance<Skill>().InstanceId;
            Logger.Debug($"Skill instance {skillInstanceID} was just cast. use /Takeover {skillInstanceID} to control");
            res2.WriteInt32(skillInstanceID); // Unique Instance ID of Skill Cast
            res2.WriteFloat(monster.X);//Effect Object X
            res2.WriteFloat(monster.Y);//Effect Object y
            res2.WriteFloat(monster.Z + 100);//Effect Object z

            //orientation related
            res2.WriteFloat(client.Character.X);//Rotation Along X Axis if above 0
            res2.WriteFloat(client.Character.Y);//Rotation Along Y Axis if above 0
            res2.WriteFloat(client.Character.Heading);//Rotation Along Z Axis if above 0

            res2.WriteInt32(600021);// effect id
            res2.WriteInt32(monster.InstanceId); //must be set to int32 contents. int myTargetID = packet.Data.ReadInt32();
            res2.WriteInt32(0);//unknown

            res2.WriteInt32(client.Character.Heading);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_data_notify_eo_data, res2, ServerType.Area);

            //makes Effect disappear after float seconds
            IBuffer res5 = BufferProvider.Provide();
            res5.WriteInt32(skillInstanceID);
            res5.WriteFloat(10);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_eo_notify_disappear_schedule, res5, ServerType.Area);

            //makes effect have a sphere of collision???
            IBuffer res6 = BufferProvider.Provide();
            res6.WriteInt32(skillInstanceID);
            res6.WriteFloat(10000);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_eo_base_notify_sphere, res6, ServerType.Area);

            //testing
            IBuffer res8 = BufferProvider.Provide();
            res8.WriteInt32(0);
            res8.WriteInt32(0);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_eo_update_state, res8, ServerType.Area);
        }

    }
}
