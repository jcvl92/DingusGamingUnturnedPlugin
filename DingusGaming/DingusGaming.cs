﻿using System;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;

namespace DingusGaming
{
    public class DGPlugin : Rocket.Core.Plugins.RocketPlugin
    {
        protected override void Load()
        {
            //is run after start by Rocket but still at initial load of the plugin
            Logger.LogWarning("\tPlugin loaded successfully!");
        }

        public void FixedUpdate()
        {
            //is called every game update
        }

        public static void messagePlayer(UnturnedPlayer player, string text)
        {
            List<string> strs = UnturnedChat.wrapMessage(text);
            foreach (string str in strs)
            {
                UnturnedChat.Say(player, str);
            }
        }

        public static UnturnedPlayer getPlayer(string name)
        {
            return UnturnedPlayer.FromName(name);
        }
    }
}