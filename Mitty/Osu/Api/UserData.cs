using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mitty.Osu.Api
{
    struct UserData
    {
        [JsonProperty("user_id")]
        public int UserId { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("join_date")]
        public DateTime JoinDate { get; private set; }
        [JsonProperty("count300")]
        public int Count300 { get; private set; }
        [JsonProperty("count100")]
        public int Count100 { get; private set; }
        [JsonProperty("count50")]
        public int Count50 { get; private set; }
        [JsonProperty("playcount")]
        public int Playcount { get; private set; }
        [JsonProperty("ranked_score")]
        public long RankedScore { get; private set; }
        [JsonProperty("total_score")]
        public long TotalScore { get; private set; }
        [JsonProperty("pp_rank")]
        public int PPRank { get; private set; }
        [JsonProperty("level")]
        public float Level { get; private set; }
        [JsonProperty("pp_raw")]
        public float PPRaw { get; private set; }
        [JsonProperty("accuracy")]
        public decimal Accuracy { get; private set; }
        [JsonProperty("count_rank_ss")]
        public int CountRankSS { get; private set; }
        [JsonProperty("count_rank_ssh")]
        public int CountRankSSH { get; private set; }
        [JsonProperty("count_rank_s")]
        public int CountRankS { get; private set; }
        [JsonProperty("count_rank_sh")]
        public int CountRankSH { get; private set; }
        [JsonProperty("count_rank_a")]
        public int CountRankA { get; private set; }
        [JsonProperty("country")]
        public string Country { get; private set; }
        [JsonProperty("total_seconds_played")]
        public int TotalSecondsPlayed { get; private set; }
        [JsonProperty("pp_country_rank")]
        public int PPCountryRank { get; private set; }
        [JsonProperty("events")]
        public Dictionary<string, string>[] Events { get; private set; }
    }
}
