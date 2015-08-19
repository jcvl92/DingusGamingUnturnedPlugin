using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace DingusGaming
{
    public class DGPlugin : RocketPlugin
    {
        //contains helper functions for persisting data and centralizing common functions

        protected override void Load()
        {
            //Initialize components
            Store.Currency.init();
            Store.Stores.init();
            Party.Parties.init();

            Logger.LogWarning("DingusGaming Plugin Loaded!");
        }

        protected override void Unload()
        {
            //is called by Rocket before shutting down
            Steam.OnServerShutdown.Invoke();
        }

        public void FixedUpdate()
        {
            //is called every game update
        }

        /********** HELPER FUNCTIONS **********/

        public static UnturnedPlayer getKiller(UnturnedPlayer player, EDeathCause cause, CSteamID murderer)
        {
            if (cause == EDeathCause.KILL)
                //return !murderer.m_SteamID.Equals(90071992547409920) && !player.CSteamID.Equals(murderer);
                //get the last player that damaged them before they die(timeout is 30 seconds)
                return getPlayer(murderer);
            else
                return null;
        }

        public static void disableCommands()
        {

        }

        public static void enableCommands()
        {

        }

        public static void messagePlayer(UnturnedPlayer player, string message)
        {
            List<string> strs = UnturnedChat.wrapMessage(message);
            foreach (string str in strs)
                UnturnedChat.Say(player, "0" + str);
        }

        public static void broadcastMessage(string text)
        {
            List<string> strs = UnturnedChat.wrapMessage(text);
            foreach (string str in strs)
                UnturnedChat.Say(str);
        }

        public static UnturnedPlayer getPlayer(string name)
        {
            return UnturnedPlayer.FromName(name);
        }

        public static void givePlayerItem(UnturnedPlayer player, ushort itemID, byte quantity)
        {
            player.GiveItem(itemID, quantity);
        }

        public static string getConstantID(UnturnedPlayer player)
        {
            return player.CSteamID.ToString();
        }

        public static UnturnedPlayer getPlayer(CSteamID playerID)
        {
            return UnturnedPlayer.FromCSteamID(playerID);
        }

        
    }
}