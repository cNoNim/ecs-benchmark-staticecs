using System.Runtime.CompilerServices;
using Benchmark.Core;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct RenderSystem
		: ISystem, World<StaticWorld>.IQueryBlock.Read<PositionComponent, SpriteComponent, UnitComponent, DataComponent>
	{
		private readonly Framebuffer _framebuffer;

		public RenderSystem(Framebuffer framebuffer) =>
			_framebuffer = framebuffer;

		public void Update() =>
			W.Query()
			 .ReadBlock<PositionComponent, SpriteComponent, UnitComponent, DataComponent>()
			 .For(ref this);

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(
			uint count,
			World<StaticWorld>.EntityBlock entitiesBlock,
			BlockR<PositionComponent> position,
			BlockR<SpriteComponent> sprite,
			BlockR<UnitComponent> unit,
			BlockR<DataComponent> data)
		{
			for (uint i = 0; i < count; i++)
				RenderSystemForEach(
					_framebuffer,
					in position[i].Value,
					in sprite[i].Value,
					in unit[i].Value,
					in data[i].Value);
		}
	}
}

}
