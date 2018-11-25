using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using PvPController.Variables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
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
                new SqlColumn(Wrath, MySqlDbType.Float),
                new SqlColumn(Endurance, MySqlDbType.Float),
                new SqlColumn(Titan, MySqlDbType.Float),
                new SqlColumn(InflictBuffId, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffId, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32)));
            
            sqlCreator.EnsureTableStructure(new SqlTable(DbConsts.ProjectileTable,
                new SqlColumn(Id, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(Damage, MySqlDbType.Int32),
                new SqlColumn(Wrath, MySqlDbType.Float),
                new SqlColumn(InflictBuffId, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffId, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32)));

           sqlCreator.EnsureTableStructure(new SqlTable(DbConsts.BuffTable,
                new SqlColumn(Id, MySqlDbType.Int32) { Primary = true },
                new SqlColumn(Name, MySqlDbType.Text) { Length = 255 },
                new SqlColumn(Damage, MySqlDbType.Int32),
                new SqlColumn(Knockback, MySqlDbType.Float),
                new SqlColumn(Defense, MySqlDbType.Int32),
                new SqlColumn(Wrath, MySqlDbType.Float),
                new SqlColumn(Endurance, MySqlDbType.Float),
                new SqlColumn(Titan, MySqlDbType.Float),
                new SqlColumn(InflictBuffId, MySqlDbType.Int32),
                new SqlColumn(InflictBuffDuration, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffId, MySqlDbType.Int32),
                new SqlColumn(ReceiveBuffDuration, MySqlDbType.Int32)));
        }

        public static QueryResult QueryReader(string query, params object[] args) {
            return db.QueryReader(query, args);
        }

        /// <summary>
        /// Performs an SQL query
        /// </summary>
        /// <param Name="query">The SQL statement</param>
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
        /// Deletes the contents of an entire row.
        /// </summary>
        /// <param Name="table">The table to delete from</param>
        /// <param Name="id">The ID of the data being deleted</param>
        public static void DeleteRow(string table, int id) {
            Query("DELETE FROM {0} WHERE ID = {1}".SFormat(table, id));
        }

        /// <summary>
        /// Performs a series of sql statements in a transaction.
        /// This allows for fast mass querying as opposed to querying
        /// one statement at a time.
        /// </summary>
        /// <param name="queries"></param>
        public static void PerformTransaction(string[] queries) {
            var conn = IsMySql
                ? (DbConnection)new MySqlConnection(db.ConnectionString)
                : new SqliteConnection(db.ConnectionString);

            conn.Open();

            using (var cmd = conn.CreateCommand()) {
                using (var transaction = conn.BeginTransaction()) {
                    foreach (string query in queries) {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            conn.Close();
        }

        /// <summary>
        /// Writes the changed attribute of an item to the sql database.
        /// </summary>
        public static bool Update<T>(string table, int index, string column, T value) {
            bool selectAll = index <= 0;

            if (value is string) value = (T)Convert.ChangeType(value.ToString().SqlString(), typeof(T));

            string sourceId = !selectAll ? " WHERE ID = {0}".SFormat(index) : "";
            return Query(string.Format("UPDATE {0} SET {1} = {2}{3}", table, column, value, sourceId));
        }

        /// <summary>
        /// Creates all the default stats of items, projectiles, and buffs and puts it into
        /// the mysql/sqlite database.
        /// </summary>
        public static void InitDefaultTables() {
            List<string> queries = new List<string>();

            var tableList = new[] { DbConsts.ItemTable, DbConsts.ProjectileTable, DbConsts.BuffTable };
            foreach(string table in tableList) {
                queries.Add("DELETE FROM {0}".SFormat(table));
            }

            for (int x = 0; x < Main.maxItemTypes; x++) {
                queries.Add(GetDefaultValueSqlString(ItemTable, x));
            }

            for (int x = 0; x < Main.maxProjectileTypes; x++) {
                queries.Add(GetDefaultValueSqlString(ProjectileTable, x));
            }

            for (int x = 0; x < Main.maxBuffTypes; x++) {
                queries.Add(GetDefaultValueSqlString(BuffTable, x));
            }

            PerformTransaction(queries.ToArray());
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
        /// Gets the value of an item, projectile, or buff based off id.
        /// </summary>
        public static object GetDataWithType(string table, int id, string column, Type type) {
            MethodInfo getDataMethod = typeof(Database).GetMethod("GetData")?.MakeGenericMethod(type);

            return getDataMethod?.Invoke(null, new object[] { table, id, column } );
        }

        /// <summary>
        /// Gets the type of the sql column.
        /// </summary>
        public static Type GetType(string table, string column) {
            try {
                using (var reader = QueryReader(string.Format("SELECT {0} FROM {1}", column, table))) {
                    while (reader.Read()) {
                        return reader.Reader.GetFieldType(0);
                    }
                }
            } catch { }

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
            var inflictBuff = new BuffInfo();
            var selfBuff = new BuffInfo();
            int defense;
            float knockback;
            float wrath = 0;
            float endurance = 0;
            float titan = 0;

            switch (table) {
                case "Items":
                    Item item = new Item();
                    item.SetDefaults(id);

                    name = item.Name;
                    damage = item.damage > 0 ? item.damage : 0;
                    defense = item.defense;
                    knockback = item.knockBack;
                    int shoot = item.useAmmo == AmmoID.None ? item.shoot : -1;
                    wrath = (damage > 0).ToInt();

                    //Brand of the Inferno's buff: Striking Moment
                    if (id == 3823)
                        selfBuff = new BuffInfo(198, 5 * 60);

                    //Worm Scarf
                    if (id == 3224)
                        endurance = 0.17f;

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DbConsts.ItemTable,
                            string.Join(", ", Id, Name, Damage, Shoot, IsShootModded, ShootSpeed, Knockback, Defense, Wrath, Endurance, Titan, InflictBuffId, InflictBuffDuration, ReceiveBuffId, ReceiveBuffDuration),
                            string.Join(", ", id, name.SqlString(), damage, shoot, 0, -1, knockback, defense, wrath, endurance, titan, inflictBuff.BuffId, inflictBuff.BuffDuration, selfBuff.BuffId, selfBuff.BuffDuration));

                case "Projectiles":
                    name = Lang.GetProjectileName(id).Value;
                    damage = PresetData.PresetProjDamage.ContainsKey(id) ? PresetData.PresetProjDamage[id] : 0;
                    inflictBuff = PresetData.ProjectileDebuffs.ContainsKey(id)
                        ? PresetData.ProjectileDebuffs[id] 
                        : new BuffInfo();

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DbConsts.ProjectileTable,
                            string.Join(", ", Id, Name, Damage, Wrath, InflictBuffId, InflictBuffDuration, ReceiveBuffId, ReceiveBuffDuration),
                            string.Join(", ", id, name.SqlString(), damage, wrath, inflictBuff.BuffId, inflictBuff.BuffDuration, selfBuff.BuffId, selfBuff.BuffDuration));

                case "Buffs":
                    name = Lang.GetBuffName(id);
                    damage = 0;
                    defense = 0;
                    knockback = 0;
                    inflictBuff = PresetData.FlaskDebuffs.ContainsKey(id)
                        ? PresetData.FlaskDebuffs[id]
                        : new BuffInfo();
                    endurance = PresetData.BuffEndurance.ContainsKey(id)
                        ? PresetData.BuffEndurance[id]
                        : 0;

                    //Striking Moment
                    if (id == 198)
                        wrath = 5f;

                    return "INSERT INTO {0} ({1}) VALUES ({2})"
                        .SFormat(DbConsts.BuffTable,
                            string.Join(", ", Id, Name, Damage, Knockback, Defense, Wrath, Endurance, Titan, InflictBuffId, InflictBuffDuration, ReceiveBuffId, ReceiveBuffDuration),
                            string.Join(", ", id, name.SqlString(), damage, knockback, defense, wrath, endurance, titan, inflictBuff.BuffId, inflictBuff.BuffDuration, selfBuff.BuffId, selfBuff.BuffDuration));

                default:
                    return "";
            }
        }
    }

    /// <summary>
    /// Mapped database table/column names into constants.
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
        public const string Wrath = "Wrath"; //%damage multiplier
        public const string Endurance = "Endurance"; //%damage reduction multiplier
        public const string Titan = "Titan"; //%knockback multiplier
    }
}
