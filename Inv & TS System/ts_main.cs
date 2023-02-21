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
//|          Main Functionality for the Tool Slots System          |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


package STW_ToolSlots
{
	function serverCmdDropTool(%client,%slot)
	{
		if(!isObject(%client.player))
		{
			return;
		}
		
		%client.player.tool[%slot] = 0;
		%client.player.weaponCount--;
		messageClient(%client,'MsgItemPickup',"",%slot,0);
		//commandToClient(%client,'centerPrint',"\c3You have dropped the item that you were currently holding.",4);

		if(%client.player.currTool == %slot)
		{
			%client.player.updateArm(0);
			%client.player.unMountImage(0);
		}
	}
};
activatePackage(STW_ToolSlots);

function serverCmdSTW_sendToolSlots(%client)
{
	if(!isObject(%client.player))
	{
		commandToClient(%client,'STW_OKDlgNotify',"Data Transfer Error","<just:center>Sorry but we could not recieve what items you had/have in your tool slots because you currently do not have an existant player object.<br><color:FF0000>ERR A.2.b_Cx0");
		return;
	}
	
	%player = %client.player;
	
	for(%i = 0; %i < 5; %i++)
	{
		if(%player.tool[%i] $= "" || %player.tool[%i] $= "0")
		{
			%itemData = "Empty";
			%itemName = "Empty";
			%itemValue = 0;
		}
		else
		{
			%itemData = %player.tool[%i].getName();
			%itemName = %itemData.uiName;
			%itemValue = %itemData.value;
		}
		
		commandToClient(%client,'STW_getToolSlots',1,%itemData,%itemName,%itemValue,%i);
	}
}