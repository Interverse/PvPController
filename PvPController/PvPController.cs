using PvPController.Variables;
using PvPController.Network;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using System.Timers;

namespace PvPController {
    [ApiVersion(2, 1)]
    public class PvPController : TerrariaPlugin {

        public static Config Config;
        public static PvPPlayer[] PvPers = new PvPPlayer[Main.maxPlayers];
        public static PvPHandler PvPHandler = new PvPHandler();
        public static Timer PvPTimer = new Timer(1000) { Enabled = true };
        
        public override string Name => "PvP Controller";
        public override string Author => "Johuan";
        public override string Description => "Adds customizability to pvp";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public PvPController(Main game) : base(game) { }
        
        /// <summary>
        /// Creates a config if one doesn't exist, reads the config and stores it into the plugin memory,
        /// connects the sql database, starts hooks and PvPTimer, initialize commands,
        /// and stores whether the server has Server Side Characters enabled.
        /// </summary>
        public override void Initialize() {
            Config = Config.Read(Config.ConfigPath);
            if (!File.Exists(Config.ConfigPath)) {
                Config.Write(Config.ConfigPath);
            }
            
            Database.ConnectDB();

            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
            ServerApi.Hooks.NetGetData.Register(this, GetData);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            
            PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;

            PvPTimer.Elapsed += PvPTimerElapsed;

            PluginCommands.RegisterCommands();
        }

        /// <summary>
        /// Disposes timers, deregisters hooks, and writes all changes of config to .json
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
                ServerApi.Hooks.NetGetData.Deregister(this, GetData);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);

                PlayerHooks.PlayerPostLogin -= OnPlayerPostLogin;

                PvPHandler.Unsubscribe();

                PvPTimer.Elapsed -= PvPTimerElapsed;
                PvPTimer.Enabled = false;
                PvPTimer.Dispose();

                Config.Write(Config.ConfigPath);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds a player who just logged in to the plugin-stored collection of players.
        /// </summary>
        /// <param Name="e"></param>
        private void OnPlayerPostLogin(PlayerPostLoginEventArgs e) {
            PvPers[e.Player.Index] = new PvPPlayer(e.Player.Index);
        }

        /// <summary>
        /// Adds the player to the plugin-stored collection of players.
        /// </summary>
        /// <param Name="args"></param>
        private void OnJoin(JoinEventArgs args) {
            PvPers[args.Who] = new PvPPlayer(args.Who);
        }

        /// <summary>
        /// Sets default config values if a config doesn't exist 
        /// after the server has loaded the game
        /// </summary>
        /// <param Name="args"></param>
        private void OnGamePostInitialize(EventArgs args) {
            if (Config.SetDefaultValues()) {
                Database.InitDefaultTables();
            }
            Config.Write(Config.ConfigPath);
        }

        /// <summary>
        /// Displays a pvp interface on the right side of a client, showing
        /// statistics about their weapon and self, such as damage, weapons,
        /// projectiles, knockback, criticals, et cetera.
        /// </summary>
        private void PvPTimerElapsed(object sender, ElapsedEventArgs e) {
            foreach (var player in PvPers) {
                if (player.ConnectionAlive && player.TPlayer.hostile && player.SeeTooltip) {
                    Interface.DisplayInterface(player);
                }
            }
        }

        /// <summary>
        /// Processes data so it can be used in <see cref="Network.PvPHandler"/>.
        /// </summary>
        /// <param Name="args">The data needed to be processed.</param> 
        private void GetData(GetDataEventArgs args) {
            MemoryStream data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
            PvPPlayer attacker = PvPers[args.Msg.whoAmI];

            if (attacker == null || !attacker.TPlayer.active || !attacker.ConnectionAlive) return;
            if (!attacker.TPlayer.hostile) return;

            DataHandler.HandleData(args, data, attacker);
        }
    }
}