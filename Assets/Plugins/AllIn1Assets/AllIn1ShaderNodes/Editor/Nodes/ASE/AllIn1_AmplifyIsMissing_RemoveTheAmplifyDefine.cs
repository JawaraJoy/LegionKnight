#if AMPLIFY_SHADER_EDITOR

//This file exists so the error wall explains itself. When Amplify Shader Editor is missing or too
//old while its AMPLIFY_SHADER_EDITOR define is still set, every ASE node fails with CS0246, and the
//first thing a user reads is a file path. This one tells them the fix: remove AMPLIFY_SHADER_EDITOR
//from Project Settings > Player > Scripting Define Symbols, or install/update Amplify Shader Editor.
//With a healthy Amplify this compiles to an invisible empty type. Never rename it casually
namespace AllIn1Assets.AllIn1ShaderNodes
{
	internal static class AmplifyPresenceSentinel
	{
		private static readonly System.Type amplifyPresenceCheck = typeof(AmplifyShaderEditor.ParentNode);
	}
}

#endif
