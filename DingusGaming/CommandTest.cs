using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;
using UnityEngine;

namespace DingusGaming.Store
{
    public class CommandTest : IRocketCommand
    {
        private const string NAME = "test";
        private const string HELP = "";
        private const string SYNTAX = "";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;
        private Vector3 pos;
        private float rot;

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
            if (command.Length>0 && command[0].Equals("save"))
            {
                pos = caller.Position;
                rot = caller.Rotation;
            }
            else
                caller.Teleport(pos, rot);
        }
    }
}