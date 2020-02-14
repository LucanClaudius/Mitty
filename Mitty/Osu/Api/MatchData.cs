using Newtonsoft.Json;
using System;

namespace Mitty.Osu.Api
{
    public struct MatchData
    {
        [JsonProperty("match")]
        public MultiPlayerMatch Match { get; private set; }
        [JsonProperty("games")]
        public MultiPlayerGame[] Games { get; private set; }
    }

    public struct MultiPlayerMatch
    {
        [JsonProperty("match_id")]
        public int MatchId { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("start_time")]
        public DateTime StartTime { get; private set; }
        [JsonProperty("end_time")]
        public DateTime EndTime { get; private set; }
    }

    public struct MultiPlayerGame
    {
        [JsonProperty("game_id")]
        public string GameId { get; private set; }
        [JsonProperty("start_time")]
        public DateTime StartTime { get; private set; }
        [JsonProperty("end_time")]
        public DateTime EndTime { get; private set; }
        [JsonProperty("beatmap_id")]
        public string BeatmapId { get; private set; }
        [JsonProperty("play_mode")]
        public int Gamemode { get; private set; }
        [JsonProperty("match_type")]
        public int MatchId { get; private set; }
        [JsonProperty("scoring_type")]
        public int ScoringType { get; private set; }
        [JsonProperty("team_type")]
        public int TeamType { get; private set; }
        [JsonProperty("mods")]
        public int Mods { get; private set; }
        [JsonProperty("scores")]
        public MultiPlayerScore[] Scores { get; private set; }
    }

    public struct MultiPlayerScore
    {
        [JsonProperty("slot")]
        public int Slot { get; private set; }
        [JsonProperty("team")]
        public int Team { get; private set; }
        [JsonProperty("user_id")]
        public int UserId { get; private set; }
        [JsonProperty("score")]
        public int Score { get; private set; }
        [JsonProperty("maxcombo")]
        public int MaxCombo { get; private set; }
        [JsonProperty("rank")]
        public int Rank { get; private set; }
        [JsonProperty("count50")]
        public int Count50 { get; private set; }
        [JsonProperty("count100")]
        public int Count100 { get; private set; }
        [JsonProperty("count300")]
        public int Count300 { get; private set; }
        [JsonProperty("countmiss")]
        public int CountMiss { get; private set; }
        [JsonProperty("countgeki")]
        public int CountGeki { get; private set; }
        [JsonProperty("countkatu")]
        public int CountKatu { get; private set; }
        [JsonProperty("perfect")]
        public int Perfect { get; private set; }
        [JsonProperty("pass")]
        public int Pass { get; private set; }
        [JsonProperty("enabled_mods")]
        public int EnabledMods { get; private set; }
    }
}
