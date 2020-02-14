using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Mitty
{
    public class Database
    {
        private static MySqlConnection Connection = new MySqlConnection(Bot.configJson["ConnectionString"]);
        private static void Dispose() => Connection.Dispose();

        public static async Task<string> GetOsuUser(ulong discordId)
        {
            await Connection.OpenAsync();
            var cmd = Connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM userlink WHERE discordid = '{discordId}'";
            var result = await ReadUserLinkAsync(cmd.ExecuteReader());
            await Connection.CloseAsync();
            return result.Count > 0 ? result[0].OsuName : null;
        }

        public static async Task SetOsuUser(ulong discordId, string osuName)
        {
            await Connection.OpenAsync();
            var cmd = Connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO userlink (discordid, osuname) VALUES ('{discordId}', '{osuName}') ON DUPLICATE KEY UPDATE osuname = '{osuName}'";
            await cmd.ExecuteNonQueryAsync();
            await Connection.CloseAsync();
        }

        public static async Task<string> GetLastMapId(ulong channelId)
        {
            await Connection.OpenAsync();
            var cmd = Connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM comparemap WHERE channelid = '{channelId}'";
            var result = await ReadCompareMapAsync(cmd.ExecuteReader());
            await Connection.CloseAsync();
            return result.Count > 0 ? result[0].MapId : null;
        }

        public static async Task SetLastMap(ulong channelId, int beatmapId)
        {
            await Connection.OpenAsync();
            var cmd = Connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO comparemap (channelid, mapid) VALUES ('{channelId}', '{beatmapId}') ON DUPLICATE KEY UPDATE mapid = '{beatmapId}'";
            await cmd.ExecuteNonQueryAsync();
            await Connection.CloseAsync();
        }

        private static async Task<List<DatabaseUser>> ReadUserLinkAsync(DbDataReader reader)
        {
            var result = new List<DatabaseUser>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new DatabaseUser()
                    {
                        DiscordId = reader.GetString(0),
                        OsuName = reader.GetString(1)
                    };
                    result.Add(post);
                }
            }
            return result;
        }

        private static async Task<List<CompareMap>> ReadCompareMapAsync(DbDataReader reader)
        {
            var result = new List<CompareMap>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new CompareMap()
                    {
                        ChannelId = reader.GetString(0),
                        MapId = reader.GetString(1)
                    };
                    result.Add(post);
                }
            }
            return result;
        }

        private struct DatabaseUser
        {
            public string DiscordId;
            public string OsuName;
        }

        private struct CompareMap
        {
            public string ChannelId;
            public string MapId;
        }
    }
}
