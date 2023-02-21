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
//|                           Version: 3                           |\\
//|                        Profiling System                        |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//VARS

//FUNCTIONS
function GameConnection::STW_SaveProfile(%client)
{
	%client.STW_SaveStats();
	%client.STW_SavePlayerData();
	%client.STW_SaveInventory();
	%client.STW_SaveInfo();
}

function GameConnection::STW_LoadProfile(%client)
{
	%client.STW_LoadStats();
	%client.STW_LoadPlayerData();
	%client.STW_LoadInventory();
	%client.STW_LoadInfo();
}

function GameConnection::STW_ResetProfile(%client)
{
	%client.STW_ResetStats();
	%client.STW_ResetPlayerData();
	%client.STW_ResetInventory();
	%client.STW_ResetInfo();
}

function GameConnection::STW_SaveStats(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Stats.txt";
	%client.STW_Money = mFloatLength(%client.STW_Money,0);
	
	if(isFile(%filepath))
	{
		fileDelete(%filepath);
		%level = %client.STW_Level;
		%rank = %client.STW_Rank;
		%exp = %client.STW_EXP;
		%reqexp = %client.STW_Level * 50 * %client.STW_Level * 5;
		%cps = %client.STW_CPs;
		%money = %client.STW_Money;
		%kills = %client.getKills();
		%deaths = %client.getDeaths();
		//%expplus = %client.STW_EXPPlus SPC %client.STW_EXPPlus_Expire;
	
		//if(%client.hasMobileSpawn)
		//{
			//%mobilespawn = %client.STW_MobileSpawn;
		//}
	
		%file = new FileObject();
		%file.openForWrite(%filepath);
		%file.writeLine("Level "@%level);
		%file.writeLine("Rank "@%rank);
		%file.writeLine("EXP "@%exp);
		%file.writeLine("Req.EXP "@%reqexp);
		%file.writeLine("CP(s) "@%cps);
		%file.writeLine("Money "@%money);
		%file.writeLine("Kills "@%kills);
		%file.writeLine("Deaths "@%deaths);
		%file.close();
		%file.delete();
	}
	else
	{
		%client.STW_ResetStats();
	}
}

function GameConnection::STW_LoadStats(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Stats.txt";
	
	if(isFile(%filepath))
	{
		%file = new FileObject();
		%file.openForRead(%filepath);
		%client.STW_Level = getWord(%file.readLine(),1);
		%client.STW_Rank = getWord(%file.readLine(),1);
		%client.STW_EXP = getWord(%file.readLine(),1);
		%client.STW_ReqEXP = getWord(%file.readLine(),1);
		%client.STW_CPs = getWord(%file.readLine(),1);
		%client.STW_Money = getWord(%file.readLine(),1);
		%client.STW_Armor = getWord(%file.readLine(),1);
		%client.setKills(getWord(%file.readLine(),1));
		%client.setDeaths(getWord(%file.readLine(),1));
		//%client.STW_EXPPlus = getWord(%file.readLine(),1);
		//%client.STW_EXPPlus_Expired = getWord(%file.readLine(),1);
		//%client.STW_MobileSpawn = getWord(%file.readLine(),1);
		%file.close();
		%file.delete();	
	}
	else
	{
		%client.STW_ResetStats();
	}
}

function GameConnection::STW_ResetStats(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Stats.txt";
	%client.STW_Level = "1";
	%client.STW_Rank = "0";
	%client.STW_EXP = "0";
	%client.STW_ReqEXP = %client.STW_Level * 50 * %client.STW_Level * 5;
	%client.STW_CPs = "0";
	%client.STW_Money = "0";
	%client.setKills(0);
	%client.setDeaths(0);
	//%client.STW_EXPPlus = "0";
	//%client.STW_EXPPlus_Expired = "0";
	//%client.STW_MobileSpawn = "";
	
	%file = new FileObject();
	%file.openForWrite(%filepath);
	%file.writeLine("Level "@%client.STW_Level);
	%file.writeLine("Rank "@%client.STW_Rank);
	%file.writeLine("EXP "@%client.STW_EXP);
	%file.writeLine("Req.EXP "@%client.STW_ReqEXP);
	%file.writeLine("CP(s) "@%client.STW_CPs);
	%file.writeLine("Money "@%client.STW_Money);
	%file.writeLine("Kills "@%client.getKills());
	%file.writeLine("Deaths "@%client.getDeaths());
	//%file.writeLine("Level "@%level);
	//%file.writeLine("Level "@%level);
	%file.close();
	%file.delete();
}

function GameConnection::STW_SavePlayerData(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Player Data.txt";
	%player = %client.player;
	
	if(isFile(%filepath))
	{
		if(isObject(%player))
		{
			%file = new FileObject();
			%file.openForWrite(%filepath);
			%file.writeLine("DataBlock "@%player.getDatablock());
			%file.writeLine("Armor "@%player.STW_Armor);
			%file.close();
			%file.delete();
		}
		else
		{
			%client.spawnPlayer();
			%client.STW_SavePlayerData();
		}
	}
	else
	{
		%client.STW_ResetPlayerData();
	}
}

function GameConnection::STW_LoadPlayerData(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Player Data.txt";
	%player = %client.player;
	
	if(isFile(%filepath))
	{
		if(isObject(%player))
		{
			%file = new FileObject();
			%file.openForRead(%filepath);
			%player.changeDatablock(getWord(%file.readLine(),1));
			%player.STW_Armor = getWord(%file.readLine(),1);
			%file.close();
			%file.delete();
		}
		else
		{
			%client.spawnPlayer();
			%client.STW_LoadPlayerData();
		}
	}
	else
	{
		%client.STW_ResetPlayerData();
	}
}

function GameConnection::STW_ResetPlayerData(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Player Data.txt";
	%player = %client.player;
	
	if(isObject(%player))
	{
		%player.changeDatablock("STWPlayerNormal");
		%player.STW_Armor = "0";
		
		%file = new FileObject();
		%file.openForWrite(%filepath);
		%file.writeLine("DataBlock "@%player.getDatablock());
		%file.writeLine("Armor "@%player.STW_Armor);
		%file.close();
		%file.delete();
	}
	else
	{
		%client.spawnPlayer();
		%client.STW_ResetPlayerData();
	}
}

function GameConnection::STW_SaveInfo(%client)
{
    %filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/info.txt";
    %muted = %client.isMuted;
    %muteTimeRemaining = %client.muteTimeRemaining;
	%warnings = %client.STW_Warnings;
	%kickedAmount = %client.kickedAmount;
	%bannedAmount = %client.bannedAmount;
	%createClan = %client.STW_canCreateClan;
	%useStore = %client.STW_canUseStore;
	%useInventory = %client.STW_canUseInventory;
	%canSuggest = %client.STW_canSuggest;
	%canReport = %client.STW_canReport;
	
	if(isFile(%filepath))
	{
		%file = new FileObject();
		%file.openForWrite(%filepath);
		%file.writeLine("Muted "@%muted);
		%file.writeLine("MTRemaining "@%muteTimeRemaining);
		%file.writeLine("Warnings "@%warnings);
		%file.writeLine("KickedAmount "@%kickedAmount);
		%file.writeLine("BannedAmount "@%bannedAmount);
		%file.writeLine("CreateClan "@%createClan);
		%file.writeLine("UseStore "@%useStore);
		%file.writeLine("UseInventory "@%useInventory);
		%file.writeLine("canSuggest "@%canSuggest);
		%file.writeLine("canReport "@%canReport);
		%file.close();
		%file.delete();
	}
	else
	{
		%client.STW_ResetInfo();
	}
}

function GameConnection::STW_LoadInfo(%client)
{
    %filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/info.txt";
	
    if(isFile(%filepath))
    {
        %file = new FileObject();
        %file.openForRead(%filepath);
        %client.isMuted = getWord(%file.readLine(),1);
        %client.muteTimeRemaining = getWord(%file.readLine(),1);
        %client.STW_Warnings = getWord(%file.readLine(),1);
        %client.STW_KickedAmount = getWord(%file.readLine(),1);
        %client.STW_BannedAmount = getWord(%file.readLine(),1);
		%client.STW_canCreateClan = getWord(%file.readLine(),1);
		%client.STW_canUseStore = getWord(%file.readLine(),1);
		%client.STW_canUseInventory = getWord(%file.readLine(),1);
		%client.STW_canSuggest = getWord(%file.readLine(),1);
		%client.STW_canReport = getWord(%file.readLine(),1);
        %file.close();
        %file.delete();
    }
    else
    {
        %client.STW_ResetInfo();
    }
}

function GameConnection::STW_ResetInfo(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/info.txt";
	%client.isMuted = 0;
    %client.muteTimeRemaining = 0;
    %client.STW_Warnings = 0;
    %client.kickedAmount = 0;
    %client.bannedAmount = 0;
	%client.STW_canCreateClan = 1;
	%client.STW_canUseStore = 1;
	%client.STW_canUseInventory = 1;
	%client.STW_canSuggest = 1;
	%client.STW_canReport = 1;
	
	%file = new FileObject();
	%file.openForWrite(%filepath);
	%file.writeLine("Muted "@%client.isMuted);
	%file.writeLine("MTRemaining "@%client.muteTimeRemaining);
	%file.writeLine("Warnings "@%client.STW_Warnings);
	%file.writeLine("KickedAmount "@%client.kickedAmount);
	%file.writeLine("BannedAmount "@%client.bannedAmount);
	%file.writeLine("CreateClan "@%client.STW_canCreateClan);
	%file.writeLine("UseStore "@%client.STW_canUseStore);
	%file.writeLine("UseInventory "@%client.STW_canUseInventory);
	%file.writeLine("canSuggest "@%client.STW_canSuggest);
	%file.writeLine("canReport "@%client.STW_canReport);
	%file.close();
	%file.delete();
}

function serverCmdSTW_DebugResetC(%client,%user)
{
	if(%client.isAdmin)
	{
		findclientbyname(%user).STW_ResetProfile();
		messageClient(%client,'',"\c3" @ findclientbyname(%user).name SPC "\c6has been resetted by debugging.");
		findclientbyname(%user).player.changeDatablock("PlayerStandardArmor");
	}
}

function serverCmdSTW_DebugC(%client,%user)
{
	if(%client.isAdmin)
	{
		findclientbyname(%user).hasSTWClient = 1;
		messageClient(%client,'',"\c3" @ findclientbyname(%user).name SPC"\c6has been debugged.");
	}
}