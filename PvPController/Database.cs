using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;
using TShockAPI.DB;

namespace PvPController {
    public static class Database {
        public static Dictionary<int, ItemInfo> itemInfo = new Dictionary<int, ItemInfo>(Main.maxItemTypes);
        public static Dictionary<int, ItemInfo> projectileInfo = new Dictionary<int, ItemInfo>(Main.maxProjectileTypes);
        public static Dictionary<int, ItemInfo> buffInfo = new Dictionary<int, ItemInfo>(Main.maxBuffTypes);
        public static bool isMySql { get { return db.GetSqlType() == SqlType.Mysql; } }

        public static IDbConnection db;

        /// <summary>
        /// Connects the mysql/sqlite file for the plugin, creating one if a file doesn't already exist.
        /// </summary>
        public static void ConnectDB() {
            if (TShock.Config.StorageType.ToLower() == "sqlite")
                db = new SqliteConnection(string.Format("uri=file://{0},Version=3",
                    Path.Combine(TShock.SavePath, "PvPController.sqlite")));
            else if (TShock.Config.StorageType.ToLower() == "mysql") {
                try {
                    var host = TShock.Config.MySqlHost.Split(':');
                    db = new MySqlConnection {
                        ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}",
                            host[0],
                            host.Length == 1 ? "3306" : host[1],
                            TShock.Config.MySqlDbName,
                            TShock.Config.MySqlUsername,
                            TShock.Config.MySqlPassword)
                    };
                } catch (MySqlException x) {
                    TShock.Log.Error(x.ToString());
                    throw new Exception("MySQL not setup correctly.");
                }
            } else
                throw new Exception("Invalid storage type.");

            var sqlCreator = new SqlTableCreator(db,
                isMySql
                    ? (IQueryBuilder)new MysqlQueryCreator()
                    : (IQueryBuilder)new SqliteQueryCreator());
            
            sqlCreator.EnsureTableStructure(new SqlTable("Items",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("Damage", MySqlDbType.Int32),
                new SqlColumn("Shoot", MySqlDbType.Int32),
                new SqlColumn("IsShootModded", MySqlDbType.Int32),
                new SqlColumn("ShootSpeed", MySqlDbType.Float),
                new SqlColumn("Knockback", MySqlDbType.Float),
                new SqlColumn("Defense", MySqlDbType.Int32),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32)));
            
            sqlCreator.EnsureTableStructure(new SqlTable("Projectiles",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("Damage", MySqlDbType.Int32),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32)));

            SqlTable table3 = new SqlTable("Buffs",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table3);
        }

        public static QueryResult QueryReader(string query, params object[] args) {
            return db.QueryReader(query, args);
        }

        public static bool Query(string query) {
            db.Open();
            bool success = true;
            try {
                using (var conn = db.CreateCommand()) {
                    conn.CommandText = query;
                    conn.ExecuteNonQuery();
                }
            } catch {
                success = false;
            }
            db.Close();
            return success;
        }

        /// <summary>
        /// Writes the changed attribute of an item to the sql(ite) file.
        /// </summary>
        public static bool Update<T>(string type, int index, string key, T value) {
            bool selectAll = index <= 0;
            
            if (!selectAll) {
                GetItemInfoByType(type)[index].SetData(key, value);
            } else {
                var iteminfoDict = GetItemInfoByType(type);

                for (int x = 0; x < iteminfoDict.Count; x++) {
                    iteminfoDict[x].SetData(key, value);
                }
            }

            if (value is string) value = (T)Convert.ChangeType("'" + value + "'", typeof(T));

            string sourceID = !selectAll ? " WHERE ID = {0}".SFormat(index) : "";
            return Query(string.Format("UPDATE {0} SET {1} = {2}{3}", type, key, value, sourceID));
        }

        public static Dictionary<int, ItemInfo> GetItemInfoByType(string type) {
            switch (type) {
                case "Items":
                    return itemInfo;

                case "Projectiles":
                    return projectileInfo;

                case "Buffs":
                    return buffInfo;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates all the default stats of items, projectiles, and buffs and puts it into
        /// the mysql/sqlite file.
        /// </summary>
        public static void InitDefaultTables() {
            var conn = isMySql 
                ? (DbConnection)new MySqlConnection(db.ConnectionString) 
                : (DbConnection)new SqliteConnection(db.ConnectionString);

            conn.Open();

            using (var cmd = conn.CreateCommand()) {
                using (var transaction = conn.BeginTransaction()) {
                    cmd.CommandText = "DELETE FROM Items";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM Projectiles";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM Buffs";
                    cmd.ExecuteNonQuery();

                    for (int x = 0; x < Main.maxItemTypes; x++) {
                        Item item = new Item();
                        item.SetDefaults(x);

                        string name = MiscUtils.SanitizeString(item.Name);

                        int damage = item.damage;
                        int defense = item.defense;
                        float knockback = item.knockBack;
                        int shoot = item.useAmmo == AmmoID.None ? item.shoot : -1;

                        cmd.CommandText =
                            "INSERT INTO Items (ID, Name, Damage, Shoot, IsShootModded, ShootSpeed, Knockback, Defense, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration) " +
                            "VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})"
                            .SFormat(x, name, damage, shoot, 0, -1, knockback, defense, 0, 0, 0, 0);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxProjectileTypes; x++) {
                        string name = MiscUtils.SanitizeString(Lang.GetProjectileName(x).Value);

                        int damage = 0;
                        int inflictBuff = 0;
                        int inflictBuffDuration = 0;

                        if (ProjectileUtils.presetProjDamage.ContainsKey(x)) {
                            damage = ProjectileUtils.presetProjDamage[x];
                        }
                        if (ProjectileUtils.projectileDebuffs.ContainsKey(x)) {
                            inflictBuff = ProjectileUtils.projectileDebuffs[x].buffid;
                            inflictBuffDuration = ProjectileUtils.projectileDebuffs[x].buffDuration;
                        }

                        cmd.CommandText =
                            "INSERT INTO Projectiles (ID, Name, Damage, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration) " +
                            "VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6})"
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
                            "INSERT INTO Buffs (ID, Name, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration) " +
                            "VALUES ({0}, '{1}', {2}, {3}, {4}, {5})"
                            .SFormat(x, name, inflictBuff, inflictBuffDuration, 0, 0);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            conn.Close();
        }

        /// <summary>
        /// Loads data from the mysql/sqlite file to the Dictionaries in this Database.
        /// </summary>
        public static void LoadDatabase() {
            using (var reader = QueryReader("SELECT * FROM Items")) {
                while (reader.Read()) {
                    int id = reader.Get<int>("ID");
                    itemInfo[id] = new ItemInfo();
                    itemInfo[id].SetData("ID", id);
                    itemInfo[id].SetData("Name", reader.Get<string>("Name"));
                    itemInfo[id].SetData("Damage", reader.Get<int>("Damage"));
                    itemInfo[id].SetData("Shoot", reader.Get<int>("Shoot"));
                    itemInfo[id].SetData("IsShootModded", reader.Get<int>("IsShootModded") == 1 ? true : false);
                    itemInfo[id].SetData("ShootSpeed", reader.Get<float>("ShootSpeed"));
                    itemInfo[id].SetData("Knockback", reader.Get<float>("Knockback"));
                    itemInfo[id].SetData("Defense", reader.Get<int>("Defense"));
                    itemInfo[id].SetData("Debuff", new BuffDuration(reader.Get<int>("InflictBuffID"), reader.Get<int>("InflictBuffDuration")));
                    itemInfo[id].SetData("SelfBuff", new BuffDuration(reader.Get<int>("ReceiveBuffID"), reader.Get<int>("ReceiveBuffDuration")));
                }
            }

            using (var reader = QueryReader("SELECT * FROM Projectiles")) {
                while (reader.Read()) {
                    int id = reader.Get<int>("ID");
                    projectileInfo[id] = new ItemInfo();
                    projectileInfo[id].SetData("ID", id);
                    projectileInfo[id].SetData("Name", reader.Get<string>("Name"));
                    projectileInfo[id].SetData("Damage", reader.Get<int>("Damage"));
                    projectileInfo[id].SetData("Debuff", new BuffDuration(reader.Get<int>("InflictBuffID"), reader.Get<int>("InflictBuffDuration")));
                    projectileInfo[id].SetData("SelfBuff", new BuffDuration(reader.Get<int>("ReceiveBuffID"), reader.Get<int>("ReceiveBuffDuration")));
                }
            }

            using (var reader = QueryReader("SELECT * FROM Buffs")) {
                while (reader.Read()) {
                    var id = reader.Get<int>("ID");
                    buffInfo[id] = new ItemInfo();
                    buffInfo[id].SetData("ID", id);
                    buffInfo[id].SetData("Name", reader.Get<string>("Name"));
                    buffInfo[id].SetData("Debuff", new BuffDuration(reader.Get<int>("InflictBuffID"), reader.Get<int>("InflictBuffDuration")));
                    buffInfo[id].SetData("SelfBuff", new BuffDuration(reader.Get<int>("ReceiveBuffID"), reader.Get<int>("ReceiveBuffDuration")));
                }
            }
        }
    }
}
