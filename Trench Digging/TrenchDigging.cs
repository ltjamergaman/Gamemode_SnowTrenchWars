function LilDebug(%msg)
{
	if($LilDebug == 1)
	{
		echo(%msg);
	}
}

// ______________
//|              |
//| Server Cmd's |
//|______________|

function serverCmdSpeedDig(%client)
{
	if(%client.isAdmin || %client.isSuperAdmin)
	{
		%client.player.updateArm(AdminShovelImage,0);
		%client.player.mountImage(AdminShovelImage,0);
	}
}
function serverCmdSpeedPlace(%client)
{
	if(%client.isAdmin)
	{
		%client.player.updateArm(AdminDirtImage,0);
		%client.player.mountImage(AdminDirtImage,0);
	}
}
function serverCmdInfiniteDigging(%client)
{
	if(%client.isAdmin)
	{
		if(%client.isInfiniteMiner)
		{
			%client.isInfiniteMiner = 0;
			%client.trenchDirt = %client.oldTrenchDirt;
			messageClient(%client,'',"\c3Infinite Digging is now \c6OFF");
			%client.updateDirt();
		}
		else
		{
			%client.isInfiniteMiner = 1;
			%client.oldTrenchDirt = %client.trenchDirt;
			%client.trenchDirt = "�";
			messageClient(%client,'',"\c3Infinite Digging is now \c6ON");
			%client.updateDirt();
		}
	}
}
function serverCmdDumpDirt(%client)
{
	if(%client.isInfiniteMiner)
	{
		%client.centerPrint("\c3You have infinite digging mode on! Why would you want to dump infinite blocks?",2);
		return;
	}
	%client.dumpedDirt = 0;
	%eyePoint = %client.player.getEyePoint();
	%eyeVector = %client.player.getEyeVector();
	%raycast = containerRayCast(%eyePoint,vectorAdd(%eyePoint,vectorScale(%eyeVector,10)),$TypeMasks::fxBrickObjectType);
	%obj = firstWord(%raycast);
	lilDebug("Fired recast and returned "@%obj);
	if(isObject(%obj))
	{
		%rayPos = posFromRaycast(%raycast);
		%DBName = %obj.getDatablock().getName();
		if(%DBName $= "brick2xCubeDirtData" || %DBName $= "brick4xCubeDirtData" || %DBName $= "brick8xCubeDirtData" || %DBName $= "brick16xCubeDirtData" || %DBName $= "brick32xCubeDirtData" || %DBName $= "brick64xCubeDirtData")
		{
			%Pos = findSmallBrick(%obj,%rayPos,"brick2xCubeDirtData");
			LilDebug("%pos $= "@%pos);

			%Normal = normalFromRaycast(%raycast);
			%Scale = vectorScale(%Normal,1);
			%Pos = vectorSub(vectorAdd(%Pos,%Scale),"0 0 1");

			if(%Pos !$= "")
			{
				lilDebug("%Pos !$= \"\"");
				for(%i=0;%i<4;%i++)
				{
					LilDebug("%i == "@%i);
					switch(%i)
					{
						case 0:
							%checkPos = vectorAdd(%Pos,"0.5 0.5 1.5");
						case 1:
							%checkPos = vectorAdd(%Pos,"0.5 -0.5 1.5");
						case 2:
							%checkPos = vectorAdd(%Pos,"-0.5 0.5 1.5");
						case 3:
							%checkPos = vectorAdd(%Pos,"-0.5 -0.5 1.5");
					}
					if(getBrickAtPos(%checkPos,"1.5 1.5 1.5") == 0)
					{
						LilDebug("Woo, found pos at "@%checkPos);
						%Add = %checkPos;
						break;
					}
					else
					{
						LilDebug("A brick ("@getBrickAtPos(%checkPos,"1.5 1.5 1.5")@") returned for %i at "@%i@" at pos "@%checkPos);
					}
				}
			}
			else
			{
				lilDebug("Returning?");
				return;
			}
			%wholeBricks = mFloor(%client.trenchDirt / 8);
			%dumpDB = brick4xCubeDirtData;
			%dumpDiv = 8;
			%dumpAdd = 2;
		}
		else if(%DBName $= "brick8x16DirtData" || %DBName $= "brick8x8DirtData" || %DBName $= "brick4x4DirtData" || %DBName $= "brick2x2DirtData" || %DBName $= "brick1x1DirtData")
		{
			%Pos = findSmallBrick(%obj,%rayPos,"brick1x1DirtData");
			LilDebug("%pos = "@%pos);

			%Normal = normalFromRaycast(%raycast);
			%Scale = vectorScale(%Normal,0.5);
			%Pos = vectorSub(vectorAdd(%Pos,%Scale),"0 0 0.6");

			if(%Pos !$= "")
			{
				for(%x=-0.75;%x<=0.75;%x+=0.5)
				{
					for(%y=-0.75;%y<=0.75;%y+=0.5)
					{
						%possiblePos = vectorAdd(%Pos,%x SPC %y SPC "0.6");
						if(getBrickAtPos(%possiblePos,"1.9 1.9 0.5") == 0)
						{
							LilDebug("Woo, found pos at "@%possiblePos);
							%Add = %possiblePos;
							%quickStop = 1;
							break;
						}
						else
						{
							LilDebug("A brick ("@getBrickAtPos(%possiblePos,"1.9 1.9 0.5")@") returned for %x,%y at "@%x SPC %y@" at pos "@%possiblePos);
						}
					}
					if(%quickStop)
					{
						break;
					}
				}
			}
			else
			{
				lilDebug("returning?");
				return;
			}
			%wholeBricks = mFloor(%client.trenchDirt / 16);
			%dumpDB = brick4x4DirtData;
			%dumpDiv = 16;
			%dumpAdd = 0.6;
		}

		%leftOver = %client.trenchDirt - %wholeBricks;
		%newPos = %Add;
		for(%i=1;%i<=%wholeBricks;%i++)
		{
			%colorID = %client.trenchBrick[%client.trenchDirt - 1];
			%brick = new fxDtsBrick()
			{
				position = %newPos;
				datablock = %dumpDB;
				colorId = %colorID;
				colorFxId = 0;
				shapeFxId = 0;
				client = %obj.getGroup().client;
			};
			%brick.isPlanted = 1;
			%brick.setTrusted(1);
			%Error = %brick.plant();
			if(%Error && %Error != 2)
			{
				lilDebug("D: %error == "@%error);
				%brick.delete();
				%stop = 1;
			}
			if(%stop)
			{
				if(%client.dumpedDirt == 0)
				{
					%client.bottomPrint("\c3You can't dump dirt there!",2);
					%client.schedule(2000,updateDirt);
				}
				%client.dumpedDirt = 0;
				break;
			}
			%client.dumpedDirt++;
			%client.trenchDirt -= %dumpDiv;
			%obj.getGroup().add(%brick);
			%newPos = vectorAdd(%newPos,"0 0" SPC %dumpAdd);
			%client.updateDirt();
		}
	}
	else
		lilDebug("Not an object");
}

// ________________
//|                |
//| Main Functions |
//|________________|

function TakeChunk(%client,%take)
{
	%eyePoint = %client.player.getEyePoint();
	%eyeVector = %client.player.getEyeVector();
	%pos = %client.player.getPosition();
	%raycast = containerRayCast(%eyePoint,vectorAdd(%eyePoint,vectorScale(%eyeVector,10)),$TypeMasks::fxBrickObjectType);
	%obj = firstWord(%raycast);
	%rayPos = posFromRaycast(%raycast);
	if(isObject(%obj))
	{
		if(%client.trenchDirt < $TrenchDig::dirtCount)
		{
			if(%obj.getDatablock().isTrenchDirt)
			{
// 				Fixing getting stuck within new bricks
				%raycast2 = containerRayCast(%pos,vectorAdd(%pos,"0 0 -0.1"),$TypeMasks::fxBrickObjectType,%client.player);
				%obj2 = firstWord(%raycast2);
				if(%obj2 == %obj)
				{
					%client.player.addVelocity("0 0 2");
				}
				%obj.refill(%client,%take,%rayPos);
			}
		}
		else
		{
			%client.centerPrint("\c3You do not have enough room for any more dirt!",1);
		}
	}
}
function ShootChunk(%client)
{
	if(%client.trenchDirt <= 0 && !%client.isInfiniteMiner)
	{
		%client.centerPrint("\c3You have no dirt to release!",1);
		return;
	}
	else
	{
		%eyePoint = %client.player.getEyePoint();
		%eyeVector = %client.player.getEyeVector();
		%scale = vectorScale(%eyeVector,10);
		%add = vectorAdd(%eyePoint,%scale);
		%raycast = containerRaycast(%eyePoint,%add,$TypeMasks::FxBrickObjectType);
		%obj = firstWord(%raycast);
		if(isObject(%obj) && %obj.getDatablock().isTrenchDirt)
		{
			%rayPos = posFromRaycast(%raycast);
			%DBName = %obj.getDatablock().getName();
			if(%DBName $= "brick2xCubeDirtData" || %DBName $= "brick4xCubeDirtData" || %DBName $= "brick8xCubeDirtData" || %DBName $= "brick16xCubeDirtData" || %DBName $= "brick32xCubeDirtData" || %DBName $= "brick64xCubeDirtData")
			{
				%Pos = findSmallBrick(%obj,%rayPos,"brick2xCubeDirtData");
				%Diff = vectorSub(%Pos,%rayPos);
				%else = 1;
				switch$(%DBName)
				{
					case "brick2xCubeDirtData":
						if(getWord(%Diff,2) == 1)
						{
							%Add = vectorSub(%Pos,"0 0 1");
							%else = 0;
						}
					case "brick4xCubeDirtData":
						if(getWord(%Diff,2) == 1)
						{
							%Add = vectorSub(%Pos,"0 0 1");
							%else = 0;
						}
					case "brick8xCubeDirtData":
						if(getWord(%Diff,2) == 1)
						{
							%Add = vectorSub(%Pos,"0 0 1");
							%else = 0;
						}
					case "brick16xCubeDirtData":
						if(getWord(%Diff,2) == 1)
						{
							%Add = vectorSub(%Pos,"0 0 1");
							%else = 0;
						}
				}
				if(%else)
				{
					%Normal = normalFromRaycast(%raycast);
					%Scale = vectorScale(%Normal,1);
					%Add = vectorAdd(%Pos,%Scale);
				}
				%colorID = %client.trenchBrick[%client.trenchDirt];
				%brick = new fxDtsBrick()
				{
					position = %Add;
					datablock = brick2xCubeDirtData;
					colorId = %colorID;
					colorFxId = 0;
					shapeFxId = 0;
					client = %obj.getGroup().client;
				};
				%brick.isPlanted = 1;
				%brick.setTrusted(1);
				%Error = %brick.plant();
				if(%Error && %error != 2)
				{
					%brick.delete();
					return;
				}
				%obj.getGroup().add(%brick);
				if(!%client.isInfiniteMiner)
				{
					%client.trenchDirt--;
					%client.updateDirt();
				}
				%brick.checkBricks(%client);
			}
			else if(%DBName $= "brick8x16DirtData" || %DBName $= "brick8x8DirtData" || %DBName $= "brick4x4DirtData" || %DBName $= "brick2x2DirtData" || %DBName $= "brick1x1DirtData")
			{
				%Pos = findSmallBrick(%obj,%rayPos,"brick1x1DirtData");
				%Diff = vectorSub(%Pos,%rayPos);
				if(getWord(%Diff,2) $= "0.3" || getWord(%Diff,2) >= 0.29)
				{
					%Add = getWord(%Pos,0) SPC getWord(%Pos,1) SPC getWord(vectorSub(%Pos,"0 0 0.6"),2);
				}
				else
				{
					%Normal = normalFromRaycast(%raycast);
					%Scale = vectorScale(%Normal,0.5);
					%Add = vectorAdd(%Pos,%Scale);
				}
				%colorID = %client.trenchBrick[%client.trenchDirt];
				%brick = new fxDtsBrick()
				{
					position = %Add;
					datablock = brick1x1DirtData;
					colorId = %colorID;
					colorFxId = 0;
					shapeFxId = 0;
					client = %obj.getGroup().client;
				};
				%brick.isPlanted = 1;
				%brick.setTrusted(1);
				%Error = %brick.plant();
				if(%Error && %error != 2)
				{
					%brick.delete();
					return;
				}
				%obj.getGroup().add(%brick);
				if(!%client.isInfiniteMiner)
				{
					%client.trenchDirt--;
					%client.updateDirt();
				}
				%brick.checkBricks();
			}
		}
	}
}

// ___________________
//|                   |
//| Support Functions |
//|___________________|

function fxDtsBrick::Refill(%this,%client,%take,%rayPos)
{
	if(isObject(%this))
	{
		switch$(%this.getDatablock().getName())
		{
			case "brick64xCubeDirtData":
				%size = "64x";
				%wantSize = "brick32xCubeDirtData";
			case "brick32xCubeDirtData":
				%size = "32x";
				%wantSize = "brick16xCubeDirtData";
			case "brick16xCubeDirtData":
				%size = "16x";
				%wantSize = "brick8xCubeDirtData";
			case "brick8xCubeDirtData":
				%size = "8x";
				%wantSize = "brick4xCubeDirtData";
			case "brick4xCubeDirtData":
				%size = "4x";
				%wantSize = "brick2xCubeDirtData";
			case "brick2xCubeDirtData":
				if(%take)
				{
					onTrenchDig(%client,%pos,%this);
				}
				return;
			case "brick8x16DirtData":
				%size = "8x16";
				%wantSize = "brick8x8DirtData";
			case "brick8x8DirtData":
				%size = "8x8";
				%wantSize = "brick4x4DirtData";
			case "brick4x4DirtData":
				%size = "4x4";
				%wantSize = "brick2x2DirtData";
			case "brick2x2DirtData":
				%size = "2x2";
				%wantSize = "brick1x1DirtData";
			case "brick1x1DirtData":
				if(%take)
				{
					onTrenchDig(%client,%pos,%this);
				}
				return;
			default:
				return;
		}
		%pos = %this.getPosition();
		%colorId = %this.getColorId();
		%colorFxId = %this.getColorFxId();
		%shapeFxId = %this.getShapeFxId();
		%rotation = %this.rotation;
		%smallPos = findSmallBrick(%this,%rayPos,%wantSize);
		%group = %this.getGroup();
		%this.delete();
		schedule(0,0,fillPos,%pos,%size,%client,%colorId,%colorFx,%shapeFx,%rotation,%group);
		schedule(5,0,continueRefill,%client,%take,%rayPos,%smallPos);
	}
}
function continueRefill(%client,%take,%rayPos,%smallPos)
{
	%fillBrick = getBrickAtPos(%smallPos,"0 0 0");
	%fillBrick.Refill(%client,%take,%rayPos);
}
function getBrickAtPos(%pos,%box)
{
	InitContainerBoxSearch(%pos,%box,$TypeMasks::fxBrickObjectType);
	%obj = containerSearchNext();
	if(isObject(%obj))
	{
		return %obj;
	}
	else
	{
		return 0;
	}
}
function fillPos(%pos,%size,%client,%colorId,%colorFx,%shapeFx,%rotation,%brickGroup)
{
	if(%size $= "64x")
	{
		%DB = "brick32xCubeDirtData";
		for(%x=8;%x>=-8;%x-=16)
		{
			for(%y=8;%y>=-8;%y-=16)
			{
				for(%z=8;%z>=-8;%z-=16)
				{
					%newPos = vectorAdd(%pos,%x SPC %y SPC %z);
					%brick = new fxDtsBrick()
					{
						position = %newPos;
						datablock = %DB;
						colorId = %colorId;
						colorFxId = %colorFxId;
						shapeFxId = %shapeFxId;
						client = %brickGroup.client;
					};
					%brick.isPlanted = 1;
					%brick.setTrusted(1);
					%error = %brick.plant();
					if(%error && %error != 2)
					{
						%brick.delete();
					}
					else
					{
						%brickGroup.add(%brick);
					}
				}
			}
		}
	}
	else if(%size $= "32x")
	{
		%DB = "brick16xCubeDirtData";
		for(%x=4;%x>=-4;%x-=8)
		{
			for(%y=4;%y>=-4;%y-=8)
			{
				for(%z=4;%z>=-4;%z-=8)
				{
					%newPos = vectorAdd(%pos,%x SPC %y SPC %z);
					%brick = new fxDtsBrick()
					{
						position = %newPos;
						datablock = %DB;
						colorId = %colorId;
						colorFxId = %colorFxId;
						shapeFxId = %shapeFxId;
						client = %brickGroup.client;
					};
					%brick.isPlanted = 1;
					%brick.setTrusted(1);
					%error = %brick.plant();
					if(%error && %error != 2)
					{
						%brick.delete();
					}
					else
					{
						%brickGroup.add(%brick);
					}
				}
			}
		}
	}
	else if(%size $= "16x")
	{
		%DB = "brick8xCubeDirtData";
		for(%x=2;%x>=-2;%x-=4)
		{
			for(%y=2;%y>=-2;%y-=4)
			{
				for(%z=2;%z>=-2;%z-=4)
				{
					%newPos = vectorAdd(%pos,%x SPC %y SPC %z);
					%brick = new fxDtsBrick()
					{
						position = %newPos;
						datablock = %DB;
						colorId = %colorId;
						colorFxId = %colorFxId;
						shapeFxId = %shapeFxId;
						client = %brickGroup.client;
					};
					%brick.isPlanted = 1;
					%brick.setTrusted(1);
					%error = %brick.plant();
					if(%error && %error != 2)
					{
						%brick.delete();
					}
					else
					{
						%brickGroup.add(%brick);
					}
				}
			}
		}
	}
	else if(%size $= "8x")
	{
		%DB = "brick4xCubeDirtData";
		for(%x=1;%x>=-1;%x-=2)
		{
			for(%y=1;%y>=-1;%y-=2)
			{
				for(%z=1;%z>=-1;%z-=2)
				{
					%newPos = vectorAdd(%pos,%x SPC %y SPC %z);
					%brick = new fxDtsBrick()
					{
						position = %newPos;
						datablock = %DB;
						colorId = %colorId;
						colorFxId = %colorFxId;
						shapeFxId = %shapeFxId;
						client = %brickGroup.client;
					};
					%brick.isPlanted = 1;
					%brick.setTrusted(1);
					%error = %brick.plant();
					if(%error && %error != 2)
					{
						%brick.delete();
					}
					else
					{
						%brickGroup.add(%brick);
					}
				}
			}
		}
	}
	else if(%size $= "4x")
	{
		%DB = "brick2xCubeDirtData";
		for(%x=0.5;%x>=-0.5;%x--)
		{
			for(%y=0.5;%y>=-0.5;%y--)
			{
				for(%z=0.5;%z>=-0.5;%z--)
				{
					%newPos = vectorAdd(%pos,%x SPC %y SPC %z);
					%brick = new fxDtsBrick()
					{
						position = %newPos;
						datablock = %DB;
						colorId = %colorId;
						colorFxId = %colorFxId;
						shapeFxId = %shapeFxId;
						client = %brickGroup.client;
					};
					%brick.isPlanted = 1;
					%brick.setTrusted(1);
					%error = %brick.plant();
					if(%error && %error != 2)
					{
						%brick.delete();
					}
					else
					{
						%brickGroup.add(%brick);
					}
				}
			}
		}
	}
	else if(%size $= "8x16")
	{
		%DB = "brick8x8DirtData";
		for(%x=2;%x>=-2;%x-=4)
		{
			%rot = getWord(%rotation,3);
			if(mCeil(%rot) == 90 || mFloor(%rot) == 90)
			{
				%newPos = vectorAdd(%pos,%x SPC "0 0");
			}
			else
			{
				%newPos = vectorAdd(%pos,"0" SPC %x SPC "0");
			}
			%brick = new fxDtsBrick()
			{
				position = %newPos;
				datablock = %DB;
				colorId = %colorId;
				colorFxId = %colorFxId;
				shapeFxId = %shapeFxId;
				client = %brickGroup.client;
			};
			%brick.isPlanted = 1;
			%brick.setTrusted(1);
			%error = %brick.plant();
			if(%error && %error != 2)
			{
				%brick.delete();
			}
			else
			{
				%brickGroup.add(%brick);
			}
		}
	}
	else if(%size $= "8x8")
	{
		%DB = "brick4x4DirtData";
		for(%z=1;%z>-2;%z-=2)
		{
			for(%x=1;%x>-2;%x-=2)
			{
				%newPos = vectorAdd(%pos,%x SPC %z SPC "0");
				%brick = new fxDtsBrick()
				{
					position = %newPos;
					datablock = %DB;
					colorId = %colorId;
					colorFxId = %colorFxId;
					shapeFxId = %shapeFxId;
					client = %brickGroup.client;
				};
				%brick.isPlanted = 1;
				%brick.setTrusted(1);
				%error = %brick.plant();
				if(%error && %error != 2)
				{
					%brick.delete();
				}
				else
				{
					%brickGroup.add(%brick);
				}
			}
		}
	}
	else if(%size $= "4x4")
	{
		%DB = "brick2x2DirtData";
		for(%z=0.5;%z>-1;%z--)
		{
			for(%x=0.5;%x>-1;%x--)
			{
				%newPos = vectorAdd(%pos,%x SPC %z SPC "0");
				%brick = new fxDtsBrick()
				{
					position = %newPos;
					datablock = %DB;
					colorId = %colorId;
					colorFxId = %colorFxId;
					shapeFxId = %shapeFxId;
					client = %brickGroup.client;
				};
				%brick.isPlanted = 1;
				%brick.setTrusted(1);
				%error = %brick.plant();
				if(%error && %error != 2)
				{
					%brick.delete();
				}
				else
				{
					%brickGroup.add(%brick);
				}
			}
		}
	}
	else if(%size $= "2x2")
	{
		%DB = "brick1x1DirtData";
		for(%z=0.25;%z>=-0.25;%z-=0.5)
		{
			for(%x=0.25;%x>=-0.25;%x-=0.5)
			{
				%newPos = vectorAdd(%pos,%x SPC %z SPC "0");
				%brick = new fxDtsBrick()
				{
					position = %newPos;
					datablock = %DB;
					colorId = %colorId;
					colorFxId = %colorFxId;
					shapeFxId = %shapeFxId;
					client = %brickGroup.client;
				};
				%brick.isPlanted = 1;
				%brick.setTrusted(1);
				%error = %brick.plant();
				if(%error && %error != 2)
				{
					%brick.delete();
				}
				else
				{
					%brickGroup.add(%brick);
				}
			}
		}
	}
}

function onTrenchDig(%client,%pos,%brick)
{
	if(%client.trenchDirt >= $TrenchDig::DirtCount)
	{
		%client.centerPrint("\c3You do not have enough room for any more dirt!",1);
		return 0;
	}
	else
	{
		if(!%client.isInfiniteMiner)
		{
			%client.trenchDirt++;
			%client.trenchBrick[%client.trenchDirt] = %brick.getColorID();
		}
		%brick.delete();
		%client.updateDirt();
		return 1;
	}
}

function findSmallBrick(%brick,%Pos,%wantBrick)
{
	%DBName = %brick.getDatablock().getName();
	if(%wantBrick $= "brick2xCubeDirtData")
	{
		%increment = 1;
		switch$(%DBName)
		{
			case "brick64xCubeDirtData":
				%Min = -15.5;
				%Max = 15.5;
			case "brick32xCubeDirtData":
				%Min = -7.5;
				%Max = 7.5;
			case "brick16xCubeDirtData":
				%Min = -3.5;
				%Max = 3.5;
			case "brick8xCubeDirtData":
				%Min = -1.5;
				%Max = 1.5;
			case "brick4xCubeDirtData":
				%Min = -0.5;
				%Max = 0.5;
			case "brick2xCubeDirtData":
				return %brick.getPosition();
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick4xCubeDirtData")
	{
		%increment = 2;
		switch$(%DBName)
		{
			case "brick64xCubeDirtData":
				%Min = -15;
				%Max = 15;
			case "brick32xCubeDirtData":
				%Min = -7;
				%Max = 7;
			case "brick16xCubeDirtData":
				%Min = -3;
				%Max = 3;
			case "brick8xCubeDirtData":
				%Min = -1;
				%Max = 1;
			case "brick4xCubeDirtData":
				return %brick.getPosition();
			case "brick2xCubeDirtData":
				return 0 TAB %brick.getPosition();
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick8xCubeDirtData")
	{
		%increment = 4;
		switch$(%DBName)
		{
			case "brick64xCubeDirtData":
				%Min = -14;
				%Max = 14;
			case "brick32xCubeDirtData":
				%Min = -6;
				%Max = 6;
			case "brick16xCubeDirtData":
				%Min = -2;
				%Max = 2;
			case "brick8xCubeDirtData":
				return %brick.getPosition();
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick16xCubeDirtData")
	{
		%increment = 8;
		switch$(%DBName)
		{
			case "brick64xCubeDirtData":
				%Min = -8;
				%Max = 8;
			case "brick32xCubeDirtData":
				%Min = -4;
				%Max = 4;
			case "brick16xCubeDirtData":
				return %brick.getPosition();
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick32xCubeDirtData")
	{
		%increment = 16;
		switch$(%DBName)
		{
			case "brick64xCubeDirtData":
				%Min = -8;
				%Max = 8;
			case "brick32xCubeDirtData":
				return %brick.getPosition();
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick1x1DirtData")
	{
		%brickCheck = 1;
		%increment = 0.5;
		switch$(%DBName)
		{
			case "brick8x16DirtData":
				%XMin = -3.75;
				%XMax = 3.75;
				%ZMin = -1.75;
				%ZMax = 1.75;
			case "brick8x8DirtData":
				%XMin = -1.75;
				%XMax = 1.75;
				%ZMin = -1.75;
				%Zmax = 1.75;
			case "brick4x4DirtData":
				%XMin = -0.75;
				%XMax = 0.75;
				%ZMin = -0.75;
				%Zmax = 0.75;
			case "brick2x2DirtData":
				%XMin = -0.25;
				%XMax = 0.25;
				%ZMin = -0.25;
				%Zmax = 0.25;
			case "brick1x1DirtData":
				return %brick.getPosition();
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick2x2DirtData")
	{
		%brickCheck = 1;
		%increment = 1;
		switch$(%DBName)
		{
			case "brick8x16DirtData":
				%XMin = -3.5;
				%XMax = 3.5;
				%ZMin = -1.5;
				%ZMax = 1.5;
			case "brick8x8DirtData":
				%XMin = -1.5;
				%XMax = 1.5;
				%ZMin = -1.5;
				%Zmax = 1.5;
			case "brick4x4DirtData":
				%XMin = -0.5;
				%XMax = 0.5;
				%ZMin = -0.5;
				%Zmax = 0.5;
			case "brick2x2DirtData":
				return %brick.getPosition();
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick4x4DirtData")
	{
		%brickCheck = 1;
		%increment = 2;
		switch$(%DBName)
		{
			case "brick8x16DirtData":
				%XMin = -3;
				%XMax = 3;
				%ZMin = -1;
				%ZMax = 1;
			case "brick8x8DirtData":
				%XMin = -1;
				%XMax = 1;
				%ZMin = -1;
				%Zmax = 1;
			case "brick4x4DirtData":
				return %brick.getPosition();
			case "brick2x2DirtData":
				return 0 TAB %brick.getPosition() TAB 2;
			case "brick1x1DirtData":
				return 0 TAB %brick.getPosition() TAB 1;
			default:
				return;
		}
	}
	else if(%wantBrick $= "brick8x8DirtData")
	{
		%brickCheck = 1;
		%increment = 4;
		switch$(%DBName)
		{
			case "brick8x16DirtData":
				%XMin = -2;
				%XMax = 2;
				%ZMin = 0;
				%ZMax = 0;
			case "brick8x8DirtData":
				return %brick.getPosition();
			default:
				return;
		}
	}
	if(%brickCheck)
	{
		for(%x = %XMin;%x <= %XMax;%x += %increment)
		{
			for(%z = %ZMin;%z <= %ZMax;%z += %increment)
			{
				%rot = getWord(%brick.rotation,3);
				if(mCeil(%rot) == 90 || mFloor(%rot) == 90)
				{
					%smallPos = vectorAdd(%brick.getPosition(),%x SPC %z SPC "0");
				}
				else
				{
					%smallPos = vectorAdd(%brick.getPosition(),%z SPC %x SPC "0");
				}
				%Distance = vectorDist(%Pos,%smallPos);
				if(%Smallest $= "" || %Distance < %Smallest)
				{
					%SmallestPos = %smallPos;
					%Smallest = %Distance;
				}
			}
		}
	}
	else
	{
		for(%x = %Min;%x <= %Max;%x += %increment)
		{
			for(%y = %Min;%y <= %Max;%y += %increment)
			{
				for(%z = %Min;%z <= %Max;%z += %increment)
				{
					%rot = getWord(%brick.rotation,3);
					%smallPos = vectorAdd(%brick.getPosition(),%x SPC %y SPC %z);
					%Distance = vectorDist(%Pos,%smallPos);
					if(%Smallest $= "" || %Distance < %Smallest)
					{
						%SmallestPos = %smallPos;
						%Smallest = %Distance;
					}
				}
			}
		}
	}
	return %SmallestPos;
}
function GameConnection::updateDirt(%this)
{
	if(%this.isInfiniteMiner)
	{
		%Dirt = "�";
	}
	else
	{
		%Dirt = %this.trenchDirt;
	}
	//%this.bottomPrint("\c3" @ %Dirt @ "\c6/\c3" @ $TrenchDig::dirtCount @ " dirt",-1);
}


// ______________________
//|                      |
//| Regrouping Functions |
//|______________________|

function fxDtsBrick::checkBricks(%this,%client)
{
	%pos1 = %this.getPosition();
	%DBName = %this.getDatablock().getName();
	%Data = %this.getDatablock();
	%rot=round(getWord(%this.rotation,3));
	if(%rot==90 || %rot == 270)
	{
		%sizeX= %Data.brickSizeY;
		%sizeY = %Data.brickSizeX;
	}
	else
	{
		%sizeX = %Data.brickSizeX;
		%sizeY = %Data.brickSizeY;
	}
	if(%DBName $= "brick8x16DirtData" || %DBName $= "brick8x8DirtData" || %DBName $= "brick4x4DirtData" || %DBName $= "brick2x2DirtData" || %DBName $= "brick1x1DirtData")
	{
		%sizeZ = %Data.brickSizeZ;
	}
	else
	{
		%sizeZ = %Data.brickSizeZ*0.2+0.3;
	}
	%box = (%sizeX*0.5+0.6) SPC (%sizeY*0.5+0.6) SPC %sizeZ;
	if(%DBName $= "brick2xCubeDirtData")
	{
		%size = 1;
		%stringSize = "1";
		%oppSize = -1;
		%div = 2;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else if(%DBName $= "brick4xCubeDirtData")
	{
		%size = 2;
		%stringSize = "2";
		%oppSize = "-2";
		%div = 1;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else if(%DBName $= "brick8xCubeDirtData")
	{
		%size = 4;
		%stringSize = "4";
		%oppSize = "-4";
		%div = 0.5;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else if(%DBName $= "brick16xCubeDirtData")
	{
		%size = 8;
		%stringSize = "8";
		%oppSize = "-8";
		%div = 0.25;
		%large = 1;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else if(%DBName $= "brick32xCubeDirtData")
	{
		%size = 16;
		%stringSize = "16";
		%oppSize = "-16";
		%div = 0.125;
		%large = 1;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else if(%DBName $= "brick1x1DirtData")
	{
		%brickCheck = 1;
		%size = 0.5;
		%stringSize = "0.5";
		%oppSize = -0.5;
		%div = 4;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else if(%DBName $= "brick2x2DirtData")
	{
		%brickCheck = 1;
		%size = 1;
		%stringSize = "1";
		%oppSize = "-1";
		%div = 2;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else if(%DBName $= "brick4x4DirtData")
	{
		%brickCheck = 1;
		%size = 2;
		%stringSize = "2";
		%oppSize = "-2";
		%div = 1;
		InitContainerBoxSearch(%pos1,%box,$TypeMasks::fxBrickObjectType);
	}
	else
	{
		return;
	}
//	lilDebug("The datablock is "@%DBName);

	%puzzleComplete = 0;
	%puzzlePos = "0 0 0";
//	lilDebug("Started up the container search...");
	while(isObject(%next = containerSearchNext()))
	{
		if(%next.getDatablock().getName() $= %DBName && %next.isPlanted && %next.getDatablock().isTrenchDirt)
		{
			%pos2 = %next.getPosition();
			%Sub = vectorSub(%pos1,%pos2);
//			lilDebug("Found a "@%DBName@" brick and the sub is "@ %Sub);
			
			if(%brickCheck)
			{
				switch$(%Sub)
				{
				case %stringSize SPC "0 0":
					%puzzleComplete++;
					%smallBrick[1,0] = %next;
					%puzzleDir[1,-1] += 1;
					%puzzleDir[1,1] += 1;
				case %oppSize SPC "0 0":
					%puzzleComplete++;
					%smallBrick[-1,0] = %next;
					%puzzleDir[-1,1] += 1;
					%puzzleDir[-1,-1] += 1;
				case "0" SPC %stringSize SPC "0":
					%puzzleComplete++;
					%smallBrick[0,1] = %next;
					%puzzleDir[1,1] += 1;
					%puzzleDir[-1,1] += 1;
				case "0" SPC %oppSize SPC "0":
					%puzzleComplete++;
					%smallBrick[0,-1] = %next;
					%puzzleDir[1,-1] += 1;
					%puzzleDir[-1,-1] += 1;
				case %stringSize SPC %stringSize SPC "0":
					%puzzleComplete++;
					%smallBrick[1,1] = %next;
					%puzzleDir[1,1] += 1;
				case %stringSize SPC %oppSize SPC "0":
					%puzzleComplete++;
					%smallBrick[1,-1] = %next;
					%puzzleDir[1,-1] += 1;
				case %oppSize SPC %stringSize SPC "0":
					%puzzleComplete++;
					%smallBrick[-1,1] = %next;
					%puzzleDir[-1,1] += 1;
				case %oppSize SPC %oppSize SPC "0":
					%puzzleComplete++;
					%smallBrick[-1,-1] = %next;
					%puzzleDir[-1,-1] += 1;
				}
			}
			else
			{
				switch$(%Sub)
				{
				case %stringSize SPC "0 0":
					%puzzleComplete++;
					%smallBrick[1,0,0] = %next;
					%puzzleDir[1,-1,1] += 1;
					%puzzleDir[1,1,1] += 1;
					%puzzleDir[1,-1,-1] += 1;
					%puzzleDir[1,1,-1] += 1;
				case %oppSize SPC "0 0":
					%puzzleComplete++;
					%smallBrick[-1,0,0] = %next;
					%puzzleDir[-1,1,1] += 1;
					%puzzleDir[-1,-1,1] += 1;
					%puzzleDir[-1,1,-1] += 1;
					%puzzleDir[-1,-1,-1] += 1;
				case "0" SPC %stringSize SPC "0":
					%puzzleComplete++;
					%smallBrick[0,1,0] = %next;
					%puzzleDir[1,1,1] += 1;
					%puzzleDir[-1,1,1] += 1;
					%puzzleDir[1,1,-1] += 1;
					%puzzleDir[-1,1,-1] += 1;
				case "0" SPC %oppSize SPC "0":
					%puzzleComplete++;
					%smallBrick[0,-1,0] = %next;
					%puzzleDir[1,-1,1] += 1;
					%puzzleDir[-1,-1,1] += 1;
					%puzzleDir[1,-1,-1] += 1;
					%puzzleDir[-1,-1,-1] += 1;
				case %stringSize SPC %stringSize SPC "0":
					%puzzleComplete++;
					%smallBrick[1,1,0] = %next;
					%puzzleDir[1,1,-1] += 1;
					%puzzleDir[1,1,1] += 1;
				case %stringSize SPC %oppSize SPC "0":
					%puzzleComplete++;
					%smallBrick[1,-1,0] = %next;
					%puzzleDir[1,-1,1] += 1;
					%puzzleDir[1,-1,-1] += 1;
				case %oppSize SPC %stringSize SPC "0":
					%puzzleComplete++;
					%smallBrick[-1,1,0] = %next;
					%puzzleDir[-1,1,1] += 1;
					%puzzleDir[-1,1,-1] += 1;
				case %oppSize SPC %oppSize SPC "0":
					%puzzleComplete++;
					%smallBrick[-1,-1,0] = %next;
					%puzzleDir[-1,-1,1] += 1;
					%puzzleDir[-1,-1,-1] += 1;

				case %stringSize SPC "0" SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[1,0,1] = %next;
					%puzzleDir[1,-1,1] += 1;
					%puzzleDir[1,1,1] += 1;
				case %oppSize SPC "0" SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[-1,0,1] = %next;
					%puzzleDir[-1,1,1] += 1;
					%puzzleDir[-1,-1,1] += 1;
				case "0" SPC %stringSize SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[0,1,1] = %next;
					%puzzleDir[1,1,1] += 1;
					%puzzleDir[-1,1,1] += 1;
				case "0" SPC %oppSize SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[0,-1,1] = %next;
					%puzzleDir[1,-1,1] += 1;
					%puzzleDir[-1,-1,1] += 1;
				case %stringSize SPC %stringSize SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[1,1,1] = %next;
					%puzzleDir[1,1,1] += 1;
				case %stringSize SPC %oppSize SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[1,-1,1] = %next;
					%puzzleDir[1,-1,1] += 1;
				case %oppSize SPC %stringSize SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[-1,1,1] = %next;
					%puzzleDir[-1,1,1] += 1;
				case %oppSize SPC %oppSize SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[-1,-1,1] = %next;
					%puzzleDir[-1,-1,1] += 1;
				case "0 0" SPC %stringSize:
					%puzzleComplete++;
					%smallBrick[0,0,1] = %next;
					%puzzleDir[1,1,1] += 1;
					%puzzleDir[-1,1,1] += 1;
					%puzzleDir[-1,-1,1] += 1;
					%puzzleDir[1,-1,1] += 1;

				case %stringSize SPC "0" SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[1,0,-1] = %next;
					%puzzleDir[1,-1,-1] += 1;
					%puzzleDir[1,1,-1] += 1;
				case %oppSize SPC "0" SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[-1,0,-1] = %next;
					%puzzleDir[-1,1,-1] += 1;
					%puzzleDir[-1,-1,-1] += 1;
				case "0" SPC %stringSize SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[0,1,-1] = %next;
					%puzzleDir[1,1,-1] += 1;
					%puzzleDir[-1,1,-1] += 1;
				case "0" SPC %oppSize SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[0,-1,-1] = %next;
					%puzzleDir[1,-1,-1] += 1;
					%puzzleDir[-1,-1,-1] += 1;
				case %stringSize SPC %stringSize SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[1,1,-1] = %next;
					%puzzleDir[1,1,-1] += 1;
				case %stringSize SPC %oppSize SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[1,-1,-1] = %next;
					%puzzleDir[1,-1,-1] += 1;
				case %oppSize SPC %stringSize SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[-1,1,-1] = %next;
					%puzzleDir[-1,1,-1] += 1;
				case %oppSize SPC %oppSize SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[-1,-1,-1] = %next;
					%puzzleDir[-1,-1,-1] += 1;
				case "0 0" SPC %oppSize:
					%puzzleComplete++;
					%smallBrick[0,0,-1] = %next;
					%puzzleDir[1,1,-1] += 1;
					%puzzleDir[-1,1,-1] += 1;
					%puzzleDir[-1,-1,-1] += 1;
					%puzzleDir[1,-1,-1] += 1;
				}
			}
		}
		else
		{
//			lilDebug("Found a "@%next.getDatablock().getName()@" brick...");
		}
	}
	if(%puzzleComplete >= 3 && %brickCheck)
	{
//		lilDebug("Found "@%puzzleComplete@" bricks surrounding...");
		for(%a = -1;%a <= 1;%a++)
		{
			for(%b = -1;%b <= 1;%b++)
			{
				%cur = %puzzleDir[%a,%b];
				if(%highest $= "" || %cur > %highest)
				{
					%highest = %cur;
					%highA = %a;
					%highB = %b;
				}
			}
		}
		if(%highest < 3)
		{
//			lilDebug("Aren't enough bricks to make the desired grouping...");
			return;
		}
//		lilDebug("Got the highest direction...");
		%pos = vectorAdd(%this.getPosition(),%highA/%div*-1 SPC %highB/%div*-1 SPC "0");
		%BGClient = %this.getGroup().client;
		%BG = %this.getGroup();

		%bColor[%this.getColorID()]++;
		%bColor[%smallBrick[%highA,0].getColorID()]++;
		%bColor[%smallBrick[%highA,%highB].getColorID()]++;
		%bColor[%smallBrick[0,%highB].getColorID()]++;
		for(%i=0;%i<64;%i++)
		{
			%currColor = %bColor[%i];
			if(%majorColor $= "" || %currColor > %majorCount)
			{
				%majorCount = %currColor;
				%majorColor = %i;
			}
		}

		%this.delete();
		%smallBrick[%highA,0].delete();
		%smallBrick[0,%highB].delete();
		%smallBrick[%highA,%highB].delete();
//		lilDebug("Deleted the bricks...");

		schedule(1,0,finishRegrouping,%pos,%DBName,%majorColor,%BGClient,%BG,%client);
	}
	else if(%puzzleComplete >= 7)
	{
//		lilDebug("Found "@%puzzleComplete@" bricks surrounding�");
//		lilDebug("Checking which direction to use�");
		for(%a = -1;%a <= 1;%a++)
		{
			for(%b = -1;%b <= 1;%b++)
			{
				for(%c = -1;%c <= 1;%c++)
				{
					%cur = %puzzleDir[%a,%b,%c];
					if(%highest $= "" || %cur > %highest)
					{
						%highest = %cur;
						%highA = %a;
						%highB = %b;
						%highC = %c;
					}
				}
			}
		}
		if(%highest < 7)
		{
//			lilDebug("The highest in one direction is "@%highest);
//			lilDebug("Aren't enough bricks to make the desired grouping...");
			return;
		}
//		lilDebug("Got the highest direction...");
		%pos = vectorAdd(%this.getPosition(),%highA/%div*-1 SPC %highB/%div*-1 SPC %highC/%div*-1);
		%BGClient = %this.getGroup().client;
		%BG = %this.getGroup();

		%bColor[%this.getColorID()]++;
		%bColor[%smallBrick[%highA,0,0].getColorID()]++;
		%bColor[%smallBrick[0,%highB,0].getColorID()]++;
		%bColor[%smallBrick[%highA,%highB,0].getColorID()]++;
		%bColor[%smallBrick[%highA,0,%highC].getColorID()]++;
		%bColor[%smallBrick[0,%highB,%highC].getColorID()]++;
		%bColor[%smallBrick[%highA,%highB,%highC].getColorID()]++;
		%bColor[%smallBrick[0,0,%highC].getColorID()]++;
		for(%i=0;%i<64;%i++)
		{
			%currColor = %bColor[%i];
			if(%majorColor $= "" || %currColor > %majorCount)
			{
				%majorCount = %currColor;
				%majorColor = %i;
			}
		}

		%this.schedule(0,delete);
		%smallBrick[%highA,0,0].schedule(0,delete);
		%smallBrick[0,%highB,0].schedule(0,delete);
		%smallBrick[%highA,%highB,0].schedule(0,delete);
		%smallBrick[%highA,0,%highC].schedule(0,delete);
		%smallBrick[0,%highB,%highC].schedule(0,delete);
		%smallBrick[%highA,%highB,%highC].schedule(0,delete);
		%smallBrick[0,0,%highC].schedule(0,delete);
//		lilDebug("Deleted the bricks�");

		if(%large)
		{
			%sched = 100;
		}
		else
		{
			%sched = 1;
		}

		schedule(%sched,0,finishRegrouping,%pos,%DBName,%majorColor,%BGclient,%BG,%client);
	}
	else
	{
//		lilDebug("Didn't find enough surrounding bricks!");
	}
}

function finishRegrouping(%pos,%DBName,%colorID,%client,%BG,%difClient)
{
	if(%DBName $= "brick2xCubeDirtData")
	{
		%DB = brick4xCubeDirtData;
	}
	else if(%DBName $= "brick4xCubeDirtData")
	{
		%DB = brick8xCubeDirtData;
	}
	else if(%DBName $= "brick8xCubeDirtData")
	{
		%DB = brick16xCubeDirtData;
	}
	else if(%DBName $= "brick16xCubeDirtData")
	{
		%DB = brick32xCubeDirtData;
	}
	else if(%DBName $= "brick32xCubeDirtData")
	{
		%DB = brick64xCubeDirtData;
	}
	else if(%DBName $= "brick1x1DirtData")
	{
		%DB = brick2x2DirtData;
	}
	else if(%DBName $= "brick2x2DirtData")
	{
		%DB = brick4x4DirtData;
	}
	else if(%DBName $= "brick4x4DirtData")
	{
		%DB = brick8x8DirtData;
	}
	%newBrick = new fxDtsBrick()
	{
		position = %pos;
		datablock = %DB;
		colorId = %colorId;
		client = %client;
		colorFxId = 0;
		shapeFxId = 0;
	};
	%newBrick.isPlanted=1;
	%newBrick.setTrusted(1);
	%error = %newBrick.plant();
//	lilDebug("Called .plant() and it returned: "@ %error);
	if(%error && %error != 2)
	{
		%newBrick.delete();
		return;
	}
	%BG.add(%newBrick);

// Fixing getting stuck within the brick
	if(isObject(%client.player))
	{
		%pos = %client.player.getPosition();
		%raycast = containerRayCast(%pos,vectorAdd(%pos,"0 0 -0.1"),$TypeMasks::fxBrickObjectType,%client.player);
		%obj = firstWord(%raycast);
		if(%obj == %newBrick)
		{
			%client.player.addVelocity("0 0 2");
		}
	}

//	lilDebug("All was successful! Checking for more groupage...");
	if(%DBName !$= brick32xCubeDirtData)
	{
		%newBrick.schedule(2,checkBricks);
	}
}