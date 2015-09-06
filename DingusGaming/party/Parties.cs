using System;
using System.Collections.Generic;
using DingusGaming.Store;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace DingusGaming.Party
{
    public class Parties
    {
        private static readonly List<Party> parties = new List<Party>();
        private static readonly List<Invite> invites = new List<Invite>();
        private static readonly HashSet<CSteamID> chatToggles = new HashSet<CSteamID>();
        public static bool showDeathMessages = true;

        public static void init()
        {
            registerOnPlayerDeath();
            registerOnPlayerDisconnected();
        }

        private static void registerOnPlayerDeath()
        {
            //notify party of death
            UnturnedPlayerEvents.OnPlayerDeath +=
                delegate(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
                {
                    if (showDeathMessages)
                    {
                        UnturnedPlayer killer = DGPlugin.getKiller(player, cause, murderer);
                        if (killer == null)
                            getParty(player)?.tellParty(player.CharacterName + " has died!");
                        else
                        {
                            getParty(player)?.tellParty(player.CharacterName + " has been killed by " + killer.CharacterName + "!");
                            getParty(killer)?.tellParty(killer.CharacterName + " has killed " + player.CharacterName + "!", killer);
                        }
                    }

                    CommandTeleport.playerDied(player);
                };
        }

        private static void registerOnPlayerDisconnected()
        {
            U.Events.OnPlayerDisconnected += delegate(UnturnedPlayer player)
            {
                //remove them from their party
                var party = getParty(player);
                party?.removeMember(player);
                party?.tellParty(player.CharacterName + " has disconnected!");

                //clear pending invites
                removeInvite(player);
            };
        }

        public static void toggleChat(UnturnedPlayer player)
        {
            if (!chatToggles.Remove(player.CSteamID))
                chatToggles.Add(player.CSteamID);
        }

        public static void toggleChat(UnturnedPlayer player, bool enabled)
        {
            if (enabled)
            {
                chatToggles.Add(player.CSteamID);
            }
            else
            {
                chatToggles.Remove(player.CSteamID);
            }
        }

        public static bool isChatToggled(UnturnedPlayer player)
        {
            return chatToggles.Contains(player.CSteamID);
        }

        public static void invitePlayer(UnturnedPlayer caller, UnturnedPlayer player)
        {
            if (getParty(player) != null)
            {
                DGPlugin.messagePlayer(caller,
                    player.CharacterName + " is already in a party. They must /leave it before you can invite them.");
                return;
            }
            if (getInvite(player) != null)
            {
                DGPlugin.messagePlayer(caller,
                    player.CharacterName +
                    " already has an invite pending. They must /decline it before you can invite them.");
                return;
            }

            invites.Add(new Invite(caller, player));
            DGPlugin.messagePlayer(player,
                caller.CharacterName + " is inviting you to join a party. Enter /accept or /decline to respond.");
            DGPlugin.messagePlayer(caller, "Invite sent to " + player.CharacterName + ".");
        }

        private static Invite getInvite(UnturnedPlayer player)
        {
            foreach (var invite in invites)
                if (invite.playerRequested.Equals(player.CSteamID))
                    return invite;
            return null;
        }

        public static void acceptInvite(UnturnedPlayer caller)
        {
            var invite = getInvite(caller);
            if (invite == null)
                DGPlugin.messagePlayer(caller, "You have no pending invites!");
            else if (getParty(caller) != null)
            {
                DGPlugin.messagePlayer(caller, "You cannot accept an invite while in a party!");
                invites.Remove(invite);
            }
            else
            {
                invite.join();
                invites.Remove(invite);
                removeSentInvites(caller);
            }
        }

        public static void removeSentInvites(UnturnedPlayer player)
        {
            List<Invite> invitesToRemove = new List<Invite>();
            foreach (var invite in invites)
                if (invite.requester.Equals(player.CSteamID))
                    invitesToRemove.Add(invite);
            foreach (var inviteToRemove in invitesToRemove)
                invites.Remove(inviteToRemove);
        }

        public static void declineInvite(UnturnedPlayer caller)
        {
            var invite = getInvite(caller);
            if (invite == null)
                DGPlugin.messagePlayer(caller, "You have no pending invites!");
            else
            {
                DGPlugin.messagePlayer(DGPlugin.getPlayer(invite.requester),
                    caller.CharacterName + " declined your invite.");
                DGPlugin.messagePlayer(caller,
                    "You have declined " + DGPlugin.getPlayer(invite.requester).CharacterName + "'s party invite.");
                invites.Remove(invite);
            }
        }

        public static void removeInvite(UnturnedPlayer player)
        {
            var invite = getInvite(player);
            if (invite != null)
                invites.Remove(invite);
        }

        public static Party createParty(UnturnedPlayer leader)
        {
            var newParty = new Party(leader);
            parties.Add(newParty);
            return newParty;
        }

        public static Party getParty(UnturnedPlayer player)
        {
            foreach (var party in parties)
                if (party.isMember(player))
                    return party;
            return null;
        }

        public static void disbandParty(Party party)
        {
            var members = party.getMembers();
            party.tellParty("Party has been disbanded!");
            party.disband();
            parties.Remove(party);

            //remove all related invites
            var toRemove = new List<Invite>();
            foreach (var invite in invites)
                foreach (var member in members)
                    if (invite.requester.Equals(member))
                        toRemove.Add(invite);
            foreach (var invite in toRemove)
                invites.Remove(invite);
        }
    }
}