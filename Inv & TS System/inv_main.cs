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
//|           Main Functionality for the Inventory System          |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//VARS
$STW::Gamemode::InventorySystem::MaxSlots = "7"; //default is 7 - don't mess with this, you'll most likely break the mod and the client's client mods

exec("./ts_main.cs");
exec("./Store.cs");
exec("./armor/armorRegistry.cs");

//FUNCTIONS
function GameConnection::STW_SaveInventory(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Inventory Slots.txt";
	
	if(isFile(%filepath))
	{
		%file = new FileObject();
		%file.openForWrite(%filepath);
		for(%i = 1; %i <= $STW::Gamemode::InventorySystem::MaxSlots; %i++)
		{
			%file.writeLine("InventorySlot["@%i@"] "@%client.STW_InventorySlot[%i]);
		}
		%file.close();
		%file.delete();
	}
	else
	{
		%client.STW_ResetInventory();
	}
}

function GameConnection::STW_LoadInventory(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Inventory Slots.txt";
	
	if(isFile(%filepath))
	{
		%file = new FileObject();
		%file.openForRead(%filepath);
		for(%i = 1; %i <= $STW::Gamemode::InventorySystem::MaxSlots; %i++)
		{
			%client.STW_InventorySlot[%i] = getWord(%file.readLine(),1);
		}
		%file.close();
		%file.delete();	
	}
	else
	{
		%client.STW_ResetInventory();
	}
}

function GameConnection::STW_ResetInventory(%client)
{
	%filepath = "config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/Inventory Slots.txt";
	
	for(%i = 1; %i <= $STW::Gamemode::InventorySystem::MaxSlots; %i++)
	{
		%client.STW_InventorySlot[%i] = "Empty";
	}
	
	%file = new FileObject();
	%file.openForWrite(%filepath);
	for(%i = 1; %i <= $STW::Gamemode::InventorySystem::MaxSlots; %i++)
	{
		%file.writeLine("InventorySlot["@%i@"] "@%client.STW_InventorySlot[%i]);
	}
	%file.close();
	%file.delete();
}

function GameConnection::STW_addToInventory(%client,%item)
{
	for(%i = 1; %i <= $STW::Gamemode::InventorySystem::MaxSlots; %i++)
	{
		if(%client.STW_InventorySlot[%i] $= %item)
		{
			return messageClient(%client,'',STWOS.startMessage@"You already have that weapon in your inventory.");
		}
		else if(%client.STW_InventorySlot[%i] $= "Empty")
		{			
			%client.STW_InventorySlot[%i] = %item;
			//echo(%client.name SPC %item); //debugging purposes
			return;
		}
	}
}

function GameConnection::STW_removeFromInventory(%client,%slot,%item)
{
	if(isObject(%item))
	{
		%client.STW_InventorySlot[%slot] = "Empty";
	}
}

function serverCmdSTW_SellItem(%client,%invslot)
{
	if(%invslot <= $STW::Gamemode::InventorySystem::MaxSlots && %invslot > 0)
	{
		if(%client.STW_InventorySlot[%invslot] !$= "Empty")
		{
			%item = %client.STW_InventorySlot[%invslot];
			%client.STW_addMoney(%item.Value);
			commandToClient(%client,'STW_getClientStats');
			commandToClient(%client,'STW_getInventory');
			%client.STW_removeFromInventory(%invslot,%client.STW_InventorySlot[%invslot]);
		}
		else
		{
			messageClient(%client,'',STWOS.startMessage@"That inventory slot is empty.");
			commandToClient(%client,'STW_OKDlgNotify',"Inventory Error","<just:center>That inventory slot is empty.");
			return;
		}
	}
}

function serverCmdSTW_EquipItem(%client,%invslot)
{
	if(%invslot <= $STW::Gamemode::InventorySystem::MaxSlots && %invslot > 0)
	{
		if(%client.STW_InventorySlot[%invslot] !$= "Empty")
		{
			%item = %client.STW_InventorySlot[%invslot].getID();
			%slot = -1;
			
			for(%i = 0; %i <= 4; %i++)
			{
				if(%client.player.tool[%i] == 0)
					%slot = %i;

				if(%client.player.tool[%i] == %item)
				{
					commandToClient(%client,'STW_OKDlgNotify',"Tool Slot Clarification","<just:center>You already have that item in one of your tool slots.");
					return;
				}
			}
			if(%slot == -1)
			{
				commandToClient(%client,'STW_OKDlgNotify',"Tool Slots Full","<just:center>You must remove a weapon from one of your tool slots first before you can equip a weapon from your inventory.");
				return;
			}
			
			%client.player.tool[%slot] = %item;
			MessageClient(%client,'MsgItemPickup','',%slot,%item);
			commandToClient(%client,'STW_OKDlgNotify',"Tool Equipped","<just:center>You have equipped the selected weapon from your inventory into one of your tool slots.");
		}
		else
		{
			commandToClient(%client,'STW_OKDlgNotify',"Inventory EMPTY","<just:center>Inventory Slot # "@%invslot@" is empty!");
			return;
		}
	}
}

//===============================================================|
//                                                               |
//                   Data Transfer Functions                     |
//                           {Begin}                             |
//                                                               |
//===============================================================|

//serverCmdSTW_sendInventory() - client obj
//used for sending the client obj it's inventory slots
function serverCmdSTW_sendInventory(%client)
{
	for(%i = 1; %i <= $STW::Gamemode::InventorySystem::MaxSlots; %i++)
	{
		%weaponData[%i] = %client.STW_InventorySlot[%i];
		if(%weaponData[%i] !$= "Empty")
		{
			%weaponName[%i] = %weaponData[%i].uiName;
			%weaponDamage[%i] = %weaponData[%i].image.projectile.directDamage;
			%weaponMaxAmmo[%i] = %weaponData[%i].maxAmmo;
			%weaponValue[%i] = %weaponData[%i].value;
		}
		else
		{
			%weaponName[%i] = "Empty";
			%weaponDamage[%i] = 0;
			%weaponMaxAmmo[%i] = 0;
			%weaponValue[%i] = 0;
		}
		commandToClient(%client,'STW_getInventory',1,%weaponData[%i],%weaponName[%i],%weaponDamage[%i],%weaponMaxAmmo[%i],%weaponValue[%i],%i);
	}
}