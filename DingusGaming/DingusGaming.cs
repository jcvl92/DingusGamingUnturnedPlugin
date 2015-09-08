using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using DingusGaming.Party;
using DingusGaming.Store;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DingusGaming
{
    public class DGPlugin : RocketPlugin
    {
        //contains helper functions for persisting data and centralizing common functions
        private static VehicleManager vehicleManager;

        protected override void Load()
        {
            //Initialize components
            Currency.init();
            Stores.init();
            Parties.init();

            Logger.LogWarning("DingusGaming Plugin Loaded!");

            vehicleManager = ((VehicleManager) typeof (VehicleManager).GetField("manager", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null));

            U.Settings.Instance.AutomaticSave.Interval = 5*60;

            UnturnedPlayerEvents.OnPlayerChatted += delegate (UnturnedPlayer player, ref Color color, string message, EChatMode chatMode)
            {
                //TODO: put in color changing logic here
            };

            //Save every 5 minutes
            Timer saveTimer = new Timer(5*60*1000);
            saveTimer.Elapsed += delegate
            {
                Currency.saveBalances();
                Logger.LogWarning("DGPlugin state saved.");
            };
            saveTimer.Start();
        }

        protected override void Unload()
        {
            //is called by Rocket before shutting down
            //Steam.OnServerShutdown.Invoke();
            Currency.saveBalances();
        }

        public void FixedUpdate()
        {
            //is called every game update
        }

        /********** HELPER FUNCTIONS **********/

        public static UnturnedPlayer getKiller(UnturnedPlayer player, EDeathCause cause, CSteamID murderer)
        {
            if (cause == EDeathCause.GUN || cause == EDeathCause.MELEE ||
                cause == EDeathCause.PUNCH || cause == EDeathCause.ROADKILL)
                return getPlayer(murderer);
            return null;
        }

        public static void teleportPlayer(UnturnedPlayer player, UnturnedPlayer target)
        {
            removeFromVehicle(player);

            //put them into the target's vehicle, if they are in one
            if (target.Player.Movement.getVehicle() != null)
                addToVehicle(player, target.Player.Movement.getVehicle().index);
            else
                player.Teleport(target);
        }

        public static void teleportPlayer(UnturnedPlayer player, Vector3 position, float rotation)
        {
            removeFromVehicle(player);

            //level them with the ground
            position.y = LevelGround.getHeight(position);

            player.Teleport(position, rotation);
        }

        public static void teleportPlayer(UnturnedPlayer player, string nodeName)
        {
            removeFromVehicle(player);

            player.Teleport(nodeName);
        }

        public static void teleportPlayerInRadius(UnturnedPlayer player, Vector3 position, float radius)
        {
            System.Random random = new System.Random();

            //change the x and z values to be within the radius
            float newX = position.x + (float)(random.NextDouble() * radius * 2) - radius;
            float newZ = position.z + (float)(random.NextDouble() * radius * 2) - radius;

            //rotate them towards the center of the point
            float rotation = (float)(Math.Atan2(newX - position.x, newZ - position.z)*180/Math.PI) + 180;

            position.x = newX;
            position.z = newZ;

            teleportPlayer(player, position, rotation);
        }

        public static void removeFromVehicle(UnturnedPlayer player)
        {
            if(player.Player.Movement.getVehicle() != null)
                vehicleManager.askExitVehicle(player.CSteamID, player.Position);
        }

        public static void addToVehicle(UnturnedPlayer player, ushort index)
        {
            vehicleManager.askEnterVehicle(player.CSteamID, index);
        }

        public static void disableCommands()
        {
            //TODO: implement this
        }

        public static void enableCommands()
        {
            //TODO: implement this
        }

        public static void respawnPlayer(UnturnedPlayer player)
        {
            //zero their last respawn time in order to circumvent the timer(using reflection)
            typeof (PlayerLife).GetField("_lastRespawn", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player.Player.life, 0);

            //respawn them
            player.Player.life.sendRespawn(false);
            player.Player.life.askRespawn(player.CSteamID, false);
        }

        public static void messagePlayer(UnturnedPlayer player, string message)
        {
            var strs = UnturnedChat.wrapMessage(message);
            foreach (var str in strs)
                UnturnedChat.Say(player, str);
        }

        public static void broadcastMessage(string text)
        {
            var strs = UnturnedChat.wrapMessage(text);
            foreach (var str in strs)
                UnturnedChat.Say(str);
        }

        public static List<DictionaryEntry> convertFromDictionary(IDictionary dictionary)
        {
            var entries = new List<DictionaryEntry>(dictionary.Count);
            foreach (var key in dictionary.Keys)
                entries.Add(new DictionaryEntry(key, dictionary[key]));
            return entries;
        }

        public static Dictionary<TKey, TValue> convertToDictionary<TKey, TValue>(List<DictionaryEntry> entries)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var entry in entries)
                dictionary[(TKey) entry.Key] = (TValue) entry.Value;
            return dictionary;
        }

        public static UnturnedPlayer getPlayer(string name)
        {
            return UnturnedPlayer.FromName(name);
        }

        public static bool givePlayerItem(UnturnedPlayer player, ushort itemID, byte quantity)
        {
            return player.GiveItem(itemID, quantity);
        }

        public static string getConstantID(UnturnedPlayer player)
        {
            return player.CSteamID.ToString();
        }

        public static UnturnedPlayer getPlayer(CSteamID playerID)
        {
            return UnturnedPlayer.FromCSteamID(playerID);
        }

        public static void writeToFile(object obj, string fileName)
        {
            var serializer = new XmlSerializer(obj.GetType(), "");
            XmlTextWriter xmlTextWriter = null;

            try
            {
                xmlTextWriter = new XmlTextWriter("DGPLugin_" + fileName, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                serializer.Serialize(xmlTextWriter, obj);
            }
            finally
            {
                xmlTextWriter?.Close();
            }
        }

        public static T readFromFile<T>(string fileName)
        {
            var serializer = new XmlSerializer(typeof (T));
            XmlTextReader xmlTextReader = null;

            try
            {
                xmlTextReader = new XmlTextReader("DGPLugin_" + fileName);

                if (serializer.CanDeserialize(xmlTextReader))
                    return (T) serializer.Deserialize(xmlTextReader);
                return default(T);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
            finally
            {
                xmlTextReader?.Close();
            }
        }
    }
}