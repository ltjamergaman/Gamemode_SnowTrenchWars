function charLimTAB(%string,%maxChars)
{
	%wordCount = getWordCount(%string);
	%length = strLen(%string);
	
	if(%length > %maxChars)
	{
		%restOfStr = getSubStr(%string,%maxChars,%length);
		%string = getSubStr(%string,0,%maxChars);
		%restlength = %length - %maxChars;
		
		for(%i = 1; %i < %wordCount; %i++)
		{
			if(%restlength > 0)
			{
				%maxCharsb = %maxChars * %i;
				%restOfStr = getSubStr(%string,%maxCharsb,%length);
				%string = %string TAB getSubStr(%restOfStr,%maxCharsb,%maxChars);
				%restlength -= %maxChars;
			}
		}
	}
	
	return %string;
}