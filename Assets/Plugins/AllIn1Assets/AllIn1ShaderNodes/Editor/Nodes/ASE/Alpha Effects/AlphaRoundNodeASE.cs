using System;
using static AllIn1ShaderNodes.Constants;

#if AMPLIFY_SHADER_EDITOR
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes("Alpha Round", EFFECT_CATEGORY_ALPHA_EFFECTS, "")]
	public class AlphaRoundNodeASE : AbstractAlphaEffectNode
	{
		private const string FUNC_NAME = @"AlphaRound({0})";

		protected override void CommonInit(int uniqueId)
		{
			base.CommonInit(uniqueId);

			Init();
		}

		protected override string GetFuncName()
		{
			return FUNC_NAME;
		}
	}
}
#endif
