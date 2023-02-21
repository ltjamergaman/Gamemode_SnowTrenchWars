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
//|           Main Functionality for the Leveling System           |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//LEVELING SYSTEM VERSION
$STW::Gamemode::Leveling::Version = "3";

//EXP GAINED/LOSSED
$STW::Gamemode::Leveling::EXPKillGain = "750"; //default is 750
$STW::Gamemode::Leveling::EXPCPGain = "400"; //default is 400

//MONEY GAINED/LOSSED
$STW::Gamemode::Leveling::MoneyKillGain = "600 1200"; //default is 600-1200
$STW::Gamemode::Leveling::MoneyCPGain = "100 500"; //default is 100-500
$STW::Gamemode::Leveling::MoneyKillLoss = "50 300"; //default is 50-300

//MAX VARS
$STW::Gamemode::Leveling::MaxLevel = "20"; //default is 20
$STW::Gamemode::Leveling::MaxRank = "4"; //default is 4
$STW::Gamemode::Leveling::MaxEXP = "999999";
$STW::Gamemode::Leveling::MaxMoney = "9999999";
$STW::Gamemode::Leveling::MaxCPs = "9999";

package STW_LevelUp
{
	function GameConnection::onDeath(%client,%killerPlayer,%killerClient,%damageType,%damageLoc)
	{
		parent::onDeath(%client,%killerPlayer,%killerClient,%damageType,%damageLoc);
		%killerClient.STW_ReqEXP = %killerClient.STW_Level * 50 * %killerClient.STW_Level * 5;
		%client.STW_ReqEXP = %client.STW_Level * 50 * %client.STW_Level * 5;
		%exp = $STW::Gamemode::Leveling::EXPKillGain;
		%money = getRandom(getWord($STW::Gamemode::Leveling::MoneyKillGain,0),getWord($STW::Gamemode::Leveling::MoneyKillGain,1));
		
		if(%client != %killerClient)
		{
			if(%client.STW_isInClan && isObject(%client.STW_Clan))
			{
				%clana = %client.STW_Clan;
			}
			if(%killerClient.STW_isInClan && isObject(%killerClient.STW_Clan))
			{
				%clanb = %killerClient.STW_Clan;
			}
			if(%clana !$= "" && %clanb !$= "")
			{
				if(%clana == %clanb)
				{
					%killerClient.STW_decMoney(getRandom(getWord($STW::Gamemode::Leveling::MoneyKillLoss,0),getWord($STW::Gamemode::Leveling::MoneyKillLoss,1)),0);
					STW_Clan_MessageAll(%clanb,"\c3"@%killerClient.name@" \c6has slain a fellow clan member named \c2"@%client.name@"\c6!");
				}
				else
				{
					%killerClient.STW_addEXP(%exp);
					%killerClient.STW_addMoney(%money,2);
				}
			}
			else
			{
				%killerClient.STW_addEXP(%exp);
				%killerClient.STW_addMoney(%money,2);
			}
			
			commandToClient(%client,'STW_TabNotify',"<color:FF0000>" @ %killerClient.name @ " <color:FFFFFF>has killed you.");
			commandToClient(%killerClient,'STW_TabNotify',"You have killed <color:FF0000>" @ %client.name @ "<br><color:FFFFFF>+<color:FFFF00>$" @ %money @ " & " @ %exp SPC "EXP");
		}
		
		commandToClient(%client,'STW_UpdateHUDStats',0);
		commandToClient(%client,'STW_getClientStats');
		commandToClient(%killerClient,'STW_UpdateHUDStats',0);
		commandToClient(%killerClient,'STW_getClientStats');
	}
};
activatePackage(STW_LevelUp);

//LEVEL UP FUNCTIONS
function GameConnection::STW_addEXP(%client,%amount)
{
	if(%amount > 0 || %amount < 0)
    {
		if(%client.STW_EXP < %client.STW_ReqEXP)
		{
			%client.STW_EXP += mFloatLength(%amount,0);
			
            if(%client.STW_EXP >= %client.STW_ReqEXP)
            {
                if(%client.STW_Level < $STW::Gamemode::Leveling::MaxLevel)
                {
                    %client.STW_Level++;
                    %client.STW_EXP = 0;
                    %client.STW_ReqEXP = %client.STW_Level * 50 * %client.STW_Level * 5;
                    messageClient(%client,'',STWOS.startMessage@"You have leveled up! (\c2"@%client.STW_Level@"\c6)");
                    STWOS.sendMessage("\c2"@%client.name@" \c6has leveled up! (\c2"@%client.STW_Level@"\c6)");
                }
            }
        }
		else if(%client.STW_Level < $STW::Gamemode::Leveling::MaxLevel)
		{
			%client.STW_Level++;
			%client.STW_EXP = 0;
			%client.STW_ReqEXP = %client.STW_Level * 50 * %client.STW_Level * 5;
		}
		else if(%client.STW_Setting_AutoRankUp)
		{
			serverCmdSTW_RankUp(%client);
		}
		
		%client.STW_TotalEXP += %amount;
		//%client.centerPrint("\c6+\c3"@%amount@" EXP",2);
		commandToClient(%client,'STW_getClientStats');
    }

}

function GameConnection::STW_addMoney(%client,%amount,%justcheck)
{
    if(%amount > 0)
    {
		if(%justCheck == 2)
		{
			//do nothing
		}
		else if(%justCheck == 1)
		{
			messageClient(%client,'',"<just:center>\c6+\c3$"@%amount@"<just:left>");
		}
		else
		{
			messageClient(%client,'',"\c6+\c3$"@%amount);
		}
		
		%client.STW_Money += mFloatLength(%amount,0);
		%client.STW_Money = mFloatLength(%client.STW_Money,0);
		commandToClient(%client,'STW_getClientStats');
    }
    else
    {
        error("STW Leveling - ERROR, cannot add characters to client's money or decrease client's money!");
    }
}

function GameConnection::STW_decMoney(%client,%amount,%justcheck)
{
    if(%amount > 0)
    {
		if(%client.STW_Money < %amount)
		{
			error("STW Leveling - ERROR, cannot set client's money to a negative amount!" NL "GameConnection::STW_DecMoney");
			return;
		}
		if(%justCheck)
		{
			messageClient(%client,'',"<just:center>\c6-\c3$"@%amount@"<just:left>");
		}
		else
		{
			messageClient(%client,'',"\c6-\c3$"@%amount);
		}
		
		%client.STW_Money -= mFloatLength(%amount,0);
		%client.STW_Money = mFloatLength(%client.STW_Money,0);
		commandToClient(%client,'STW_getClientStats');
    }
    else
    {
        error("STW Leveling - ERROR, cannot add characters to client's money or increase client's money!" NL "GameConnection::STW_DecMoney");
    }
}

function GameConnection::STW_addCP(%client,%amount,%check)
{
	if(%amount > 0)
	{
		%cps = %amount;
		%exp = $STW::Gamemode::Leveling::EXPCPGain;
		%money = getRandom(getWord($STW::Gamemode::Leveling::MoneyCPGain,0),getWord($STW::Gamemode::Leveling::MoneyCPGain,1));
		
		if(%check)
		{
			%client.STW_addEXP(%exp);
			%client.STW_addMoney(%money,2);
			commandToClient(%client,'STW_TabNotify',"You captured <color:FFFF00>" @ %cps SPC "<color:FFFFFF>point(s)<br>+<color:FFFF00>$" @ %money @ " & " @ %exp SPC "EXP");
		}
		else
		{
			messageClient(%client,'',"\c6+\c3"@%cps@" CP(s)");
		}
		
		%client.STW_CPs += mFloatLength(%cps,0);
		commandToClient(%client,'STW_getClientStats');
		commandToClient(%client,'STW_UpdateHUDStats');
	}
	else
	{
		error("STW Leveling - ERROR, cannot add characters to client's CP(s) or decrease client's CP(s)!");
	}
}

//SERVER COMMANDS

function serverCmdSTW_RankUp(%client)
{
	if(%client.STW_Level >= $STW::Gamemode::Leveling::MaxLevel && %client.STW_Rank < $STW::Gamemode::Leveling::MaxRank && %client.STW_EXP >= %client.STW_ReqEXP)
	{
		%client.STW_Level = "1";
		%client.STW_EXP = "0";
		%client.STW_Money = 0;
		%client.STW_ReqEXP = %client.STW_Level * 50 * %client.STW_Level * 5;
		%client.STW_Rank++;
		%client.STW_resetInventory();
		//messageClient(%client,'',STWOS.startMessage@"You have ranked up! (\c2"@%client.STW_Rank@"\c6)");
		commandToClient(%client,'STW_OKDlgNotify',"Rank Up","<just:center>You have ranked up!<br>(<color:00FF00>"@%client.STW_Rank@"<color:000000>)");
		commandToClient(%client,'STW_getClientStats');
	}
	else if(%client.STW_Level >= $STW::Gamemode::Leveling::MaxLevel && %client.STW_Rank >= $STW::Gamemode::Leveling::MaxRank && %client.STW_EXP >= %client.STW_ReqEXP)
	{
		commandToClient(%client,'STW_OKDlgNotify',"Rank Up Error","<just:center>You are already the max rank possible!");
	}
	else
	{
		commandToClient(%client,'STW_OKDlgNotify',"Rank Up Error","<just:center>Sorry, you cannot <color:FFFF00>Rank Up <color:000000>because your Career Statistics do not meet the required specifications.");
	}
}

function serverCmdSTW_DonateMoney(%client,%user,%amount)
{
	if(%user !$= "" && isObject(findclientbyname(%user)) == true)
	{
		%user = findclientbyname(%user);
		%uName = %user.name;
		%cName = %client.name;
		%amount = mFloatLength(%amount,0);
		%client.STW_Money = mFloatLength(%client.STW_Money,0);
		%user.STW_Money = mFloatLength(%user.STW_Money,0);
		
		if(!%user.STW_allowsDonations)
		{
			commandToClient(%client,'STW_OKDlgNotify',"Donation Error","<just:center><color:FFFF00>"@%uName@" <color:000000>is not accepting donations.");
			return;
		}		
		if(%amount > 0)
		{
			if(%client.STW_Money >= %amount)
			{
				%client.STW_Money -= mFloatLength(%amount,0);
				%client.STW_Money = mFloatLength(%client.STW_Money,0);
				%user.STW_Money += mFloatLength(%amount,0);
				%user.STW_Money = mFloatLength(%user.STW_Money,0);
				messageClient(%client,'',STWOS.startMessage@"You have donated \c3$"@%amount@" \c6to \c4"@%uName@"\c6.");
				messageClient(%user,'',STWOS.startMessage@"\c4"@%cName@" \c6has donated \c3$"@%amount@" \c6to you.");
				commandToClient(%client,'STW_getClientStats');
				commandToClient(%user,'STW_getClientStats');
			}
			else
			{
				messageClient(%client,'',STWOS.startMessage@"You have unsufficient funds!");
				commandToClient(%client,'STW_OKDlgNotify',"Donation Transaction Incomplete","<color:EE0000>ERR A_02<br>Validation Incompletion Detected<br><br><color:000000><just:center>You have unsufficient funds.");
			}
		}
		else
		{
			messageClient(%client,'',STWOS.startMessage@"Please enter a number to donate money.");
		}
	}
	else
	{
		commandToClient(%client,'STW_OKDlgNotify',"Donation Transaction Incomplete","<color:EE0000>ERR A_02<br>Validation Incompletion Detected<br><br><color:000000><just:center>You must enter a name that is valid.");
	}
}

function serverCmdDonate(%client,%user,%amount) //for those who don't want to use the gui
{
	serverCmdSTW_DonateMoney(%client,%user,%amount);
}