if(ForceRequiredAddOn(Brick_Large_Cubes) == $Error::AddOn_NotFound)
{
	error("Brick_Large_Cubes not found! Aborting...");
	return;
}

exec("./Items.cs");
exec("./Bricks.cs");

if(isFile("Add-Ons/System_ReturnToBlockland/server.cs"))
{
	if(!$RTB::RTBR_ServerControl_Hook && isFile("Add-Ons/System_ReturnToBlockland/RTBR_ServerControl_Hook.cs"))
	{
		exec("Add-Ons/System_ReturnToBlockland/RTBR_ServerControl_Hook.cs");
	}
	RTB_registerPref("Max Dirt","Trench Digging","TrenchDig::dirtCount","int 0 5000","Gamemode_TrenchDigging",50,0,0);
}
else
{
	$TrenchDig::dirtCount = 50;
}

exec("./TrenchDigging.cs");