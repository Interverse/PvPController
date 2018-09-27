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
using Terraria.ID;
using TShockAPI;
using TShockAPI.DB;

namespace PvPController {
    public static class Database {
        public static Dictionary<int, ItemInfo> itemInfo = new Dictionary<int, ItemInfo>(Main.maxItemTypes);
        public static Dictionary<int, ItemInfo> projectileInfo = new Dictionary<int, ItemInfo>(Main.maxProjectileTypes);
        public static Dictionary<int, ItemInfo> buffInfo = new Dictionary<int, ItemInfo>(Main.maxBuffTypes);

        public static IDbConnection db;

        /// <summary>
        /// Connects the sql(ite) file to the plugin, creating one if a file doesn't already exist.
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

            SqlTableCreator sqlCreator = new SqlTableCreator(db,
                db.GetSqlType() == SqlType.Sqlite
                    ? (IQueryBuilder)new SqliteQueryCreator()
                    : new MysqlQueryCreator());

            SqlTable table1 = new SqlTable("Items",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("Damage", MySqlDbType.Int32),
                new SqlColumn("Shoot", MySqlDbType.Int32),
                new SqlColumn("IsShootModded", MySqlDbType.Int32),
                new SqlColumn("ShootSpeed", MySqlDbType.Float),
                new SqlColumn("Defense", MySqlDbType.Int32),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table1);

            SqlTable table2 = new SqlTable("Projectiles",
                new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
                new SqlColumn("Name", MySqlDbType.String),
                new SqlColumn("Damage", MySqlDbType.Int32),
                new SqlColumn("InflictBuffID", MySqlDbType.Int32),
                new SqlColumn("InflictBuffDuration", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffID", MySqlDbType.Int32),
                new SqlColumn("ReceiveBuffDuration", MySqlDbType.Int32));
            sqlCreator.EnsureTableStructure(table2);

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

        public static int Query(string query, params object[] args) {
            return db.Query(query, args);
        }

        /// <summary>
        /// Writes the changed attribute of an item to the sql(ite) file.
        /// </summary>
        public static void UpdateItems(ItemInfo iteminfo) {
            var query =
                string.Format(
                    "UPDATE Items SET Name = '{0}', Damage = {1}, Shoot = {2}, IsShootModded = {3}, ShootSpeed = {4}, Defense = {5}, InflictBuffID = {6}, InflictBuffDuration = {7}, ReceiveBuffID = {8}, ReceiveBuffDuration = {9} WHERE ID = @0",
                    MiscUtils.SanitizeString(iteminfo.name), iteminfo.damage, iteminfo.shoot, iteminfo.isShootModded ? 1 : 0, iteminfo.shootSpeed, iteminfo.defense, iteminfo.debuff.buffid, iteminfo.debuff.buffDuration, iteminfo.selfBuff.buffid, iteminfo.selfBuff.buffDuration);
                
            Query(query, iteminfo.id);
        }

        /// <summary>
        /// Writes the changed attribute of a projectile to the sql(ite) file.
        /// </summary>
        public static void UpdateProjectiles(ItemInfo iteminfo) {
            var query =
                string.Format(
                    "UPDATE Projectiles SET Name = '{0}', Damage = {1}, InflictBuffID = {2}, InflictBuffDuration = {3}, ReceiveBuffID = {4}, ReceiveBuffDuration = {5} WHERE ID = @0",
                    MiscUtils.SanitizeString(iteminfo.name), iteminfo.damage, iteminfo.debuff.buffid, iteminfo.debuff.buffDuration, iteminfo.selfBuff.buffid, iteminfo.selfBuff.buffDuration);
            
            Query(query, iteminfo.id);
        }

        /// <summary>
        /// Writes the changed attribute of a buff to the sql(ite) file.
        /// </summary>
        public static void UpdateBuffs(ItemInfo iteminfo) {
            var query =
                string.Format(
                    "UPDATE Buffs SET Name = '{0}', InflictBuffID = {2}, InflictBuffDuration = {3}, ReceiveBuffID = {4}, ReceiveBuffDuration = {5} WHERE ID = @0",
                    MiscUtils.SanitizeString(iteminfo.name), iteminfo.debuff.buffid, iteminfo.debuff.buffDuration, iteminfo.selfBuff.buffid, iteminfo.selfBuff.buffDuration);
            
            Query(query, iteminfo.id);
        }

        /// <summary>
        /// Creates all the default stats of items, projectiles, and buffs and puts it into
        /// the sql(ite) file.
        /// </summary>
        public static void InitDefaultTables() {
            SqliteConnection conn = new SqliteConnection(db.ConnectionString);
            
            conn.Open();

            using (SqliteCommand cmd = new SqliteCommand(conn)) {
                using (SqliteTransaction transaction = conn.BeginTransaction()) {
                    for (int x = 0; x < Main.maxItemTypes; x++) {
                        Item item = new Item();
                        item.SetDefaults(x);

                        string name = MiscUtils.SanitizeString(item.Name);

                        int damage = item.damage;
                        int defense = item.defense;
                        int shoot = item.useAmmo == AmmoID.None ? item.shoot : -1;

                        cmd.CommandText =
                            "INSERT INTO Items (ID, Name, Damage, Shoot, IsShootModded, ShootSpeed, Defense, InflictBuffID, InflictBuffDuration, ReceiveBuffID, ReceiveBuffDuration) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})"
                            .SFormat(x, name, damage, shoot, 0, -1, defense, 0, 0, 0, 0);
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

        /// <summary>
        /// Loads data from the sql(ite) file to the Dictionaries in this Database.
        /// </summary>
        public static void LoadDatabase() {
            using (var reader = QueryReader("SELECT * FROM Items")) {
                while (reader.Read()) {
                    int id = reader.Get<int>("ID");
                    itemInfo[id] = new ItemInfo();
                    itemInfo[id].id = id;
                    itemInfo[id].name = reader.Get<string>("Name");
                    itemInfo[id].damage = reader.Get<int>("Damage");
                    itemInfo[id].shoot = reader.Get<int>("Shoot");
                    itemInfo[id].isShootModded = reader.Get<int>("IsShootModded") == 1 ? true : false;
                    itemInfo[id].shootSpeed = reader.Get<float>("ShootSpeed");
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
