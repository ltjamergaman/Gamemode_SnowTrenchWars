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
//|                Main Functionality for the Store                |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//============================================//
//                                            //
// Snow Trench Wars Weapon Manager Stuff Here //
//                                            //
//============================================//

//Save Direction
$STW::Gamemode::WeaponManager::SaveDir = "config/server/Snow Trench Wars/Weapon Manager/this.cs";

//Object Creation
if(!isObject(STW_WeaponManager))
{
	new ScriptObject("STW_WeaponManager")
	{
		name = "Snow Trench Wars Weapon Manager";
		isSTWOS = 1;
		weaponCount = 0;
		ARCount = 0;
		SMGCount = 0;
		LMGCount = 0;
		ShotgunCount = 0;
		SniperRifleCount = 0;
		PistolCount = 0;
		ExplosiveCount = 0;
		MeleeCount = 0;
	};
}

//==================================//
//        STWWM Functions           //
//==================================//

function STW_WeaponManager::AddWeaponData(%this,%item)
{
	for(%i = 1; %i <= %this.weaponCount; %i++)
	{
		if(%this.weaponData[%i] $= %item)
		{
			return;
		}
	}
	
	%this.weaponCount++;
	%this.weaponData[%this.weaponCount] = %item;
	%this.save($STW::Gamemode::WeaponManager::SaveDir);
	echo("\c5NEW ITEM >>>>>>>>" NL %item SPC "AR" SPC %item.isAR SPC "SMG" SPC %item.isSMG SPC "LMG" SPC %item.isLMG SPC "Shotgun" SPC %item.isShotgun SPC "SR" SPC %item.isSniperRifle SPC "Pistol" SPC %item.isPistol SPC "Expl." SPC %item.isExplosive SPC "Melee" SPC %item.isMelee);
}

//================================//
//                                //
//      STORE FUNCTIONALITY       //
//                                //
//================================//

function serverCmdSTW_BuyItem(%client,%itemData)
{
	%client.STW_Money = mFloatLength(%client.STW_Money,0);
	if(%itemData $= "")
	{
		return;
	}
	else if(getSubStr(%itemData,4,5) $= "armor")
	{
		//%itemData = "STW_"@%itemData;
		%armorType = %itemData.type;
		%armorValue = %itemData.value;
		%armorAmount = %itemData.amount;
		%armorName = %itemData.name;
		
		if(%client.STW_Money < %armorValue)
		{
			messageClient(%client,'',STWOS.startMessage@"You have unsufficient funds.");
			commandToClient(%client,'STW_OKDlgNotify',"Store","<just:center>You have unsufficient funds.");
			return;
		}
		
		%client.player.STW_Armor = %armorAmount;
		messageClient(%client,'',STWOS.startMessage@"You have bought \c4"@%armorName@"\c3.");
		%client.STW_decMoney(%armorValue);
		commandToClient(%client,'STW_getClientStats');
		commandToClient(%client,'STW_UpdateHUDStats');
		commandToClient(%client,'STW_OKDlgNotify',"Warning","<just:center>Please remember that armor will not save when you leave the game or when you die.");
		STWOS.Store["TotalMoneySpent"] += %armorValue;
		STWOS.Store["TotalMoneySpent"] = mFloatLength(STWOS.Store["TotalMoneySpent"]);
	
		return;
	}
	else if(getSubStr(%itemData,strLen(%itemData)-4,4) !$= "Item")
	{
		return;
	}
		
	
	%slot = -1;
	%item = %itemData.getID();
	for(%x = 0; %x <= 4; %x++)
	{
		if(%client.player.tool[%x] == 0)
			%slot = %x;

		if(%client.player.tool[%x] == %item || %client.inventorySlot[%x] == %item)
		{
			MessageClient(%client,'',"You already have a(n) "@%itemData.uiName@". If it's not in your Tool Slots, check your inventory. (say /inventory)");
			return;
		}
	}
	for(%i = 1; %i <= 7; %i++)
	{
		if(%client.STW_InventorySlot[%i] $= %itemData)
		{
			return messageClient(%client,'',STWOS.startMessage@"You already have a(n) "@%itemData.uiName@" in your inventory.");
		}
	}
	if(%slot == -1)
	{
		MessageClient(%client,'',"Your Tool Slots are full. Try removing a weapon from one of your Tool Slots.");
		return;
	}
	if(%client.STW_Money < %itemData.value)
	{
		messageClient(%client,'',STWOS.startMessage@"You have unsufficient funds.");
		commandToClient(%client,'STW_OKDlgNotify',"Store","<just:center>You have unsufficient funds.");
		return;
	}
	
	%client.STW_decMoney(%itemData.value);
	%client.player.tool[%slot] = %item;
	%client.STW_addToInventory(%itemData);
	commandToClient(%client,'STW_getClientStats');
	commandToClient(%client,'STW_UpdateHUDStats');
	commandToClient(%client,'STW_getToolSlots');
	MessageClient(%client,'MsgItemPickup','',%slot,%item);
	STWOS.Store["TotalMoneySpent"] += %itemData.value;
	STWOS.Store["TotalMoneySpent"] = mFloatLength(STWOS.Store["TotalMoneySpent"]);
}

function serverCmdSTW_RemoveItem(%client,%slot)
{
	if(!isObject(%client.player))
	{
		commandToClient(%client,'STW_OKDlgNotify',"Data Transfer Error","<just:center>Sorry but we could not recieve what items you had/have in your tool slots because you currently do not have an existant player object.<br><color:FF0000>ERR A.2.b_Cx0");
		return;
	}
	
	serverCmdDropTool(%client,%slot);
	commandToClient(%client,'STW_getToolSlots');
}

//============================//
//   TRANSFER DATA FUNCTIONS  //
//============================//

function serverCmdSTW_populateItemList(%client,%category)
{
	//%category -
	//1 = ARs
	//2 = SMGs
	//3 = LMGs
	//4 = Shotguns
	//5 = Snipers
	//6 = Pistols
	//7 = Armor
	//8 = PlayerTypes
	//9 = Research/Upgrades
	//10 = ???
	
	switch$(%category)
	{
		case 1:
		for(%i = 1; %i <= STW_WeaponManager.weaponCount; %i++)
		{
			%weaponData[%i] = STW_WeaponManager.weaponData[%i];
			if(%weaponData[%i].isAR)
			{
				%a++;
				commandToClient(%client,'STW_PopulateItemList',%category,1,%weaponData[%i].uiname,%weaponData[%i].value,%weaponData[%i],%a);
			}
		}
		
		case 2:
		for(%i = 1; %i <= STW_WeaponManager.weaponCount; %i++)
		{
			%weaponData[%i] = STW_WeaponManager.weaponData[%i];
			if(%weaponData[%i].isSMG)
			{
				%a++;
				commandToClient(%client,'STW_PopulateItemList',%category,1,%weaponData[%i].uiname,%weaponData[%i].value,%weaponData[%i],%a);
			}
		}
		
		case 3:
		for(%i = 1; %i <= STW_WeaponManager.weaponCount; %i++)
		{
			%weaponData[%i] = STW_WeaponManager.weaponData[%i];
			if(%weaponData[%i].isLMG)
			{
				%a++;
				commandToClient(%client,'STW_PopulateItemList',%category,1,%weaponData[%i].uiname,%weaponData[%i].value,%weaponData[%i],%a);
			}
		}
		
		case 4:
		for(%i = 1; %i <= STW_WeaponManager.weaponCount; %i++)
		{
			%weaponData[%i] = STW_WeaponManager.weaponData[%i];
			if(%weaponData[%i].isShotgun)
			{
				%a++;
				commandToClient(%client,'STW_PopulateItemList',%category,1,%weaponData[%i].uiname,%weaponData[%i].value,%weaponData[%i],%a);
			}
		}
		
		case 5:
		for(%i = 1; %i <= STW_WeaponManager.weaponCount; %i++)
		{
			%weaponData[%i] = STW_WeaponManager.weaponData[%i];
			if(%weaponData[%i].isSniperRifle)
			{
				%a++;
				commandToClient(%client,'STW_PopulateItemList',%category,1,%weaponData[%i].uiname,%weaponData[%i].value,%weaponData[%i],%a);
			}
		}
		
		case 6:
		for(%i = 1; %i <= STW_WeaponManager.weaponCount; %i++)
		{
			%weaponData[%i] = STW_WeaponManager.weaponData[%i];
			if(%weaponData[%i].isPistol)
			{
				%a++;
				commandToClient(%client,'STW_PopulateItemList',%category,1,%weaponData[%i].uiname,%weaponData[%i].value,%weaponData[%i],%a);
			}
		}
		
		case 7:
		for(%i = 0; %i < $STW::Gamemode::Store::Item::ArmorCount; %i++)
		{
			%weaponData[%i] = STW_ArmorManager.getObject(%i);
			%a++;
			commandToClient(%client,'STW_PopulateItemList',%category,1,%weaponData[%i].name,%weaponData[%i].value,%weaponData[%i].getName(),%a);
		}
		
		case 8:
		//blah
		
		case 9:
		//blah
		
		case 10:
		//blah
	}
}