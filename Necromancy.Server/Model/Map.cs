using System;
using System.Collections.Generic;
using Arrowgene.Services.Logging;
using Arrowgene.Services.Tasks;
using Necromancy.Server.Data.Setting;
using Necromancy.Server.Logging;
using Necromancy.Server.Packet.Response;

namespace Necromancy.Server.Model
{
    public class Map
    {
        public const int NewCharacterMapId = 1001902;

        private readonly NecLogger _logger;
        private readonly NecServer _server;

        public Map(MapSetting setting, NecServer server)
        {
            _server = server;
            _logger = LogProvider.Logger<NecLogger>(this);
            ClientLookup = new ClientLookup();
            NpcSpawns = new Dictionary<int, NpcSpawn>();
            MonsterSpawns = new Dictionary<int, MonsterSpawn>();
            Id = setting.Id;
            X = setting.X;
            Y = setting.Y;
            Z = setting.Z;
            Country = setting.Country;
            Area = setting.Area;
            Place = setting.Place;
            Orientation = setting.Orientation;
            MonsterTasks = new TaskManager();

            //Assign Unique Instance ID to each NPC per map. Add to dictionary stored with the Map object
            List<NpcSpawn> npcSpawns = server.Database.SelectNpcSpawnsByMapId(setting.Id);
            foreach (NpcSpawn npcSpawn in npcSpawns)
            {
                uint instanceID = server.Instances.CreateInstance<NpcSpawn>().InstanceId;
                //Console.WriteLine($"Just Assigned instance number {instanceID} for setting id {setting.Id}");
                npcSpawn.InstanceId = instanceID;
                NpcSpawns.Add((int)instanceID, npcSpawn);
            }

            List<MonsterSpawn> monsterSpawns = server.Database.SelectMonsterSpawnsByMapId(setting.Id);
            foreach (MonsterSpawn monsterSpawn in monsterSpawns)
            {
                //MonsterSpawn monster = (MonsterSpawn)server.Instances.CreateInstance<MonsterSpawn>((IInstance)monsterSpawn);
                uint instanceID = server.Instances.CreateInstance<MonsterSpawn>().InstanceId;
                monsterSpawn.InstanceId = instanceID;
                MonsterSpawns.Add((int)monsterSpawn.InstanceId, monsterSpawn);

                monsterSpawn.monsterCoords.Clear();
                List<MonsterCoord> coords = server.Database.SelectMonsterCoordsByMonsterId(monsterSpawn.MonsterId);
                foreach (MonsterCoord monsterCoord in coords)
                {
                    monsterSpawn.monsterCoords.Add(monsterCoord);
                }
            }
        }

        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public string Country { get; set; }
        public string Area { get; set; }
        public string Place { get; set; }
        public int Orientation { get; set; }
        public string FullName => $"{Country}/{Area}/{Place}";
        public ClientLookup ClientLookup { get; }
        public Dictionary<int, NpcSpawn> NpcSpawns { get; }
        public Dictionary<int, MonsterSpawn> MonsterSpawns { get; }

        public TaskManager MonsterTasks;

        public void EnterForce(NecClient client)
        {
            Enter(client);
            _server.Router.Send(new RecvMapChangeForce(this), client);
        }

        public void Enter(NecClient client)
        {
            if (client.Map != null)
            {
                client.Map.Leave(client);
            }

            _logger.Info(client, $"Entering Map: {Id}:{FullName}", client);
            ClientLookup.Add(client);
            client.Map = this;
            client.Character.MapId = Id;
            client.Character.X = X;
            client.Character.Y = Y;
            client.Character.Z = Z;

            foreach (MonsterSpawn monsterSpawn in this.MonsterSpawns.Values)
            {
                monsterSpawn.SpawnActive = true; ;
            }
            RecvDataNotifyCharaData myCharacterData = new RecvDataNotifyCharaData(client.Character, client.Soul.Name);
            _server.Router.Send(this, myCharacterData, client);
        }

        public void Leave(NecClient client)
        {
            _logger.Info(client, $"Leaving Map: {Id}:{FullName}", client);
            ClientLookup.Remove(client);
            client.Map = null;

            RecvObjectDisappearNotify objectDisappearData = new RecvObjectDisappearNotify(client.Character.InstanceId);
            _server.Router.Send(this, objectDisappearData, client);

            foreach (MonsterSpawn monsterSpawn in this.MonsterSpawns.Values)
            {
                monsterSpawn.SpawnActive = false;
            }

        }
    }
}
