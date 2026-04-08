using Benchmark.Core;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct RenderSystem : ISystem
	{
		private readonly Framebuffer _framebuffer;
		public RenderSystem(Framebuffer framebuffer) => _framebuffer = framebuffer;

		public void Update() =>
			W.Query().For(
				_framebuffer,
				static (ref Framebuffer framebuffer, in PositionComponent position, in SpriteComponent sprite,
						in UnitComponent unit, in DataComponent data) =>
				{
					RenderSystemForEach(framebuffer, in position.Value, in sprite.Value, in unit.Value, in data.Value);
				});
	}
}

}
