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
using static PvPController.DBConsts;

namespace PvPController {
    public static class Database {
        public static bool IsMySql { get { return db.GetSqlType() == SqlType.Mysql; } }

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
                IsMySql
                    ? (IQueryBuilder)new MysqlQueryCreator()
                    : (IQueryBuilder)new SqliteQueryCreator());
            
            sqlCreator.EnsureTableStructure(new SqlTable(DBConsts.ItemTable,
                new SqlColumn(ID, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(Damage, MySqlDbType.Int32),
                new SqlColumn(Shoot, MySqlDbType.Int32),
                new SqlColumn(IsShootModded, MySqlDbType.Int32),
                new SqlColumn(ShootSpeed, MySqlDbType.Float),
                new SqlColumn(Knockback, MySqlDbType.Float),
                new SqlColumn(Defense, MySqlDbType.Int32),
                new SqlColumn(InflictBuffID, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffID, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32)));
            
            sqlCreator.EnsureTableStructure(new SqlTable(DBConsts.ProjectileTable,
                new SqlColumn(ID, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(Damage, MySqlDbType.Int32),
                new SqlColumn(InflictBuffID, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffID, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32)));

            SqlTable table3 = new SqlTable(DBConsts.BuffTable,
                new SqlColumn(ID, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(InflictBuffID, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffID, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table3);
        }

        public static QueryResult QueryReader(string query, params object[] args) {
            return db.QueryReader(query, args);
        }

        public static bool Query(string query) {
            bool success = true;
            db.Open();
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

            if (value is string) value = (T)Convert.ChangeType(value.ToString().SQLString(), typeof(T));

            string sourceID = !selectAll ? " WHERE ID = {0}".SFormat(index) : "";
            return Query(string.Format("UPDATE {0} SET {1} = {2}{3}", type, key, value, sourceID));
        }

        /// <summary>
        /// Creates all the default stats of items, projectiles, and buffs and puts it into
        /// the mysql/sqlite file.
        /// </summary>
        public static void InitDefaultTables() {
            var conn = IsMySql 
                ? (DbConnection)new MySqlConnection(db.ConnectionString) 
                : (DbConnection)new SqliteConnection(db.ConnectionString);

            conn.Open();

            using (var cmd = conn.CreateCommand()) {
                using (var transaction = conn.BeginTransaction()) {
                    var tableList = new[] { DBConsts.ItemTable, DBConsts.ProjectileTable, DBConsts.BuffTable };
                    foreach(string table in tableList) {
                        cmd.CommandText = "DELETE FROM {0}".SFormat(table);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxItemTypes; x++) {
                        cmd.CommandText = GetDefaultValueSQLString(ItemTable, x);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxProjectileTypes; x++) {
                        cmd.CommandText = GetDefaultValueSQLString(ProjectileTable, x);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxBuffTypes; x++) {
                        cmd.CommandText = GetDefaultValueSQLString(BuffTable, x);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            conn.Close();
        }

        public static T GetData<T> (string table, int id, string column) {
            using (var reader = QueryReader(string.Format("SELECT {0} FROM {1} WHERE ID = {2}", column, table, id.ToString()))) {
                while (reader.Read()) {
                    return reader.Get<T>(column);
                }
            }

            return default(T);
        }

        public static Type GetType(string table, string column) {
            using (var reader = QueryReader(string.Format("SELECT {0} FROM {1}", column, table))) {
                while (reader.Read()) {
                    return reader.Reader.GetFieldType(0);
                }
            }

            return default(Type);
        }

        public static BuffDuration GetBuffDuration(string table, int id, bool isInflictDebuff) {
            return isInflictDebuff
                ? new BuffDuration(GetData<int>(table, id, InflictBuffID), GetData<int>(table, id, InflictBuffDuration))
                : new BuffDuration(GetData<int>(table, id, ReceiveBuffID), GetData<int>(table, id, ReceiveBuffDuration));
        }

        public static string GetDefaultValueSQLString(string table, int id) {
            string name;
            int damage;
            int defense;
            int inflictBuff;
            int inflictBuffDuration;

            switch (table) {
                case "Items":
                    Item item = new Item();
                    item.SetDefaults(id);

                    name = item.Name;
                    damage = item.damage;
                    defense = item.defense;
                    float knockback = item.knockBack;
                    int shoot = item.useAmmo == AmmoID.None ? item.shoot : -1;

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DBConsts.ItemTable,
                            string.Join(", ", ID, Name, Damage, Shoot, IsShootModded, ShootSpeed, Knockback, Defense, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration),
                            string.Join(", ", id, name.SQLString(), damage, shoot, 0, -1, knockback, defense, 0, 0, 0, 0));

                case "Projectiles":
                    name = Lang.GetProjectileName(id).Value;
                    damage = 0;
                    inflictBuff = 0;
                    inflictBuffDuration = 0;

                    if (PresetData.PresetProjDamage.ContainsKey(id)) {
                        damage = PresetData.PresetProjDamage[id];
                    }
                    if (PresetData.ProjectileDebuffs.ContainsKey(id)) {
                        inflictBuff = PresetData.ProjectileDebuffs[id].buffid;
                        inflictBuffDuration = PresetData.ProjectileDebuffs[id].buffDuration;
                    }

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DBConsts.ProjectileTable,
                            string.Join(", ", ID, Name, Damage, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration),
                            string.Join(", ", id, name.SQLString(), damage, inflictBuff, inflictBuffDuration, 0, 0));

                case "Buffs":
                    name = Lang.GetBuffName(id);
                    inflictBuff = 0;
                    inflictBuffDuration = 0;
                    if (PresetData.FlaskDebuffs.ContainsKey(id)) {
                        inflictBuff = PresetData.FlaskDebuffs[id].buffid;
                        inflictBuffDuration = PresetData.FlaskDebuffs[id].buffDuration;
                    }

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DBConsts.BuffTable,
                            string.Join(", ", ID, Name, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration),
                            string.Join(", ", id, name.SQLString(), inflictBuff, inflictBuffDuration, 0, 0));

                default:
                    return "";
            }
        }

        public static void DeleteRow(string table, int id) {
            Query("DELETE FROM {0} WHERE ID = {1}".SFormat(table, id));
        }
    }

    public static class DBConsts {
        public static string ItemTable = "Items";
        public static string ProjectileTable = "Projectiles";
        public static string BuffTable = "Buffs";

        public static string ID = "ID";
        public static string Name = "Name";
        public static string Damage = "Damage";
        public static string Shoot = "Shoot";
        public static string IsShootModded = "IsShootModded";
        public static string ShootSpeed = "ShootSpeed";
        public static string Knockback = "Knockback";
        public static string Defense = "Defense";
        public static string InflictBuffID = "InflictBuffID";
        public static string InflictBuffDuration = "InflictBuffDuration";
        public static string ReceiveBuffID = "ReceiveBuffID";
        public static string ReceiveBuffDuration = "ReceiveBuffDuration";
    }
}
