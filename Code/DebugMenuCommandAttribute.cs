using System;

namespace ModCore
{
	[AttributeUsage(AttributeTargets.Method)]
	public class DebugMenuCommand : Attribute
	{
		public string CommandName { get; }
		public string[] CommandAliases { get; }
		public bool CaseSensitive { get; }

		public DebugMenuCommand(string commandName, string[] commandAliases = null, bool caseSensitive = false)
		{
			CommandName = commandName;
			CommandAliases = commandAliases;
			CaseSensitive = caseSensitive;
		}
	}
}