using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct UnitSpriteSystem<TTag> : ISystem
		where TTag : struct, ITag
	{
		private readonly SpriteMask _character;
		public UnitSpriteSystem(SpriteMask character) => _character = character;

		public void Update() =>
			W.Query<All<TTag>, None<SpawnTag, DeadTag>>().For(
				_character,
				static (ref SpriteMask character, ref SpriteComponent sprite) =>
				{
					sprite.Value.Character = character;
				});
	}
}

}
