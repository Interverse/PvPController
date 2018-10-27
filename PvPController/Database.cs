using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using PvPController.Variables;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using PvPController.Utilities;
using Terraria;
using Terraria.ID;
using TShockAPI;
using TShockAPI.DB;
using static PvPController.DbConsts;

namespace PvPController {
    public static class Database {
        public static bool IsMySql => db.GetSqlType() == SqlType.Mysql;

        public static IDbConnection db;

        /// <summary>
        /// Connects the mysql/sqlite database for the plugin, creating one if the database doesn't already exist.
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
            
            sqlCreator.EnsureTableStructure(new SqlTable(DbConsts.ItemTable,
                new SqlColumn(Id, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(Damage, MySqlDbType.Int32),
                new SqlColumn(Shoot, MySqlDbType.Int32),
                new SqlColumn(IsShootModded, MySqlDbType.Int32),
                new SqlColumn(ShootSpeed, MySqlDbType.Float),
                new SqlColumn(Knockback, MySqlDbType.Float),
                new SqlColumn(Defense, MySqlDbType.Int32),
                new SqlColumn(InflictBuffId, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffId, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32)));
            
            sqlCreator.EnsureTableStructure(new SqlTable(DbConsts.ProjectileTable,
                new SqlColumn(Id, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(Damage, MySqlDbType.Int32),
                new SqlColumn(InflictBuffId, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffId, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32)));

            SqlTable table3 = new SqlTable(DbConsts.BuffTable,
                new SqlColumn(Id, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(InflictBuffId, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffId, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table3);
        }

        public static QueryResult QueryReader(string query, params object[] args) {
            return db.QueryReader(query, args);
        }

        /// <summary>
        /// Performs an SQL query
        /// </summary>
        /// <param name="query">The SQL statement</param>
        /// <returns>
        /// Returns true if the statement was successful.
        /// Returns false if the statement failed.
        /// </returns>
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
        /// Writes the changed attribute of an item to the sql database.
        /// </summary>
        public static bool Update<T>(string type, int index, string key, T value) {
            bool selectAll = index <= 0;

            if (value is string) value = (T)Convert.ChangeType(value.ToString().SqlString(), typeof(T));

            string sourceId = !selectAll ? " WHERE ID = {0}".SFormat(index) : "";
            return Query(string.Format("UPDATE {0} SET {1} = {2}{3}", type, key, value, sourceId));
        }

        /// <summary>
        /// Creates all the default stats of items, projectiles, and buffs and puts it into
        /// the mysql/sqlite database.
        /// </summary>
        public static void InitDefaultTables() {
            var conn = IsMySql 
                ? (DbConnection)new MySqlConnection(db.ConnectionString) 
                : (DbConnection)new SqliteConnection(db.ConnectionString);

            conn.Open();

            using (var cmd = conn.CreateCommand()) {
                using (var transaction = conn.BeginTransaction()) {
                    var tableList = new[] { DbConsts.ItemTable, DbConsts.ProjectileTable, DbConsts.BuffTable };
                    foreach(string table in tableList) {
                        cmd.CommandText = "DELETE FROM {0}".SFormat(table);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxItemTypes; x++) {
                        cmd.CommandText = GetDefaultValueSqlString(ItemTable, x);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxProjectileTypes; x++) {
                        cmd.CommandText = GetDefaultValueSqlString(ProjectileTable, x);
                        cmd.ExecuteNonQuery();
                    }

                    for (int x = 0; x < Main.maxBuffTypes; x++) {
                        cmd.CommandText = GetDefaultValueSqlString(BuffTable, x);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            conn.Close();
        }

        /// <summary>
        /// Gets the value of an item, projectile, or buff based off id.
        /// </summary>
        public static T GetData<T> (string table, int id, string column) {
            using (var reader = QueryReader(string.Format("SELECT {0} FROM {1} WHERE ID = {2}", column, table, id.ToString()))) {
                while (reader.Read()) {
                    return reader.Get<T>(column);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Gets the type of the sql column.
        /// </summary>
        public static Type GetType(string table, string column) {
            using (var reader = QueryReader(string.Format("SELECT {0} FROM {1}", column, table))) {
                while (reader.Read()) {
                    return reader.Reader.GetFieldType(0);
                }
            }

            return default(Type);
        }

        /// <summary>
        /// Gets the <see cref="BuffInfo"/> of an item, projectile, or buff.
        /// </summary>
        public static BuffInfo GetBuffInfo(string table, int id, bool isInflictDebuff) =>
            isInflictDebuff ? new BuffInfo(GetData<int>(table, id, InflictBuffId), GetData<int>(table, id, InflictBuffDuration))
                : new BuffInfo(GetData<int>(table, id, ReceiveBuffId), GetData<int>(table, id, ReceiveBuffDuration));

        /// <summary>
        /// Gets the default values of an item, projectile, or buff and
        /// puts it into an sql query form.
        /// </summary>
        /// <returns>The default values in an sql statement</returns>
        public static string GetDefaultValueSqlString(string table, int id) {
            string name;
            int damage;
            int inflictBuff;
            int inflictBuffDuration;

            switch (table) {
                case "Items":
                    Item item = new Item();
                    item.SetDefaults(id);

                    name = item.Name;
                    damage = item.damage;
                    int defense = item.defense;
                    float knockback = item.knockBack;
                    int shoot = item.useAmmo == AmmoID.None ? item.shoot : -1;

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DbConsts.ItemTable,
                            string.Join(", ", Id, Name, Damage, Shoot, IsShootModded, ShootSpeed, Knockback, Defense, InflictBuffId, InflictBuffDuration, ReceiveBuffId, ReceiveBuffDuration),
                            string.Join(", ", id, name.SqlString(), damage, shoot, 0, -1, knockback, defense, 0, 0, 0, 0));

                case "Projectiles":
                    name = Lang.GetProjectileName(id).Value;
                    damage = 0;
                    inflictBuff = 0;
                    inflictBuffDuration = 0;

                    if (PresetData.PresetProjDamage.ContainsKey(id)) {
                        damage = PresetData.PresetProjDamage[id];
                    }
                    if (PresetData.ProjectileDebuffs.ContainsKey(id)) {
                        inflictBuff = PresetData.ProjectileDebuffs[id].BuffId;
                        inflictBuffDuration = PresetData.ProjectileDebuffs[id].BuffDuration;
                    }

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DbConsts.ProjectileTable,
                            string.Join(", ", Id, Name, Damage, InflictBuffId, InflictBuffDuration, ReceiveBuffId, ReceiveBuffDuration),
                            string.Join(", ", id, name.SqlString(), damage, inflictBuff, inflictBuffDuration, 0, 0));

                case "Buffs":
                    name = Lang.GetBuffName(id);
                    inflictBuff = 0;
                    inflictBuffDuration = 0;
                    if (PresetData.FlaskDebuffs.ContainsKey(id)) {
                        inflictBuff = PresetData.FlaskDebuffs[id].BuffId;
                        inflictBuffDuration = PresetData.FlaskDebuffs[id].BuffDuration;
                    }

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DbConsts.BuffTable,
                            string.Join(", ", Id, Name, InflictBuffId, InflictBuffDuration, ReceiveBuffId, ReceiveBuffDuration),
                            string.Join(", ", id, name.SqlString(), inflictBuff, inflictBuffDuration, 0, 0));

                default:
                    return "";
            }
        }

        /// <summary>
        /// Deletes the contents of an entire row.
        /// </summary>
        /// <param name="table">The table to delete from</param>
        /// <param name="id">The ID of the data being deleted</param>
        public static void DeleteRow(string table, int id) {
            Query("DELETE FROM {0} WHERE ID = {1}".SFormat(table, id));
        }
    }

    /// <summary>
    /// Mapped database names into constants.
    /// </summary>
    public static class DbConsts {
        public const string ItemTable = "Items";
        public const string ProjectileTable = "Projectiles";
        public const string BuffTable = "Buffs";

        public const string Id = "ID";
        public const string Name = "Name";
        public const string Damage = "Damage";
        public const string Shoot = "Shoot";
        public const string IsShootModded = "IsShootModded";
        public const string ShootSpeed = "ShootSpeed";
        public const string Knockback = "Knockback";
        public const string Defense = "Defense";
        public const string InflictBuffId = "InflictBuffID";
        public const string InflictBuffDuration = "InflictBuffDuration";
        public const string ReceiveBuffId = "ReceiveBuffID";
        public const string ReceiveBuffDuration = "ReceiveBuffDuration";
    }
}
