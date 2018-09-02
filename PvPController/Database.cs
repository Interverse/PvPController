using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using PvPController.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace PvPController {
    public class Database {
        public Dictionary<int, ItemInfo> itemInfo = new Dictionary<int, ItemInfo>(Main.maxItemTypes);
        public Dictionary<int, ItemInfo> projectileInfo = new Dictionary<int, ItemInfo>(Main.maxProjectileTypes);
        public Dictionary<int, ItemInfo> buffInfo = new Dictionary<int, ItemInfo>(Main.maxBuffTypes);

        private IDbConnection _db;

        public Database(IDbConnection db) {
            _db = db;
            var sqlCreator = new SqlTableCreator(_db,
                _db.GetSqlType() == SqlType.Sqlite
                    ? (IQueryBuilder)new SqliteQueryCreator()
                    : new MysqlQueryCreator());

            var table1 = new SqlTable("Items",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("VanillaDamage", MySqlDbType.Int32),
                new SqlColumn("ModdedDamage", MySqlDbType.Int32),
                new SqlColumn("Shoot", MySqlDbType.Int32),
                new SqlColumn("Defense", MySqlDbType.Int32),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table1);

            var table2 = new SqlTable("Projectiles",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("Damage", MySqlDbType.Int32),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table2);

            var table3 = new SqlTable("Buffs",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table3);
        }

        public static Database InitDb(string name) {
            IDbConnection db;
            if (TShock.Config.StorageType.ToLower() == "sqlite")
                db =
                    new SqliteConnection(string.Format("uri=file://{0},Version=3",
                        Path.Combine(TShock.SavePath, name + ".sqlite")));
            else if (TShock.Config.StorageType.ToLower() == "mysql") {
                try {
                    var host = TShock.Config.MySqlHost.Split(':');
                    db = new MySqlConnection {
                        ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}",
                            host[0],
                            host.Length == 1 ? "3306" : host[1],
                            TShock.Config.MySqlDbName,
                            TShock.Config.MySqlUsername,
                            TShock.Config.MySqlPassword
                            )
                    };
                } catch (MySqlException x) {
                    TShock.Log.Error(x.ToString());
                    throw new Exception("MySQL not setup correctly.");
                }
            } else
                throw new Exception("Invalid storage type.");
            var database = new Database(db);
            return database;
        }

        public QueryResult QueryReader(string query, params object[] args) {
            return _db.QueryReader(query, args);
        }

        public int Query(string query, params object[] args) {
            return _db.Query(query, args);
        }

        public void UpdateItems(ItemInfo iteminfo) {
            var query =
                string.Format(
                    "UPDATE Items SET Name = '{0}', VanillaDamage = {1}, ModdedDamage = {2}, Shoot = {3}, Defense = {4}, InflictBuffID = {5}, InflictBuffDuration = {6}, ReceiveBuffID = {7}, ReceiveBuffDuration = {8} WHERE ID = @0",
                    MiscUtils.SanitizeString(iteminfo.name), iteminfo.vanillaDamage, iteminfo.damage, iteminfo.shoot, iteminfo.defense, iteminfo.debuff.buffid, iteminfo.debuff.buffDuration, iteminfo.selfBuff.buffid, iteminfo.selfBuff.buffDuration);
                
            Query(query, iteminfo.id);
        }

        public void UpdateProjectiles(ItemInfo iteminfo) {
            var query =
                string.Format(
                    "UPDATE Projectiles SET Name = '{0}', Damage = {1}, InflictBuffID = {2}, InflictBuffDuration = {3}, ReceiveBuffID = {4}, ReceiveBuffDuration = {5} WHERE ID = @0",
                    MiscUtils.SanitizeString(iteminfo.name), iteminfo.damage, iteminfo.debuff.buffid, iteminfo.debuff.buffDuration, iteminfo.selfBuff.buffid, iteminfo.selfBuff.buffDuration);
            
            Query(query, iteminfo.id);
        }

        public void UpdateBuffs(ItemInfo iteminfo) {
            var query =
                string.Format(
                    "UPDATE Buffs SET Name = '{0}', InflictBuffID = {2}, InflictBuffDuration = {3}, ReceiveBuffID = {4}, ReceiveBuffDuration = {5} WHERE ID = @0",
                    MiscUtils.SanitizeString(iteminfo.name), iteminfo.debuff.buffid, iteminfo.debuff.buffDuration, iteminfo.selfBuff.buffid, iteminfo.selfBuff.buffDuration);
            
            Query(query, iteminfo.id);
        }

        public void InitDefaultTables() {
            var conn = new SqliteConnection(_db.ConnectionString);

            conn.Open();

            using (var cmd = new SqliteCommand(conn)) {
                using (var transaction = conn.BeginTransaction()) {
                    for (int x = 0; x < Main.maxItemTypes; x++) {
                        Item item = new Item();
                        item.SetDefaults(x);

                        string name = MiscUtils.SanitizeString(item.Name);

                        int damage = item.damage;
                        int defense = item.defense;
                        int shoot = item.shoot;

                        cmd.CommandText =
                            "INSERT INTO Items (ID, Name, VanillaDamage, ModdedDamage, Shoot, Defense, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})"
                            .SFormat(x, name, damage, damage, shoot, defense, 0, 0, 0, 0);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxProjectileTypes; x++) {
                        string name = MiscUtils.SanitizeString(Lang.GetProjectileName(x).Value);
                        
                        int damage = 0;
                        int inflictBuff = 0;
                        int inflictBuffDuration = 0;

                        if (MiscData.projectileDamage.ContainsKey(x)) {
                            damage = MiscData.projectileDamage[x];
                        }
                        if (MiscData.projectileDebuffs.ContainsKey(x)) {
                            inflictBuff = MiscData.projectileDebuffs[x].buffid;
                            inflictBuffDuration = MiscData.projectileDebuffs[x].buffDuration;
                        }

                        cmd.CommandText = 
                            "INSERT INTO Projectiles (ID, Name, Damage, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6})"
                            .SFormat(x, name, damage, inflictBuff, inflictBuffDuration, 0, 0);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxBuffTypes; x++) {
                        string name = MiscUtils.SanitizeString(Lang.GetBuffName(x));

                        int inflictBuff = 0;
                        int inflictBuffDuration = 0;
                        if (MiscData.flaskDebuffs.ContainsKey(x)) {
                            inflictBuff = MiscData.flaskDebuffs[x].buffid;
                            inflictBuffDuration = MiscData.flaskDebuffs[x].buffDuration;
                        }

                        cmd.CommandText = 
                            "INSERT INTO Buffs (ID, Name, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration) VALUES ({0}, '{1}', {2}, {3}, {4}, {5})"
                            .SFormat(x, name, inflictBuff, inflictBuffDuration, 0, 0);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            conn.Close();
        }

        public void LoadDatabase() {
            using (var reader = QueryReader("SELECT * FROM Items")) {
                while (reader.Read()) {
                    int id = reader.Get<int>("ID");
                    itemInfo[id] = new ItemInfo();
                    itemInfo[id].id = id;
                    itemInfo[id].name = reader.Get<string>("Name");
                    itemInfo[id].vanillaDamage = reader.Get<int>("VanillaDamage");
                    itemInfo[id].damage = reader.Get<int>("ModdedDamage");
                    itemInfo[id].shoot = reader.Get<int>("Shoot");
                    itemInfo[id].defense = reader.Get<int>("Defense");
                    itemInfo[id].debuff = new BuffDuration(reader.Get<int>("InflictBuffID"), reader.Get<int>("InflictBuffDuration"));
                    itemInfo[id].selfBuff = new BuffDuration(reader.Get<int>("ReceiveBuffID"), reader.Get<int>("ReceiveBuffDuration"));
                }
            }

            using (var reader = QueryReader("SELECT * FROM Projectiles")) {
                while (reader.Read()) {
                    int id = reader.Get<int>("ID");
                    projectileInfo[id] = new ItemInfo();
                    projectileInfo[id].id = id;
                    projectileInfo[id].name = reader.Get<string>("Name");
                    projectileInfo[id].damage = reader.Get<int>("Damage");
                    projectileInfo[id].debuff = new BuffDuration(reader.Get<int>("InflictBuffID"), reader.Get<int>("InflictBuffDuration"));
                    projectileInfo[id].selfBuff = new BuffDuration(reader.Get<int>("ReceiveBuffID"), reader.Get<int>("ReceiveBuffDuration"));
                }
            }

            using (var reader = QueryReader("SELECT * FROM Buffs")) {
                while (reader.Read()) {
                    var id = reader.Get<int>("ID");
                    buffInfo[id] = new ItemInfo();
                    buffInfo[id].id = id;
                    buffInfo[id].name = reader.Get<string>("Name");
                    buffInfo[id].debuff = new BuffDuration(reader.Get<int>("InflictBuffID"), reader.Get<int>("InflictBuffDuration"));
                    buffInfo[id].selfBuff = new BuffDuration(reader.Get<int>("ReceiveBuffID"), reader.Get<int>("ReceiveBuffDuration"));
                }
            }
        }
    }
}
