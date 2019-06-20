namespace Necromancy.Server.Packet.Id
{
    /// <summary>
    /// Necromancy Message Server
    /// 
    /// Recv OP Codes: (Area Server Switch: 0x00495B83)
    ///
    /// 0x86EA
    /// 0x3DD0
    /// 0x1F5A
    /// 0x10DA
    /// 0x794
    /// 0x494
    /// 0x2FA
    /// 0x1Da
    /// 0x1A6
    /// 0x1A6 + 0x5
    /// 0xC24E
    /// 0xA7BF
    /// 0x9899
    /// 0x903A
    /// 0x8C2F
    /// 0x88FB
    /// 0x8820
    /// 0x8778
    /// 0x8778 + 0x4
    /// 0xDE90
    /// 0xD170
    /// 0xCB6D
    /// 0xC54F
    /// 0xC3EE
    /// 0xC2A1 (investigate 004C2F8E)
    /// 0xC2A1 + 0xD3 (investigate 004C2F8E)
    /// 0xEFA4
    /// 0xE819
    /// 0xE1F8
    /// 0xE051
    /// 0xDF31
    /// 0xDEB7
    /// 0xDEB7 + 0xB
    /// 0xFA0B
    /// 0xF447
    /// 0xF1A0
    /// 0xEFDD (investigate 004CEC2C)
    /// 0xEFDD + 0x4E (investigate 004CEC2C)
    /// 0xFDB2
    /// 0xFCC0
    /// 0xFC28
    /// 0xFB79
    /// 0xFC1A
    /// 0xFED8
    /// 0xFE2F
    /// 0xFDB8
    /// 0xFDB8 + 0x31
    /// 0xFF00 (investigate 004D13C3)
    /// 0xFF00 + 0xD6 (investigate 004D13C3)
    /// 0xFEB7
    /// 0xFCF3 (investigate 004D042D)
    /// 0xFCF3 + 0x85 (investigate 004D042D)
    /// 0xFC75
    /// 0xFC75 + 0x38
    /// 0xF71C
    /// 0xF633
    /// 0xF495
    /// 0xF602
    /// 0xF95B
    /// 0xF7E7
    /// 0xF7E7 + 0x9
    /// 0xF9F9
    /// 0xF6AF
    /// 0xF393
    /// 0xF212
    /// 0xF330
    /// 0xF3A1
    /// 0xEDA6
    /// 0xEA1C
    /// 0xE897 (investigate 004CC897)
    /// 0xE897 + 0xF9 (investigate 004CC897)
    /// 0xEE43
    /// 0xEDB3 (investigate 004CDE73)
    /// 0xEDB3 + 0x65 (investigate 004CDE73)
    /// 0xEEB7 (investigate 004CE472)
    /// 0xEEB7 + 0xD6 (investigate 004CE472)
    /// 0xECBA
    /// 0xEB47
    /// 0xEB47 + 0x34
    /// 0xED4C
    /// 0xE748
    /// 0xE4B2
    /// 0xE207
    /// 0xE462
    /// 0xE7BB (investigate 004CC261)
    /// 0xE7BB + 0x2C (investigate 004CC261)
    /// 0xE526
    /// 0xE5FF
    /// 0xE07E (investigate 004CB292)
    /// 0xE07E + 0xCD (investigate 004CB292)
    /// 0xE039
    /// 0xE039 + 0x4
    /// 0xD688
    /// 0xD46E
    /// 0xD349
    /// 0xD1CB
    /// 0xD1A9
    /// 0xD1A9 + 0x14
    /// 0xDA4A
    /// 0xD7D8
    /// 0xD68C (investigate 004C8EE5)
    /// 0xD68C + 0xC6 (investigate 004C8EE5)
    /// 0xDBF1
    /// 0xDB5E
    /// 0xDA5C
    /// 0xDB53
    /// 0xDD52
    /// 0xDC5B
    /// 0xDC5B + 0x5B
    /// 0xDDD3
    /// 0xDB88
    /// 0xD909
    /// 0xD804
    /// 0xD8D5
    /// 0xD972
    /// 0xD597
    /// 0xD493 (investigate 004C7D1F)
    /// 0xD493 + 0xE7 (investigate 004C7D1F)
    /// 0xD5B5 (investigate 004C86F2)
    /// 0xD5B5 + 0xC8 (investigate 004C86F2)
    /// 0xD400 (investigate 004C7645)
    /// 0xD400 + 0x3F (investigate 004C7645)
    /// 0xD1F6
    /// 0xD2D6
    /// 0xCF24
    /// 0xCC54
    /// 0xCB94 (investigate 004C5085)
    /// 0xCB94 + 0xA3 (investigate 004C5085)
    /// 0xCFDC
    /// 0xCF29 (investigate 004C5F8E)
    /// 0xCF29 + 0x29 (investigate 004C5F8E)
    /// 0xD04A (investigate 004C6663)
    /// 0xD04A + 0xE9 (investigate 004C6663)
    /// 0xCDC9
    /// 0xCCE2
    /// 0xCD63
    /// 0xCE36
    /// 0xC8AD
    /// 0xC6F2
    /// 0xC8AD
    /// 0xC6F2
    /// 0xC68B
    /// 0xC68B + 0x64
    /// 0xCA35
    /// 0xC96F
    /// 0xC9FF
    /// 0xCAB1
    /// 0xC701
    /// 0xC7E1
    /// 0xC542
    /// 0xC444
    /// 0xC444 + 0x36
    /// 0xC543
    /// 0x840E
    /// 0xAF76
    /// 0xAB14
    /// 0xA90C
    /// 0xA7E8  (investigate 004BB352)
    /// 0xA7E8 + 0xD3 (investigate 004BB352)
    /// 0xBD7E
    /// 0xBA61
    /// 0xB684
    /// 0xB586
    /// 0xB417
    /// 0xB417 + 0x1E
    /// 0xC0BB
    /// 0xBFEA
    /// 0xBF0D
    /// 0xBD90
    /// 0xBD90 + 0x9
    /// 0xC1DC
    /// 0xC0D8 (investigate 004C21A0)
    /// 0xC0D8 + 0xD7 (investigate 004C21A0)
    /// 0xC206 (investigate 004C2984)
    /// 0xC026 + 0x46 (investigate 004C2984)
    /// 0xC003 (investigate 004C1A1E)
    /// 0xC003 + 0x75 (investigate 004C1A1E)
    /// 0xBF34
    /// 0xBFE9
    /// 0xBBA5
    /// 0xBA71 (investigate 004C02F0)
    /// 0xBA71 + 0xF4 (investigate 004C02F0)
    /// 0xBCC2
    /// 0xBC0A
    /// 0xBCAB
    /// 0xBD72
    /// 0xB813
    /// 0xB6B1
    /// 0xB782
    /// 0xBA11
    /// 0xB619
    /// 0xB619 + 0x18
    /// 0xB195
    /// 0xB0BF
    /// 0xAF7F (investigate 004BD4CC)
    /// 0xAF7F + 0xB8 (investigate 004BD4CC)
    /// 0xB319
    /// 0xB1CA (investigate 004BE4A3)
    /// 0xB1CA + 0xC8 (investigate 004BE4A3)
    /// 0xB317 (investigate 004BEB5D)
    /// 0xB317 + 0x86 (investigate 004BEB5D) 
    /// 0xB0E5 (investigate 004BDD95) 
    /// 0xB0E5 + 0x2A (investigate 004BDD95) 
    /// 0xAE27
    /// 0xABF2
    /// 0xAB8A
    /// 0xAB8A + 0x3B
    /// 0xAF49
    /// 0xAE2B
    /// 0xAEE9
    /// 0xAF6D
    /// 0xAD6D
    /// 0xAD6D + 0x5B
    /// 0xAA8F
    /// 0xA938
    /// 0xA9C2
    /// 0xAADD
    /// 0xA084
    /// 0x9D52
    /// 0x9B08
    /// 0x9A44
    /// 0x98D3
    /// 0x998F
    /// 0xA4D3
    /// 0xA21E
    /// 0xA0E3 (investigate 004B98BB) 
    /// 0xA0E3 + 0xAA (investigate 004B98BB) 
    /// 0xA54C
    /// 0xA508 (investigate 004BA68E) 
    /// 0xA508 + 0x41 (investigate 004BA68E) 
    /// 0xA611 (investigate 004BAC6C) 
    /// 0xA611 + 0xE7 (investigate 004BAC6C) 
    /// 0xA45C
    /// 0xA2B7
    /// 0xA43B
    /// 0xA4A5
    /// 0x9F31
    /// 0x9DE2
    /// 0x9D5A
    /// 0x9D5A + 0x3C
    /// 0x9F70 (investigate 004B9266) 
    /// 0x9F70 + 0x95 (investigate 004B9266) 
    /// 0x9E19
    /// 0x9E19 + 0x5C
    /// 0x9CA1
    /// 0x9BC6
    /// 0x9BC6 + 0x74
    /// 0x9CCF
    /// 0x9A79
    /// 0x9A79 + 0x30
    /// 0x9578
    /// 0x924E
    /// 0x919C
    /// 0x906A
    /// 0x906A + 0x7E
    /// 0x9700
    /// 0x9666
    /// 0x95E6
    /// 0x95E6 + 0x78
    /// 0x97D9
    /// 0x971B
    /// 0x971B + 0x46
    /// 0x9870
    /// 0x96EA
    /// 0x935B
    /// 0x9289
    /// 0x9289 + 0x55
    /// 0x951B
    /// 0x9201
    /// 0x9201 + 0x26
    /// 0x8D9B
    /// 0x8CC6 (investigate 004B483D) 
    /// 0x8CC6 + 0xCC (investigate 004B483D) 
    /// 0x8F15
    /// 0x8DBC
    /// 0x8E92
    /// 0x8F84
    /// 0x8BB4
    /// 0x89FF
    /// 0x89FF + 0x7
    /// 0x8BD2
    /// 0x88A1
    /// 0x88A1 + 0xD
    /// -----
    /// 0x662F
    /// 0x531B
    /// 0x4C6F
    /// 0x4440
    /// 0x42B8
    /// 0x3F38
    /// 0x3E0B
    /// 0x3E0B + 0x74
    /// 0x77A7
    /// 0x6FB2
    /// 0x6A7A
    /// 0x6912
    /// 0x68AA
    /// 0x66EC
    /// 0x679B
    /// 0x7FC5
    /// 0x7CF0
    /// 0x7A6F
    /// 0x79A2
    /// 0x789E
    /// 0x793E
    /// 0x839A
    /// 0x825D
    /// 0x8066 (investigate 004B1728) 
    /// 0x8066 + 0x2B (investigate 004B1728) 
    /// 0x855C
    /// 0x8487 (investigate 004B2631) 
    /// 0x8487 + 0xC2 (investigate 004B2631)
    /// 0x85C6 (investigate 004B2C99)
    /// 0x85C6 + 0xDF (investigate 004B2C99)
    /// 0x8299 (investigate 004B1EE6)
    /// 0x8299 + 0xFC (investigate 004B1EE6)
    /// 0x7D53
    /// 0x7D1C (investigate 004B09BA)
    /// 0x7D1C + 0xF (investigate 004B09BA)
    /// 0x7F34
    /// 0x7D75
    /// 0x7F09
    /// 0x7F50
    /// 0x7BB3
    /// 0x7B5D
    /// 0x7B5D + 0x29
    /// 0x7CB2
    /// 0x79A9
    /// 0x7A5C
    /// 
    /// </summary>
    
    public enum AreaPacketId : ushort
    {
        send_base_check_version = 0x5705,
        recv_base_check_version_r = 0x9699,
    }
}