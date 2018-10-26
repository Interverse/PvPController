using Microsoft.Xna.Framework;
using PvPController.Variables;
using PvPController.Variables;
using PvPController.Network;
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
using System.Linq;

namespace PvPController {
    [ApiVersion(2, 1)]
    public class PvPController : TerrariaPlugin {

        public static bool debug = false;

        public static Config config;
        public static PvPPlayer[] pvpers = new PvPPlayer[Main.maxPlayers];
        public static PvPHandler pvpHandler = new PvPHandler();
        public static Timer timer = new Timer(500) { Enabled = true };
        
        public override string Name => "PvP Controller";
        public override string Author => "Johuan";
        public override string Description => "Adds customizability to pvp";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public PvPController(Main game) : base(game) { }
        
        /// <summary>
        /// Creates a config if one doesn't exist, reads the config and stores it into the plugin memory,
        /// connects the sql(ite) database, starts hooks and timer, initialize commands,
        /// and stores whether the server has Server Side Characters enabled.
        /// </summary>
        public override void Initialize() {
            config = Config.Read(Config.configPath);
            if (!File.Exists(Config.configPath)) {
                config.Write(Config.configPath);
            }
            
            Database.ConnectDB();

            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
            ServerApi.Hooks.NetGetData.Register(this, GetData);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            
            PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;

            timer.Elapsed += PvPTimerElapsed;

            PluginCommands.registerCommands();
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

                timer.Dispose();

                pvpHandler.Unsubscribe();

                config.Write(Config.configPath);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds a player who just logged in to the plugin-stored collection of players.
        /// </summary>
        /// <param name="e"></param>
        private void OnPlayerPostLogin(PlayerPostLoginEventArgs e) {
            pvpers[e.Player.Index] = new PvPPlayer(e.Player.Index);
        }

        /// <summary>
        /// Adds the player to the plugin-stored collection of players.
        /// </summary>
        /// <param name="args"></param>
        private void OnJoin(JoinEventArgs args) {
            pvpers[args.Who] = new PvPPlayer(args.Who);
        }

        /// <summary>
        /// Sets default config values if a config doesn't exist 
        /// after the server has loaded the game
        /// </summary>
        /// <param name="args"></param>
        private void OnGamePostInitialize(EventArgs args) {
            config.SetDefaultValues();
            config.Write(Config.configPath);
        }

        /// <summary>
        /// Displays a pvp interface on the right side of a client, showing
        /// statistics about their weapon and self, such as damage, weapons,
        /// projectiles, knockback, criticals, et cetera.
        /// </summary>
        private void PvPTimerElapsed(object sender, ElapsedEventArgs e) {
            for (int x = 0; x < pvpers.Length; x++) {
                if (pvpers[x].ConnectionAlive && pvpers[x].TPlayer.hostile && pvpers[x].seeTooltip) {
                    Interface.DisplayInterface(pvpers[x]);
                }
            }
        }

        /// <summary>
        /// Processes data so it can be used in <see cref="PvPHandler"/>.
        /// </summary>
        /// <param name="args">The data needed to be processed.</param> 
        private void GetData(GetDataEventArgs args) {
            MemoryStream data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length);
            PvPPlayer attacker = pvpers[args.Msg.whoAmI];

            if (attacker == null || !attacker.TPlayer.active || !attacker.ConnectionAlive) return;
            if (!attacker.TPlayer.hostile) return;

            DataHandler.HandleData(args, data, attacker);
        }
    }
}