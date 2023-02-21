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
//|        Support Functions for Increasing/Decreasing Values      |\\
//|----------------------------------------------------------------|\\
//////////////////////////////////||\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

function GameConnection::incKills(%client,%amount)
{
	if(%client.getKills() $= "")
	{
		%client.setKills(0);
		%client.incKills(%amount);
	}
	if(%amount > 0)
	{
		%client.setKills(%client.getKills() + %amount);
	}
}

function GameConnection::incDeaths(%client,%amount)
{
	if(%client.getDeaths() $= "")
	{
		%client.setDeaths(0);
		%client.incDeaths(%amount);
	}
	if(%amount > 0)
	{
		%client.setDeaths(%client.getDeaths() + %amount);
	}
}

function GameConnection::decKills(%client,%amount)
{
	if(%client.getKills() $= "")
	{
		%client.setKills(0);
		%client.decKills(%amount);
	}
	if(%amount > 0)
	{
		%client.setKills(%client.getKills() - %amount);
	}
}

function GameConnection::decDeaths(%client,%amount)
{
	if(%client.getDeaths() $= "")
	{
		%client.setDeaths(0);
		%client.decDeaths(%amount);
	}
	if(%amount > 0)
	{
		%client.setDeaths(%client.getDeaths() - %amount);
	}
}