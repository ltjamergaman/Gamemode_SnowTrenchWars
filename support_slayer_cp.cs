//*****************************************************************************
//                                                                            *
// Slayer_TT_onCPCapture is found in Gamemode_Slayer_Territory/TT.cs          *
// fxDtsBrick::setCPControl is found in Gamemode_Slayer/Dependencies/brick.cs *
// This bit of code is for supporting STW's CP system                         *
//                                                                            *
//*****************************************************************************

package STW_SupportSlyrCP
{
	//function Slayer_TT_onCPCapture(%mini,%brick,%color,%old_color,%client)
	//{
	//	parent::Slayer_TT_onCPCapture(%mini,%brick,%color,%old_color,%client);
	//	%client.STW_AddCP(1,1);
	//}
	
	function fxDtsBrick::setCPControl(%this,%color,%reset,%client)
	{
		parent::setCPControl(%this,%color,%reset,%client);
		
		if(isObject(%client))
		{
			%mini = getMinigameFromObject(%client);
			
			if(!isSlayerMinigame(%mini))
				return;
			if(!minigameCanUse(%client,%this))
				return;
			
			if(!%reset)
				%client.STW_AddCP(1,1);
		}
	}
};
activatePackage(STW_SupportSlyrCP);