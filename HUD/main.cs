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
//|          Main Functionality for the Client HUD System          |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

function serverCmdSTW_SendHUDStats(%client)
{
	if(isObject(%client.player))
	{
		%player = %client.player;
		
		%health = %player.getDatablock().maxDamage - %player.getDamageLevel();
		%maxhealth = %player.getDatablock().maxDamage;
		%armor = %player.STW_Armor;
		%maxarmor = 200;
		%level = %client.STW_Level;
		%rank = %client.STW_Rank;
		%exp = %client.STW_EXP;
		%reqexp = %client.STW_ReqEXP;
		%money = %client.STW_Money;
		%cps = %client.STW_CPs;
	}
	else
	{
		%health = 0;
		%maxhealth = -1;
		%armor = 0;
		%maxarmor = 200;
		%level = %client.STW_Level;
		%rank = %client.STW_Rank;
		%exp = %client.STW_EXP;
		%reqexp = %client.STW_ReqEXP;
		%money = %client.STW_Money;
		%cps = %client.STW_CPs;
	}
	
	%stats = %health SPC %maxhealth SPC %armor SPC %maxarmor SPC %level SPC %rank SPC %exp SPC %reqexp SPC %money SPC %cps;
	commandToClient(%client,'STW_UpdateHUDStats',1,%stats);
}