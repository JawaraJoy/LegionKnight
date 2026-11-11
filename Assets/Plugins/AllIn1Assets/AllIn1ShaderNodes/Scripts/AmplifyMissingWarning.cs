#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AllIn1Assets.AllIn1ShaderNodes
{
	//Amplify leaves its AMPLIFY_SHADER_EDITOR define behind when it is removed from a project, and
	//the ASE nodes then fail with CS0246 errors. This prints what happened and how to fix it. It
	//lives in the runtime assembly on purpose: that assembly has no ASE code, so it keeps compiling
	//and loading in exactly the state where the editor assembly is a wall of errors. The define is
	//read at runtime, never through #if, so a copy built while things were healthy still checks
	public static class AmplifyMissingWarning
	{
		[InitializeOnLoadMethod]
		private static void ScheduleCheck()
		{
			if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Secondary)
				return;

			EditorApplication.delayCall += Check;

			//A failed compile blocks domain reloads, so breaking the project mid session (deleting
			//Amplify, adding the define by hand) raises no load event. The already loaded copy of this
			//assembly hears about it here and explains right when the errors appear
			UnityEditor.Compilation.CompilationPipeline.compilationFinished += context => EditorApplication.delayCall += Check;
		}

		private static void Check()
		{
			if (!IsAmplifyDefineSet()) return;
			if (AmplifyAsmdefExists()) return;

			Debug.LogError("[AllIn1] THE AMPLIFY_SHADER_EDITOR DEFINE IS SET BUT AMPLIFY SHADER EDITOR IS NOT IN THIS PROJECT, " +
				"OR IS TOO OLD TO HAVE AN ASSEMBLY DEFINITION. THE ALLIN1 SHADER NODES ASE SCRIPTS CANNOT COMPILE.");
			Debug.LogWarning("[AllIn1] How to fix it: remove AMPLIFY_SHADER_EDITOR from Project Settings > Player > " +
				"Scripting Define Symbols, or install/update Amplify Shader Editor. Amplify leaves that define behind " +
				"when it is removed from a project.");
		}

		//The active target's defines are the ones compilation uses
		private static bool IsAmplifyDefineSet()
		{
			BuildTargetGroup activeGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

			string[] defineSymbols = new string[0];
			PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(activeGroup), out defineSymbols);

			for (int i = 0; i < defineSymbols.Length; i++)
			{
				if (defineSymbols[i] == "AMPLIFY_SHADER_EDITOR") return true;
			}

			return false;
		}

		//Exact file name match: FindAssets matches substrings, and only the asmdef proves an Amplify
		//new enough for the ASE nodes to compile against
		private static bool AmplifyAsmdefExists()
		{
			string[] guids = AssetDatabase.FindAssets("AmplifyShaderEditor t:AssemblyDefinitionAsset");

			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);

				if (System.IO.Path.GetFileNameWithoutExtension(path) == "AmplifyShaderEditor") return true;
			}

			return false;
		}
	}
}
#endif
