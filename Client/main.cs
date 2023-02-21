function serverCmdSTW_SendClientStats(%client)
{
	//cancel($STW::Gamemode::Client::Schedule_[%client.bl_id]);
	%client.STW_ReqEXP = %client.STW_Level * 50 * %client.STW_Level * 5;
	%level = %client.STW_Level;
	%rank = %client.STW_Rank;
	%exp = %client.STW_EXP;
	%reqexp = %client.STW_ReqEXP;
	%cps = %client.STW_CPs;
	%money = %client.STW_Money;
	if(isObject(%client.player))
	{
		%armor = %client.player.STW_Armor;
		%playertype = %client.player.getDatablock().uiname;
	}
	else
	{
		%armor = "N/A";
		%playertype = "N/A";
	}
	
	%stats = "stats" SPC %level SPC %rank SPC %exp SPC %reqexp SPC %cps SPC %money SPC %armor SPC %playertype;
	commandToClient(%client,'STW_getClientStats',1,%stats);
	//$STW::Gamemode::Client::Schedule_[%client.bl_id] = %client.schedule(10000,"STW_UpdateGUIStats");
}

function serverCmdSTW_SendWeaponsInfo(%client)
{
	for(%i = 0; %i <= STW_WeaponManager.weaponCount; %i++)
	{
		%weaponData[%i] = STW_WeaponManager.weaponData[%i];
		%weaponName[%i] = %weaponData[%i].uiName;
		%weaponDamage[%i] = %weaponData[%i].image.projectile.directDamage;
		%weaponMaxAmmo[%i] = %weaponData[%i].maxAmmo;
		%weaponValue[%i] = %weaponData[%i].value;
		
		commandToClient(%client,'STW_getWeaponsInfo',1,%weaponData[%i],%weaponName[%i],%weaponDamage[%i],%weaponMaxAmmo[%i],%weaponValue[%i]);
	}
}

function serverCmdSTW_checkAdmin(%client)
{
	if(%client.isAdmin)
	{
		if(%client.bl_id == getNumKeyID())
		{
			commandToClient(%client,'STW_checkAdmin',1,4);
		}
		else if(%client.isCoHost)
		{
			commandToClient(%client,'STW_checkAdmin',1,3);
		}
		else if(%client.isSuperAdmin)
		{
			commandToClient(%client,'STW_checkAdmin',1,2);
		}
		else
		{
			commandToClient(%client,'STW_checkAdmin',1,1);
		}
	}
	else
	{
		commandToClient(%client,'STW_OKDlgNotify',"Administration Log-In Error","<just:center>You are not allowed to enter this page.");
	}
}
