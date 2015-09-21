using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Steamworks;
using UnityEngine;
using Random=System.Random;

namespace DingusGaming.Party
{
    public class Party
    {
        private readonly List<UnturnedPlayer> members = new List<UnturnedPlayer>();
        private UnturnedPlayer leader;
        private static readonly Color chatColor = Color.cyan;
        private CSteamID steamGroup;
        private static List<ulong> claimedIDs = new List<ulong>();
        private static readonly Random random = new Random();

        public Party(UnturnedPlayer leader)
        {
            //create the party's fake steam group
            var buffer = new byte[sizeof(ulong)];
            random.NextBytes(buffer);
            ulong newID = BitConverter.ToUInt64(buffer, 0);
            steamGroup = new CSteamID(newID);
            claimedIDs.Add(newID);

            //set and add the leader
            this.leader = leader;
            leader.Player.SteamChannel.SteamPlayer.playerID.SteamGroupID = steamGroup;
            members.Add(leader);
        }

        public ReadOnlyCollection<UnturnedPlayer> getMembers()
        {
            return members.AsReadOnly();
        }

        public void disband()
        {
            //remove everyone from the steam group
            foreach(var member in members)
                member.Player.SteamChannel.SteamPlayer.playerID.SteamGroupID = CSteamID.Nil;

            //unclaim the ID
            claimedIDs.Remove(steamGroup.m_SteamID);

            //clear the members list
            members.Clear();
            
            //remove this party from the party list
            Parties.removeParty(this);
        }

        public CSteamID getSteamGroup()
        {
            return steamGroup;
        }

        public void setSteamGroup(CSteamID group)
        {
            steamGroup = group;
            foreach (var member in members)
                member.Player.SteamChannel.SteamPlayer.playerID.SteamGroupID = group;
        }

        public bool isMember(UnturnedPlayer player)
        {
            foreach(UnturnedPlayer member in members)
                if (member.Equals(player))
                    return true;
            return false;
        }

        public bool isLeader(UnturnedPlayer player)
        {
            return leader.Equals(player);
        }

        public string getInfo()
        {
            var info = "";

            foreach (var member in members)
                info += member.CharacterName + (isLeader(member) ? "[L](" : "(") +
                        (member.Dead ? "dead" : member.Health + "/100") + "), ";

            return info.Substring(0, info.Length - 2);
        }

        public UnturnedPlayer getLeader()
        {
            return leader;
        }

        public void tellParty(string text)
        {
            foreach (var member in members)
                DGPlugin.messagePlayer(member, text, chatColor);
        }

        public void tellParty(string text, UnturnedPlayer skipPlayer)
        {
            foreach (var member in members)
                if(!member.Equals(skipPlayer))
                    DGPlugin.messagePlayer(member, text, chatColor);
        }

        public void chat(UnturnedPlayer caller, string text)
        {
            if (isMember(caller))
                tellParty(caller.CharacterName + (isLeader(caller) ? "[L]: " : "[P]: ") + text);
            else
                DGPlugin.messagePlayer(caller, "Error, you are not in this party.");
        }

        public void addMember(UnturnedPlayer player)
        {
            //add the player to the steam group for this party
            player.Player.SteamChannel.SteamPlayer.playerID.SteamGroupID = steamGroup;
            
            members.Add(player);

            tellParty(player.CharacterName + " has joined the party!", player);
            DGPlugin.messagePlayer(player, "You have joined the party!", chatColor);
        }

        public void kickMember(UnturnedPlayer caller, UnturnedPlayer player)
        {
            if (!isMember(player))
            {
                DGPlugin.messagePlayer(caller, player.CharacterName + " is not in your party.");
                return;
            }

            if (leader.Equals(caller))
            {
                removeMember(player);
                DGPlugin.messagePlayer(player, "You were removed from the party.");
            }
            else
                DGPlugin.messagePlayer(caller,
                    "Only the party leader(" + leader.CharacterName + ") can kick party members.");
        }

        public void removeMember(UnturnedPlayer player)
        {
            //remove the player from the steam group for this party
            player.Player.SteamChannel.SteamPlayer.playerID.SteamGroupID = CSteamID.Nil;

            members.RemoveAt(members.FindIndex(0, x => x.Equals(player)));

            Parties.toggleChat(player, false);

            //promote a new leader if the leader was removed
            if (members.Count > 1)
            {
                if (leader.Equals(player))
                {
                    leader = members.First();
                    tellParty(leader.CharacterName + " has been made party leader!");
                }
            }
            else
            {
                Parties.disbandParty(this);
            }
        }

        public void makeLeader(UnturnedPlayer caller, UnturnedPlayer player)
        {
            if (caller.Equals(player))
            {
                DGPlugin.messagePlayer(caller, "You are already the party leader.");
                return;
            }
            if (isMember(player))
            {
                if (isLeader(caller))
                {
                    leader = player;
                    tellParty(player.CharacterName + " has been made party leader!");
                }
                else
                {
                    DGPlugin.messagePlayer(caller,
                        "Only the party leader(" + leader.CharacterName + ") switch leaders.");
                }
            }
            else
                DGPlugin.messagePlayer(caller, "Could not find " + player.CharacterName + " in your party.");
        }
    }
}