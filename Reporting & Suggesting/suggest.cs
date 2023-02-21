$STW::Gamemode::Suggesting::FilePath = "config/server/Snow Trench Wars/Suggestions Inbox";
$STW::Gamemode::Suggesting::FileExt = ".txt";
$STW::Gamemode::Suggesting::PostCount = getFileCount(%filepath @ "/" @ $STW::Gamemode::Suggesting::FileExt);

function serverCmdSTW_makeSuggestion(%client,%subject,%comment,%othertext)
{
	if(%subject $= "" || %comment $= "")
	{
		return;
	}
	if(%subject $= "Other" && %othertext !$= "")
	{
		%other = 1;
	}
	
	$STW::Gamemode::Suggesting::PostCount++;
	%filepath = $STW::Gamemode::Suggesting::FilePath @ "/" @ $STW::Gamemode::Suggesting::PostCount @ "(" @ %client.getBLID() @ ")" @ $STW::Gamemode::Suggesting::FileExt;
	
	%file = new FileObject();
	%file.openForWrite(%filepath);
	%file.writeLine(%client.name);
	%file.writeLine(%client.getBLID());
	%file.writeLine(%subject);
	%file.writeLine(%comment);
	%file.writeLine(getDateTime());
	
	if(%other)
	{
		%file.writeLine(%othertext);
	}
	
	%file.close();
	%file.delete();
	
	//STWOS.logAction(%client,"Suggest",%text);
	//setmodpaths(getmodpaths());
	//STWOS.updateReportPosts();
	commandToClient(%client,'STW_OKDlgNotify',"Suggestion Sent","<just:center>Your suggestion has been sent successfully and will be read by an administrator soon.");
}