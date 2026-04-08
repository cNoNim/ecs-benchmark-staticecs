using System.Runtime.CompilerServices;
using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct StateSpriteSystem<TTag>
		: ISystem, World<StaticWorld>.IQueryBlock.Write<SpriteComponent>
		where TTag : struct, ITag
	{
		private readonly SpriteMask _character;

		public StateSpriteSystem(SpriteMask character) =>
			_character = character;

		public void Update() =>
			W.Query<All<TTag>>()
			 .WriteBlock<SpriteComponent>()
			 .For(ref this);

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(uint count, World<StaticWorld>.EntityBlock entities, Block<SpriteComponent> sprite)
		{
			for (uint i = 0; i < count; i++)
				sprite[i].Value.Character = _character;
		}
	}
}

}
