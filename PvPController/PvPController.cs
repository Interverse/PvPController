using Microsoft.Xna.Framework;
using PvPController.PvPVariables;
using PvPController.Utilities;
using PvPController.PacketHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Reflection;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using System.Timers;

namespace PvPController {
    [ApiVersion(2, 1)]
    public class PvPController : TerrariaPlugin {

        public static Config config;
        public static PvPPlayer[] pvpers = new PvPPlayer[Main.maxPlayers];
        public static PvPProjectile[] projectiles = new PvPProjectile[Main.maxProjectiles];
        public static PvPHandler pvpHandler = new PvPHandler();
        public static Database database;
        public static Timer timer = new Timer(500) { Enabled = true };

        public static bool isSSC;
        
        public override string Name => "PvP Controller";
        public override string Author => "Johuan";
        public override string Description => "Adds customizability to pvp";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public PvPController(Main game) : base(game) { }

        public override void Initialize() {
            config = Config.Read(Config.configPath);
            if (!File.Exists(Config.configPath)) {
                config.Write(Config.configPath);
            }

            database = Database.InitDb("PvPDatabase");
            database.LoadDatabase();

            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
            ServerApi.Hooks.NetGetData.Register(this, GetData);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            
            PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;

            GetDataHandlers.NewProjectile += OnNewProjectile;

            timer.Elapsed += PvPTimerElapsed;

            isSSC = Main.ServerSideCharacter;

            PluginCommands.registerCommands();
        }

        private void OnGamePostInitialize(EventArgs args) {
            config.SetDefaultValues();
            config.Write(Config.configPath);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                ServerApi.Hooks.NetGetData.Deregister(this, GetData);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);

                PlayerHooks.PlayerPostLogin -= OnPlayerPostLogin;

                GetDataHandlers.NewProjectile -= OnNewProjectile;

                pvpHandler.Unsubscribe();

                config.Write(Config.configPath);
            }
            base.Dispose(disposing);
        }

        private void OnPlayerPostLogin(PlayerPostLoginEventArgs e) {
            pvpers[e.Player.Index] = new PvPPlayer(e.Player.Index);
        }

        private void OnJoin(JoinEventArgs args) {
            pvpers[args.Who] = new PvPPlayer(args.Who);
        }

        private void PvPTimerElapsed(object sender, ElapsedEventArgs e) {
            for (int x = 0; x < pvpers.Length; x++) {
                if (pvpers[x].ConnectionAlive && pvpers[x].TPlayer.hostile && pvpers[x].seeTooltip)
                    PvPUtils.DisplayInterface(pvpers[x]);
            }
        }

        //Stores newly created projectiles into a list along with its originated item
        public void OnNewProjectile(object sender, GetDataHandlers.NewProjectileEventArgs args) {
            if (!PvPController.config.enablePlugin) return;

            PvPPlayer player = new PvPPlayer(args.Owner);

            if (player == null || !player.TPlayer.hostile) return;

            PvPItem weapon;
            if (MiscData.accessoryOrArmorProjectiles.ContainsKey(args.Type)) {
                weapon = new PvPItem();
                weapon.damage = MiscData.accessoryOrArmorProjectiles[args.Type];
                weapon.name = Lang.GetProjectileName(args.Type).ToString();
            } else if (MiscData.fromWhatWeapon.ContainsKey(args.Type)) {
                weapon = player.FindPlayerItem(MiscData.fromWhatWeapon[args.Type]);
            } else {
                weapon = player.GetPlayerItem();
            }

            projectiles[args.Identity] = new PvPProjectile(args.Type);
            projectiles[args.Identity].SetOwner(args.Owner);
            projectiles[args.Identity].SetOriginatedItem(weapon);
            projectiles[args.Identity].PerformProjectileAction();
        }

        private void GetData(GetDataEventArgs args) {
            MemoryStream data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
            PvPPlayer attacker = pvpers[args.Msg.whoAmI];

            if (attacker == null || !attacker.TPlayer.active || !attacker.ConnectionAlive) return;
            if (!attacker.TPlayer.hostile) return;

            switch (args.MsgID) {
                case PacketTypes.PlayerHurtV2:
                    PvPPlayer target = pvpers[data.ReadByte()];
                    PlayerDeathReason playerHitReason = PlayerDeathReason.FromReader(new BinaryReader(data));
                    if (target == null || !target.ConnectionAlive || !target.Active) return;
                    if (playerHitReason.SourcePlayerIndex == -1) {
                        target.lastHitBy = null;
                        return;
                    }

                    PvPProjectile projectile = playerHitReason.SourceProjectileIndex == -1 ? null : projectiles[playerHitReason.SourceProjectileIndex];
                    PvPItem weapon = projectile == null ? attacker.GetPlayerItem() : projectile.itemOriginated;

                    int int1 = data.ReadInt16(); //damage
                    int int2 = data.ReadByte(); //knockback

                    int inflictedDamage = PvPController.config.enableDamageChanges ? target.GetDamageDealt(attacker, weapon, projectile) : int1;
                    int damageReceived = target.GetDamageReceived(inflictedDamage);
                    data.ReadByte(); data.ReadByte();
                    int knockback = int2 - 1;

                    int crit = attacker.GetCrit(weapon);

                    target.lastHitBy = attacker;
                    target.lastHitWeapon = weapon;
                    target.lastHitProjectile = projectile;

                    DataHandler.OnPlayerHurtted(args, attacker, target, weapon, projectile, playerHitReason,
                        inflictedDamage, damageReceived, knockback, crit);

                    break;

                case PacketTypes.TogglePvp:
                    DataHandler.OnPvPToggled(attacker);
                    break;

                case PacketTypes.PlayerSlot:
                    data.ReadByte();
                    int slot = data.ReadByte();
                    DataHandler.OnPlayerSlotUpdated(attacker, slot);
                    break;

                case PacketTypes.PlayerDeathV2:
                    DataHandler.OnPlayerDead(attacker);
                    break;

                case PacketTypes.ProjectileDestroy:
                    DataHandler.OnProjectileDestroyed(data);
                    break;

                case PacketTypes.PlayerUpdate:
                    DataHandler.OnPlayerUpdated(data, attacker);
                    break;
            }
        }
    }
}