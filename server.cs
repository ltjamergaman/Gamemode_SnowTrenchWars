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
//|                     Add-On File Version: 6                     |\\
//|                               By                               |\\
//|                   Lt. Jamergaman BLID: 23108                   |\\
//|          Do not modify or (re)distribute this add-on!          |\\
//|                      © 2010 Lt. Jamergaman.                    |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//GAMEMODE VERSION
$STW::Gamemode::Version = "6";
warn("STW Gamemode Version: "@$STW::Gamemode::Version);

//EXECUTION OF MAIN FILES
warn("STW - Executing main files");
setModPaths(getModPaths());
exec("./main.cs");

//PRE-MADE VARS
$STW::Gamemode::MaxDirt = "768";
$TrenchDigging::DirtCount = $STW::Gamemode::MaxDirt;
$STW::Gamemode::Server::Password = "6stw_dev";
$Pref::Server::Password = $STW::Gamemode::Server::Password;
$STW::Gamemode::StartingWeapon = "TAssaultRifleItem";

function STW_SavePrefs()
{
	export("$STW::Gamemode*","config/server/Snow Trench Wars/prefs.cs");
}

//PRE-MADE MISC FUNCTIONS
function fcbn(%client)
{
    return findclientbyname(%client);
}

function fpbn(%client)
{
    return findclientbyname(%client).player;
}

function fcbblid(%blid)
{
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		
		if(%client.bl_id == %blid)
		{
			return %client;
		}
	}
}

function getTime()
{
	%hour = getSubStr(getWord(getDateTime(), 1), 0, strPos(getWord(getDateTime(), 1), ":"));
	%minute = getSubStr(getWord(getDateTime(), 1), 3, 2);

	if(%hour >= 12 && %hour != 24)
	{
		if(%hour >= 13)
		{
			%hour -= 12;
		}

		%word = "PM";
	}
	else
	{
		%word = "AM";
	}

	return %hour @ ":" @ %minute SPC %word;
}

function getDate()
{
	return getWord(getDateTime(),0);
}

//SOUND DATABLOCKS
if(isObject(Chat_Beep1))
{
	Chat_Beep1.delete();
}
if(isObject(Chat_Beep2))
{
	Chat_Beep2.delete();
}

new AudioProfile(Chat_Beep1)
{
	fileName = "Add-Ons/Client_SnowTrenchWars/Sounds/Chat_Beep1.wav";
	description = AudioClosest3d;
	preload = false;
};

new AudioProfile(Chat_Beep2 : Chat_Beep1)
{
	fileName = "Add-Ons/Client_SnowTrenchWars/Sounds/Chat_Beep2.wav";
};