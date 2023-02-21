new ScriptGroup("STW_ArmorManager")
{
	armorCount = 0;
};

function STW_registerArmorTypes(%check)
{
	if(%check)
	{
		deleteVariables("$STW::Gamemode::Store::Item::Armor*");
		$STW::Gamemode::Store::Item::ArmorCount = 0;
		STW_ArmorManager.clear();
	}
	
	%armorFile = findNextFile("./*.txt");
	if(%armorFile $= "")
	{
		return;
	}
	
	%file = new FileObject();
	%file.openForRead(%armorFile);
	%armorType = %file.readLine();
	%armorValue = %file.readLine();
	%armorAmount = %file.readLine();
	%armorName = %file.readLine();
	%file.close();
	%file.delete();
	$STW::Gamemode::Store::Item::ArmorCount++;
	
	new ScriptObject("STW_"@%armorType)
	{
		Type = %armorType;
		Value = %armorValue;
		Amount = %armorAmount;
		Name = %armorName;
	};
	
	STW_ArmorManager.add("STW_"@%armorType);
	STW_ArmorManager.armorCount = $STW::Gamemode::Store::Item::ArmorCount;
	STW_RegisterArmorTypes(0);
}

STW_RegisterArmorTypes(1);
STW_RegisterArmorTypes(1); //double check because sometimes doing once will only register a few out of all of them