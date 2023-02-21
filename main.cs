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
//|                       Main Functionality                       |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//OS VARS FOR OS CREATION
$STW::Gamemode::OS::Filepath = "config/server/Snow Trench Wars/OS.cs";
$STW::Gamemode::OS::DefaultOS = "add-ons/gamemode_snowtrenchwars/default_os.txt";
warn("STW Operation sSystem Filepath: "@$STW::Gamemode::OS::Filepath);
warn("STW Operations System Default OS: "@$STW::Gamemode::OS::DefaultOS);
$STW::Gamemode::OS::Name = "Snow Trench Wars Operations System";
$STW::Gamemode::OS::ShortName = "STW[OS]";
$STW::Gamemode::OS::StartMessage = "<color:FFFF00>"@$STW::Gamemode::OS::ShortName@"<color:FFFFFF>: ";

if(isObject(STWOS))
{
	STWOS.delete();
}

fileCopy($STW::Gamemode::OS::DefaultOS,$STW::Gamemode::OS::Filepath);
exec($STW::Gamemode::OS::Filepath);

//MORE FILE EXECUTION
warn("STW - Main Functionality File Execution");

if(!$STW::Gamemode::WeaponData::Executed)
{
	$STW::Gamemode::WeaponData::Executed = 1;
}

exec("./Inv & TS System/inv_main.cs");
exec("./Inv & TS System/ts_main.cs");
exec("./HUD/main.cs");
exec("./PBS/pbs.cs");
exec("./client/main.cs");
exec("./Player Datablocks/player_Datablocks.cs");
exec("./onJoinLeave.cs");
exec("./chat.cs");
exec("./rules.cs");
exec("./profiling.cs");
exec("./support_ai.cs");
exec("./support_Inc_Dec_Functions.cs");
exec("./Admin_Functions.cs");
exec("./clans.cs");
exec("./Level System/main.cs");
exec("./support_slayer_cp.cs");
exec("./Reporting & Suggesting/Report.cs");
exec("./Reporting & Suggesting/Suggest.cs");
STW_SavePrefs();

function serverCmdSTW_DumpDirt(%client,%amount)
{
}

//SOUND TO CLIENT FUNCTIONS
function STW_PlayChatSound(%client)
{
	if(%client !$= "")
	{
		if(isObject(findclientbyname(%client)))
		{
			commandToClient(findclientbyname(%client),'STW_PlayChatSound');
		}
	}
}