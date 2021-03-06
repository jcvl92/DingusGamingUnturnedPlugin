using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;

namespace DingusGaming.Events.Arena
{
    public class CommandArena : IRocketCommand
    {
        private const string NAME = "arena";
        private const string HELP = "Schedule arena at your location or update arena location.";
        private const string SYNTAX = "";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;
        private static ArenaEvent arenaEvent = null;

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
            if (arenaEvent == null)
            {
                arenaEvent = new ArenaEvent(caller.Position, startItem: 1036, dropItem: 1021);
                EventScheduler.scheduleEvent(arenaEvent, 30, snapToHour:true, minimumPlayers:5, durationSeconds:60, countDownTimes:new uint[]{600, 300, 60, 10, 5, 4, 3, 2, 1});
                DGPlugin.messagePlayer(caller, "Arena scheduled and set at your location.");
            }
            else
            {
                arenaEvent.locations.Add(caller.Position);
                DGPlugin.messagePlayer(caller, "Your location has been added to the Arena location pool.");
            }
        }
    }
}