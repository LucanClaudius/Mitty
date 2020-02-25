using Mitty.Commands;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mitty.Osu.Api
{
    static class OsuApi
    {
        private static readonly string apiUrl = "https://osu.ppy.sh/api/";
        private static readonly string keyParameter = "?k=" + Bot.configJson["OsuToken"];
        private static readonly string gamemodeParameter = "&m=";
        private static readonly string userParameter = "&u=";
        private static readonly string limitParameter = "&limit=";
        private static readonly string typeParameter = "&type=";
        private static readonly string modsParameter = "&mods=";
        private static readonly string beatmapIdParameter = "&b=";
        private static readonly string eventDaysParameter = "&event_days=";
        private static readonly string matchIdParameter = "&mp=";
        private static readonly string includeConvertParameter = "&a=";

        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss",
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static async Task<ScoreData[]> Recent(Input input)
        {
            string url = await FormatBaseScoreUrl(input, "get_user_recent");
            string html = await Mitty.Api.Call(url);

            ScoreData[] scores = JsonConvert.DeserializeObject<ScoreData[]>(html, settings);

            if (scores.Length == 0)
                throw new Exception("No recent score found.");

            if (input.IsPassOnly)
                return Array.FindAll(scores, score => score.Rank != "F");
            else
                return scores;
        }

        public static async Task<ScoreData[]> Best(Input input)
        {
            string url = await FormatBaseScoreUrl(input, "get_user_best");
            string html = await Mitty.Api.Call(url);

            ScoreData[] scores = JsonConvert.DeserializeObject<ScoreData[]>(html, settings);

            if (scores.Length == 0)
                throw new Exception("No score found.");

            if (input.IsRecentSorting)
                return scores.OrderByDescending(x => x.Date).ToArray();
            else
                return scores;
        }

        public static async Task<ScoreData[]> UserTop(Input input)
        {
            OsuUser osuUser = await CheckOsuUser(input);

            string url = apiUrl +
                "get_user_best" +
                keyParameter +
                gamemodeParameter + input.Gamemode +
                userParameter + osuUser.Identifier +
                typeParameter + osuUser.Type +
                limitParameter + "100";
            string html = await Mitty.Api.Call(url);

            ScoreData[] scores = JsonConvert.DeserializeObject<ScoreData[]>(html, settings);

            if (scores.Length == 0)
                throw new Exception("No score found.");

            return scores;
        }

        public static async Task<ScoreData[]> Score(Input input)
        {
            if (string.IsNullOrEmpty(input.BeatmapId))
                throw new Exception("No beatmap ID provided");

            string _modsParameter = "";

            if (input.ModRequestNumber.HasValue)
                _modsParameter = modsParameter + input.ModRequestNumber;

            OsuUser osuUser = await CheckOsuUser(input);

            string url = apiUrl +
                "get_scores" +
                keyParameter +
                gamemodeParameter + input.Gamemode +
                userParameter + osuUser.Identifier +
                typeParameter + osuUser.Type +
                beatmapIdParameter + input.BeatmapId +
                _modsParameter;

            string html = await Mitty.Api.Call(url);

            ScoreData[] scores = JsonConvert.DeserializeObject<ScoreData[]>(html, settings);

            if (scores.Length == 0)
                throw new Exception("No score found.");

            if (input.IsRecentSorting)
                return scores.OrderByDescending(x => x.Date).ToArray();
            else
                return scores;
        }

        public static async Task<ScoreData[]> Leaderboard(string beatmapId, int gamemode)
        {
            string url = apiUrl +
                "get_scores" +
                keyParameter +
                gamemodeParameter + gamemode +
                beatmapIdParameter + beatmapId +
                limitParameter + 100;

            string html = await Mitty.Api.Call(url);

            return JsonConvert.DeserializeObject<ScoreData[]>(html, settings);
        }

        public static async Task<UserData> User(Input input)
        {
            OsuUser osuUser = await CheckOsuUser(input);

            string url = apiUrl +
                "get_user" +
                keyParameter +
                gamemodeParameter + input.Gamemode +
                userParameter + osuUser.Identifier +
                typeParameter + osuUser.Type +
                eventDaysParameter + 31;

            string html = await Mitty.Api.Call(url);

            UserData[] userData = JsonConvert.DeserializeObject<UserData[]>(html, settings);

            if (userData.Length == 0)
                throw new Exception("User not found");

            return userData[0];
        }

        public static async Task<UserData> User(string osuName)
        {
            string url = apiUrl +
                "get_user" +
                keyParameter +
                userParameter + osuName +
                typeParameter + "string";

            string html = await Mitty.Api.Call(url);

            UserData[] userData = JsonConvert.DeserializeObject<UserData[]>(html, settings);

            if (userData.Length == 0)
                throw new Exception("User not found");

            return userData[0];
        }

        public static async Task<UserData> User(int osuId)
        {
            string url = apiUrl +
                "get_user" +
                keyParameter +
                userParameter + osuId +
                typeParameter + "id";

            string html = await Mitty.Api.Call(url);

            UserData[] userData = JsonConvert.DeserializeObject<UserData[]>(html, settings);

            if (userData.Length == 0)
                throw new Exception("User not found");

            return userData[0];
        }

        public static async Task<BeatmapData[]> Beatmap(string beatmapId, int modRequestNumber, int gamemode)
        {
            string url = apiUrl +
                "get_beatmaps" +
                keyParameter +
                gamemodeParameter + gamemode +
                beatmapIdParameter + beatmapId +
                modsParameter + modRequestNumber +
                includeConvertParameter + "1";

            string html = await Mitty.Api.Call(url);

            BeatmapData[] beatmapData = JsonConvert.DeserializeObject<BeatmapData[]>(html, settings);

            if (beatmapData.Length == 0)
                throw new Exception("Beatmap not found");

            return beatmapData;
        }

        public static async Task<MatchData> Match(string matchId)
        {
            string url = apiUrl +
                "get_match" +
                keyParameter +
                matchIdParameter + matchId;

            string html = await Mitty.Api.Call(url);
            return JsonConvert.DeserializeObject<MatchData>(html, settings);
        }

        private static async Task<string> FormatBaseScoreUrl(Input input, string api)
        {
            string limit;

            if (input.IsPassOnly || input.IsRecentSorting && input.Command != "top")
                limit = "50";
            else if (input.IsRecentSorting && input.Command == "top")
                limit = "100";
            else if (input.IsList)
                limit = (input.Page * 5).ToString();
            else
                limit = input.Page.ToString();

            OsuUser osuUser = await CheckOsuUser(input);

            return apiUrl +
                api +
                keyParameter +
                gamemodeParameter + input.Gamemode +
                userParameter + osuUser.Identifier +
                typeParameter + osuUser.Type +
                limitParameter + limit;
        }

        private static async Task<OsuUser> CheckOsuUser(Input input)
        {
            if (!string.IsNullOrEmpty(input.AnyString))
            {
                return new OsuUser()
                {
                    Identifier = input.AnyString,
                    Type = "string"
                };
            }
            else
            {
                string osuName = await Database.GetOsuUser(input.DiscordUserId);

                if (string.IsNullOrEmpty(osuName))
                    throw new Exception("No user provided");

                var osuUser = new OsuUser()
                {
                    Identifier = osuName,
                    Type = "id"
                };

                return osuUser;
            }
        }

        private struct OsuUser
        {
            public string Identifier;
            public string Type;
        }
    }
}
