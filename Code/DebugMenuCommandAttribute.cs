using System;

namespace ModCore
{
	[AttributeUsage(AttributeTargets.Method)]
	public class DebugMenuCommand : Attribute
	{
		public string CommandName { get; }
		public string[] CommandAliases { get; }

		public DebugMenuCommand(string commandName, string[] commandAliases = null)
		{
			CommandName = commandName;
			CommandAliases = commandAliases;
		}
	}
}