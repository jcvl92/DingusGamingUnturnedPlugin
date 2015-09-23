using System.Collections.Generic;
using DingusGaming.Events.Arena;
using DingusGaming.helper;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace DingusGaming
{
    public class CommandUnstuck : IRocketCommand
    {
        private const string NAME = "unstuck";
        private const string HELP = "";
        private const string SYNTAX = "";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;

        public bool RunFromConsole
        {
            get { return RUN_FROM_CONSOLE; }
        }

        public string Name
        {
            get { return NAME; }
        }

        public string Help
        {
            get { return HELP; }
        }

        public string Syntax
        {
            get { return SYNTAX; }
        }

        public List<string> Aliases { get; } = new List<string>();

        public bool AllowFromConsole
        {
            get { return ALLOW_FROM_CONSOLE; }
        }

        public List<string> Permissions { get; } = new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer) caller, command);
        }

        public void Execute(UnturnedPlayer caller, string[] command)
        {
            //save the caller state
            PlayerState playerState = PlayerState.getState(caller);
            PlayerState.clearInventory(caller);

            //respawn and restore the player on death and revival, respectively
            LifeUpdated playerDied = null;
            playerDied = delegate (bool isDead)
            {
                if (isDead)
                {
                    LifeUpdated playerRevived = null;
                    playerRevived = delegate (bool isDead2)
                    {
                        if (!isDead2)
                            playerState.setCompleteState(caller);

                        caller.Player.PlayerLife.OnUpdateLife -= playerRevived;
                    };
                    caller.Player.PlayerLife.OnUpdateLife += playerRevived;

                    DGPlugin.respawnPlayer(caller);
                }

                caller.Player.PlayerLife.OnUpdateLife -= playerDied;
            };
            caller.Player.PlayerLife.OnUpdateLife += playerDied;

            //kill the caller
            caller.Damage(100, new Vector3(), EDeathCause.KILL, ELimb.SPINE, caller.CSteamID);
        }
    }
}