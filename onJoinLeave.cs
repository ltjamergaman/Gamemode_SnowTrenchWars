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
//|      Main Functionality for Client Joining/Leaving System      |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

package STW_onJoinLeave
{
	function GameConnection::autoAdminCheck(%client)
	{
        //Identification creation & loading
        if(!isFile("config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/ID.txt"))
        {
            %client.STW_ID = getRandomString(5)@"-"@getRandomString(4)@"-"@getRandomString(4)@"_"@getRandom(10,99);
            %file = new FileObject();
            %file.openForWrite("config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/ID.txt");
            %file.writeLine(%client.STW_ID);
            %file.close();
            %file.delete();
        }
        else
        {
            %file = new FileObject();
            %file.openForRead("config/server/Snow Trench Wars/Profiles/"@%client.bl_id@"/ID.txt");
            %client.STW_ID = %file.readLine();
            %file.close();
            %file.delete();
        }
		
		echo("\c2STW-:-autoAdminCheck - Client: "@%client.name@" | Looking for client mod...");
		commandToClient(%client,'STW_CheckForClient',%client.STW_ID); //checking if the client has the client mod, if not then they will be kicked when trying to enter the game
		//$STW::ClientCheckTimeout[%client] = %client.schedule(3000,"delete","STWOS +- ERROR: Please download the latest <a:lt-jamergaman-bl.weebly.com/uploads/1/3/7/8/13789132/Client_SnowTrenchWars.zip>Client Mod</a>.");
		echo("\c2STW-:-autoAdminCheck - Starting timeout for client mod check...");
		
		parent::autoAdminCheck(%client);
		
		if(%client.bl_id == $STW::Gamemode::CoHostBLID && $STW::Gamemode::CoHostBLID !$= "")
		{
			serverCmdSTW_makeCoHost(STWOS.getID(),%client);
		}
		
	}
	
	function GameConnection::onClientEnterGame(%client)
	{
        if(!%client.hasSTWClient)
        {
			error("STW-:-onClientEnterGame - Client: "@%client.name@" does not have the client mod, kicking...");
            //%client.delete("STWOS +- ERROR: Please download the latest <a:lt-jamergaman-bl.weebly.com/uploads/1/3/7/8/13789132/Client_SnowTrenchWars.zip>Client Mod</a>."); //since the client could not enter the game without the client mod, the client will be kicked and forced to download the client mod.
        }
        else
        {
			echo("\c2STW-:-onClientEnterGame - Client: "@%client.name@" has the client mod, overriding kick and loading profile...");
			%client.STW_LoadProfile();
			parent::onClientEnterGame(%client);
			serverCmdSTW_sendClientStats(%client);
			serverCmdSTW_SendWeaponsInfo(%client);
			commandToClient(%client,'STW_createHUD');
		}
	}
	
	function GameConnection::onClientLeaveGame(%client)
	{
		//%client.STW_SaveProfile();
		%client.STW_SaveStats();
		%client.STW_SavePlayerData();
		%client.STW_SaveInventory();
		%client.STW_SaveInfo();
		cancel(%client.decMuteTimeLoop);
		return parent::onClientLeaveGame(%client);
	}
	
	function GameConnection::spawnPlayer(%client)
	{
		parent::spawnPlayer(%client);
		if(%client.hasSpawnedOnce)
		{
			//%client.STW_SaveProfile();
		}
		%client.player.STW_Armor = 0;
		serverCmdSTW_SendClientStats(%client);
		serverCmdSTW_SendHUDStats(%client);
	}
};
activatePackage(STW_onJoinLeave);

function serverCmdSTW_HasClient(%client,%check)
{
	//wanna make sure that the client actually has the client mod
    if(%check == %client.STW_ID)
    {
		cancel($STW::ClientCheckTimeout[%client]);
		echo("\c2STW_HasClient - Client: "@%client.name@" was checked for the client mod, and has it-proceeding with onClientEnterGame code...");
        %client.hasSTWClient = 1; //now we can proceed with this client's statistic loading operations
    }
}