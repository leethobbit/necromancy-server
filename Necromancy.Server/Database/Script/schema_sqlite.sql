CREATE TABLE IF NOT EXISTS `setting` (
  `key`   TEXT NOT NULL,
  `value` TEXT NOT NULL,
  PRIMARY KEY (`key`)
);

CREATE TABLE IF NOT EXISTS `account` (
  `id`               INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
  `name`             TEXT                              NOT NULL,
  `normal_name`      TEXT                              NOT NULL,
  `hash`             TEXT                              NOT NULL,
  `mail`             TEXT                              NOT NULL,
  `mail_verified`    INTEGER                           NOT NULL,
  `mail_verified_at` DATETIME DEFAULT NULL,
  `mail_token`       TEXT     DEFAULT NULL,
  `password_token`   TEXT     DEFAULT NULL,
  `state`            INTEGER                           NOT NULL,
  `last_login`       DATETIME DEFAULT NULL,
  `created`          DATETIME                          NOT NULL,
  CONSTRAINT `uq_account_name` UNIQUE (`name`),
  CONSTRAINT `uq_account_normal_name` UNIQUE (`normal_name`),
  CONSTRAINT `uq_account_mail` UNIQUE (`mail`)
);

CREATE TABLE IF NOT EXISTS `nec_soul` (
  `id`         INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
  `account_id` INTEGER                           NOT NULL,
  `name`       TEXT                              NOT NULL,
  `level`      INTEGER                           NOT NULL,
  `created`    DATETIME                          NOT NULL,  
  `password`   TEXT     DEFAULT NULL,
  CONSTRAINT `fk_nec_soul_account_id` FOREIGN KEY (`account_id`) REFERENCES `account` (`id`),
  CONSTRAINT `uq_nec_soul_name` UNIQUE (`name`)
);

CREATE TABLE IF NOT EXISTS `nec_skilltree_item` (
  `id`         INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
  `skill_id`   INTEGER                           NOT NULL,
  `char_id`    INTEGER                           NOT NULL,
  `level`      INTEGER                           NOT NULL
);

CREATE TABLE IF NOT EXISTS `nec_shortcut_bar` (
  `id`    INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
  `slot0` INTEGER                           NOT NULL,
  `slot1` INTEGER                           NOT NULL,
  `slot2` INTEGER                           NOT NULL,
  `slot3` INTEGER                           NOT NULL,
  `slot4` INTEGER                           NOT NULL,
  `slot5` INTEGER                           NOT NULL,
  `slot6` INTEGER                           NOT NULL,
  `slot7` INTEGER                           NOT NULL,
  `slot8` INTEGER                           NOT NULL,
  `slot9` INTEGER                           NOT NULL,
  `action0` INTEGER                           NOT NULL,
  `action1` INTEGER                           NOT NULL,
  `action2` INTEGER                           NOT NULL,
  `action3` INTEGER                           NOT NULL,
  `action4` INTEGER                           NOT NULL,
  `action5` INTEGER                           NOT NULL,
  `action6` INTEGER                           NOT NULL,
  `action7` INTEGER                           NOT NULL,
  `action8` INTEGER                           NOT NULL,
  `action9` INTEGER                           NOT NULL
);

CREATE TABLE IF NOT EXISTS `nec_character` (
  `id`                INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
  `account_id`        INTEGER                           NOT NULL,
  `soul_id`           INTEGER                           NOT NULL,
  `slot`              INTEGER                           NOT NULL,
  `map_id`            INTEGER                           NOT NULL,
  `x`                 REAL                              NOT NULL,
  `y`                 REAL                              NOT NULL,
  `z`                 REAL                              NOT NULL,
  `name`              TEXT                              NOT NULL,
  `race_id`           INTEGER                           NOT NULL,
  `sex_id`            INTEGER                           NOT NULL,
  `hair_id`           INTEGER                           NOT NULL,
  `hair_color_id`     INTEGER                           NOT NULL,
  `face_id`           INTEGER                           NOT NULL,
  `alignment_id`      INTEGER                           NOT NULL,
  `strength`          INTEGER                           NOT NULL,
  `vitality`          INTEGER                           NOT NULL,
  `dexterity`         INTEGER                           NOT NULL,
  `agility`           INTEGER                           NOT NULL,
  `intelligence`      INTEGER                           NOT NULL,
  `piety`             INTEGER                           NOT NULL,
  `luck`              INTEGER                           NOT NULL,
  `class_id`          INTEGER                           NOT NULL,
  `level`             INTEGER                           NOT NULL,
  `shortcut_bar0_id`  INTEGER                           NOT NULL,
  `shortcut_bar1_id`  INTEGER                           NOT NULL,
  `shortcut_bar2_id`  INTEGER                           NOT NULL,
  `shortcut_bar3_id`  INTEGER                           NOT NULL,
  `shortcut_bar4_id`  INTEGER                           NOT NULL,
  `created`           DATETIME                          NOT NULL,
  CONSTRAINT `fk_nec_character_account_id` FOREIGN KEY (`account_id`) REFERENCES `account` (`id`),
  CONSTRAINT `fk_nec_character_soul_id` FOREIGN KEY (`soul_id`) REFERENCES `nec_soul` (`id`),
  CONSTRAINT `uq_nec_character_soul_id_name` UNIQUE (`soul_id`, `name`),
  CONSTRAINT `uq_nec_character_soul_id_slot` UNIQUE (`soul_id`, `slot`)
);

CREATE TABLE IF NOT EXISTS `nec_npc_spawn` (
  `id`         INTEGER PRIMARY KEY NOT NULL, 
  `npc_id`     INTEGER             NOT NULL,
  `model_id`   INTEGER             NOT NULL,
  `level`      INTEGER             NOT NULL,
  `name`       TEXT                NOT NULL,
  `title`      TEXT                NOT NULL,
  `map_id`     INTEGER             NOT NULL,
  `x`          REAL                NOT NULL,
  `y`          REAL                NOT NULL,
  `z`          REAL                NOT NULL,
  `active`     INTEGER             NOT NULL,
  `heading`    INTEGER             NOT NULL,
  `size`       INTEGER             NOT NULL,
  `visibility` INTEGER             NOT NULL,
  `created`    DATETIME            NOT NULL,
  `updated`    DATETIME            NOT NULL,
  `icon`       INTEGER             NOT NULL,
  `status`     INTEGER             NOT NULL,
  `status_x`   INTEGER             NOT NULL,
  `status_y`   INTEGER             NOT NULL,
  `status_z`   INTEGER             NOT NULL
);

CREATE TABLE IF NOT EXISTS `nec_monster_coords` (
  `id`         INTEGER PRIMARY KEY NOT NULL, 
  `monster_id` INTEGER             NOT NULL,
  `map_id`     INTEGER             NOT NULL,
  `coord_idx`  INTEGER             NOT NULL,
  `x`          REAL                NOT NULL,
  `y`          REAL                NOT NULL,
  `z`          REAL                NOT NULL
);

CREATE TABLE IF NOT EXISTS `nec_monster_spawn` (
  `id`         INTEGER PRIMARY KEY NOT NULL, 
  `monster_id` INTEGER             NOT NULL,
  `model_id`   INTEGER             NOT NULL,
  `level`      INTEGER             NOT NULL,
  `name`       TEXT                NOT NULL,
  `title`      TEXT                NOT NULL,
  `map_id`     INTEGER             NOT NULL,
  `x`          REAL                NOT NULL,
  `y`          REAL                NOT NULL,
  `z`          REAL                NOT NULL,
  `active`     INTEGER             NOT NULL,
  `heading`    INTEGER             NOT NULL,
  `size`       INTEGER             NOT NULL,
  `created`    DATETIME            NOT NULL,
  `updated`    DATETIME            NOT NULL
);

CREATE TABLE IF NOT EXISTS `nec_item` (
  `id`   INTEGER PRIMARY KEY NOT NULL, 
  `name` TEXT                NOT NULL
);