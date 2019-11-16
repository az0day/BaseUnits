using System;

namespace ZZXUnit.Tests.Entities.SpiderOdds
{
    [Serializable]
    public enum ThirdSportEnum
    {
        None = -1,

        Soccer = 0, // S
        Basketball = 1, // BB
        AmericanFootball = 2, // UF
        Hockey = 3, // HK
        Tennis = 4, // TN // ONLY LIVE 

        Baseball = 5, // BE
        ESports = 6, // ES // ONLY LIVE 
        Rugby = 7, // RB
        RugbyUnion = 8, // RU

        TableTennis = 9, // ONLY LIVE
        Boxing = 10,
        MixedMartialArts = 11,
        Golf = 12,
        Badminton = 13,
        Snooker = 14,
        Volleyball = 15,

        MuayThai = 16, // MT // 有滚球，5 回合
        Cricket = 17, // CK // 有显示比分

        // No Live
        BeachSoccer = 18, // BC
        Cycling = 19, // CY
        Darts = 20, // DT
        Enterainment = 21, // ET
        Handball = 22, // HB
        WaterPolo = 23, // WP
        Squash = 24, // QQ
        WinterSport = 25, // WI

        IceHockey, // IH
        FinancialBets, // FB
        Pool, // PL
        Billiard, // BL
        MotoGP, // GP
        Athletics, // AT
        Archery, // AR
        Chess, // CH
        Diving, // DV
        Equestrian, // EQ
        Entertainment, // ET
        Canoeing, // CN
        CombatSports, // CS
        Gymnastics, // GM
        FloorBall, // FL
        Novelties, // NT
        Olympic, // OL
        Others, // OT
        Politics, // PO
        Swimming, // MN
        Weightlifting, // WG
        WinterSports, // WI
        Speedway, // WS
    }
}
