using UnityEngine;
using System.Collections;

public class Team {

	public static int ErrorNoFreePosition = -1;
	public static int NoPosition = -10;

	public int mId = 0;
	public int mMemberCount = 0;
	public Color32[] mColors;

	Player[] members;

	public Team(int id, int memberCount, Color32[] color)
	{
		this.mId = id;
		this.mMemberCount = memberCount;
		this.mColors = color;

		members = new Player[TeamLibrary.mMaxNumberOfTeamMember];
	}

	public Player[] GetAllMember()
	{
		return members;
	}

	public Player GetMember(int index)
	{
		return null;
	}

	public int GetFirstFreePosition()
	{
		for(int i=0; i< TeamLibrary.mMaxNumberOfTeamMember; i++)
		{
			if (members[i] == null)
			{
				return i;
			}
		}
		// kein freier Playerslot in members gefunden
		Debug.LogError ("kein freier Playerslot in members gefunden");
		return ErrorNoFreePosition;
	}

	public int AddMember(Player player)
	{
//		int position = -1;
		for(int i=0; i< TeamLibrary.mMaxNumberOfTeamMember; i++)
		{
			if (members[i] == null)
			{
				mMemberCount++;			// increment Member Count
				members[i] = player;	// add Player to Team
				player.team = this;		// add Team to Player
				player.teamPos = i;		// add Team Pos to Player
				return i;
			}
		}
		// kein freier Playerslot in members gefunden
		Debug.LogError ("// kein freier Playerslot in members gefunden");
		return ErrorNoFreePosition;
	}

	public void RemoveMember(Player player)
	{
		for(int i=0; i< TeamLibrary.mMaxNumberOfTeamMember; i++)
		{
			if (members[i].Equals(player))
			{
				mMemberCount--;
				members[i] = null;
				player.team = null;
				player.teamPos = NoPosition;
				return;
			}
		}
		// hier kommt man nur an wenn spieler nicht entfert wurde
		Debug.LogError("Spieler wurde nicht aus Team " + mId + " entfernt!");
	}

}
