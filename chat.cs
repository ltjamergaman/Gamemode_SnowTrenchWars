//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//|----------------------------------------------------------------|\\
//|   /-------------/ |------------------| |---|    |---|    |---| |\\
//|  /             /  |                  | |   |    |   |    |   | |\\
//| |   |---------/   |------|    |------| |   |    |   |    |   | |\\
//|  \   \                   |    |        |   |    |   |    |   | |\\
//|    \   \                 |    |        |   |    |   |    |   | |\\
//|      \   \               |    |        |   |    |   |    |   | |\\
//|        \   \             |    |        |   |    |   |    |   | |\\
//| 	     \   \           |    |        |   |    |   |    |   | |\\
//| 		   \   \         |    |         \   \   |   |   /   /  |\\
//|   /---------|  |         |    |          \   |--|   |--|   /   |\\
//|  /            /          |    |           \               /    |\\
//| /------------/           |----|            \-------------/     |\\
//|----------------------------------------------------------------|\\
//|                           Version: 1                           |\\
//|             Main Functionality for the Chat System             |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//VARS
$STW::Gamemode::ChatFilter = 1; //is on by default

package STW_Chat
{
    function serverCmdMessageSent(%client,%message)
    {
        if(%client.isSTWOS)
        {
            messageAll('',%client.startMessage @ %message);
        }
        else
        {
            if(%client.isMuted)
            {
                messageClient(%client,'',STWOS.startMessage@"You are muted, you have "@%client.muteTimeRemaining@" second(s) left before you are unmuted.");
            }
            else
            {
				 //so we can set it back after the client has sent their message
                %oldPrefix = %client.clanPrefix;

				if(%client.isAdmin || %client.isModerator)
				{
			//		if(%client.STW_isInClan && isObject(%client.STW_Clan)) //checks to see if the client is in a clan or not
			//		{
			//			%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6["@%client.STW_Clan.clanColor@%client.STW_Clan.name@" \c0D"@%client.STW_Clan.division@"\c6] \c6[\c4"@%client.STW_Level@"\c6] ";
			//		}
			//		else
			//		{
			//			%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6[\c4"@%client.STW_Level@"\c6] ";
			//		}
			//
					%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority> ";
				}
			//	else if(%client.STW_isInClan && isObject(%client.STW_Clan)) //checks to see if the client is in a clan or not 
			//	{
			//		%client.clanPrefix = "\c6["@%client.STW_Clan.clanColor@%client.STW_Clan.name@" \c0D"@%client.STW_Clan.division@"\c6] \c6[\c4"@%client.STW_Level@"\c6] ";
			//	}
            //    else
            //    {
            //        %client.clanPrefix = "\c6[\c4"@%client.STW_Level@"\c6] "; //since the client isn't in a clan, we're just gonna set the level instead.
            //    }
			//	
                parent::serverCmdMessageSent(%client,%message);   
				//now the client doesn't need to worry about anything :)
                %client.clanPrefix = %oldPrefix; 
			}
        }
    }
	
	function serverCmdTeamMessageSent(%client,%message)
	{
		//if the client is muted, they cannot talk
		if(%client.isMuted)
        {
            messageClient(%client,'',STWOS.startMessage@"You are muted, you have "@%client.muteTimeRemaining@" second(s) left before you are unmuted.");
        }
		//if not then we just continue on with the procedure
        else
        {
			 //so we can set it back after the client has sent their message
            %oldPrefix = %client.clanPrefix;
			%oldSuffix = %client.clanSuffix;
			
			//checks to see if the client is in a clan or not
			if(%client.STW_isInClan && isObject(%client.STW_Clan))
			{
				//checks to see if the client is admin or moderator
				if(%client.isAdmin || %client.isModerator)
				{
					//checks to see if the client is a clan admin but not the clan leader
					if(%client.STW_isClanAdmin && %client.STW_Clan.LeaderBLID != %client.bl_id)
					{
						%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6["@%client.STW_Clan.clanColor@%client.STW_Clan.name@" \c0A\c6] ";
					}
					else if(%client.STW_Clan.LeaderBLID != %client.bl_id)
					{
						%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6["@%client.STW_Clan.clanColor@%client.STW_Clan.name@" \c4M\c6] ";
					}
					else if(%client.STW_Clan.LeaderBLID == %client.bl_id)
					{
						%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6["@%client.STW_Clan.clanCOlor@%client.STW_Clan.name@" \c3L\c6] ";
					}
				}
				else
				{
					if(%client.STW_isClanAdmin && %client.STW_Clan.clanLeaderBLID != %client.bl_id)
					{
						%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6["@%client.STW_Clan.clanColor@%client.STW_Clan.name@" \c0A\c6] ";
					}
					else if(%client.STW_Clan.LeaderBLID != %client.bl_id)
					{
						%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6["@%client.STW_Clan.clanColor@%client.STW_Clan.name@" \c4M\c6] ";
					}
					else if(%client.STW_Clan.LeaderBLID == %client.bl_id)
					{
						%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority>\c6["@%client.STW_Clan.clanCOlor@%client.STW_Clan.name@" \c3L\c6] ";
					}
				}
			}
			else
			{
				if(%client.isAdmin || %client.isModerator)
				{
					%client.clanPrefix = "<bitmap:Add-ons/Gamemode_SnowTrenchWars/UI/Images/serverAuthority> ";
				}
				else
				{
					%client.clanPrefix = %oldPrefix;
				}
			}
			
			//sets the clients clan suffix to their level to
			//notify the clan/team members what the clients level is
			%client.clanSuffix = "\c6[\c4"@%client.STW_Level@"\c6]";
			
			//if the client is in a clan -
			if(%client.STW_isInClan && isObject(%client.STW_Clan))
			{
				for(%i = 0; %i < ClientGroup.getCount();%i++)
				{
					%clients = ClientGroup.getObject(%i);
					
					//- we check to see if others are in the same clan
					if(%clients.STW_isInClan && isObject(%clients.STW_Clan) && %clients.STW_Clan == %client.STW_Clan)
					{
						messageClient(%clients,'',%client.clanPrefix@"\c3"@%client.name@"\c7"@%client.clanSuffix@"\c6: "@%message);
					}
				}
			}
			//if not then we just parent the function
			else
			{
				parent::serverCmdTeamMessageSent(%client,%message);
			}

			 //now the client doesn't need to worry about anything :)
            %client.clanPrefix = %oldPrefix;
			%client.clanSuffix = %oldSuffix;
        }
	}
};
activatePackage(STW_Chat);