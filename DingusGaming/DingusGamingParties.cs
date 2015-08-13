using System;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace DingusGaming
{
	public class Parties
	{
		static List<Party> parties = new List<Party>();
		static List<Invite> invites = new List<Invite>();

		static Parties()
		{
			//notify party of death
			UnturnedPlayerEvents.OnPlayerDeath +=
				delegate (UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
				{
					Party party = Parties.getParty(player);
					if (party != null)
						party.tellParty(player.CharacterName + " has died!");
				};

			//TODO: implement this!
			/*
			//remove them from their party
			Party party = Parties.getParty(this.Player);
			if (party != null)
			{
				party.removeMember(this.Player);
				party.tellParty(this.Player.CharacterName + " has disconnected!");
			}

			//clear pending invites
			Parties.removeInvite(this.Player);
			*/
		}

		public class Invite
		{
			public Party party;
			public UnturnedPlayer requester, playerRequested;

			public Invite(UnturnedPlayer requester, Party party, UnturnedPlayer playerRequested)
			{
				this.requester = requester;
				this.party = party;
				this.playerRequested = playerRequested;
			}
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
				if (invite.playerRequested.Equals(player))
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
				invite.party.addMember(invite.playerRequested);
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
				DGPlugin.messagePlayer(invite.requester, caller.CharacterName + " declined your invite.");
				DGPlugin.messagePlayer(caller, "You have declined "+invite.requester.CharacterName+"'s party invite.");
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
			{
				if (party.isMember(player))
					return party;
			}
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

	public class Party
	{
		private UnturnedPlayer leader;
		private List<UnturnedPlayer> members;

		public Party(UnturnedPlayer leader)
		{
			this.leader = leader;
			members = new List<UnturnedPlayer>();
			addMember(leader);
		}

		public void disband()
		{
			members.Clear();
		}

		public bool isMember(UnturnedPlayer player)
		{
			//return members.Contains(player);
			foreach (UnturnedPlayer member in members)
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
			string info = "";

			foreach (UnturnedPlayer member in members)
				info += member.CharacterName + (isLeader(member) ? "[L](" : "(") + (member.Dead ? "dead" : member.Health + "/100") + "), ";

			return info.Substring(0, info.Length - 2);
		}

		public UnturnedPlayer getLeader()
		{
			return leader;
		}

		public void tellParty(string text)
		{
			foreach (UnturnedPlayer member in members)
				DGPlugin.messagePlayer(member, text);
		}

		public void chat(UnturnedPlayer caller, string text)
		{
			if (isMember(caller))
				tellParty(caller.CharacterName + (isLeader(caller) ? "[L]: " : "[P]: ") + text);
			else
				DGPlugin.messagePlayer(caller, "Error, you are not in this party.");
		}

		//done through invites
		public void addMember(UnturnedPlayer player)
		{
			members.Add(player);
			tellParty(player.CharacterName + " has joined the party!");
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
			members.RemoveAt(members.FindIndex(0, x => x.Equals(player)));

			//promote a new leader if the leader was removed
			if (leader.Equals(player))
			{
				if (members.Count > 1)
				{
					leader = members.First();
					tellParty(leader.CharacterName + " has been made party leader!");
				}
				else
				{
					Parties.disbandParty(this);
				}
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
					DGPlugin.messagePlayer(caller, "Only the party leader("+leader.CharacterName+") switch leaders.");
				}
			}
			else
				DGPlugin.messagePlayer(caller, "Could not find " + player.CharacterName + " in your party.");
		}
	}

	/********** COMMANDS **********/

	public class CommandInvite : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "invite"; }
		}

		public string Help
		{
			get { return "Invite a player to your party."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "inv", "pinv", "pinvite" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			//check for parameter vaidity
			if (command.Length == 0)
			{
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/invite PlayerName\".");
				return;
			}

			string playerName = String.Join(" ", command);

			//check for player existence
			UnturnedPlayer player = DGPlugin.getPlayer(playerName);
			if (player == null)
			{
				DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
				return;
			}

			//if caller is in a party, invite. otherwise create first
			Party party = Parties.getParty(caller);
			if (party != null)
			{
				if (party.isLeader(caller))
					Parties.invitePlayer(caller, player);
				else
					DGPlugin.messagePlayer(caller, "Only the party leader(" + party.getLeader().CharacterName + ") can invite members.");
			}
			else
			{
				Parties.createParty(caller);
				Parties.invitePlayer(caller, player);
			}
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandKick : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "kick"; }
		}

		public string Help
		{
			get { return "Kick a player from your party."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pkick", "remove" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			//check for parameter vaidity
			if (command.Length == 0)
			{
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/kick PlayerName\".");
				return;
			}

			string playerName = String.Join(" ", command);

			//check for player existence
			UnturnedPlayer player = DGPlugin.getPlayer(playerName);
			if (player == null)
			{
				DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
				return;
			}

			Party party = Parties.getParty(caller);
			if (party != null)
				party.kickMember(caller, player);
			else
				DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandChat : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "p"; }
		}

		public string Help
		{
			get { return "Send a message to your party."; }
		}

		public string Syntax
		{
			get { return "<message>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "party", "pchat", "partychat" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			//check for parameter vaidity
			if (command.Length == 0)
			{
				DGPlugin.messagePlayer(caller, "No message entered. Format is \"/p message\".");
				return;
			}

			string message = String.Join(" ", command);

			Party party = Parties.getParty(caller);
			if (party != null)
				party.chat(caller, message);
			else
				DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandLeave : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "leave"; }
		}

		public string Help
		{
			get { return "Leave your current party."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pleave", "leaveparty", "quit", "pquit" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			Party party = Parties.getParty(caller);

			if (party != null)
			{
				party.removeMember(caller);
				party.tellParty(caller.CharacterName + " has left the party.");
				DGPlugin.messagePlayer(caller, "You have left the party.");
			}
			else
				DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandAccept : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "accept"; }
		}

		public string Help
		{
			get { return "Accept a party invitation."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "paccept", "invaccept" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			Parties.acceptInvite(caller);
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandDecline : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "decline"; }
		}

		public string Help
		{
			get { return "Decline a party invitation."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pdecline", "invdecline" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			Parties.declineInvite(caller);
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandLeader : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "leader"; }
		}

		public string Help
		{
			get { return "Transfer party leader to another member."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pleader" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			//check for parameter vaidity
			if (command.Length == 0)
			{
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/leader PlayerName\".");
				return;
			}

			string playerName = String.Join(" ", command);

			//check for player existence
			UnturnedPlayer player = DGPlugin.getPlayer(playerName);
			if (player == null)
			{
				DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
				return;
			}

			Party party = Parties.getParty(caller);
			if (party != null)
				party.makeLeader(caller, player);
			else
				DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandDisband : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "disband"; }
		}

		public string Help
		{
			get { return "Disband your party."; }
		}

		public string Syntax
		{
			get { return ""; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "disbandparty" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			Party party = Parties.getParty(caller);
			if (party != null)
			{
				if (party.isLeader(caller))
				{
					party.tellParty("Your party has been disbanded!");
					party.disband();
				}
				else
				{
					DGPlugin.messagePlayer(caller, "Only the party leader("+party.getLeader().CharacterName+") can disband that party.");
				}
			}
			else
				DGPlugin.messagePlayer(caller, "You are not in a party.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandTeleport : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "teleport"; }
		}

		public string Help
		{
			get { return "Teleport to a party member."; }
		}

		public string Syntax
		{
			get { return "<player>"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "tp", "pteleport", "ptp" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			//check for parameter vaidity
			if (command.Length == 0)
			{
				DGPlugin.messagePlayer(caller, "Invalid amount of players. Format is \"/tp PlayerName\".");
				return;
			}

			string playerName = String.Join(" ", command);

			//check for player existence
			UnturnedPlayer player = DGPlugin.getPlayer(playerName);
			if (player == null)
			{
				DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
				return;
			}

			Party party = Parties.getParty(caller);
			if (party != null)
			{
				if (party.isMember(player))
				{
					caller.Teleport(player);
				}
				else
					DGPlugin.messagePlayer(caller, player.CharacterName + " is not in your party. You can only teleport to party members.");
			}
			else
				DGPlugin.messagePlayer(caller, "You are not in a party. You can only teleport to party members.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}

	public class CommandInfo : IRocketCommand
	{
		public bool RunFromConsole
		{
			get { return false; }
		}

		public string Name
		{
			get { return "info"; }
		}

		public string Help
		{
			get { return "Get info on your party or a party member."; }
		}

		public string Syntax
		{
			get { return "(<player>)"; }
		}

		public List<string> Aliases
		{
			get { return new List<string> { "pinfo", "partyinfo", "inf", "pinf" }; }
		}

		public bool AllowFromConsole
		{
			get { return false; }
		}

		public List<string> Permissions
		{
			get { return new List<string>(); }
		}

		public void Execute(UnturnedPlayer caller, string[] command)
		{
			string playerName = String.Join(" ", command);

			Party party = Parties.getParty(caller);
			if (party != null)
			{
				//get info on the whole party
				if (command.Length == 0)
				{
					string info = party.getInfo();
					DGPlugin.messagePlayer(caller, info);
				}

				//get info on a member
				else
				{
					//check for player existence
					UnturnedPlayer player = DGPlugin.getPlayer(playerName);
					if (player == null)
					{
						DGPlugin.messagePlayer(caller, "Failed to find player named \"" + playerName + "\"");
						return;
					}

					if (party.isMember(player))
					{
						string info = "Name: " + player.CharacterName + ", " +
									(player.Dead ? "Player is dead." :
									"Health: " + player.Health + ", " +
									"Hunger: " + player.Hunger + ", " +
									"Thirst: " + player.Thirst + ", " +
									"Infection: " + player.Infection);

						DGPlugin.messagePlayer(caller, info);
					}
					else
						DGPlugin.messagePlayer(caller, player.CharacterName + " is not in your party. You can only get info on party members.");
				}
			}
			else
				DGPlugin.messagePlayer(caller, "You are not in a party. You can only get info on party members.");
		}

		public void Execute(IRocketPlayer caller, string[] command)
		{
			Execute((UnturnedPlayer)caller, command);
		}
	}
}