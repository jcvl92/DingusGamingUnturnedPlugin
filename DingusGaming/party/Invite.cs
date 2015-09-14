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
            var party = Parties.getParty(DGPlugin.getPlayer(requester));

            if (party == null)
                Parties.createParty(DGPlugin.getPlayer(requester), DGPlugin.getPlayer(playerRequested));
            else
                party.addMember(DGPlugin.getPlayer(playerRequested));
        }
    }
}