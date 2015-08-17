namespace DingusGaming
{
	public class Invite
		{
			public Party party;
			public CSteamID requester, playerRequested;

			public Invite(UnturnedPlayer requester, Party party, UnturnedPlayer playerRequested)
			{
				this.requester = requester.CSteamID;
				this.party = party;
				this.playerRequested = playerRequested.CSteamID;
			}
		}
}