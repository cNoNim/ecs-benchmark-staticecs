using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct StateSpriteSystem<TTag> : ISystem
		where TTag : struct, ITag
	{
		private readonly SpriteMask _character;
		public StateSpriteSystem(SpriteMask character) => _character = character;

		public void Update() =>
			W.Query<All<TTag>>().For(
				_character,
				static (ref SpriteMask character, ref SpriteComponent sprite) =>
				{
					sprite.Value.Character = character;
				});
	}
}

}
