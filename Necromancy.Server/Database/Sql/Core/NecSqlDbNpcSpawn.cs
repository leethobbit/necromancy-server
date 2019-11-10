using System.Collections.Generic;
using System.Data.Common;
using Necromancy.Server.Model;

namespace Necromancy.Server.Database.Sql.Core
{
    public abstract partial class NecSqlDb<TCon, TCom> : SqlDb<TCon, TCom>
        where TCon : DbConnection
        where TCom : DbCommand
    {
        private const string SqlInsertNpcSpawn =
            "INSERT INTO `nec_npc_spawn` (`npc_id`, `model_id`, `level`,  `name`, `title`, `map_id`, `x`, `y`, `z`, `active`, `heading`, `size`, `visibility`, `created`, `updated`, `icon`) VALUES (@npc_id, @model_id, @level, @name, @title, @map_id, @x, @y, @z, @active, @heading, @size, @visibility, @created, @updated, @icon);";

        private const string SqlSelectNpcSpawns =
            "SELECT `id`, `npc_id`, `model_id`, `level`, `name`, `title`, `map_id`, `x`, `y`, `z`, `active`, `heading`, `size`, `visibility`, `created`, `updated` , `icon` FROM `nec_npc_spawn`;";

        private const string SqlSelectNpcSpawnsByMapId =
            "SELECT `id`, `npc_id`, `model_id`, `level`, `name`, `title`, `map_id`, `x`, `y`, `z`, `active`, `heading`, `size`, `visibility`, `created`, `updated` , `icon` FROM `nec_npc_spawn` WHERE `map_id`=@map_id;";

        private const string SqlUpdateNpcSpawn =
            "UPDATE `nec_npc_spawn` SET `npc_id`=@npc_id, `model_id`=@model_id, `level`=@level,  `name`=@name, `title`=@title, `map_id`=@map_id, `x`=@x, `y`=@y, `z`=@z, `active`=@active, `heading`=@heading, `size`=@size, `visibility`=@visibility, `created`=@created, `updated`=@updated , `icon`=@icon WHERE `id`=@id;";

        private const string SqlDeleteNpcSpawn =
            "DELETE FROM `nec_npc_spawn` WHERE `id`=@id;";

        public bool InsertNpcSpawn(NpcSpawn npcSpawn)
        {
            int rowsAffected = ExecuteNonQuery(SqlInsertNpcSpawn, command =>
            {
                AddParameter(command, "@npc_id", npcSpawn.NpcId);
                AddParameter(command, "@model_id", npcSpawn.ModelId);
                AddParameter(command, "@level", npcSpawn.Level);
                AddParameter(command, "@name", npcSpawn.Name);
                AddParameter(command, "@title", npcSpawn.Title);
                AddParameter(command, "@map_id", npcSpawn.MapId);
                AddParameter(command, "@x", npcSpawn.X);
                AddParameter(command, "@y", npcSpawn.Y);
                AddParameter(command, "@z", npcSpawn.Z);
                AddParameter(command, "@active", npcSpawn.Active);
                AddParameter(command, "@heading", npcSpawn.Heading);
                AddParameter(command, "@size", npcSpawn.Size);
                AddParameter(command, "@visibility", npcSpawn.Visibility);
                AddParameter(command, "@created", npcSpawn.Created);
                AddParameter(command, "@updated", npcSpawn.Updated);
                AddParameter(command, "@icon", npcSpawn.Icon);
            }, out long autoIncrement);
            if (rowsAffected <= NoRowsAffected || autoIncrement <= NoAutoIncrement)
            {
                return false;
            }

            npcSpawn.Id = (int) autoIncrement;
            return true;
        }

        public List<NpcSpawn> SelectNpcSpawns()
        {
            List<NpcSpawn> npcSpawns = new List<NpcSpawn>();
            ExecuteReader(SqlSelectNpcSpawns, reader =>
            {
                while (reader.Read())
                {
                    NpcSpawn npcSpawn = ReadNpcSpawn(reader);
                    npcSpawns.Add(npcSpawn);
                }
            });
            return npcSpawns;
        }

        public List<NpcSpawn> SelectNpcSpawnsByMapId(int mapId)
        {
            List<NpcSpawn> npcSpawns = new List<NpcSpawn>();
            ExecuteReader(SqlSelectNpcSpawnsByMapId,
                command => { AddParameter(command, "@map_id", mapId); },
                reader =>
                {
                    while (reader.Read())
                    {
                        NpcSpawn npcSpawn = ReadNpcSpawn(reader);
                        npcSpawns.Add(npcSpawn);
                    }
                });
            return npcSpawns;
        }

        public bool UpdateNpcSpawn(NpcSpawn npcSpawn)
        {
            int rowsAffected = ExecuteNonQuery(SqlUpdateNpcSpawn, command =>
            {
                AddParameter(command, "@npc_id", npcSpawn.NpcId);
                AddParameter(command, "@model_id", npcSpawn.ModelId);
                AddParameter(command, "@level", npcSpawn.Level);
                AddParameter(command, "@name", npcSpawn.Name);
                AddParameter(command, "@title", npcSpawn.Title);
                AddParameter(command, "@map_id", npcSpawn.MapId);
                AddParameter(command, "@x", npcSpawn.X);
                AddParameter(command, "@y", npcSpawn.Y);
                AddParameter(command, "@z", npcSpawn.Z);
                AddParameter(command, "@active", npcSpawn.Active);
                AddParameter(command, "@heading", npcSpawn.Heading);
                AddParameter(command, "@size", npcSpawn.Size);
                AddParameter(command, "@visibility", npcSpawn.Visibility);
                AddParameter(command, "@created", npcSpawn.Created);
                AddParameter(command, "@updated", npcSpawn.Updated);
                AddParameter(command, "@icon", npcSpawn.Updated);
                AddParameter(command, "@id", npcSpawn.Id);
            });
            return rowsAffected > NoRowsAffected;
        }

        public bool DeleteNpcSpawn(int npcSpawnId)
        {
            int rowsAffected = ExecuteNonQuery(SqlDeleteNpcSpawn,
                command => { AddParameter(command, "@id", npcSpawnId); });
            return rowsAffected > NoRowsAffected;
        }

        private NpcSpawn ReadNpcSpawn(DbDataReader reader)
        {
            NpcSpawn npcSpawn = new NpcSpawn();
            npcSpawn.Id = GetInt32(reader, "id");
            npcSpawn.ModelId = GetInt32(reader, "model_id");
            npcSpawn.NpcId = GetInt32(reader, "npc_id");
            npcSpawn.Level = GetByte(reader, "level");
            npcSpawn.Name = GetString(reader, "name");
            npcSpawn.Title = GetString(reader, "title");
            npcSpawn.MapId = GetInt32(reader, "map_id");
            npcSpawn.X = GetFloat(reader, "x");
            npcSpawn.Y = GetFloat(reader, "y");
            npcSpawn.Z = GetFloat(reader, "z");
            npcSpawn.Active = GetBoolean(reader, "active");
            npcSpawn.Heading = GetByte(reader, "heading");
            npcSpawn.Size = GetInt16(reader, "size");
            npcSpawn.Visibility = GetInt32(reader, "visibility");
            npcSpawn.Created = GetDateTime(reader, "created");
            npcSpawn.Updated = GetDateTime(reader, "updated");
            npcSpawn.Icon = GetInt32(reader, "icon");
            return npcSpawn;
        }
    }
}
