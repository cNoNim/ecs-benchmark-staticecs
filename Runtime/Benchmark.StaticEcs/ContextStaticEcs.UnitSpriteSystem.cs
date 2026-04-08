using System;
using System.Runtime.CompilerServices;
using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct UnitSpriteSystem<TTag> : ISystem, W.IQueryBlock.Write<SpriteComponent>
		where TTag : struct, ITag
	{
		private readonly SpriteMask _character;
		public UnitSpriteSystem(SpriteMask character) => _character = character;

		public void Update() => W.Query<All<TTag>, None<SpawnTag, DeadTag>>()
								 .WriteBlock<SpriteComponent>()
								 .For(ref this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(uint count, W.EntityBlock entities, Block<SpriteComponent> sprite)
		{
			for (uint i = 0; i < count; i++)
			{
				sprite[i].Value.Character = _character;
			}
		}
	}
}

}
