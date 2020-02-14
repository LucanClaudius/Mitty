using Newtonsoft.Json;
using System;

namespace Mitty.Osu.Api
{
    public struct ScoreData
    {
        [JsonProperty("beatmap_id")]
        public string BeatmapId { get; private set; }
        [JsonProperty("score_id")]
        public long ScoreId { get; private set; }
        [JsonProperty("score")]
        public int Score { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("maxcombo")]
        public int MaxCombo { get; private set; }
        [JsonProperty("count50")]
        public int Count50 { get; private set; }
        [JsonProperty("count100")]
        public int Count100 { get; private set; }
        [JsonProperty("count300")]
        public int Count300 { get; private set; }
        [JsonProperty("countmiss")]
        public int CountMiss { get; private set; }
        [JsonProperty("countkatu")]
        public int CountKatu { get; private set; }
        [JsonProperty("countgeki")]
        public int CountGeki { get; private set; }
        [JsonProperty("perfect")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool Perfect { get; private set; }
        [JsonProperty("enabled_mods")]
        public int EnabledMods { get; private set; }
        [JsonProperty("user_id")]
        public int UserId { get; private set; }
        [JsonProperty("date")]
        public DateTime Date { get; private set; }
        [JsonProperty("rank")]
        public string Rank { get; private set; }
        [JsonProperty("pp")]
        public float PP { get; private set; }
        [JsonProperty("replay_available")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool ReplayAvailable { get; private set; }
    }
}
