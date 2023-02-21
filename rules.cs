$STW::Gamemode::Ruling::Filepath = "Add-ons/Gamemode_SnowTrenchWars/Rules";
$STW::Gamemode::Ruling::FileExt = ".txt";
$STW::Gamemode::Ruling::RuleCount = 0; //getFileCount("Add-ons/Gamemode_SnowTrenchWars/Rules");

function STW_loadRules()
{
	%filepath = $STW::Gamemode::Ruling::Filepath;
	%ext = $STW::Gamemode::Ruling::FileExt;
	deleteVariables("$STW::Gamemode::Ruling::R*");
	
	for(%i = getFileCount(%filepath @ "/*.txt"); %i >= 1; %i--)
	{
		if(%fileFile $= "")
		{
			%fileFile = findFirstFile(%filepath @ "/*" @ %ext);
		}
		if(%fileFile $= "")
		{
			error("STW_loadRules() - There are no rules in the rules directory to load!");
			return;
		}
		
		//echo(%filefile);
		%file = new FileObject();
		%file.openForRead(%fileFile);
		%text = %file.readLine();
		//echo(%text);
		%file.close();
		%file.delete();
		$STW::Gamemode::Ruling::RuleCount++;
		$STW::Gamemode::Ruling::Rule[$STW::Gamemode::Ruling::RuleCount] = %text;
		%prevFile = %fileFile;
		%fileFile = findNextFile(%filepath @ "/*" @ %ext);
		
		//echo(%filefile);
		//echo(%prevFile);
		
		if(%fileFile $= %prevFile)
		{
			error("STW_loadRules() - There is only one rule in the rules directory to load, cancelling...");
			return;
		}
		if(%message !$= "")
		{
			%message = %message SPC "#" SPC $STW::Gamemode::Ruling::RuleCount@"...";
		}
		else
		{
			%message = "\c4STW_loadRules() - Loading rule # "@$STW::Gamemode::Ruling::RuleCount@"...";
		}
		
	}
	
	echo(%message);
	echo("STW_loadRules() - Rule(s) loaded!");
}

function serverCmdSTW_sendRulesList(%client)
{
	warn("serverCmdSTW_sendRulesList() - Client: "@%client.name@" is loading the rules list...");
	STW_loadRules();
	
	for(%i = 1; %i <= $STW::Gamemode::Ruling::RuleCount; %i++)
	{
		commandToClient(%client,'STW_populateRulesList',1,%i TAB $STW::Gamemode::Ruling::Rule[%i],%i);
	}
}

function serverCmdSTW_addNewRule(%client,%text)
{
	if(!%client.isSuperAdmin)
	{
		commandToClient(%client,'STW_OKDlgNotify',"Authority Clarification Failed","<just:center>You do not have the proper authority permission to use this piece of functionality. If this is a problem to you, please contact someone of higher authority.");
		return;
	}
	
	%rule = %text;
	$STW::Gamemode::Ruling::RuleCount++;
	%filepath = $STW::Gamemode::Ruling::Filepath @ "/Rule_" @ $STW::Gamemode::Ruling::RuleCount @ $STW::Gamemode::Ruling::FileExt;
	
	%file = new FileObject();
	%file.openForWrite(%filepath);
	%file.writeLine(%text);
	%file.close();
	%file.delete();
	
	STWOS.logAdminAction(%client,"Rule",%text);
	setmodpaths(getmodpaths());
	commandToClient(%client,'STW_populateRulesList');
}

function serverCmdSTW_removeRule(%client,%rule)
{
}

function serverCmdSTW_modifyRule(%client,%rule,%text)
{
}