using Rocket.RocketAPI;
using Steamworks;

namespace DingusGaming.Party
{
    public class Invite
    {
        public Party party;
        public CSteamID requester, playerRequested;

        public Invite(RocketPlayer requester, Party party, RocketPlayer playerRequested)
        {
            this.requester = requester.CSteamID;
            this.party = party;
            this.playerRequested = playerRequested.CSteamID;
        }
    }
}