using System.Collections.Generic;
using Necromancy.Server.Model;
using Arrowgene.Services.Buffers;
using Necromancy.Server.Common;
using Necromancy.Server.Packet.Id;
using System.Threading;
using System;
using Necromancy.Server.Common.Instance;
using Necromancy.Server.Packet.Response;
using System.Threading.Tasks;

namespace Necromancy.Server.Chat.Command.Commands
{
    /// <summary>
    /// Quick mob test commands.
    /// </summary>
    public class CharaCommand : ServerChatCommand
    {
        public CharaCommand(NecServer server) : base(server)
        {
        }

        public override void Execute(string[] command, NecClient client, ChatMessage message,
            List<ChatResponse> responses)
        {
            if (command[0] == null)
            {
                responses.Add(ChatResponse.CommandError(client, $"Invalid argument: {command[0]}"));
                return;
            }
            Character character2 = null;
            if (uint.TryParse(command[1], out uint x))
            {
                IInstance instance = Server.Instances.GetInstance(x);
                if (instance is Character character)
                {
                    character2 = character;
                }
                else
                {
                    responses.Add(ChatResponse.CommandError(client, $"Please provide a character instance id"));
                    return;
                }
            }

            if (!int.TryParse(command[2], out int y))
            {
                responses.Add(ChatResponse.CommandError(client, $"Please provide a value to test"));
                return;
            }


            switch (command[0])
            {
                case "hp":
                    IBuffer res = BufferProvider.Provide();
                    res.WriteInt32(y);
                    character2.currentHp = y;
                    Router.Send(client, (ushort)AreaPacketId.recv_chara_update_hp, res, ServerType.Area);
                    break;

                case "dead":
                    SendBattleReportStartNotify(client, character2);
                    //recv_battle_report_noact_notify_dead = 0xCDC9,
                    IBuffer res2 = BufferProvider.Provide();
                    res2.WriteInt32(character2.InstanceId);
                    res2.WriteInt32(y); // death type? 1 = death, 2 = death and message, 3 = unconscious, beyond that = nothing
                    res2.WriteInt32(0);
                    res2.WriteInt32(0);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_noact_notify_dead, res2, ServerType.Area);
                    SendBattleReportEndNotify(client, character2);
                    break;

                case "pose":
                    IBuffer res3 = BufferProvider.Provide();
                    //recv_battle_attack_pose_start_notify = 0x7CB2,
                    res3.WriteInt32(y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_attack_pose_start_notify, res3, ServerType.Area);
                    break;

                case "pose2":
                    IBuffer res4 = BufferProvider.Provide();
                    res4.WriteInt32(character2.InstanceId);//Character ID
                    res4.WriteInt32(y); //Character pose
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_chara_pose_notify, res4, ServerType.Area, client);

                    break;

                case "emotion":
                    //recv_emotion_notify_type = 0xF95B,
                    IBuffer res5 = BufferProvider.Provide();
                    res5.WriteInt32(character2.InstanceId);
                    res5.WriteInt32(y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_emotion_notify_type, res5, ServerType.Area);
                    break;

                case "deadstate":
                    //recv_charabody_notify_deadstate = 0xCC36, // Parent = 0xCB94 // Range ID = 03
                    IBuffer res6 = BufferProvider.Provide();
                    res6.WriteInt32(character2.InstanceId);
                    res6.WriteInt32(y);//4 here causes a cloud and the model to disappear, 5 causes a mist to happen and disappear
                    res6.WriteInt32(y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_charabody_notify_deadstate, res6, ServerType.Area);
                    break;

                case "start":
                    SendBattleReportStartNotify(client, character2);
                    IBuffer res7 = BufferProvider.Provide();
                    //recv_battle_report_action_item_enchant = 0x6BDC,
                    res7.WriteInt32(517);
                    res7.WriteInt32(y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_action_item_enchant, res7, ServerType.Area);
                    SendBattleReportEndNotify(client, character2);
                    break;

                case "end":
                    IBuffer res8 = BufferProvider.Provide();
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_end_notify, res8, ServerType.Area);
                    break;

                case "gimmick":
                    //recv_data_notify_gimmick_data = 0xBFE9,
                    IBuffer res9 = BufferProvider.Provide();
                    res9.WriteInt32(y); //Gimmick instance id
                    res9.WriteFloat(client.Character.X + 100);
                    res9.WriteFloat(client.Character.Y);
                    res9.WriteFloat(client.Character.Z);
                    res9.WriteByte(client.Character.Heading);
                    res9.WriteInt32(y); //Gimmick number (from gimmick.csv)
                    res9.WriteInt32(0); //Gimmick State
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_data_notify_gimmick_data, res9, ServerType.Area);
                    break;

                case "bodystate":
                    //recv_charabody_state_update_notify = 0x1A0F, 
                    IBuffer res10 = BufferProvider.Provide();
                    res10.WriteInt32(character2.InstanceId);
                    res10.WriteInt32(y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_charabody_state_update_notify, res10, ServerType.Area);
                    break;

                case "charastate":
                    //recv_chara_notify_stateflag = 0x23D3, 
                    IBuffer res11 = BufferProvider.Provide();
                    res11.WriteInt32(character2.InstanceId);
                    res11.WriteInt32(y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_chara_notify_stateflag, res11, ServerType.Area);
                    break;

                case "spirit":
                    //recv_charabody_notify_spirit = 0x36A6, 
                    IBuffer res12 = BufferProvider.Provide();
                    res12.WriteInt32(character2.InstanceId);
                    res12.WriteByte((byte)y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_charabody_notify_spirit, res12, ServerType.Area);
                    break;

                case "abyss":
                    //recv_charabody_self_notify_abyss_stead_pos = 0x679B,
                    IBuffer res13 = BufferProvider.Provide();
                    res13.WriteFloat(character2.X);
                    res13.WriteFloat(character2.Y);
                    res13.WriteFloat(character2.Z);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_charabody_self_notify_abyss_stead_pos, res13, ServerType.Area);
                    break;

                case "charadata":
                    RecvDataNotifyCharaData cData = new RecvDataNotifyCharaData(character2, client.Soul.Name/*Should be client of supplied instanceID. this is a band-aid*/);
                    Router.Send(Server.Clients.GetAll(), cData.ToPacket());
                    break;

                case "charabodydata":
                    //recv_data_notify_charabody_data = 0x906A,
                    IBuffer res14 = BufferProvider.Provide();
                    res14.WriteInt32(character2.InstanceId + 10000); //Instance ID of dead body
                    res14.WriteInt32(character2.InstanceId); //Reference to actual player's instance ID
                    res14.WriteCString("soulname"); // Soul name 
                    res14.WriteCString($"{character2.Name}"); // Character name
                    res14.WriteFloat(character2.X + 200); // X
                    res14.WriteFloat(character2.Y); // Y
                    res14.WriteFloat(character2.Z); // Z
                    res14.WriteByte(character2.Heading); // Heading
                    res14.WriteInt32(0);

                    int numEntries = 19;
                    res14.WriteInt32(numEntries);//less than or equal to 19
                    for (int i = 0; i < numEntries; i++)
                    {
                        res14.WriteInt32(0);
                    }

                    numEntries = 19;
                    res14.WriteInt32(numEntries);
                    for (int i = 0; i < numEntries; i++)
                    {
                        res14.WriteInt32(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);
                        res14.WriteInt32(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);

                        res14.WriteByte(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);//bool
                        res14.WriteByte(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);
                        res14.WriteByte(0);
                    }

                    numEntries = 19;
                    res14.WriteInt32(numEntries);
                    for (int i = 0; i < numEntries; i++)
                    {
                        res14.WriteInt32(0);
                    }

                    res14.WriteInt32(0); //Race ID
                    res14.WriteInt32(0); //Gender ID
                    res14.WriteByte(0); //Hair style
                    res14.WriteByte(0); //Hair color
                    res14.WriteByte(0); //Face id
                    res14.WriteInt32(1);// 0 = bag, 1 for dead? (Can't enter soul form if this isn't 0 or 1 i think).
                    res14.WriteInt32(0);//4 = ash pile, not sure what this is.
                    res14.WriteInt32(0);
                    res14.WriteInt32(y); //death pose 0 = faced down, 1 = head chopped off, 2 = no arm, 3 = faced down, 4 = chopped in half, 5 = faced down, 6 = faced down, 7 and up "T-pose" the body (ONLY SEND 1 IF YOU ARE CALLING THIS FOR THE FIRST TIME)
                    res14.WriteByte(0);//crim status (changes icon on the end also), 0 = white, 1 = yellow, 2 = red, 3 = red with crim icon, 
                    res14.WriteByte(0);// (bool) Beginner protection
                    res14.WriteInt32(1);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_data_notify_charabody_data, res14, ServerType.Area);
                    break;

                case "scaleopen":
                    IBuffer res0 = BufferProvider.Provide();
                    res0.WriteInt32(0); //1 = cinematic, 0 Just start the event without cinematic
                    res0.WriteByte(0);
                    Router.Send(client, (ushort)AreaPacketId.recv_event_start, res0, ServerType.Area);

                    IBuffer res15 = BufferProvider.Provide();
                    //recv_raisescale_view_open = 0xC25D, // Parent = 0xC2E5 // Range ID = 01  // was 0xC25D
                    res15.WriteInt16(1); //Basic revival rate %
                    res15.WriteInt16(5); //Penalty %
                    res15.WriteInt16((short)y); //Offered item % (this probably changes with recv_raisescale_update_success_per)
                    res15.WriteInt16(4); //Dimento medal addition %
                    Router.Send(client, (ushort)AreaPacketId.recv_raisescale_view_open, res15, ServerType.Area);
                    break;

                case "event":
                    IBuffer res16 = BufferProvider.Provide();
                    //recv_event_start = 0x1B5C, 
                    res16.WriteInt32(0);
                    res16.WriteByte(0);
                    Router.Send(client, (ushort)AreaPacketId.recv_event_start, res16, ServerType.Area);

                    IBuffer res17 = BufferProvider.Provide();
                    //recv_event_quest_report = 0xE07E,
                    res17.WriteInt32(0);
                    Router.Send(client, (ushort)AreaPacketId.recv_event_quest_report, res17, ServerType.Area);

                    IBuffer res18 = BufferProvider.Provide();
                    //recv_event_block_message_end_no_object = 0x1AB,
                    //Router.Send(client, (ushort)AreaPacketId.recv_event_block_message_end_no_object, res18, ServerType.Area);

                    IBuffer res19 = BufferProvider.Provide();
                    Router.Send(client, (ushort)AreaPacketId.recv_event_sync, res19, ServerType.Area);
                    break;

                case "popup":
                    IBuffer res22 = BufferProvider.Provide();
                    //recv_event_start = 0x1B5C, 
                    res22.WriteInt32(0);
                    res22.WriteByte((byte)y);
                    //Router.Send(client, (ushort)AreaPacketId.recv_event_start, res22, ServerType.Area);

                    IBuffer res21 = BufferProvider.Provide();
                    //recv_normal_system_message = 0xAE2B,
                    res21.WriteCString("ToBeFound");
                    Router.Send(client, (ushort)AreaPacketId.recv_normal_system_message, res21, ServerType.Area);

                    IBuffer res23 = BufferProvider.Provide();
                    //recv_event_end = 0x99D, 
                    res23.WriteByte((byte)y);
                    //Router.Send(client, (ushort)AreaPacketId.recv_event_end, res23, ServerType.Area);
                    break;

                case "recv":
                    IBuffer res24 = BufferProvider.Provide();
                    //recv_auction_receive_item_r = 0xB1CA,
                    res24.WriteInt32(y);
                    Router.Send(client, (ushort)AreaPacketId.recv_auction_receive_gold_r, res24, ServerType.Area);
                    break;

                case "string":
                    IBuffer res26 = BufferProvider.Provide();
                    //recv_charabody_notify_loot_item = 0x8CDE, // Parent = 0x8CC6 // Range ID = 01
                    res26.WriteByte(0);
                    res26.WriteByte(0);
                    res26.WriteInt16(0);

                    res26.WriteInt16((short)y);
                    res26.WriteCString("adad"); // Length 0x31 
                    res26.WriteCString("adad"); // Length 0x5B
                    Router.Send(client, (ushort)AreaPacketId.recv_charabody_notify_loot_item, res26, ServerType.Area);
                    break;

                case "ac":
                    IBuffer res27 = BufferProvider.Provide();
                    //recv_item_update_ac 
                    res27.WriteInt64(10200101);
                    res27.WriteInt16((short)y);
                    Router.Send(client, (ushort)AreaPacketId.recv_item_update_ac, res27, ServerType.Area);
                    break;

                case "alignment":
                    IBuffer res28 = BufferProvider.Provide();
                    //recv_chara_update_alignment_param = 0xB435,
                    res28.WriteInt32(1);
                    res28.WriteInt32(2);
                    res28.WriteInt32(3);
                    Router.Send(client, (ushort)AreaPacketId.recv_chara_update_alignment_param, res28, ServerType.Area);
                    break;

                case "shop":
                    IBuffer res29 = BufferProvider.Provide();
                    //recv_shop_notify_open = 0x52FD, // Parent = 0x5243 // Range ID = 02
                    res29.WriteInt16((short)y); //Shop type
                    res29.WriteInt32(0);
                    res29.WriteInt32(0);
                    res29.WriteByte(0);
                    Router.Send(client, (ushort)AreaPacketId.recv_shop_notify_open, res29, ServerType.Area);
                    break;

                case "dura":

                    IBuffer res30 = BufferProvider.Provide();
                    res30.WriteInt64(10200101);
                    res30.WriteInt32(y); // MaxDura points
                    Router.Send(client, (ushort)AreaPacketId.recv_item_update_maxdur, res30, ServerType.Area);

                    //recv_item_update_durability = 0x1F5A, 
                    IBuffer res31 = BufferProvider.Provide();
                    res31.WriteInt64(10200101);
                    res31.WriteInt32(y-1);
                    Router.Send(client, (ushort)AreaPacketId.recv_item_update_durability, res31, ServerType.Area);
                    break;

                case "sc":
                    IBuffer res32 = BufferProvider.Provide();
                    //recv_shop_sell_check_r = 0x4E8D,
                    res32.WriteInt32(0);
                    Router.Send(client, (ushort)AreaPacketId.recv_shop_sell_check_r, res32, ServerType.Area);
                    break;

                case "view":
                    IBuffer res33 = BufferProvider.Provide();
                    //recv_chara_view_landing_notify = 0x14DA, 
                    res33.WriteInt32(y);
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_chara_view_landing_notify, res33, ServerType.Area);
                    break;

                case "itemnum":
                    IBuffer res34 = BufferProvider.Provide();
                    //recv_item_update_num = 0x5F8, 
                    res34.WriteInt64(10200101);
                    res34.WriteByte(25);
                    Router.Send(client, (ushort)AreaPacketId.recv_item_update_num, res34, ServerType.Area);
                    break;

                case "damage":
                    int hp = character2.currentHp;
                    client.Character.damage(hp, character2.InstanceId);
                    IBuffer res35 = BufferProvider.Provide();
                    res35.WriteInt32(hp);
                    Router.Send(client, (ushort)AreaPacketId.recv_chara_update_hp, res35, ServerType.Area);
                    break;

                case "union":
                    IBuffer res36 = BufferProvider.Provide();
                    res36.WriteInt32(client.Character.InstanceId);
                    res36.WriteInt32(8888 /*client.Character.UnionId*/);
                    res36.WriteCString("Trade_Union"); 
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_chara_notify_union_data, res36, ServerType.Area);
                    break;

                case "xunion":
                    IBuffer res37 = BufferProvider.Provide();
                    res37.WriteInt32(client.Character.InstanceId);
                    res37.WriteInt32(0 /*client.Character.UnionId*/);
                    res37.WriteCString(""); 
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_chara_notify_union_data, res37, ServerType.Area);
                    break;

                case "itemforth":
                    IBuffer res38 = BufferProvider.Provide();
                    res38.WriteInt32(client.Character.InstanceId); //item ID?
                    res38.WriteInt32(10200101); //Owner going 'forth'  id?
                    res38.WriteInt32(client.Character.InstanceId); //item state setting?
                    Router.Send(client.Map, (ushort)AreaPacketId.recv_chara_update_notify_item_forth, res38, ServerType.Area);
                    break;

                case "disconnect":
                    NecClient DeadManClient = Server.Clients.GetByCharacterInstanceId(x);
                    IBuffer res39 = BufferProvider.Provide();
                    res39.WriteInt32(client.Character.InstanceId);
                    res39.WriteInt32(client.Character.InstanceId);
                    res39.WriteInt32(client.Character.InstanceId);
                    res39.WriteInt32(client.Character.InstanceId);
                    res39.WriteInt32(client.Character.InstanceId);
                    res39.WriteInt32(client.Character.InstanceId);
                    res39.WriteInt32(client.Character.InstanceId);
                    Router.Send(DeadManClient, (ushort)AreaPacketId.recv_chara_update_notify_item_forth, res39, ServerType.Area);
                    break;

                case "crime":
                    //for (byte i = 0; i < y; i++)
                    {
                        NecClient crimeClient = Server.Clients.GetByCharacterInstanceId(x);
                        IBuffer res40 = BufferProvider.Provide();
                        res40.WriteInt32(crimeClient.Character.InstanceId);
                        res40.WriteByte((byte)y);
                        client.Character.criminalState = (byte)y;
                        Logger.Debug($"Setting crime level for Character {crimeClient.Character.Name} to {y}");
                        Router.Send(crimeClient, (ushort)AreaPacketId.recv_chara_update_notify_crime_lv, res40, ServerType.Area);
                        Router.Send(crimeClient.Map, (ushort)AreaPacketId.recv_charabody_notify_crime_lv, res40, ServerType.Area, crimeClient);
                        Thread.Sleep(400);
                    }
                    break;

                case "inherit":
                    //for (byte i = 0; i < y; i++)
                    {
                        NecClient inheritClient = Server.Clients.GetByCharacterInstanceId(x);
                        IBuffer res41 = BufferProvider.Provide();
                        res41.WriteInt32(y);
                        res41.WriteInt32(0x64);//less than or equal to 0x64
                        for (int i = 0; i < 0x64; i++) //limit is the int32 above
                        {
                            res41.WriteInt32(i);
                            res41.WriteFixedString("127.0.0.1", 0x10); //size is 0x10
                        }
                        res41.WriteInt32(client.Character.InstanceId);
                        res41.WriteFixedString("127.0.0.1", 0x10); //size is 0x10
                        res41.WriteByte((byte)y);
                        Router.Send(inheritClient, (ushort)MsgPacketId.recv_chara_get_inheritinfo_r, res41, ServerType.Msg);

                        Thread.Sleep(400);
                    }
                    break;

                default:
                    Logger.Error($"There is no recv of type : {command[0]} ");
                    break;
            }
        }

        public override AccountStateType AccountState => AccountStateType.User;
        public override string Key => "chara";
        public override string HelpText => "usage: `/chara [command] [instance id] [y]` - Fires a recv to the game of command type with [instance id] as target and [y] number as an argument.";

        private void SendBattleReportStartNotify(NecClient client, IInstance instance)
        {
            IBuffer res4 = BufferProvider.Provide();
            res4.WriteInt32(instance.InstanceId);
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_start_notify, res4, ServerType.Area);
        }
        private void SendBattleReportEndNotify(NecClient client, IInstance instance)
        {
            IBuffer res4 = BufferProvider.Provide();
            Router.Send(client.Map, (ushort)AreaPacketId.recv_battle_report_end_notify, res4, ServerType.Area);
        }
    }

}
