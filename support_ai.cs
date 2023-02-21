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
//|           Support functions for killing AI Hole Bots           |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//VARS
$STW::Gamemode::Leveling::EXPBotKillGain = "375"; //default is 375
$STW::Gamemode::Leveling::EXPBotAssistKillGain = "188"; //default is 188
$STW::Gamemode::Leveling::MoneyBotKillGain = "300 600"; //default is 300-600
$STW::Gamemode::Leveling::MoneyBotAssistKillGain = "150 300"; //default is 150-300
////////////
$STW::Gamemode::AI::TriggerData::MSTickPeriod = "150";
$STW::Gamemode::AI::TriggerData::SpawnRate = "1 10";

//FUNCTIONS
package STW_AISupport
{
	//REALLY LONG ASS RE-WRITE
	function Armor::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType)
	{
		//echo(%obj); //used for testing
		%client = %obj.client;
		%attacker = %sourceObject.client;
		//echo(%obj SPC %sourceObject);
		//echo(%this SPC %obj);
		
		if(%attacker.STW_isInClan && isObject(%attacker.STW_Clan))
		{
			%clana = %attacker.STW_Clan;
		}
		if(%client.STW_isInClan && isObject(%client.STW_Clan))
		{
			%clanb = %client.STW_Clan;
		}
		if(%obj.getClassName() $= "AIPlayer" && %obj.isHoleBot)
		{
			%obj = %obj.player;
			
			if($STW::Gamemode::AI::Attacker_[%obj.getID()] !$= "" && $STW::Gamemode::AI::Attacker_[%obj.getID()] != %attacker.player.getID())
			{
				//echo($STW::Gamemode::AI::Attacker_[%obj.getID()]);
				//echo($STW::Gamemode::AI::Attacker_[%obj.getID()].client.name);
				
				if($STW::Gamemode::AI::AssistAttacker_[%obj.getID()] != %attacker.player.getID())
				{
					$STW::Gamemode::AI::AssistAttacker_[%obj.getID()] = %attacker.player.getID();
					//echo($STW::Gamemode::AI::AssistAttacker_[%obj.getID()]);
					//echo($STW::Gamemode::AI::AssistAttacker_[%obj.getID()].client.name);
				}
			}
			else
			{
				$STW::Gamemode::AI::Attacker_[%obj.getID()] = %attacker.player.getID();
				//echo($STW::Gamemode::AI::Attacker_[%obj.getID()]);
				//echo($STW::Gamemode::AI::Attacker_[%obj.getID()].client.name);
			}
			if(%obj.STW_Armor > 0)
			{
				%obj.STW_Armor -= %damage;
			}
			else
			{
				return parent::damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
			}
		}
		else if(%clana !$= "" && %clanb !$= "")
		{
			if(%clana == %clanb)
			{
				if(%clana.isTeam)
				{
					if(%clana.FriendlyFire)
					{
						if(%client.player.STW_Armor > 0)
						{
							%client.player.STW_Armor -= %damage;
							commandToClient(%client,'STW_UpdateHUDStats',0);
							commandToClient(%client,'STW_getClientStats');
							return;
						}
						else
						{
							parent::damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
							commandToClient(%client,'STW_UpdateHUDStats',0);
							commandToClient(%client,'STW_getClientStats');
							return;
						}
					}
				}
				else
				{
					if(%client.player.STW_Armor > 0)
					{
						%client.player.STW_Armor -= %damage;
						commandToClient(%client,'STW_UpdateHUDStats',0);
						commandToClient(%client,'STW_getClientStats');
						return;
					}
					else
					{
						parent::damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
						commandToClient(%client,'STW_UpdateHUDStats',0);
						commandToClient(%client,'STW_getClientStats');
						return;
					}
				}
			}
			else
			{
				if(%client.player.STW_Armor > 0)
				{
					%client.player.STW_Armor -= %damage;
					commandToClient(%client,'STW_UpdateHUDStats',0);
					commandToClient(%client,'STW_getClientStats');
					return;
				}
				else
				{
					parent::damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
					commandToClient(%client,'STW_UpdateHUDStats',0);
					commandToClient(%client,'STW_getClientStats');
					return;
				}
			}
		}
		else 
		{
			if(%client.player.STW_Armor > 0)
			{
				%client.player.STW_Armor -= %damage;
				commandToClient(%client,'STW_UpdateHUDStats',0);
				commandToClient(%client,'STW_getClientStats');
				return;
			}
			else
			{
				parent::damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
				commandToClient(%client,'STW_UpdateHUDStats',0);
				commandToClient(%client,'STW_getClientStats');
				return;
			}
		}
	}
	//FINALLY FRICKIN' ENDS
	
	function Armor::onDisabled(%this,%obj,%enabled)
	{
		//echo("//////////////////////////////");
		//echo(%obj.getID()); //used for testing
		//echo($STW::Gamemode::AI::Attacker_[%obj.getID()]);
		//echo($STW::Gamemode::AI::AssistAttacker_[%obj.getID()]);
		
		if($STW::Gamemode::AI::Attacker_[%obj.getID()] !$= "") //making sure the bot was attacked by a client
		{
			%money = getRandom(getWord($STW::Gamemode::Leveling::MoneyBotKillGain,0),getWord($STW::Gamemode::Leveling::MoneyBotKillGain,1));
			%exp = $STW::Gamemode::Leveling::EXPBotKillGain;
			%assistMoney = getRandom(getWord($STW::Gamemode::Leveling::MoneyBotAssistKillGain,0),getWord($STW::Gamemode::Leveling::MoneyBotAssistKillGain,1));
			%assistEXP = $STW:Gamemode::Leveling::EXPBotAssistKillGain;
			
			%killer = $STW::Gamemode::AI::Attacker_[%obj.getID()].client.getID(); //%killer is defined as the attacker of the bot
			//messageClient(%killer,'',"<just:center>\c6You have killed \c0"@%obj.name@"<just:left>"); //sends a message to the client in the center top part of the screen "(white)You have killed (red)<botnamehere>"
			messageAll('',"\c0"@%killer.name@" killed "@%obj.name);
			%killer.STW_addMoney(%money,2); //gives %killer a random amount of money
			%killer.STW_addEXP(%exp); //gives %killer exp
			%killer.incKills(1);
			commandToClient(%killer,'STW_UpdateHUDStats',0);
			commandToClient(%killer,'STW_getClientStats');
			commandToClient(%killer,'STW_TabNotify',"You have killed <color:FF0000>" @ %obj.name @ "<br><color:FFFFFF>+<color:FFFF00>$" @ %money @ " & " @ %EXP SPC "EXP");
			
			if($STW::Gamemode::AI::AssistAttacker_[%obj.getID()] !$= "") //making sure if there is an assist attacker or not
			{
				%assistKiller = $STW::Gamemode::AI::AssistAttacker_[%obj.getID()].client.getID(); //%assistKiller is defined as the assist attacker of the bot
				//messageClient(%assistKiller,'',"<just:center>\c6You have assisted \c2"@%killer.name@" \c6in killing \c0"@%obj.name@"<just:left>"); //sends a message to the client in the center top part of the screen "(white)You have assisted (green)<attackername> (white) in killing (red)<botnamehere>"
				messageAll('',"\c0"@%assistKiller.name@" has assisted "@%killer.name@" in killing "@%obj.name);
				%assistKiller.STW_addMoney(%assistMoney,2); //gives %assistKiller a random amount of money
				%assistKiller.STW_addEXP(%assistEXP); //gives %assistKiller exp
				commandToClient(%assistkiller,'STW_UpdateHUDStats',0);
				commandToClient(%assistkiller,'STW_getClientStats');
				commandToClient(%assistKiller,'STW_TabNotify',"Assisted in killing <color:FF0000>" @ %obj.name @ "<br><color:FFFFFF>+<color:FFFF00>$" @ %assistMoney @ " & " @ %assistEXP SPC "EXP");
			}
		}
		
		$STW::Gamemode::AI::Attacker_[%obj.getID()] = ""; //resets %obj's attacker (not like it would matter that much since %obj will be deleted
		$STW::Gamemode::AI::AssistAttacker_[%obj.getID()] = ""; //resets %obj's assist attacker (not like it would matter that much since %obj will be deleted
		deleteVariables("$STW::Gamemode::AI::*");
		
		return Parent::onDisabled(%this,%obj,%enabled); //returns the parent of the function
	}
};
activatePackage(STW_AISupport);

datablock triggerData(ZombieSpawnArea)
{
	tickPeriodMS = $STW::Gamemode::AI::TriggerData::MSTickPeriod;
};

function ZombieSpawnArea::onEnterTrigger(%this,%trigger,%obj)
{
	messageClient(%obj.client,'',"You've entered the trigger!");
	parent::onEnterTrigger(%this,%trigger,%obj);
}

function ZombieSpawnArea::onLeaveTrigger(%this,%trigger,%obj)
{
	messageClient(%obj.client,'',"You've left the trigger!");
	parent::onLeaveTrigger(%this,%trigger,%obj);
}

function ZombieSpawnArea::onTickTrigger(%this,%trigger,%obj)
{
	%obj.client.centerPrint("You are in the trigger.",1);
	parent::onTickTrigger(%this,%trigger,%obj);
}

if(isObject("ZombieSpawnTrigger"))
{
	ZombieSpawnTrigger.delete();
}

//new trigger("ZombieSpawnTrigger")
//{
//	position = "0 0 0";
//	rotation = "1 0 0 0";
//	scale = "100 100 100 100";
//	datablock = "ZombieSpawnArea";
//	polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
//};