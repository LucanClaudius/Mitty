using Newtonsoft.Json;
using System;

namespace Mitty.Osu.Api
{
    public struct BeatmapData
    {
        [JsonProperty("approved")]
        public int Approved { get; private set; }
        [JsonProperty("submit_date")]
        public DateTime SubmitDate { get; private set; }
        [JsonProperty("approved_date")]
        public DateTime ApprovedDate { get; private set; }
        [JsonProperty("last_update")]
        public DateTime LastUpdate { get; private set; }
        [JsonProperty("artist")]
        public string Artist { get; private set; }
        [JsonProperty("beatmap_id")]
        public int BeatmapId { get; private set; }
        [JsonProperty("beatmapset_id")]
        public int BeatmapSetId { get; private set; }
        [JsonProperty("bpm")]
        public decimal bpm { get; private set; }
        [JsonProperty("creator")]
        public string Creator { get; private set; }
        [JsonProperty("creator_id")]
        public int CreatorId { get; private set; }
        [JsonProperty("difficultyrating")]
        public decimal DifficultyRating { get; private set; }
        [JsonProperty("diff_aim")]
        public decimal DiffAim { get; private set; }
        [JsonProperty("diff_speed")]
        public decimal DiffSpeed { get; private set; }
        [JsonProperty("diff_size")]
        public float DiffSize { get; private set; }
        [JsonProperty("diff_overall")]
        public float DiffOverall { get; private set; }
        [JsonProperty("diff_approach")]
        public float DiffApproach { get; private set; }
        [JsonProperty("diff_drain")]
        public float DiffDrain { get; private set; }
        [JsonProperty("hit_length")]
        public int HitLength { get; private set; }
        [JsonProperty("source")]
        public string Source { get; private set; }
        [JsonProperty("genre_id")]
        public int GenreId { get; private set; }
        [JsonProperty("language_id")]
        public int LanguageId { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("total_length")]
        public int TotalLength { get; private set; }
        [JsonProperty("version")]
        public string Version { get; private set; }
        [JsonProperty("file_md5")]
        public string FileMD5 { get; private set; }
        [JsonProperty("mode")]
        public int Mode { get; private set; }
        [JsonProperty("tags")]
        public string Tags { get; private set; }
        [JsonProperty("favorite_count")]
        public int FavoriteCount { get; private set; }
        [JsonProperty("rating")]
        public float Rating { get; private set; }
        [JsonProperty("playcount")]
        public int Playcount { get; private set; }
        [JsonProperty("passcount")]
        public int Passcount { get; private set; }
        [JsonProperty("count_normal")]
        public int CountNormal { get; private set; }
        [JsonProperty("count_slider")]
        public int CountSlider { get; private set; }
        [JsonProperty("count_spinner")]
        public int CountSpinner { get; private set; }
        [JsonProperty("max_combo")]
        public int MaxCombo { get; private set; }
        [JsonProperty("download_unavailable")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool DownloadUnavailable { get; private set; }
        [JsonProperty("audio_unavailable")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool AudioUnavailable { get; private set; }
    }
}
