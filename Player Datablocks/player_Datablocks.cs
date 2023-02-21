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
//|                       Player Datablocks                        |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

exec("Add-ons/Player_No_Jet/server.cs");
exec("Add-ons/Player_Fuel_Jet/server.cs");

//VARS?

//DATABLOCKS
datablock PlayerData(STWPlayerNormal : PlayerNoJet)
{
	maxDamage = 0.1;
	uiName = "STW Normal Player";
	
	usesTacKnife = 1;
};

datablock PlayerData(STWPlayerFast : STWPlayerNormal)
{
	uiName = "STW Fast Player";
	
	usesTacKnife = 1;
};

datablock PlayerData(STWPlayerHeavy : STWPlayerNormal)
{
	uiName = "STW Heavy Player";
	
	usesTacKnife = 1;
};

datablock PlayerData(STWPlayerFuelJet : STWPlayerNormal)
{
	uiName = "STW Fuel-Jet Player";
	
	usesTacKnife = 1;
};

//datablock PlayerData(STWPlayerDowned : STWPlayerNormal)
//{
//	uiName = "STW Downed Player";
//};

datablock PlayerData(STWPlayerNormalUp : STWPlayerNormal)
{
	uiName = "STW Normal Player UP.";
	
	usesTacKnife = 1;
};

datablock PlayerData(STWPlayerFastUp : STWPlayerFast)
{
	uiName = "STW Fast Player UP.";
	
	usesTacKnife = 1;
};

datablock PlayerData(STWPlayerHeavyUp : STWPlayerHeavy)
{
	uiName = "STW Heavy Player UP.";
	
	usesTacKnife = 1;
};

datablock PlayerData(Lt_Jamergaman : PlayerStandardArmor)
{
	maxDamage = 100 * 100 * 100 * 100 * 100; //10 billion health
	
	uiName = "Lt. Jamergaman's Player-Type";
	preload = false;
	
	usesTacKnife = 1;
};

if($STW::Gamemode::PlayerTypes::Loaded)
{
	transmitDatablocks();
}
else
{
	$STW::Gamemode::PlayerTypes::Loaded = 1;
}