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
//|             Main Functionality for the Clan System             |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//VERSION
$STW::Gamemode::Clans::Version = "1";
warn("STW Clans System Version: "@$STW::Gamemode::Clans::Version);

//VARS
$STW::Gamemode::Clans::FilePath = "config/server/Snow Trench Wars/Clans/";
$STW::Gamemode::Clans::Extension = ".txt";
$STW::Gamemode::Clans::RequiredLevel = "7";

//===============================================================|
//                                                               |
//               STW CLAN MANAGER      |     By Lt. Jamergaman   |
//                   {BEGIN}           |       BLID: 23108       |
//                                                               |
//===============================================================|

if(!isObject(STW_ClanManager))
{
	new ScriptGroup("STW_ClanManager");
}

function STW_ClanManager::loadClans(%this)
{
	for(%i = 0; %i < getFileCount($STW::Gamemode::Clans::FilePath@"*.txt"); %i++)
	{
		%clanFile = findNextFile($STW::Gamemode::Clans::FilePath@"*.txt");
		
		if(%clanFile $= "")
		{
			return;
		}
		
		%file = new FileObject();
		%file.openForRead(%clanFile);
		%name = getWord(%file.readLine(),1);
		%division = getWord(%file.readLine(),1);
		%leader = getSubStr(%file.readLine(),strLen(%file.readLine())-6,strLen(%file.readLine()));
		%leaderBLID = getWord(%file.readLine(),1);
		%members = getSubStr(%file.readLine(),strLen(%file.readLine())-8,strLen(%file.readLine()));
		%color = getWord(%file.readLine(),2);
		%memberCount = getWord(%file.readLine(),1);
		%team = getWord(%file.readLine(),1);
		%ff = getWord(%file.readLine(),1);
		%bank = getWord(%file.readLine(),1);
		%upgrades = getWord(%file.readLine(),1);
		%admins = getWord(%file.readLine(),1);
		%cas = getWord(%file.readLine(),1);
		%file.close();
		%file.delete();
		
		%clanName = "Clan_"@%name;
		
		if(isObject(%clanName.getID()))
		{
			return;
		}
		
		new ScriptObject(%clanname)
		{
			name = %name;
			division = %division;
			leader = %leader;
			leaderblid = %leaderblid;
			members = %members;
			color = %color;
			membercount = %membercount;
			isteam = %team;
			friendlyfire = %ff;
			hasbank = %bank;
			hasupgrades = %upgrades;
			hasadmins = %admins;
			allowscas = %cas;
		};
		
		STW_ClanManager.add(%clanname.getID());
	}
}

function STW_ClanManager::saveClans(%this)
{
}

function STW_ClanManager::CheckForClans(%this)
{
}

function STW_ClanManager::clear(%this)
{
	for(%i = 0; %i < %this.getCount(); %i++)
	{
		if(isObject(%this.getObject(%i)))
		{
			%this.getObject(%i).delete();
			fileDelete($STW::Gamemode::Clans::FilePath @ %this.getObject(%i).getName() @ $STW::Gamemode::Clans::Extension);
		}
		else
		{
			return;
		}
	}
}
//===============================================================|
//                                                               |
//               STW CLAN MANAGER      |     By Lt. Jamergaman   |
//                    {END}            |       BLID: 23108       |
//                                                               |
//===============================================================|

//===============================================================|
//                                                               |
//                         CLAN CREATION                         |
//                            {BEGIN}                            |
//                                                               |
//===============================================================|

//CREATE CLAN - client, name, division, color
function serverCmdSTW_Clan_Create(%client,%clanName,%clanDivision,%clanColor,%options)
{
	if(!%client.STW_canCreateClan)
	{
		commandToClient(%client,'STW_OKDlgNotify',"Profile Check","<just:center>You cannot create a clan because you do not have the privilege to do so, in which was most likely removed by an admin.<br>If you think this was a misunderstanding then consult an admin.");
		return;
	}
	if(%client.STW_Level < $STW::Gamemode::Clans::RequiredLevel && %client.STW_Rank == 0)
	{
		commandToClient(%client,'STW_OKDlgNotify',"Clan Creation Error","<just:center>You cannot create a clan because your Career Statistics do not meet the required specifications to create a clan. You must be at least level 7 or above, unless rank is above 0.");
		return;
	}
	for(%i = 0; %i < STW_ClanManager.getCount(); %i++)
	{
		%obj = STW_ClanManager.getObject(%i);
		
		if(%clanName $= %obj.name) //stops creation
		{
			//return messageClient(%client,'',STWOS.startMessage@"There's already a clan named "@%clanName@".");
			commandToClient(%client,'STW_CCFailure',"Clan Validation Error","<color:FF0000>ERR A_05<br>Clan Name Validation Error<br><br><color:000000><just:center>There's already another clan named \""@%clanName@"\".<br>Would you like to retry?");
			return;
		}
	}
	if(strLen(%clanName) > 10 || getWordCount(%clanName) > 1) //stops creation
	{
		//return messageClient(%client,'',STWOS.startMessage@"You cannot use more than 10 characters for your clan name. (this is to prevent spam and dumb names)");
		commandToClient(%client,'STW_CCFailure',"Clan Validation Error","<color:FF0000>ERR A_06<br>Clan Name Validation Error<br><br><color:000000><just:center>You must enter a name with less than 11 characters and no spaces.<br>Would you like to retry?");
		return;
	}
	if(%clanName $= "")
	{
		//return messageClient(%client,'',STWOS.startMessage@"You must enter a name for your clan.");
		commandToClient(%client,'STW_CCFailure',"Clan Validation Error","<color:FF0000>ERR A_06<br>Clan Name Validation Error<br><br><color:000000><just:center>You must enter a name with less than 11 characters and no spaces.<br>Would you like to retry?");
		return;
	}
	if(%clanDivision < 2 || %clanDivision > 99) //stops creation
	{
		if(%client.bl_id != getNumKeyID())
		{
			//return messageClient(%client,'',STWOS.startMessage@"You must choose a division between 1 and 100. (meaning you can only choose 2-99)");
			commandToClient(%client,'STW_CCFailure',"Clan Validation Error","<color:FF0000>ERR A_10<br>Clan Division Validation Error<br><br><color:000000><just:center>You must enter a Division of 2-99.<br>Would you like to retry?");
			return;
		}
	}
	if(%clanDivision $= "")
	{
		//return messageClient(%client,'',STWOS.startMessage@"You must enter a division between\c0 1 \c6and\c0 99\c6.");
		commandToClient(%client,'STW_CCFailure',"Clan Validation Error","<color:FF0000>ERR A_10<br>Clan Division Validation Error<br><br><color:000000><just:center>You must enter a Division of 2-99.<br>Would you like to retry?");
		return;
	}
	if(%clanColor < 0 || %clanColor > 8) //stops creation
	{
		//return messageClient(%client,'',STWOS.startMessage@"You must choose a color of 0-8.");
		commandToClient(%client,'STW_CCFailure',"Clan Validation Error","<color:FF0000>ERR A_12<br>Clan Color Validation Error<br><br><color:000000><just:center>You must enter a Color of 0-8.<br>Would you like to retry?");
		return;
	}
	if(%clanColor $= "")
	{
		//return messageClient(%client,'',STWOS.startMessage@"You must enter a clan color between 0 and 8.");
		commandToClient(%client,'STW_CCFailure',"Clan Validation Error","<color:FF0000>ERR A_06<br>Clan Color Validation Error<br><br><color:000000><just:center>You must enter a Color of 0-8.<br>Would you like to retry?");
		return;
	}
	if(%options !$= "")
	{
		%CIAT = getWord(%options,0);
		%FF = getWord(%options,1);
		%Bank = getWord(%options,2);
		%Upgrades = getWord(%options,3);
		%Admins = getWord(%options,4);
		%CAS = getWord(%options,5);
	}
	else
	{
		%CIAT = "1";     //default is 1
		%FF = "0";       //default is 0
		%Bank = "0";     //default is 0
		%Upgrades = "0"; //default is 0
		%Admins = "1";   //default is 1
		%CAS = "0";      //default is 0
	}
	
	//Checks clan color and sets the clans color
	%clanColor = STW_Clan_CheckColor(%clanColor);
	%clanName = trim(%clanName);
	
	//using a script object for the clan so it's easier to manage and track
    %newClan = new ScriptObject("Clan_"@%clanName)
    {
        name = %clanName;
        division = %clanDivision;
        Leader = %client.name;
		LeaderBLID = %client.bl_id;
        Members = %client.bl_id;
        clanColor = %clanColor;
		memberCount = "1";
		isTeam = %CIAT;
		FriendlyFire = %FF;
		hasBank = %Bank;
		hasUpgrades = %Upgrades;
		hasAdmins = %Admins;
		allowsCAS = %CAS;
    };
	
	%file = new FileObject();
	%file.openForWrite($STW::Gamemode::Clans::FilePath @ %newClan.getName() @ $STW::Gamemode::Clans::Extension);
	%file.writeLine("Name "@%newClan.name);
	%file.writeLine("Division "@%newClan.division);
	%file.writeLine("Leader "@%newClan.Leader);
	%file.writeLine("LeaderBLID "@%newClan.LeaderBLID);
	%file.writeLine("Members "@%newClan.Members);
	%file.writeLine("Color "@%newClan.clanColor);
	%file.writeLine("MemberCount "@%newClan.memberCount);
	%file.writeLine("isTeam "@%newClan.isTeam);
	%file.writeLine("FriendlyFire "@%newClan.FriendlyFire);
	%file.writeLine("hasBank "@%newClan.hasBank);
	%file.writeLine("hasUpgrades "@%newClan.hasUpgrades);
	%file.writeLine("hasAdmins "@%newClan.hasAdmins);
	%file.writeLine("allowsCAS "@%newClan.allowsCAS);
	%file.close();
	%file.delete();
	
	//basic clan stuff
	STW_ClanManager.add(%newClan);
	%client.STW_isInClan = 1;
	%client.STW_isClanAdmin = 1;
	%client.STW_Clan = %newClan; //so we can keep track of which clan this person is in
	//messageClient(%client,'',STWOS.startMessage@"You have created a new clan named \c2"@%clanName@" \c6in \c0Division "@%clanDivision@" \c6and your "@%clanColor@"Clan Color \c6too, you are the \c2Clan \c2Leader\c3.");
	commandToClient(%client,'STW_CCSuccess',"Clan Creation Sucess","<br><br><just:center><color:000000>You have created a new clan named <color:EEEE00>"@%clanName@" <color:000000>in <color:EE0000>Division "@%clanDivision@"<color:000000>, you are the <color:00EE00>Clan Leader<color:000000>.");
	messageAll('',STWOS.startMessage@"\c3"@%client.name@" \c6has created a new clan called \c2"@%newClan.name@" \c6in \c0Division "@%newClan.division@"\c6!");
}

//===============================================================|
//                                                               |
//                         CLAN CREATION                         |
//                             {END}                             |
//                                                               |
//===============================================================|

//===============================================================|
//                                                               |
//                        Server Commands                        |
//                            {BEGIN}                            |
//                                                               |
//===============================================================|

function serverCmdSTW_Clan_Leave(%client)
{
	STW_Clan_onLeave(%client.STW_Clan,%client);
}

function serverCmdSTW_Clan_Disband(%client)
{
	if(%client.STW_isInClan == true && isObject(%client.STW_Clan) == true)
	{
		if(%client.STW_Clan.LeaderBLID == %client.bl_id)
		{
			STW_Clan_MessageAll(%client.STW_Clan,"The clan was disbanded by the leader.");
			STW_Clan_NotifyAll(%client.STW_Clan,"Clan Disbanded","The clan was disbanded by the leader.");
			fileDelete($STW::Gamemode::Clans::FilePath @ %client.STW_Clan.getName() @ $STW::Gamemode::Clans::Extension);
			%client.STW_isInClan = 0;
			%client.STW_isClanAdmin = 0;
			%client.STW_Clan.delete();
			%client.STW_Clan = "";
		}
		else
		{
			commandToClient(%client,'STW_OKDlgNotify',"Clan Disband Error","<just:center><br>You are not the clan leader, you cannot disband this clan.");
		}
	}
}

//DELETE CLAN - client, clan name
function serverCmdSTW_Clan_Delete(%client,%clanName) //Delete a clan (Admin only)
{
	if(%client.isAdmin)
	{
		%obj = "Clan_"@%clanName;
		
		if(isObject(%obj))
		{
			for(%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%members = ClientGroup.getObject(%i);
				
				if(%members.STW_isInClan == true && isObject(%members.STW_Clan) == true && %members.STW_Clan == %obj.getID())
				{
					messageClient(%members,'',STWOS.startMessage@"\c2"@%client.name@" \c0(ADMIN) \c6has disbanded your clan.");
					%members.STW_Clan = "";
					%members.STW_isInClan = 0;
					%members.STW_isClanAdmin = 0;
				}
			}
			
			fileDelete($STW::Gamemode::Clans::FilePath @ %obj.getName() @ $STW::Gamemode::Clans::Extension);
			%obj.delete();
			messageClient(%client,'',STWOS.startMessage@"\""@%obj@"\" has been deleted.");
		}
		else
		{
			return messageClient(%client,'',STWOS.startMessage@"There is no such clan existing.");
		}
	} 
	else
	{
		return messageClient(%client,'',STWOS.startMessage@"You are not permitted to use this command.");
	}
}

//LIST CLANS - client
function serverCmdSTW_Clan_List(%client) //gives a list of all the clans
{
	for(%i = 0; %i < STW_ClanManager.getCount(); %i++) //gotta make sure we're getting all of the clan objects
	{
		%obj = STW_ClanManager.getObject(%i);
		
		if(getSubStr(%obj.getName(),0,4) $= "Clan")
		{
			messageClient(%client,'',%obj.clanColor@%obj.name);
			%r = 1;
		}
	}
	if(!%r) //had to do this since doing an else with the if argument wasn't working
	{
		return messageClient(%client,'',STWOS.startMessage@"There are no clans existing right now.");
	}
}

//INVITE TO CLAN - client, user
function serverCmdSTW_Clan_Invite(%client,%user)
{
	if(%client.STW_isInClan && isObject(%client.STW_Clan))
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			%uName = %user.name;
			%cName = %client.name;
		
			if(%user.STW_isInClan == false && isObject(%user.STW_Clan) == false)
			{
				if(%user.STW_Clan_Invite != %client.STW_Clan || %user.STW_Clan_Invite $= "")
				{
					%user.STW_Clan_Invite = %client.STW_Clan;
					messageClient(%client,'',STWOS.startMessage@"You have invited \c2"@%uName@" \c6to your clan.");
					messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c6has invited you to his/her clan. \c2Say /STW_Clan_AcceptInvite \c6to join or \c2/STW_Clan_DeclineInvite \c6to decline the invite.");
					schedule(30000,0,"STW_Clan_ExpireInvite",%client,%user,1);
				}
				else
				{
					messageClient(%client,'',STWOS.startMessage@"You already invited this person to your clan.");
					return;
				}
			}
			else
			{
				return messageClient(%client,'',STWOS.startMessage@"This person is already in a clan, you cannot invite him/her.");
			}
			if(%user == %client)
			{
				return messageClient(%client,'',STWOS.startMessage@"Now why would you invite yourself to your own clan? Idiot...");
			}
		}
	}
	else
	{
		commandToclient(%client,'STW_OKDlgNotify',"Clan Verification Error","<just:center>You are not in a clan.<br>You can only invite people to a clan when you are in the clan specified.");
	}
}

//ACCEPT CLAN INVITE - client, clan object
function serverCmdSTW_Clan_AcceptInvite(%client,%clan)
{
	if(%client.STW_Clan_Invite !$= "" && isObject(%client.STW_Clan_Invite) == true)
	{
		if(isObject(findclientbyname(%client.STW_Clan_Invite.Leader)))
		{
			%clan = %client.STW_Clan_Invite;
			%user = findclientbyname(%clan.Leader);
			%client.STW_JoinClan(%clan);
			STW_Clan_ExpireInvite("",%client,0);
			messageClient(%client,'',STWOS.startMessage@"You have accepted \c2"@%user.name@"\'s \c6clan invite.");
			messageClient(%user,'',STWOS.startMessage@"\c2"@%client.name@" \c6has accepted your clan invite.");
		}
	}
}

//DECLINE CLAN INVITE - client, user, clan object
function serverCmdSTW_Clan_DeclineInvite(%client)
{
	if(%client.STW_Clan_Invite !$= "" && isObject(%client.STW_Clan_Invite) == true)
	{
		if(isObject(findclientbyname(%client.STW_Clan_Invite.Leader)))
		{
			%user = findclientbyname(%client.STW_Clan_Invite.Leader);
			STW_Clan_ExpireInvite("",%client,0);
			messageClient(%client,'',STWOS.startMessage@"You have declined \c2"@%user.name@"\'s \c6clan invite.");
			messageClient(%user,'',STWOS.startMessage@"\c2"@%client.name@" \c6has declined your clan invite.");
		}
		else
		{
			messageClient(%client,'',STWOS.startMessage@"You have declined \c2"@%user.name@"\'s \c6clan invite.");
			STW_Clan_ExpireInvite("",%client,0);
		}
	}
}

function serverCmdSTW_Clan_IgnoreInvite(%client)
{
	if(%client.STW_Clan_Invite !$= "" && isObject(%client.STW_Clan_Invite) == true)
	{
		if(isObject(findclientbyname(%client.STW_Clan_Invite.Leader)))
		{
			%user = findclientbyname(%client.STW_Clan_Invite.Leader);
			STW_Clan_IgnoreInvite(%user,%client,1);
		}
		else
		{
			STW_Clan_IgnoreInvite(%client.STW_Clan_Invite.LeaderBLID);
		}
	}
}

//KICK FROM CLAN - client, user
function serverCmdSTW_Clan_KickMember(%client,%user)
{
	if(%client.STW_isClanAdmin == true && %client.STW_isInClan == true && isObject(%client.STW_Clan) == true)
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			%cName = %client.name;
			%uName = %user.name;
			
			if(%user.STW_isInClan == true && %user.STW_Clan == %client.STW_Clan)
			{
				if(%user != %client)
				{
					STW_Clan_MessageAll("\c2"@%cName@" \c0(CLAN ADMIN) \c6has kicked \c3"@%uName@" \c6from the clan.");
					%user.STW_LeaveClan(%user.STW_Clan);
				}
				else
				{
					commandToClient(%client,'STW_OKDlgNotify',"Clan Management Error","You cannot kick yourself from the clan.");
					messageClient(%client,'',STWOS.startMessage@"You cannot kick yourself from the clan.");
					return;
				}
			}
			else
			{
				commandToClient(%client,'STW_OKDlgNotify',"Clan Management Error","<color:FFFF00>"@%uName@" <color:000000>is not in your clan.");
				messageClient(%client,'',STWOS.startMessage@"\c3"@%uName@" \c6is not in your clan.");
				return;
			}
		}
	}
}

//ADD TO CLAN - client, user, clan object
function serverCmdSTW_Clan_AddMember(%client,%user,%clan)
{
	if(%client.bl_id == getNumKeyID() || %client.isCoHost)
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			%uName = %user.name;
			%cName = %client.name;
			
			if(isObject("Clan_"@%clan))
			{
				%clanObj = "Clan_"@%clan;
				%user.STW_JoinClan(%clan);
				messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c5(HOST) \c6has forcefully added you to a clan named \"\c3"@%clanObj.name@"\c6\".");
			}
		}
	}
}

//REMOVE FROM CLAN - client, user, clan object
function serverCmdSTW_Clan_RemoveMember(%client,%user)
{
	if(%client.bl_id == getNumKeyID() || %client.isCoHost)
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			%uName = %user.name;
			%cName = %client.name;
			
			if(%user.STW_isInClan == true && isObject(%user.STW_Clan) == true)
			{
				%user.STW_LeaveClan(%user.STW_Clan);
				messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c5(HOST) \c6has forcefully removed you from the current clan you were in.");
			}
			else
			{
				return messageClient(%client,'',STWOS.startMessage@"\c3"@%uName@" \c6is not in a clan.");
			}
		}
	}
}

function serverCmdSTW_Clan_AdminMember(%client,%user)
{
	if(%client.STW_isInClan == true && isObject(%client.STW_Clan) == true && %client.STW_Clan.LeaderBLID == %client.bl_id)
	{
		if(%client.STW_Clan.hasAdmins)
		{
			if(isObject(findclientbyname(%user)))
			{
				%user = findclientbyname(%user);
				if(%user != %client)
				{
					if(%user.STW_isInClan && isObject(%user.STW_Clan) == true && %user.STW_Clan == %client.STW_Clan && %user.STW_isClanAdmin == false)
					{
						%user.STW_isClanAdmin = 1;
						STW_Clan_MessageAll("\c2"@%client.name@" \c3has promoted \c2"@%user.name@" \c3to \c0Clan Admin\c3.");
					}
					else
					{
						commandToClient(%client,'STW_OKDlgNotify',"Clan Management Error","This person is either in another clan, or is not currently in a clan at all.");
						messageClient(%client,'',STWOS.startMessage@"This person is either in another clan, or is not currently in a clan at all.");
					}
				}
				else
				{
					commandToClient(%client,'STW_OKDlgNotify',"Clan Management Error","Why would you clan-admin yourself when you're the clan leader?");
					messageClient(%client,'',STWOS.startMessage@"Why would you clan-admin yourself when you're the clan leader?");
				}
			}
			else
			{
				commandToClient(%client,'STW_OKDlgNotify',"Clan Management Error","There is no such person that exists on this server.");
				messageClient(%client,'',STWOS.startMessage@"There is no such person that exists on this server.");
			}
		}
		else
		{
			commandToClient(%client,'STW_OKDlgNotify',"Clan Management Error","You do not have the option enabled for having clan admins.");
			messageClient(%client,'',STWOS.startMessage@"You do not have the option enabled for having clan admins.");
		}
	}
}

//EXPIRE CLAN INVITE - client, user, auto-message check
function STW_Clan_ExpireInvite(%client,%user,%check)
{
	if(%check)
	{
		if(%user.STW_Clan_Invite $= "")
		{
			return;
		}
		
		%user.STW_Clan_Invite = "";
		messageClient(%user,'',STWOS.startMessage@"\c2"@%client.name@"\'s \c6clan invite to you has expired.");
		messageClient(%client,'',STWOS.startMessage@"Your clan invite to \c2"@%user.name@" \c6has expired.");
	}
	else
		%user.STW_Clan_Invite = "";
}

//===============================================================|
//                                                               |
//                        Server Commands                        |
//                            {End}                              |
//                                                               |
//===============================================================|


//===============================================================|
//                                                               |
//                        Base Functions                         |
//                           {Begin}                             |
//                                                               |
//===============================================================|

//STW_Clan_OnJoin - clan obj , client object
//selects the clan obj for the client obj to join, if there is no clan obj, it returns an error
function STW_Clan_onJoin(%this,%client)
{
	if(isObject(%this))
	{
		%client.STW_LeaveClan(%this);
		STW_Clan_MessageAll(%this,"\c2"@%client.name@" \c6has joined the clan.");
		%client.STW_Clan = %this;
		%client.STW_isInClan = 1;
		%this.memberCount++;
		
		if(%this.Members $= "")
		{
			%this.Members = %client.bl_id;
			return;
		}
		else
		{
			%this.Members = %this.Members SPC %client.bl_id;
			return;
		}
	}
	else
	{
		commandToClient(%client,'STW_OKDlgNotify',"STW_Clan_onJoin ERROR","<just:center>There has been an unconvienent error when trying to join a non-existant clan.");
	}
}

//STW_Clan_OnLeave - clan obj , client obj
//selects the clan obj for the client obj to leave
function STW_Clan_onLeave(%this,%client)
{
	if(isObject(%client.STW_Clan) == true && %client.STW_isInClan == true)
	{
		if(%client.STW_Clan.LeaderBLID == %client.bl_id)
		{
			serverCmdSTW_Clan_Disband(%client);
			return;
		}
		
		STW_Clan_MessageAll(%client.STW_Clan,"\c2"@%client.name@" \c6has left your clan.");
		%client.STW_Clan = "";
		%client.STW_isInClan = 0;
		%this.memberCount--;
		
		for(%i = 0; %i <= %this.memberCount; %i++)
		{
			if(getWord(%this.Members,%i) == %client.bl_id)
			{
				%this.Members = strReplace(%this.Members,%client.bl_id,"");
			}
		}
	}
}

//STW_Clan_MessageAll - clan obj , text message
//messages each member in clan obj with text message
function STW_Clan_MessageAll(%this,%message)
{
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%clientb = ClientGroup.getObject(%i);
		
		if(%clientb.STW_isInClan == true && isObject(%clientb.STW_Clan) == true && %clientb.STW_Clan == %this)
		{
			messageClient(%clientb,'',STWOS.startMessage@%message);
		}
	}
}

//STW_Clan_NotifyAll - clan obj , title , text message
//notifies each member in clan obj with title & text message using stw_okdlg
function STW_Clan_NotifyAll(%this,%title,%message)
{
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%clientb = ClientGroup.getObject(%i);
		
		if(%clientb.STW_isInClan == true && isObject(%clientb.STW_Clan) == true && %clientb.STW_Clan == %this)
		{
			commandToClient(%clientb,'STW_OKDlgNotify',%title,%message);
		}
	}
}

//function STW_Clan_IgnoreInvite(%user,%client)
//{
//	%num = "0123456789";
//	
//	for(%i = 0; %i <= strLen(%num); %i++)
//	{
//		if(getSubStr(%user,%i,1) $= getSubStr(%num,%i,1))
//		{
//			%blid = %user;
//		}
//	}
//}

//STW_Clan_CheckColor - color num
//checks the color number and returns "\c(num here)"
function STW_Clan_CheckColor(%color)
{
	if(%color $= "0")
	{
		return %color = "\c0";
	}
	if(%color $= "1")
	{
		return %color = "\c1";
	}
	if(%color $= "2")
	{
		return %color = "\c2";
	}
	if(%color $= "3")
	{
		return %color = "\c3";
	}
	if(%color $= "4")
	{
		return %color = "\c4";
	}
	if(%color $= "5")
	{
		return %color = "\c5";
	}
	if(%color $= "6")
	{
		return %color = "\c6";
	}
	if(%color $= "7")
	{
		return %color = "\c7";
	}
	if(%color $= "8")
	{
		return %color = "\c8";
	}
}

//===============================================================|
//                                                               |
//                         Base Functions                        |
//                             {End}                             |
//                                                               |
//===============================================================|

//GAMECONNECTION FUNCTIONS

//callback
function GameConnection::STW_JoinClan(%client,%clanObj)
{
	STW_Clan_onJoin(%clanObj,%client);
}

//callback
function GameConnection::STW_LeaveClan(%client,%clanObj)
{
	STW_Clan_onLeave(%clanObj,%client);
}

//===============================================================|
//                                                               |
//                         Clan Settings                         |
//                            {Begin}                            |
//                                                               |
//===============================================================|

function serverCmdSTW_Clan_UpdateSettings(%client,%team,%ff,%bank,%upgrades,%admins,%cas)
{
	if(%client.STW_isInClan == true && isObject(%client.STW_Clan) == true && %client.STW_Clan.LeaderBLID == %client.bl_id)
	{
		%clan = %client.STW_Clan;
		
		%clan.isTeam = %team;
		%clan.FriendlyFire = %ff;
		%clan.hasBank = %bank;
		%clan.hasUpgrades = %upgrades;
		%clan.hasAdmins = %admins;
		%clan.allowsCAS = %cas;
		
		STW_Clan_SaveSettings(%clan);
	}
}

function serverCmdSTW_Clan_LoadSettings(%client)
{
	if(%client.STW_isInClan == true && isObject(%client.STW_Clan) == true && %client.STW_Clan.LeaderBLID == %client.bl_id)
	{
		%clan = %client.STW_Clan;
		
		%team = %clan.isTeam;
		%ff = %clan.FriendlyFire;
		%bank = %clan.hasBank;
		%upgrades = %clan.hasUpgrades;
		%admins = %clan.hasAdmins;
		%cas = %clan.allowsCAS;
		
		commandToClient(%client,'STW_Clan_LoadSettings',%team,%ff,%bank,%upgrades,%admins,%cas);
	}
}

//===============================================================|
//                                                               |
//                       Data Transferring                       |
//                            {Begin}                            |
//                                                               |
//===============================================================|

function serverCmdSTW_checkIsInClan(%client)
{
	if(%client.STW_isInClan == true && isObject(%client.STW_Clan) == true)
	{
		commandToClient(%client,'STW_successClanCheck');
	}
	else
	{
		commandToClient(%client,'STW_failureClanCheck',"Clan Access Denied","<color:FF0000>ERR A_05<br>Clan Validation Failure<br><br><just:center><color:000000>Do you want to create a new clan?");
	}
}

function serverCmdSTW_sendClanMembers(%client)
{
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%user = ClientGroup.getObject(%i);
		%userName = %user.name;
		
		if(%user.STW_isInClan == true && isObject(%user.STW_Clan) == true && %user.STW_Clan == %client.STW_Clan)
		{
			if(%user.STW_isClanAdmin)
			{
				if(%user.STW_Clan.LeaderBLID == %user.bl_id)
				{
					%admin = "CL";
				}
				else
				{
					%admin = "CA";
				}
			}
			else
			{
				%admin = "-";
			}
			
			%member = %admin TAB %userName TAB %user.score TAB %user.bl_id;
			commandToClient(%client,'STW_getClanMembers',1,%member);
		}
	}
}