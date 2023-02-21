$STW::Gamemode::Reporting::FilePath = "config/server/Snow Trench Wars/Reports Inbox";
$STW::Gamemode::Reporting::FileExt = ".txt";
$STW::Gamemode::Reporting::PostCount = getFileCount(%filepath @ "/" @ $STW::Gamemode::Reporting::FileExt);

function STW_loadReports()
{
}

function serverCmdSTW_makeReport(%client,%subject,%userid,%comment,%othertext)
{
	if(%subject $= "" || %userid $= "" || %comment $= "")
	{
		return;
	}
	if(%subject $= "Other" && %othertext !$= "")
	{
		%other = 1;
	}
	
	$STW::Gamemode::Reporting::PostCount++;
	%filepath = $STW::Gamemode::Reporting::FilePath @ "/" @ $STW::Gamemode::Reporting::PostCount @ "(" @ %client.getBLID() @ ")" @ $STW::Gamemode::Reporting::FileExt;
	
	%file = new FileObject();
	%file.openForWrite(%filepath);
	%file.writeLine(%client.name);
	%file.writeLine(%client.getBLID());
	%file.writeLine(%subject);
	%file.writeLine(%userid);
	%file.writeLine(%comment);
	%file.writeLine(getDateTime());
	
	if(%other)
	{
		%file.writeLine(%othertext);
	}
	
	%file.close();
	%file.delete();
	
	//STWOS.logAction(%client,"Report",%text);
	//setmodpaths(getmodpaths());
	//STWOS.updateReportPosts();
	commandToClient(%client,'STW_OKDlgNotify',"Report Sent","<just:center>Your report has been sent successfully and will be read by an administrator soon.");
}

function serverCmdSTW_sendReportsList(%client)
{
	//for(%i = 1; %i <= $STW::Gamemode::Reporting::PostCount; %i++)
	//{
		//commandToClient(%client,'STW_populateReportsList',1,$STW::Gamemode::Reporting::Post[%i],%i);
	//}
}