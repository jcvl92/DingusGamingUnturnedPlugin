using System.Collections.Generic;
using Rocket.Unturned;
using Steamworks;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace DingusGaming.Party
{
    public class Parties
    {
        static readonly List<Party> parties = new List<Party>();
        static readonly List<Invite> invites = new List<Invite>();
        static readonly HashSet<CSteamID> chatToggles = new HashSet<CSteamID>();

        public static void init()
        {
            registerOnPlayerDeath();
            registerOnPlayerDisconnected();
        }

        private static void registerOnPlayerDeath()
        {
            //notify party of death
            UnturnedPlayerEvents.OnPlayerDeath +=
                delegate (UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
                {
                    getParty(player)?.tellParty(player.CharacterName + " has died!");
                };
        }

        private static void registerOnPlayerDisconnected()
        {
            U.Events.OnPlayerDisconnected += delegate (UnturnedPlayer player)
            {
                //remove them from their party
                Party party = getParty(player);
                party?.removeMember(player);
                party?.tellParty(player.CharacterName + " has disconnected!");

                //clear pending invites
                removeInvite(player);
            };
        }

        public static void toggleChat(UnturnedPlayer player)
        {
            if(!chatToggles.Remove(player.CSteamID))
                chatToggles.Add(player.CSteamID);
        }

        public static void toggleChat(UnturnedPlayer player, bool enabled)
        {
            if(enabled)
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
                DGPlugin.messagePlayer(caller, player.CharacterName + " is already in a party. They must /leave it before you can invite them.");
                return;
            }
            if (getInvite(player) != null)
            {
                DGPlugin.messagePlayer(caller, player.CharacterName + " already has an invite pending. They must /decline it before you can invite them.");
                return;
            }

            invites.Add(new Invite(caller, getParty(caller), player));
            DGPlugin.messagePlayer(player, caller.CharacterName + " is inviting you to join a party. Enter /accept or /decline to respond.");
            DGPlugin.messagePlayer(caller, "Invite sent to " + player.CharacterName + ".");
        }

        private static Invite getInvite(UnturnedPlayer player)
        {
            foreach (Invite invite in invites)
                if (invite.playerRequested.Equals(player.CSteamID))
                    return invite;
            return null;
        }

        public static void acceptInvite(UnturnedPlayer caller)
        {
            Invite invite = getInvite(caller);
            if (invite == null)
                DGPlugin.messagePlayer(caller, "You have no pending invites!");
            else
            {
                invite.party.addMember(DGPlugin.getPlayer(invite.playerRequested));
                invites.Remove(invite);
            }
        }

        public static void declineInvite(UnturnedPlayer caller)
        {
            Invite invite = getInvite(caller);
            if (invite == null)
                DGPlugin.messagePlayer(caller, "You have no pending invites!");
            else
            {
                DGPlugin.messagePlayer(DGPlugin.getPlayer(invite.requester), caller.CharacterName + " declined your invite.");
                DGPlugin.messagePlayer(caller, "You have declined " + DGPlugin.getPlayer(invite.requester).CharacterName + "'s party invite.");
                invites.Remove(invite);
            }
        }

        public static void removeInvite(UnturnedPlayer player)
        {
            Invite invite = getInvite(player);
            if (invite != null)
                invites.Remove(invite);
        }

        public static void createParty(UnturnedPlayer leader)
        {
            Party newParty = new Party(leader);
            parties.Add(newParty);
        }

        public static Party getParty(UnturnedPlayer player)
        {
            foreach (Party party in parties)
                if (party.isMember(player))
                    return party;
            return null;
        }

        public static void disbandParty(Party party)
        {
            party.tellParty("Party has been disbanded!");
            party.disband();
            parties.Remove(party);

            //remove all related invites
            List<Invite> toRemove = new List<Invite>();
            foreach (Invite invite in invites)
                if (invite.party.Equals(party))
                    toRemove.Add(invite);
            foreach (Invite invite in toRemove)
                invites.Remove(invite);
        }
    }
}