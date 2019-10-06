using Arrowgene.Services.Logging;
using Arrowgene.Services.Networking.Tcp.Server.AsyncEvent;
using Necromancy.Server.Database;
using Necromancy.Server.Logging;
using Necromancy.Server.Model;
using Necromancy.Server.Packet;
using Necromancy.Server.Packet.Area;
using Necromancy.Server.Packet.Auth;
using Necromancy.Server.Packet.Msg;
using Necromancy.Server.Setting;

namespace Necromancy.Server
{
    public class NecServer
    {
        public NecSetting Setting { get; }
        public PacketRouter Router { get; }
        public SessionManager Sessions { get; }
        public ClientLookup Clients { get; set; }
        public MapLookup Map { get; set; }
        public IDatabase Database { get; set; }

        private readonly NecQueueConsumer _authConsumer;
        private readonly NecQueueConsumer _msgConsumer;
        private readonly NecQueueConsumer _areaConsumer;
        private readonly AsyncEventServer _authServer;
        private readonly AsyncEventServer _msgServer;
        private readonly AsyncEventServer _areaServer;
        private readonly NecLogger _logger;

        public NecServer(NecSetting setting)
        {
            _logger = LogProvider.Logger<NecLogger>(this);
            Setting = new NecSetting(setting);
            Clients = new ClientLookup();
            Map = new MapLookup();
            Sessions = new SessionManager();
            Router = new PacketRouter();
            Database = new NecDatabaseBuilder().Build(Setting.DatabaseSettings);
            _authConsumer = new NecQueueConsumer(Setting);
            _authConsumer.SetIdentity("Auth");
            _authConsumer.ClientDisconnected += AuthClientDisconnected;
            _msgConsumer = new NecQueueConsumer(Setting);
            _msgConsumer.SetIdentity("Msg");
            _msgConsumer.ClientDisconnected += MsgClientDisconnected;
            _areaConsumer = new NecQueueConsumer(Setting);
            _areaConsumer.SetIdentity("Area");
            _areaConsumer.ClientDisconnected += AreaClientDisconnected;

            _authServer = new AsyncEventServer(
                Setting.ListenIpAddress,
                Setting.AuthPort,
                _authConsumer
            );

            _msgServer = new AsyncEventServer(
                Setting.ListenIpAddress,
                Setting.MsgPort,
                _msgConsumer
            );

            _areaServer = new AsyncEventServer(
                Setting.ListenIpAddress,
                Setting.AreaPort,
                _areaConsumer
            );

            LoadHandler();
        }

        private void AuthClientDisconnected(NecClient client)
        {
        }

        private void MsgClientDisconnected(NecClient client)
        {
        }

        private void AreaClientDisconnected(NecClient client)
        {
        }

        public void Start()
        {
            _authServer.Start();
            _msgServer.Start();
            _areaServer.Start();
        }

        public void Stop()
        {
            _authServer.Stop();
            _msgServer.Stop();
            _areaServer.Stop();
        }

        private void LoadHandler()
        {
            // Authentication Handler
            _authConsumer.AddHandler(new send_base_check_version_auth(this));
            _authConsumer.AddHandler(new send_base_authenticate(this));
            _authConsumer.AddHandler(new send_base_get_worldlist(this));
            _authConsumer.AddHandler(new send_base_select_world(this));

            // Message Handler
            _msgConsumer.AddHandler(new send_base_check_version_msg(this));
            _msgConsumer.AddHandler(new send_base_login(this));
            _msgConsumer.AddHandler(new send_cash_buy_premium(this));
            _msgConsumer.AddHandler(new send_cash_get_url_common(this));
            _msgConsumer.AddHandler(new send_cash_get_url_common_steam(this));
            _msgConsumer.AddHandler(new send_cash_update(this));
            _msgConsumer.AddHandler(new send_channel_select(this));
            _msgConsumer.AddHandler(new send_chara_create(this));
            _msgConsumer.AddHandler(new send_chara_delete(this));
            _msgConsumer.AddHandler(new send_chara_draw_bonuspoint(this));
            _msgConsumer.AddHandler(new send_chara_get_createinfo(this));
            _msgConsumer.AddHandler(new send_chara_get_inheritinfo(this));
            _msgConsumer.AddHandler(new send_chara_get_list(this));
            _msgConsumer.AddHandler(new send_chara_select(this));
            _msgConsumer.AddHandler(new send_chara_select_back(this));
            _msgConsumer.AddHandler(new send_chara_select_back_soul_select(this));
            _msgConsumer.AddHandler(new send_friend_reply_to_link2(this));
            _msgConsumer.AddHandler(new send_friend_request_delete_friend(this));
            _msgConsumer.AddHandler(new send_friend_request_link_target(this));
            _msgConsumer.AddHandler(new send_friend_accept_request_link(this));
            _msgConsumer.AddHandler(new send_friend_request_load(this));
            _msgConsumer.AddHandler(new send_soul_authenticate_passwd(this));
            _msgConsumer.AddHandler(new send_soul_create(this));
            _msgConsumer.AddHandler(new send_soul_delete(this));
            _msgConsumer.AddHandler(new send_soul_rename(this));
            _msgConsumer.AddHandler(new send_soul_select(this));
            _msgConsumer.AddHandler(new send_soul_select_C44F(this));
            _msgConsumer.AddHandler(new send_soul_set_passwd(this));
            _msgConsumer.AddHandler(new send_system_register_error_report(this));
            _msgConsumer.AddHandler(new send_skill_request_info(this));
            _msgConsumer.AddHandler(new send_union_reply_to_invite2(this));
            _msgConsumer.AddHandler(new send_union_request_change_role(this));
            _msgConsumer.AddHandler(new Send_union_request_detail(this));
            _msgConsumer.AddHandler(new send_union_request_disband(this));
            _msgConsumer.AddHandler(new send_union_request_expel_member(this));
            _msgConsumer.AddHandler(new send_union_request_invite_target(this));
            _msgConsumer.AddHandler(new send_union_request_member_priv(this));
            _msgConsumer.AddHandler(new send_union_request_news(this));
            _msgConsumer.AddHandler(new send_union_request_secede(this));
            _msgConsumer.AddHandler(new send_union_request_set_info(this));
            _msgConsumer.AddHandler(new send_union_request_set_mantle(this));


            // Area Handler
            _areaConsumer.AddHandler(new send_base_check_version_area(this));
            _areaConsumer.AddHandler(new send_base_enter(this));
            _areaConsumer.AddHandler(new send_data_get_self_chara_data_request(this));
            _areaConsumer.AddHandler(new send_map_entry(this));
            _areaConsumer.AddHandler(new send_map_get_info(this));
            _areaConsumer.AddHandler(new send_map_enter(this));
            _areaConsumer.AddHandler(new send_sv_conf_option_change(this));
            _areaConsumer.AddHandler(new send_map_change_force_r(this));
            //_areaConsumer.AddHandler(new send_shortcut_request_data(this));
            //_areaConsumer.AddHandler(new send_skill_request_info(this));
            _areaConsumer.AddHandler(new send_sv_conf_option_request(this));
            //_areaConsumer.AddHandler(new send_get_refusallist(this));
            //_areaConsumer.AddHandler(new send_quest_get_mission_quest_works(this));
            //_areaConsumer.AddHandler(new send_quest_get_soul_mission_quest_works(this));
            //_areaConsumer.AddHandler(new send_quest_get_story_quest_works(this));
            //_areaConsumer.AddHandler(new send_party_request_draw_item_list(this));
            _areaConsumer.AddHandler(new send_battle_attack_pose(this));
            _areaConsumer.AddHandler(new send_battle_release_attack_pose(this));
            _areaConsumer.AddHandler(new send_battle_attack_start(this));
            _areaConsumer.AddHandler(new send_battle_attack_exec(this));
            _areaConsumer.AddHandler(new send_skill_request_gain(this));
            _areaConsumer.AddHandler(new send_quest_get_mission_quest_history(this));
            _areaConsumer.AddHandler(new send_quest_get_story_quest_history(this));
            _areaConsumer.AddHandler(new send_quest_get_soul_mission_quest_history(this));
            _areaConsumer.AddHandler(new send_cash_shop_open_by_menu(this));
            _areaConsumer.AddHandler(new send_stall_deregist_item(this));
            _areaConsumer.AddHandler(new send_stall_set_name(this));
            _areaConsumer.AddHandler(new send_logout_start_request(this));
            _areaConsumer.AddHandler(new send_logout_cancel_request(this));
            _areaConsumer.AddHandler(new send_event_access_object(this));
            _areaConsumer.AddHandler(new send_chat_post_message(this));
            _areaConsumer.AddHandler(new send_chara_pose(this));
            _areaConsumer.AddHandler(new send_chara_pose_ladder(this));
            _areaConsumer.AddHandler(new send_comment_set(this));
            _areaConsumer.AddHandler(new send_comment_switch(this));
            _areaConsumer.AddHandler(new send_refusallist_add_user(this));
            _areaConsumer.AddHandler(new send_blacklist_open(this));
            _areaConsumer.AddHandler(new send_emotion_update_type(this));
            _areaConsumer.AddHandler(new send_blacklist_close(this));
            _areaConsumer.AddHandler(new send_blacklist_clear(this));
            _areaConsumer.AddHandler(new send_blacklist_lock(this));
            _areaConsumer.AddHandler(new send_blacklist_unlock(this));
            _areaConsumer.AddHandler(new send_wanted_entry(this));
            _areaConsumer.AddHandler(new send_cmd_exec(this));
            _areaConsumer.AddHandler(new send_charabody_access_start(this));
            _areaConsumer.AddHandler(new send_shortcut_request_regist(this));
            _areaConsumer.AddHandler(new send_skill_start_cast(this));
            _areaConsumer.AddHandler(new send_skill_exec(this));
            _areaConsumer.AddHandler(new send_character_view_offset(this));
            _areaConsumer.AddHandler(new send_soul_dispitem_request_data(this));
            _areaConsumer.AddHandler(new send_movement_info(this));
            _areaConsumer.AddHandler(new send_battle_attack_next(this));
            _areaConsumer.AddHandler(new send_battle_guard_start(this));
            _areaConsumer.AddHandler(new send_battle_guard_end(this));
            _areaConsumer.AddHandler(new send_skill_cast_cancel_request(this));
            _areaConsumer.AddHandler(new send_loot_access_object(this));
            _areaConsumer.AddHandler(new send_open_mailbox(this));
            _areaConsumer.AddHandler(new send_create_package(this));
            _areaConsumer.AddHandler(new send_select_package_update(this));
            _areaConsumer.AddHandler(new send_package_all_delete(this));
            _areaConsumer.AddHandler(new send_item_unequip(this));
            _areaConsumer.AddHandler(new send_item_sort(this));
            _areaConsumer.AddHandler(new send_item_drop(this));
            _areaConsumer.AddHandler(new send_item_equip(this));
            _areaConsumer.AddHandler(new send_random_box_close(this));
            _areaConsumer.AddHandler(new send_stall_regist_item(this));
            _areaConsumer.AddHandler(new send_stall_set_item_price(this));
            _areaConsumer.AddHandler(new send_stall_open(this));
            _areaConsumer.AddHandler(new send_stall_close(this));
            _areaConsumer.AddHandler(new send_stall_shopping_start(this));
            _areaConsumer.AddHandler(new send_stall_shopping_abort(this));
            _areaConsumer.AddHandler(new send_item_move(this));
            _areaConsumer.AddHandler(new send_party_regist_party_recruit(this));
            _areaConsumer.AddHandler(new send_party_establish(this));
            _areaConsumer.AddHandler(new send_trade_invite(this));
            _areaConsumer.AddHandler(new send_trade_reply(this));
            _areaConsumer.AddHandler(new send_trade_set_money(this));
            _areaConsumer.AddHandler(new send_trade_add_item(this));
            _areaConsumer.AddHandler(new send_trade_remove_item(this));
            _areaConsumer.AddHandler(new send_trade_offer(this));
            _areaConsumer.AddHandler(new send_trade_revert(this));
            _areaConsumer.AddHandler(new send_trade_abort(this));
            _areaConsumer.AddHandler(new send_trade_fix(this));
            _areaConsumer.AddHandler(new send_shop_close(this));
            _areaConsumer.AddHandler(new send_shop_identify(this));
            _areaConsumer.AddHandler(new send_shop_sell_check(this));
            _areaConsumer.AddHandler(new send_shop_sell(this));
            _areaConsumer.AddHandler(new Send_shop_repair(this));
            _areaConsumer.AddHandler(new send_temple_cure_curse(this));
            _areaConsumer.AddHandler(new send_temple_close(this));
            _areaConsumer.AddHandler(new send_auction_search(this));
            _areaConsumer.AddHandler(new send_auction_close(this));
            _areaConsumer.AddHandler(new send_auction_bid(this));
            _areaConsumer.AddHandler(new send_auction_cancel_bid(this));
            _areaConsumer.AddHandler(new send_auction_exhibit(this));
            _areaConsumer.AddHandler(new send_auction_cancel_exhibit(this));
            _areaConsumer.AddHandler(new send_storage_deposit_money(this));
            _areaConsumer.AddHandler(new send_storage_draw_money(this));
            _areaConsumer.AddHandler(new send_storage_deposit_item(this));
            _areaConsumer.AddHandler(new send_storage_draw_money(this));
            _areaConsumer.AddHandler(new send_wanted_jail_close(this));
            _areaConsumer.AddHandler(new send_wanted_jail_draw_point(this));
            _areaConsumer.AddHandler(new send_wanted_jail_payment(this));
            _areaConsumer.AddHandler(new send_wanted_list_close(this));
            _areaConsumer.AddHandler(new send_union_request_establish(this));
            _areaConsumer.AddHandler(new send_union_mantle_close(this));
            _areaConsumer.AddHandler(new send_union_close_window(this));
            _areaConsumer.AddHandler(new send_quest_abort(this));
            _areaConsumer.AddHandler(new send_quest_check_target(this));
            _areaConsumer.AddHandler(new send_gem_close(this));
            _areaConsumer.AddHandler(new send_event_sync_r(this));
            _areaConsumer.AddHandler(new Send_event_soul_storage_close(this));
            _areaConsumer.AddHandler(new send_event_removetrap_select(this));
            _areaConsumer.AddHandler(new send_event_removetrap_end(this));
            _areaConsumer.AddHandler(new send_event_tresurebox_end(this));
            _areaConsumer.AddHandler(new send_event_soul_rankup_close(this));
            _areaConsumer.AddHandler(new send_event_select_channel_r(this));
            _areaConsumer.AddHandler(new send_event_quest_order_r(this));
            _areaConsumer.AddHandler(new send_event_script_play_r(this));
            _areaConsumer.AddHandler(new send_event_select_exec_r(this));
            _areaConsumer.AddHandler(new send_event_request_int_r(this));
            _areaConsumer.AddHandler(new send_charabody_self_salvage_notify_r(this));
        }
    }
}