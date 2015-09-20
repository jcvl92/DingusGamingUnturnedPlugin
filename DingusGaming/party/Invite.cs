using Rocket.Unturned.Player;
using Steamworks;

namespace DingusGaming.Party
{
    public class Invite
    {
        public CSteamID requester, playerRequested;

        public Invite(UnturnedPlayer requester, UnturnedPlayer playerRequested)
        {
            this.requester = requester.CSteamID;
            this.playerRequested = playerRequested.CSteamID;
        }

        public void join()
        {
            var party = Parties.getParty(DGPlugin.getPlayer(requester)) ?? Parties.createParty(DGPlugin.getPlayer(requester));
            party.addMember(DGPlugin.getPlayer(playerRequested));
        }
    }
}