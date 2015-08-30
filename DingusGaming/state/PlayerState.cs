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
            private bool isEquipped;
            private byte equippedPage, equipped_x, equipped_y;
            private Item[] items;

            public static PlayerInventory getInventory(UnturnedPlayer player)
            {
                //TODO: following this: https://github.com/Zamirathe/ZaupClearInventoryLib/blob/master/ZaupClearInventoryLib.cs
                Player p = player.Player;
                

                //get the items
                Item[] items = new Item[p.Inventory.Items.Length];
                p.Inventory.Items.CopyTo(items, 0);

                return new PlayerInventory
                {
                    //save the equipped item data
                    isEquipped = p.Equipment.isEquipped,
                    equippedPage = p.Equipment.equippedPage,
                    equipped_x = p.Equipment.equipped_x,
                    equipped_y = p.Equipment.equipped_y,

                    //save the holding slots


                    //save all the gear
                    items = items

                    //save all the clothes/accessories
                    //p.Clothing.
                };
            }

            public void setInventory(UnturnedPlayer player)
            {
                //askInventory?

                //equip the item if they had one equipped
                if(isEquipped)
                    player.Player.Equipment.equip(equippedPage, equipped_x, equipped_y);
            }
        }

        private class PlayerSkills
        {
            public static PlayerSkills getSkills(UnturnedPlayer player)
            {
                return new PlayerSkills
                {

                };
            }

            public void setSkills(UnturnedPlayer player)
            {

            }
        }
    }
}
