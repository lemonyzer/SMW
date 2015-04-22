using UnityEngine;
using System.Collections;

public class TeamLibrary : MonoBehaviour {

	public static int TeamIdTeamFull = -2;
	public static int TeamIdNoTeam = -1;

	public static int mNumberOfTeams = 4;
	public static int mMaxNumberOfTeamMember = 4;
	Team[] teams = new Team[mNumberOfTeams];


	void Awake()
	{
		for(int i=0; i< mNumberOfTeams; i++)
		{
			teams[i] = new Team(i, 0, TeamColor.referenceColorsVerzweigt[i]);
		}
	}

	public int NextTeam(Player player)
	{
		if (player != null)
		{
			if (player.team != null)
			{
				int nextTeamId = 0;
				int tryCount = 0;
				bool teamFound = false;
				while(!teamFound && tryCount < mNumberOfTeams)
				{
					nextTeamId = (player.team.mId + tryCount+1) % mNumberOfTeams;
					if (teams[nextTeamId].mMemberCount < mMaxNumberOfTeamMember)
					{
						teamFound = true;
					}

					tryCount++;
				}

				if (teamFound)
				{
					player.team.RemoveMember(player);
					player.team = GetTeam(nextTeamId);
					player.teamPos = player.team.AddMember(player);
					Debug.Log("Neues Team gefunden " + player.team.mId);
					return player.team.mId;
				}
				else
				{
					Debug.Log("Neues Team gefunden, bleibe im alten " + player.team.mId);
					return player.team.mId;
				}
			}
		}
		return TeamLibrary.TeamIdNoTeam;
	}

	public Team GetTeam(int teamId)
	{
		if (teams == null)
		{
			Debug.LogError("Team[] Array teams == null, not initialized during Awake()?!");
			return null;
		}
		if (teamId >=0 && teamId < mNumberOfTeams)
		{
			return teams[teamId];
		}
		else
		{
			Debug.LogError("teamId "+ teamId +" out of Bounds!!!");
			return null;
		}
	}

	public void RemovePlayer(Player player)
	{
		int teamId = player.team.mId;
		if (teamId >= 0)
		{
			if (teams[teamId].mMemberCount > 0)
			{
				teams[teamId].mMemberCount--;
				player.team = null;
			}
		}
	}

	public Team GetNewPlayerTeam()
	{
		int teamId = GetTeamIdWithLowestMemberCount();
		if (teamId >= 0)
		{
			if (teams[teamId].mMemberCount < mMaxNumberOfTeamMember)
			{
				return teams[teamId];
			}
			else
			{
				Debug.LogError ("Fehler, Team " + teamId + " ist voll: " + teams[teamId].mMemberCount + "/" + mMaxNumberOfTeamMember);
				return null;
			}
		}
		return null;
	}

	public int AddNewPlayer(Player player)
	{
		int teamId = GetTeamIdWithLowestMemberCount();
		if (teamId >= 0)
		{
			if (teams[teamId].mMemberCount < mMaxNumberOfTeamMember)
			{
				teams[teamId].mMemberCount++;
				player.team = teams[teamId];

				return teamId;
			}
			return TeamIdTeamFull;
		}
		return teamId;
	}

	public int GetTeamIdWithLowestMemberCount()
	{
		int teamIdOfFirstTeamWithLowestMemberCount = TeamIdNoTeam;
		int currentMemberCountMin = int.MaxValue;
		for(int i=0; i< mNumberOfTeams; i++)
		{
			Debug.Log("Team " + i + " Member Anzahl: " + teams[i].mMemberCount); 
			if (teams[i].mMemberCount < currentMemberCountMin)
			{
				currentMemberCountMin = teams[i].mMemberCount;
				teamIdOfFirstTeamWithLowestMemberCount = i;
			}
		}

		if (teamIdOfFirstTeamWithLowestMemberCount == -1)
		{
			Debug.LogError ("Fehler: kein Team hat mMemberCount < " + currentMemberCountMin + "!!!");
		}
		return teamIdOfFirstTeamWithLowestMemberCount;
	}

}
