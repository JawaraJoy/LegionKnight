using UnityEditor;

namespace AllIn1Assets.AllIn1ShaderNodes
{
	public static class RegistryInserter
	{
		//Declaring the plugin costs nothing: no asset is read, found or created here. The registry
		//resolves the config asset the first time something actually asks for it
		[InitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			PluginRegistry.Register(PluginID.ALLIN1_SHADER_NODES, typeof(AllIn1ShaderNodesConfig));
		}
	}
}
