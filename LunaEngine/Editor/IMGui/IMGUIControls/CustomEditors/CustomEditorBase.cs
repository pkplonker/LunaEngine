﻿using Editor;
using Editor.Properties;
using Engine;

public static class CustomEditorBase
{
	public static string GenerateName<T>(IMemberAdapter? memberInfo)
	{
		var upperName = CamelCaseRenamer.GetFormattedName(memberInfo.Name);
		var typeName = typeof(T).Name;
		var shownName = upperName == typeName ? upperName : $"{typeName} - {upperName}";
		return shownName;
	}
}