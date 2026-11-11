#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AllIn1Assets.AllIn1ShaderNodes
{
	//The include helper hard codes the render pipeline the shader graph nodes compile against, so the
	//file that shipped inside the package carries whatever pipeline the publishing project ran. Gated
	//on SessionState by pipeline like the Sprite and Vfx shader importers, it acts on a change instead
	//of on every load
	[InitializeOnLoad]
	public static class AllIn1ShaderNodesImporter
	{
		private const string TEMPLATE_LOCAL_PATH = "Templates/IncludeHelperTemplate.allin1template";
		private const string INCLUDE_HELPER_LOCAL_PATH = "../Shaders/AllIn1Node_IncludeHelper.hlsl";

		static AllIn1ShaderNodesImporter()
		{
			ConfigureIncludeHelper();
		}

		//No session latch on purpose: an attempt that fails silently mid import must retry on the next
		//reload. CreateIncludeHelper compares the file content first, so a correct helper costs one read
		private static void ConfigureIncludeHelper()
		{
			if (UnityEditor.MPE.ProcessService.level == UnityEditor.MPE.ProcessLevel.Secondary)
				return;

			RenderPipelineChecker.RefreshData();

			CreateIncludeHelper();
		}

		public static void CreateIncludeHelper()
		{
			string includeLine;
			if (RenderPipelineChecker.IsURP())
			{
				includeLine = "#define ALLIN1NODE_URP";
			}
			else if (RenderPipelineChecker.IsHDRP())
			{
				includeLine = "#define ALLIN1NODE_HDRP";
			}
			else
			{
				includeLine = "#define ALLIN1NODE_BIRP";
			}

			try
			{
				string[] results = AssetDatabase.FindAssets($"t:Script {nameof(AllIn1ShaderNodesImporter)}");
				if (results.Length == 0)
				{
					Debug.LogWarning($"[AllIn1] Could not locate {nameof(AllIn1ShaderNodesImporter)} in the project, the shader nodes include helper was not configured");
					return;
				}

				string currentFolder = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(results[0]));

				string templateFilePath = Path.GetFullPath(Path.Combine(currentFolder, TEMPLATE_LOCAL_PATH));
				string targetFilePath = Path.GetFullPath(Path.Combine(currentFolder, INCLUDE_HELPER_LOCAL_PATH));

				if (!File.Exists(templateFilePath))
				{
					Debug.LogWarning($"[AllIn1] The shader nodes include helper template is missing at '{templateFilePath}'. " +
						"The include helper keeps its current pipeline defines.");
					return;
				}

				string finalText = File.ReadAllText(templateFilePath).Replace("<DEFINE_PIPELINE_SYMBOL>", includeLine);
				finalText = EditorUtils.UnifyEOL(finalText);

				if (File.Exists(targetFilePath))
				{
					string existingText = EditorUtils.UnifyEOL(File.ReadAllText(targetFilePath));
					if (string.Equals(existingText, finalText, StringComparison.Ordinal)) return;
				}

				File.WriteAllText(targetFilePath, finalText);
				AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[AllIn1] The shader nodes include helper could not be regenerated. Details: {e}");
			}
		}
	}
}
#endif
