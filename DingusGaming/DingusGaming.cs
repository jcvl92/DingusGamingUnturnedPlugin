using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using DingusGaming.Arena;
using DingusGaming.helper;
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
        private static object fileLock = new object();
        private static event PermissionRequested permissionHolder;
        private const int saveInterval = 5*60;

        protected override void Load()
        {
            //Initialize components
            Currency.init();
            Stores.init();
            Parties.init();

            Logger.LogWarning("DingusGaming Plugin Loaded!");

            vehicleManager = ((VehicleManager) typeof (VehicleManager).GetField("manager", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null));

            UnturnedPlayerEvents.OnPlayerChatted += delegate (UnturnedPlayer player, ref Color color, string message, EChatMode chatMode)
            {
                //TODO: put in color changing logic here
            };

            UnturnedPlayerEvents.OnPlayerDeath += delegate(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
            {
                //NOTE: try-catch blocks in this function in case inventory gets bugged, as it sometimes does
                if (!ArenaEvent.isOccurring)
                {
                    //remove loadout items
                    foreach (var items in player.Inventory.Items)
                        for (int i=items.getItemCount()-1; i >= 0; --i)
                            try
                            {
                                if (isItemInLoadout(items.getItem((byte)i).Item.ItemID))
                                    player.Inventory.removeItem(items.page,
                                        items.getIndex(items.getItem((byte)i).PositionX,
                                            items.getItem((byte)i).PositionY));
                            }
                            catch (Exception) {}

                    //drop all other gear(to make room for dropping clothing)
                    foreach (var items in player.Inventory.Items)
                        for (int i=items.getItemCount()-1; i >= 0; --i)
                            try
                            {
                                player.Inventory.askDropItem(player.CSteamID, items.page, items.getItem(0).PositionX,items.getItem(0).PositionY);
                            }
                            catch (Exception){}

                    //remove clothing in the loadout
                    ushort[] loadout = PlayerInventory.loadout;
                    var p = player.Player;
                    for (int i=0; i < loadout.Length; ++i)
                    {
                        try
                        {
                            if (p.Clothing.backpack == loadout[i])
                            {
                                p.Clothing.askWearBackpack(0, 0, new byte[0]);
                                p.Inventory.removeItem(2, 0);
                            }
                            else if (p.Clothing.glasses == loadout[i])
                            {
                                p.Clothing.askWearGlasses(0, 0, new byte[0]);
                                p.Inventory.removeItem(2, 0);
                            }
                            else if (p.Clothing.hat == loadout[i])
                            {
                                p.Clothing.askWearHat(0, 0, new byte[0]);
                                p.Inventory.removeItem(2, 0);
                            }
                            else if (p.Clothing.mask == loadout[i])
                            {
                                p.Clothing.askWearMask(0, 0, new byte[0]);
                                p.Inventory.removeItem(2, 0);
                            }
                            else if (p.Clothing.pants == loadout[i])
                            {
                                p.Clothing.askWearPants(0, 0, new byte[0]);
                                p.Inventory.removeItem(2, 0);
                            }
                            else if (p.Clothing.shirt == loadout[i])
                            {
                                p.Clothing.askWearShirt(0, 0, new byte[0]);
                                p.Inventory.removeItem(2, 0);
                            }
                            else if (p.Clothing.vest == loadout[i])
                            {
                                p.Clothing.askWearVest(0, 0, new byte[0]);
                                p.Inventory.removeItem(2, 0);
                            }
                        }
                        catch (Exception){}
                    }
                }
            };

            U.Settings.Instance.AutomaticSave.Interval = saveInterval;
            Timer saveTimer = new Timer(saveInterval*1000);
            saveTimer.Elapsed += delegate
            {
                Currency.saveBalances();
                Logger.LogWarning("DGPlugin state saved.");
            };
            saveTimer.Start();
        }

        public void delaySaving(int seconds)
        {
            U.Settings.Instance.AutomaticSave.Interval += seconds;
        }

        public void clearSaveDelay()
        {
            U.Settings.Instance.AutomaticSave.Interval = saveInterval;
        }

        private bool isItemInLoadout(ushort itemID)
        {
            ushort[] loadout = PlayerInventory.loadout;
            for (int i = 0; i < loadout.Length; ++i)
                if (loadout[i] == itemID)
                    return true;
            return false;
        }

        protected override void Unload()
        {
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

        public static bool teleportPlayer(UnturnedPlayer player, UnturnedPlayer target)
        {
            removeFromVehicle(player);

            //put them into the target's vehicle, if they are in one
            InteractableVehicle vehicle = target.Player.Movement.getVehicle();
            if (vehicle != null)
                return addToVehicle(player, vehicle);

            //otherwise teleport them to the target
            player.Teleport(target);
            return true;
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
            if (player.Player.Movement.getVehicle() != null)
                vehicleManager.askExitVehicle(player.CSteamID, new Vector3(0, 0, 0));
        }

        public static bool addToVehicle(UnturnedPlayer player, InteractableVehicle vehicle)
        {
            if (player == null || vehicle == null)
                return false;

            byte seat = byte.MaxValue;
            if (vehicle.isExploded != true)
                for (byte index = 0; index < vehicle.passengers.Length; ++index)
                    if (vehicle.passengers[index] != null && vehicle.passengers[index].player == null)
                    {
                        seat = index;
                        break;
                    }

            if (seat != byte.MaxValue)
            {
                vehicleManager.channel.send("tellEnterVehicle", (ESteamCall)1, (ESteamPacket)15, new object[]{vehicle.index, seat, player.CSteamID});
                return true;
            }
            else
            {
                double angle = ((vehicle.transform.eulerAngles.y + 180) * Math.PI) / 180;
                float sin = (float)Math.Sin(angle), cos = (float)Math.Cos(angle);
                Vector3 pos = new Vector3(vehicle.transform.position.x + sin * 6, vehicle.transform.position.y, vehicle.transform.position.z + cos * 6);

                teleportPlayer(player, pos, (float)((angle * 180) / Math.PI) - 180);

                return false;
            }
        }

        public static void disableCommands()
        {
            if(UnturnedPermissions.OnPermissionRequested != null)
            {
                permissionHolder = UnturnedPermissions.OnPermissionRequested;
                UnturnedPermissions.OnPermissionRequested = null;
            }
        }

        public static void enableCommands()
        {
            if(permissionHolder != null)
            {
                UnturnedPermissions.OnPermissionRequested = permissionHolder;
                permissionHolder = null;
            }
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
            lock (fileLock)
            {
                using (XmlTextWriter writer = new XmlTextWriter("DGPLugin_" + fileName, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    new XmlSerializer(obj.GetType(), "").Serialize(writer, obj);
                }
            }
        }

        public static T readFromFile<T>(string fileName)
        {
            lock (fileLock)
            {
                try
                {
                    using (XmlTextReader xmlTextReader = new XmlTextReader("DGPLugin_" + fileName))
                    {
                        var serializer = new XmlSerializer(typeof (T));
                        if (serializer.CanDeserialize(xmlTextReader))
                            return (T) serializer.Deserialize(xmlTextReader);
                        return default(T);
                    }
                }
                catch (FileNotFoundException)
                {
                    return default(T);
                }
            }
        }
    }
}