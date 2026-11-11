#if ALLIN1_URP
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#endif

using UnityEngine;

namespace AllIn1Assets.AllIn1ShaderNodes
{
	public class AllIn1ShaderNodesConfig : AbstractPluginConfig
	{
		private const string PLUGIN_NAME	= "AllIn1ShaderNodes";
		private const string VERSION		= "1.21";
		private const PluginID PLUGIN_ID = PluginID.ALLIN1_SHADER_NODES;
		private const string DEFAULT_EXPORT_FOLDER = "Export";

		private static string[] SHADER_NAMES = { };

		private static string[] SHADER_TEMPLATES = { };


		public override string GetPluginName()
		{
			return PLUGIN_NAME;
		}

		public override PluginID GetPluginID()
		{
			return PLUGIN_ID;
		}

		public override string GetVersion()
		{
			return VERSION;
		}

		//Outside the plugin folder, which customers delete to update. Develop keeps it beside the develop config
		public string ExportFolderPath
		{
			get
			{
				string res = string.Empty;
#if ALLIN1_DEVELOP
				res = EditorUtils.CombineAssetPath(PluginConfigFolderPath, DEFAULT_EXPORT_FOLDER);
#else
				res = ReleaseExportFolderPath;
#endif
				return res;
			}
		}

		public static string ReleaseExportFolderPath => EditorUtils.CombineAssetPath(GlobalConstants.FOLDER_NAME_ASSET, GlobalConstants.FOLDER_NAME_EXPORTS, PLUGIN_NAME);

		public override void Init()
		{
			base.Init();

			//The importer regenerates it on pipeline changes. This covers a helper file deleted in between
			AllIn1ShaderNodesImporter.CreateIncludeHelper();
		}

		public override string[] GetShaderNames()
		{
			return SHADER_NAMES;
		}

		public override string[] GetShaderTemplates()
		{
			return SHADER_TEMPLATES;
		}

		public override bool IsWritableFolder(string folderPath)
		{
			return false;
		}

		public override Shader GetShaderAfterMaterialRemoval()
		{
			return null;
		}

		public override string GetRenderPipelineDocURL()
		{
			return $"{GlobalConstants.URL_DOCS_SITE}/shader-nodes/setup-and-render-pipelines";
		}

#if ALLIN1_URP
		public override void ConvertMaterialsFolder()
		{
		
		}

		public override void ConfigureRenderPipeline(UniversalRendererData universalRendererData, RenderPipelineAsset renderPipelineAsset)
		{
			ConvertMaterialsFolder();
		}
#endif

#if ALLIN1_DEVELOP
		public override string[] GetFoldersToExport()
		{
			string[] res = new string[]
			{
					PluginRegistry.GetSharedConfig().AllIn1SharedFolderPath,
					PluginRegistry.GetSharedConfig().assemblyPath,
					RootPluginPath,
			};

			return res;
		}

		//The ASE compatibility content: customers get it through the nested packages that
		//ASECompatibilityPackageBuilder builds, never through the main export. The builder reads
		//this same list, so the two packages cannot overlap or leave a gap between them
		public override string[] GetAssetPathsToExcludeFromExport()
		{
			return new string[]
			{
				EditorUtils.CombineAssetPath(RootPluginPath, "Demo/Demo-AmplifyShaderEditor.unity"),
				EditorUtils.CombineAssetPath(RootPluginPath, "Demo/Graphs/AmplifyShaderEditorGraphs"),
				EditorUtils.CombineAssetPath(RootPluginPath, "Editor/ASE Templates"),
				EditorUtils.CombineAssetPath(RootPluginPath, "Demo/Prefab/ASE"),
				EditorUtils.CombineAssetPath(RootPluginPath, "Demo/Materials/DemoMaterials/ASE"),
			};
		}
#endif

	}
}