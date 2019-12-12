using Arrowgene.Services.Buffers;
using Necromancy.Server.Common;
using Necromancy.Server.Data.Setting;
using Necromancy.Server.Model;
using Necromancy.Server.Packet.Id;
using Necromancy.Server.Packet.Response;
using Necromancy.Server.Tasks;

namespace Necromancy.Server.Packet.Area
{
    public class send_map_get_info : ClientHandler
    {
        public send_map_get_info(NecServer server) : base(server)
        {
        }

        public override ushort Id => (ushort) AreaPacketId.send_map_get_info;

        public override void Handle(NecClient client, NecPacket packet)
        {
            IBuffer res = BufferProvider.Provide();
            res.WriteInt32(client.Map.Id);
            Router.Send(client, (ushort) AreaPacketId.recv_map_get_info_r, res, ServerType.Area);

            foreach (NpcSpawn npcSpawn in client.Map.NpcSpawns.Values)
            {
                // This requires database changes to add the GGates to the Npc database!!!!!
                if (npcSpawn.Name == "GGate")
                {
                    RecvDataNotifyGGateData gGateData = new RecvDataNotifyGGateData(npcSpawn);
                    Router.Send(gGateData, client);
                }
                else
                {
                    RecvDataNotifyNpcData npcData = new RecvDataNotifyNpcData(npcSpawn);
                    Router.Send(npcData, client);
                }
            }

            foreach (MonsterSpawn monsterSpawn in client.Map.MonsterSpawns.Values)
            {
                if (!Server.SettingRepository.ModelCommon.TryGetValue(monsterSpawn.ModelId, out ModelCommonSetting modelSetting))
                {
                    return;
                }
                monsterSpawn.ModelId = modelSetting.Id;
                monsterSpawn.Size = (short)(modelSetting.Height/2);
                monsterSpawn.Radius = (short)modelSetting.Radius;
                monsterSpawn.MaxHp = 1000;
                monsterSpawn.CurrentHp = 100;
               // if (monsterSpawn.MapId != 2002104)
                {
              //      RecvDataNotifyMonsterData monsterData = new RecvDataNotifyMonsterData(monsterSpawn);
                //    Router.Send(monsterData, client);
                } //else
                {
                    MonsterTask monsterTask = new MonsterTask(Server, client, monsterSpawn);
                    monsterTask.monsterHome = monsterSpawn.monsterCoords.Find(x => x.CoordIdx == 0); //was 64
                    monsterTask.Start();
                }
            }

            foreach (NecClient otherClient in client.Map.ClientLookup.GetAll())
            {
                if (otherClient == client)
                {
                    // skip myself
                    continue;
                }

                RecvDataNotifyCharaData otherCharacterData =
                    new RecvDataNotifyCharaData(otherClient.Character, otherClient.Soul.Name);
                Router.Send(otherCharacterData, client);
            }
        }

      
    }
}
