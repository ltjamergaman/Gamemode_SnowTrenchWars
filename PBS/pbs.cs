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
//|                           Version: 2                           |\\
//| Main Functionality for the Random-Player-Boost-Setback System  |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

//DEFAULT VARS (none yet)
$STW::Gamemode::PBS::LoopTime = 15000*10; //default is 15000*10
$STW::Gamemode::PBS::Timeout = 15000*10;
$STW::Gamemode::PBS::Message = ""; //!do not change!
$STW::Gamemode::PBS::Buff = ""; //!do not change!
$STW::Gamemode::PBS::Div[0] = "/"; //!do not change!
$STW::Gamemode::PBS::Div[1] = "*"; //!do not change!
$STW::Gamemode::PBS::Div[2] = "+"; //!do not change!
$STW::Gamemode::PBS::Div[3] = "-"; //!do not change!
$STW::Gamemode::PBS::BuffMin = 2; //default is 2
$STW::Gamemode::PBS::BuffMax = 250; //default is 250
$STW::Gamemode::PBS::BS[0] = "EXP"; //!do not change!
$STW::Gamemode::PBS::BS[1] = "Money"; //!do not change!

//STW OS
new ScriptObject("STW_PBS")
{
	name = "Snow Trench Wars Player Boost Setback System";
	isSTWOS = 1;
	STW_ID = "23712618916"; //23 7 1 26 18 9 16
	Div = "";
	Buff = "";
	BS = "";
	Message = "";
	//TimeLeft = "";
};

function STW_setBuff()
{
	STW_PBS.Div = "";
	STW_PBS.Buff = "";
	STW_PBS.BS = "";
	STW_PBS.Message = "";
	
	%ranDiv = getRandom(0,3);
	%ranBuff = getRandom($STW::Gamemode::PBS::BuffMin,$STW::Gamemode::PBS::BuffMax);
	%ranBS = getRandom(0,1);
	
	if(%ranDiv == 0)
	{
		%partMsg = "\c0Setback \c0Divided \c3EXP \c3Money";
	}
	if(%ranDiv == 1)
	{
		%partMsg = "\c2Boost \c2Multiplied \c3EXP \c3Money";
	}
	if(%ranDiv == 2)
	{
		%partMsg = "\c2Boost \c2Added \c3EXP \c3Money";
	}
	if(%ranDiv == 3)
	{
		%partMsg = "\c0Setback \c0Subtracted \c3EXP \c3Money";
	}
	
	if(%ranBS == 0)
	{
		%otherMsg = getWord(%partMsg,2); //EXP
	}
	if(%ranBS == 1)
	{
		%otherMsg = getWord(%partMsg,3); //Money
	}
	
	%message = getWord(%partMsg,0) SPC "\c6-" SPC %otherMsg SPC "\c6has been" SPC getWord(%partMsg,1) SPC "\c6by" SPC %ranBuff SPC "\c6per kill.";
	$STW::Gamemode::PBS::Message = %message;
	$STW::Gamemode::PBS::Buff = %ranBuff;
	
	STW_PBS.Div = $STW::Gamemode::PBS::Div[%ranDiv];
	STW_PBS.Buff = $STW::Gamemode::PBS::Buff;
	STW_PBS.BS = getWord(%partMsg,0);
	STW_PBS.Message = $STW::Gamemode::PBS::Message;
}

function STW_loopPBS()
{
    STW_cancelPBS();
	$STW::Gamemode::PBS::TimeoutSched = schedule($STW::Gamemode::PBS::Timeout,0,"STW_startPBS");
}

function STW_startPBS()
{
	cancel($STW::Gamemode::PBS::LoopSched);
	cancel($STW::Gamemode::PBS::TimeoutSched);
	STW_setBuff();
	STWOS.sendMessage("I am choosing a \c3Player \c2Boost\c6/\c0Setback\c6...");
	STWOS.sendMessage($STW::Gamemode::PBS::Message);
    $STW::Gamemode::PBS::LoopSched = schedule($STW::Gamemode::PBS::LoopTime,0,"STW_loopPBS");
}

function STW_cancelPBS()
{
	cancel($STW::Gamemode::PBS::LoopSched);
	cancel($STW::Gamemode::PBS::TimeoutSched);
	STW_PBS.Div = "";
	STW_PBS.Buff = "";
	STW_PBS.BS = "";
	STW_PBS.Message = "";
	$STW::Gamemode::PBS::Message = "";
	$STW::Gamemode::PBS::Buff = "";
	STWOS.sendMessage("The buff has expired.");
}