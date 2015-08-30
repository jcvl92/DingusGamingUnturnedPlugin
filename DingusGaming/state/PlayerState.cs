using System.Collections.Generic;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace DingusGaming.helper
{
    internal class PlayerState
    {
        private Vector3 position;
        private float rotation;
        private PlayerInventory inventory;
        private PlayerSkills skills;
        private PlayerSurvivalStats stats;

        public static PlayerState getState(UnturnedPlayer player)
        {
            PlayerState state = new PlayerState();

            state.position = player.Position;
            state.rotation = player.Rotation;

            state.inventory = PlayerInventory.getInventory(player);

            state.skills = PlayerSkills.getSkills(player);

            state.stats = PlayerSurvivalStats.getStats(player);

            return state;
        }

        public void setCompleteState(UnturnedPlayer player)
        {
            setLocation(player);
            setInventory(player);
            setSkills(player);
            setStats(player);
        }

        public void setLocation(UnturnedPlayer player)
        {
            player.Teleport(position, rotation);
        }

        public void setInventory(UnturnedPlayer player)
        {
            inventory.setInventory(player);
        }

        public void setSkills(UnturnedPlayer player)
        {
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
                player.Hunger = (byte)(100-hunger);
                player.Infection = (byte)(100 -infection);
                player.Thirst = (byte)(100 -thirst);

                int healthDiff = health - player.Health;
                if (healthDiff > 0)
                    player.Heal((byte)healthDiff);
                else if (healthDiff < 0)
                    player.Damage((byte)-healthDiff, player.Position, EDeathCause.KILL, ELimb.SPINE, player.CSteamID);

                player.Bleeding = bleeding;
                player.Broken = broken;
            }
        }

        private class PlayerInventory
        {
            private byte equippedPage, equipped_x, equipped_y,
                bpQuality, gQuality, hQuality, mQuality, pQuality, sQuality, vQuality;
            private ushort backpack, glasses, hat, mask, pants, shirt, vest;
            private Dictionary<Items, List<ItemJar>> itemsMap;

            public static PlayerInventory getInventory(UnturnedPlayer player)
            {
                Player p = player.Player;

                //get the items
                Dictionary<Items, List<ItemJar>> itemsMap = new Dictionary<Items, List<ItemJar>>();
                foreach (Items items in p.Inventory.Items)
                {
                    List<ItemJar> itemList = new List<ItemJar>();
                    for (byte i = 0; i < items.getItemCount(); ++i)
                        itemList.Add(items.getItem(i));//TODO: possibly clone this if it doesn't work(or scrape it's details) - we will know if it works or not after this is tested after clearing
                    itemsMap.Add(items, itemList);
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
                    glasses = p.Clothing.glasses,
                    gQuality = p.Clothing.glassesQuality,
                    hat = p.Clothing.hat,
                    hQuality = p.Clothing.hatQuality,
                    mask = p.Clothing.mask,
                    mQuality = p.clothing.maskQuality,
                    pants = p.Clothing.pants,
                    pQuality = p.Clothing.pantsQuality,
                    shirt = p.Clothing.shirt,
                    sQuality = p.Clothing.shirtQuality,
                    vest = p.Clothing.vest,
                    vQuality = p.Clothing.vestQuality
                };
            }

            public void setInventory(UnturnedPlayer player)
            {
                Player p = player.Player;

                //TODO: clear the inventory first? maybe do that somewhere else

                //add clothes
                p.Clothing.askWearBackpack(backpack, bpQuality, new byte[0]);
                p.Clothing.askWearGlasses(glasses, gQuality, new byte[0]);
                p.Clothing.askWearHat(hat, hQuality, new byte[0]);
                p.Clothing.askWearMask(mask, mQuality, new byte[0]);
                p.Clothing.askWearPants(pants, pQuality, new byte[0]);
                p.Clothing.askWearShirt(shirt, sQuality, new byte[0]);
                p.Clothing.askWearVest(vest, vQuality, new byte[0]);

                foreach (Items items in p.Inventory.Items)
                    foreach(ItemJar item in itemsMap[items])
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
                Skill[][] oldSkills = player.Player.Skills.skills;
                byte[][] newSkills = new byte[oldSkills.Length][];

                for (int i = 0; i < oldSkills.Length; ++i)
                    for (int j = 0; j < oldSkills[i].Length; ++j)
                    {
                        newSkills[i] = new byte[oldSkills[i].Length];
                        newSkills[i][j] = oldSkills[i][j].level;
                    }

                return new PlayerSkills
                {
                    skills = newSkills
                };
            }

            public void setSkills(UnturnedPlayer player)
            {
                Skill[][] currentSkills = player.Player.Skills.skills;

                for (int i=0; i<currentSkills.Length; ++i)
                    for (int j = 0; j < currentSkills[i].Length; ++j)
                        currentSkills[i][j].level = skills[i][j];

                //update the client-side?
            }
        }
    }
}
