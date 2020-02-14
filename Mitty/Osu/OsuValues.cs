using System;

namespace Mitty.Osu
{
    [Flags]
    public enum Mods
    {
        None = 0,
        NoFail = 1,
        Easy = 2,
        TouchDevice = 4,
        Hidden = 8,
        HardRock = 16,
        SuddenDeath = 32,
        DoubleTime = 64,
        Relax = 128,
        HalfTime = 256,
        Nightcore = 512,
        Flashlight = 1024,
        Autoplay = 2048,
        SpunOut = 4096,
        Relax2 = 8192,
        Perfect = 16384,
        Key4 = 32768,
        Key5 = 65536,
        Key6 = 131072,
        Key7 = 262144,
        Key8 = 524288,
        FadeIn = 1048576,
        Random = 2097152,
        Cinema = 4194304,
        Target = 8388608,
        Key9 = 16777216,
        KeyCoop = 33554432,
        Key1 = 67108864,
        Key3 = 134217728,
        Key2 = 268435456,
        ScoreV2 = 536870912,
        Mirror = 1073741824,
        KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
        FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
        ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn,
        StarChangeMods = Easy | HardRock | DoubleTime | HalfTime | Nightcore
    }

    [Flags]
    enum ModsShort
    {
        None = 0,
        NF = 1,
        EZ = 2,
        TD = 4,
        HD = 8,
        HR = 16,
        SD = 32,
        DT = 64,
        RX = 128,
        HT = 256,
        NC = 512,
        FL = 1024,
        Auto = 2048,
        SO = 4096,
        AP = 8192,
        PF = 16384,
        K4 = 32768,
        K5 = 65536,
        K6 = 131072,
        K7 = 262144,
        K8 = 524288,
        FI = 1048576,
        Random = 2097152,
        Cinema = 4194304,
        Target = 8388608,
        K9 = 16777216,
        KeyCoop = 33554432,
        K1 = 67108864,
        K3 = 134217728,
        K2 = 268435456,
        ScoreV2 = 536870912,
        Mirror = 1073741824,
        KeyMod = K1 | K2 | K3 | K4 | K5 | K6 | K7 | K8 | K9 | KeyCoop,
        FreeModAllowed = NF | EZ | HD | HR | SD | FL | FI | RX | AP | SO | KeyMod,
        ScoreIncreaseMods = HD | HR | DT | FL | FI,
        StarChangeMods = EZ | HR | DT | HT | NC
    }

    enum ApprovedState
    {
        Graveyarded = -2,
        WIP = -1,
        Pending = 0,
        Ranked = 1,
        Approved = 2,
        Qualified = 3,
        Loved = 4,
        RankedState = Ranked | Approved,
        LeaderboardState = Ranked | Approved | Qualified | Loved
    }

    enum Gamemodes
    {
        osu = 0,
        Taiko = 1,
        CtB = 2,
        Mania = 3
    }

    enum GamemodesShort
    {
        o = 0,
        t = 1,
        c = 2,
        m = 3
    }

    enum GenreId
    {
        any = 0,
        unspecified = 1,
        videoGame = 2,
        anime = 3,
        rock = 4,
        pop = 5,
        other = 6,
        novelty = 7,
        hipHop = 9,
        electronic = 10
    }

    enum LanguageId
    {
        any,
        other,
        english,
        japanese,
        chinese,
        instrumental,
        korean,
        french,
        german,
        swedish,
        spanish,
        italian
    }
}