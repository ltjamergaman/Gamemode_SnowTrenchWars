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
//|                        Admin Functions                         |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//[MUTE]
$STW::Gamemode::Muting::MaxTime = 300;

function serverCmdMute(%client,%user,%time,%reason)
{
	if(%client.isAdmin || %client.isModerator)
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			%uName = %user.name;
			%cName = %client.name;
			
			if(%time > $STW::Gamemode::Muting::MaxTime)
			{
				messageClient('',"\c3Stop being an idiot.");
				return;
			}
			if(%time == -1)
			{
				if(!%user.isMuted)
				{
					%user.isMuted = 1;
					%user.muteTimeRemaning = -1;
					%user.mutedBy = %client;
					
					if(%reason $= "")
					{
						STWOS.sendMessage("\c2"@%cName@" \c6has muted \c2"@%uName@" \c6for \c0"@%time@" \c6seconds.");
						messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c6has muted you for \c0"@%time@" \c6seconds.");
					}
					else
					{
						STWOS.sendMessage("\c2"@%cName@" \c6has muted \c2"@%uName@" \c6for \c0"@%time@" \c6seconds. \c3Reason\c6: "@%reason);
						messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c6has muted you for \c0"@%time@" \c6seconds. \c3Reason\c6: "@%reason);
					}
				}
				else
				{
					messageClient(%client,'',STWOS.startMessage@"You can't mute someone who is already muted.");
					return;
				}
			}
			else if(%time >= 10)
			{
				if(!%user.isMuted)
				{
					%time = mFloatLength(%time,0);
					%user.isMuted = 1;
					%user.muteTimeRemaining = %time;
					%user.mutedBy = %client;
					decMuteTime(%user,%time);
					
					if(%reason $= "")
					{
						STWOS.sendMessage("\c2"@%cName@" \c6has muted \c2"@%uName@" \c6for \c0"@%time@" \c6seconds.");
						messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c6has muted you for \c0"@%time@" \c6seconds.");
					}
					else
					{
						STWOS.sendMessage("\c2"@%cName@" \c6has muted \c2"@%uName@" \c6for \c0"@%time@" \c6seconds. \c3Reason\c6: "@%reason);
						messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c6has muted you for \c0"@%time@" \c6seconds. \c3Reason\c6: "@%reason);
					}
				}
				else
				{
					messageClient(%client,'',STWOS.startMessage@"You can't mute someone who is already muted.");
					return;
				}
			}
			else if(%time < 10)
			{
				messageClient(%client,'',STWOS.startMessage@"You can't mute someone for less than 10 seconds.");
				return;
			}
			else if(%time $= "")
			{
				messageClient(%client,'',STWOS.startMessage@"Please enter a time higher or equal to than 10.");
				return;
			}
		}
		else
		{
			messageClient(%client,'',STWOS.startMessage@"You can't mute someone who doesn't exist on the server.");
			return;
		}
	}
	else
	{
		messageClient(%client,'',STWOS.startMessage@"You are not permitted to use this command.");
		return;
	}
}

function serverCmdUnMute(%client,%user,%check)
{
	%user = findclientbyname(%user);
	unMute(%client,%user,%check);
}

function unMute(%client,%user,%check)
{
	if(%client.isAdmin || %client.isModerator)
	{
		if(isObject(%user))
		{
			//%user = findclientbyname(%user);
			%uName = %user.name;
			%cName = %client.name;
		
			if(%user.isMuted)
			{
				%user.isMuted = 0;
				//cancel(%user.decMuteTimeLoop);
			
				if(%check)
				{
					messageClient(%user,'',STWOS.startMessage@"You are no longer muted, you can talk now.");
				}
				else
				{
					STWOS.sendMessage("\c2"@%cName@" \c6has unmuted \c2"@%uName@"\c3.");
					messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@" \c6has unmuted you, you can talk now.");
				}
			}
		}
	}
}

function decMuteTime(%client,%time)
{
	cancel(%client.decMuteTimeLoop);
	%client.muteTimeRemaining -= 1;
	if(%client.muteTimeRemaining <= 0)
	{
		unMute(%client.mutedBy,%client,1);
	}
	%client.decMuteTimeLoop = schedule(1000,0,"decMuteTime",%client,%client.muteTimeRemaining);
}

//[FORCEKILL]
function serverCmdForceKill(%client,%user,%reason1,%reason2,%reason3,%reason4,%reason5,%reason6,%reason7,%reason8,%reason9,%reason10)
{
	if(%client.isAdmin || %client.isModerator)
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			%uName = %user.name;
			%cName = %client.name;
		
			if(isObject(%user.player))
			{
				%user.player.kill();
				
				if(%reason1 $= "")
				{
					STWOS.sendMessage("\c2"@%cName@" \c6has Force-Killed \c0"@%uName@"\c6.");
					messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@"\c6 has Force-Killed you.");
				}
				else
				{
					for(%i = 0; %i <= 10; %i++)
					{
						if(%reason[%i] !$= "")
						{
							%reason = %reason SPC %reason[%i];
						}
					}
					
					STWOS.sendMessage("\c2"@%cName@" \c6has Force-Killed \c0"@%uName@"\c6. \c3Reason\c6: "@%reason);
					messageClient(%user,'',STWOS.startMessage@"\c2"@%cName@"\c6 has Force-Killed you. \c3Reason\c6: "@%reason);
				}
			}
			else
			{
				messageClient(%client,'',STWOS.startMessage@"\c2"@%uName@" \c6does not have a player object!");
			}
		}
		else
		{
			messageClient(%client,'',STWOS.startMessage@"There is no current client named \c2"@%user@" existing on the server.");
		}
	}
	else
	{
		messageClient(%client,'',STWOS.startMessage@"You are not permitted to use this command.");
	}
}

function serverCmdSTW_SendWarning(%client,%user,%title,%message)
{
	//echo(%client.name NL %user NL %title NL %message);
	//return;
	if(%client.isAdmin || %client.isModerator)
	{
		if(%user !$= "")
		{
			if(isObject(findclientbyname(""@%user@"")))
			{
				%user = findclientbyname(""@%user@"");
			
				if(%title !$= "" && %message !$= "")
				{
					commandToClient(%user,'STW_OKDlgNotify',%title,"<color:FF0000>Warning Recieved from "@%client.name@" BLID "@%client.bl_id@"<br><br><just:center><color:000000>"@%message);
				}
				else
				{
					commandToClient(%client,'STW_OKDlgNotify',"Validation Error","You must enter a title and message for the warning.");
				}
			}
			else
			{
				commandToClient(%client,'STW_OKDlgNotify',"Validation Error","You must enter a valid name.");
			}
		}
		else
		{
			commandToClient(%client,'STW_OKDlgNotify',"Validation Error","You must enter a valid name.");
		}
	}
}

function serverCmdSTW_KickPlayer(%client,%user,%message)
{
	if(%client.isAdmin || %client.isModerator)
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			if(%client.isModerator && %user.isAdmin || getNumKeyID() == %user.bl_id || %user.isCoHost && %client.bl_id != getNumKeyID())
			{
				commandToClient(%client,'STW_OKDlgNotify',"Kick","<just:center><br>You cannot kick a person who is of higher authority than you.");
				return;
			}
			if(%client == %user)
			{
				commandToClient(%client,'STW_OKDlgNotify',"Kick","<just:center><br>You cannot kick yourself, stop being incompetent.");
				return;
			}
			messageAll('MsgAdminForce',"\c3"@%client.name@" \c2has kicked \c3"@%user.name@" \c2(BLID: "@%user.bl_id@") - \""@%message@"\"");
			%user.delete("You were kicked by "@%client.name@" (BLID: "@%client.bl_id@").<br>Reason: "@%message);
		}
	}
}

function serverCmdSTW_resetPlayer(%client,%user)
{
	if(%client.isAdmin)
	{
		if(isObject(findclientbyname(%user)))
		{
			%user = findclientbyname(%user);
			%uName = %user.name;
			%cName = %client.name;
			
			if(%user !$= %client || %user.bl_id == getNumKeyID())
			{
				%user.STW_ResetPlayerData();
				%user.STW_ResetStats();
				%user.STW_ResetInventory();
				commandToClient(%user,'STW_OKDlgNotify',"Notification","<just:center><color:FFFF00>"@%cName@" <color:000000>(<color:FF0000>ADMIN<color:000000>) has reset your career statistics.");
				commandToClient(%client,'STW_OKDlgNotify',"Admin Notification","<just:center>You have just reset <color:FFFF00>"@%uName@"\'s <color:000000>career statistics.");
			}
			else
			{
				commandToClient(%client,'STW_OKDlgNotify',"Admin Notification","<just:center>You cannot reset your career statistics, only someone else-preferrably of higher authority can reset your stats.");
			}
		}
	}
}