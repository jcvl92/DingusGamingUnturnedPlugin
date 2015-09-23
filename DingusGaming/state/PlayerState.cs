using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace DingusGaming.helper
{
    public class PlayerState
    {
        private uint experience;
        private PlayerInventory inventory;
        private Vector3 position;
        private float rotation;
        private PlayerSkills skills;
        private PlayerSurvivalStats stats;
        public InteractableVehicle vehicle;

        public static PlayerState getState(UnturnedPlayer player)
        {
            var state = new PlayerState();

            state.position = player.Position;
            state.rotation = player.Rotation;

            state.inventory = PlayerInventory.getInventory(player);

            state.experience = player.Experience;
            state.skills = PlayerSkills.getSkills(player);

            state.stats = PlayerSurvivalStats.getStats(player);

            state.vehicle = player.Player.Movement.getVehicle();

            return state;
        }

        public static void clearInventory(UnturnedPlayer player)
        {
            //dequip anything they have equipped
            player.Player.Equipment.dequip();

            //remove items
            foreach (var items in player.Inventory.Items)
                for (int i=items.getItemCount()-1; i >= 0; --i)
                    player.Inventory.removeItem(items.page,
                        items.getIndex(items.getItem((byte)i).PositionX, items.getItem((byte)i).PositionY));

            //remove clothes
            var p = player.Player;
            if (p.Clothing.backpack != 0)
            {
                p.Clothing.askWearBackpack(0, 0, new byte[0]);
                p.Inventory.removeItem(2, 0);
            }
            if (p.Clothing.glasses != 0)
            {
                p.Clothing.askWearGlasses(0, 0, new byte[0]);
                p.Inventory.removeItem(2, 0);
            }
            if (p.Clothing.hat != 0)
            {
                p.Clothing.askWearHat(0, 0, new byte[0]);
                p.Inventory.removeItem(2, 0);
            }
            if (p.Clothing.mask != 0)
            {
                p.Clothing.askWearMask(0, 0, new byte[0]);
                p.Inventory.removeItem(2, 0);
            }
            if (p.Clothing.pants != 0)
            {
                p.Clothing.askWearPants(0, 0, new byte[0]);
                p.Inventory.removeItem(2, 0);
            }
            if (p.Clothing.shirt != 0)
            {
                p.Clothing.askWearShirt(0, 0, new byte[0]);
                p.Inventory.removeItem(2, 0);
            }
            if (p.Clothing.vest != 0)
            {
                p.Clothing.askWearVest(0, 0, new byte[0]);
                p.Inventory.removeItem(2, 0);
            }
        }

        public static void clearStats(UnturnedPlayer player)
        {
            player.Hunger = 0;
            player.Infection = 0;
            player.Thirst = 0;
            player.Heal(100);
            player.Bleeding = false;
            player.Broken = false;
        }

        public void setCompleteState(UnturnedPlayer player)
        {
            setInventory(player);
            setSkills(player);
            setStats(player);
            setLocation(player);
        }

        public void setLocation(UnturnedPlayer player)
        {
            if (vehicle != null)
                DGPlugin.addToVehicle(player, vehicle);
            else
                DGPlugin.teleportPlayer(player, position, rotation);
        }

        public void setInventory(UnturnedPlayer player)
        {
            inventory.setInventory(player);
        }

        public void setSkills(UnturnedPlayer player)
        {
            player.Experience = Math.Max(experience, player.Experience);
            skills.setSkills(player);
        }

        public void setStats(UnturnedPlayer player)
        {
            stats.setStats(player);
        }

        private class PlayerSurvivalStats
        {
            private bool bleeding, broken;
            private byte hunger, infection, health, thirst;

            public static PlayerSurvivalStats getStats(UnturnedPlayer player)
            {
                return new PlayerSurvivalStats
                {
                    broken = player.Broken,
                    bleeding = player.Bleeding,
                    hunger = player.Hunger,
                    infection = player.Infection,
                    health = player.Health,
                    thirst = player.Thirst
                };
            }

            public void setStats(UnturnedPlayer player)
            {
                player.Hunger = (byte) (100 - hunger);
                player.Infection = (byte) (100 - infection);
                player.Thirst = (byte) (100 - thirst);

                var healthDiff = health - player.Health;
                if (healthDiff > 0)
                    player.Heal((byte) healthDiff);
                else if (healthDiff < 0)
                    player.Damage((byte) -healthDiff, new Vector3(), EDeathCause.KILL, ELimb.SPINE, player.CSteamID);

                player.Bleeding = bleeding;
                player.Broken = broken;
            }
        }

        private class PlayerInventory
        {
            private ushort backpack, glasses, hat, mask, pants, shirt, vest;

            private byte equippedPage, equipped_x, equipped_y, bpQuality, gQuality, hQuality, mQuality, pQuality, sQuality, vQuality;

            private byte[] bpState, gState, hState, mState, pState, sState, vState;

            private Dictionary<byte, List<ItemJar>> itemsMap;

            public static PlayerInventory getInventory(UnturnedPlayer player)
            {
                var p = player.Player;

                //get the items
                var itemsMap = new Dictionary<byte, List<ItemJar>>();
                foreach (var items in p.Inventory.Items)
                {
                    var itemList = new List<ItemJar>();
                    for (byte i = 0; i < items.getItemCount(); ++i)
                    {
                        var itemJar = items.getItem(i);
                        itemList.Add(new ItemJar(itemJar.PositionX, itemJar.PositionY, itemJar.item));
                    }
                    itemsMap.Add(items.page, itemList);
                }

                return new PlayerInventory
                {
                    //save the equipped item data
                    equippedPage = p.Equipment.equippedPage,
                    equipped_x = p.Equipment.equipped_x,
                    equipped_y = p.Equipment.equipped_y,

                    //save all the gear
                    itemsMap = itemsMap,

                    //save all the clothes/accessories and their states
                    backpack = p.Clothing.backpack,
                    bpQuality = p.Clothing.backpackQuality,
                    bpState = p.Clothing.backpackState,
                    glasses = p.Clothing.glasses,
                    gQuality = p.Clothing.glassesQuality,
                    gState = p.Clothing.glassesState,
                    hat = p.Clothing.hat,
                    hQuality = p.Clothing.hatQuality,
                    hState = p.Clothing.hatState,
                    mask = p.Clothing.mask,
                    mQuality = p.clothing.maskQuality,
                    mState = p.Clothing.maskState,
                    pants = p.Clothing.pants,
                    pQuality = p.Clothing.pantsQuality,
                    pState = p.Clothing.pantsState,
                    shirt = p.Clothing.shirt,
                    sQuality = p.Clothing.shirtQuality,
                    sState = p.Clothing.shirtState,
                    vest = p.Clothing.vest,
                    vQuality = p.Clothing.vestQuality,
                    vState = p.Clothing.vestState
                };
            }

            public void setInventory(UnturnedPlayer player)
            {
                var p = player.Player;

                clearInventory(player);

                //add clothes - states are blank because I don't think they are needed
                p.Clothing.askWearBackpack(backpack, bpQuality, bpState);
                p.Clothing.askWearGlasses(glasses, gQuality, gState);
                p.Clothing.askWearHat(hat, hQuality, hState);
                p.Clothing.askWearMask(mask, mQuality, mState);
                p.Clothing.askWearPants(pants, pQuality, pState);
                p.Clothing.askWearShirt(shirt, sQuality, sState);
                p.Clothing.askWearVest(vest, vQuality, vState);

                foreach (var items in p.Inventory.Items)
                    foreach (var item in itemsMap[items.page])
                        items.addItem(item.PositionX, item.PositionY, item.Item);

                p.Equipment.tryEquip(equippedPage, equipped_x, equipped_y);

                //p.Inventory.askInventory(player.CSteamID);
            }
        }

        private class PlayerSkills
        {
            private byte[][] skills;

            public static PlayerSkills getSkills(UnturnedPlayer player)
            {
                var oldSkills = player.Player.Skills.skills;
                var newSkills = new byte[oldSkills.Length][];

                for (var i = 0; i < oldSkills.Length; ++i)
                {
                    newSkills[i] = new byte[oldSkills[i].Length];
                    for (var j = 0; j < oldSkills[i].Length; ++j)
                        newSkills[i][j] = oldSkills[i][j].level;
                }

                return new PlayerSkills
                {
                    skills = newSkills
                };
            }

            public void setSkills(UnturnedPlayer player)
            {
                var currentSkills = player.Player.Skills.skills;

                for (var i = 0; i < currentSkills.Length; ++i)
                    for (var j = 0; j < currentSkills[i].Length; ++j)
                        currentSkills[i][j].level = skills[i][j];

                //update the client-side
                player.Player.Skills.askSkills(player.CSteamID);
            }
        }
    }
}