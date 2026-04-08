using System.Runtime.CompilerServices;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct UpdateVelocitySystem
		: ISystem,
		  World<StaticWorld>.IQueryBlock.Write<VelocityComponent, UnitComponent>.Read<DataComponent, PositionComponent>
	{
		public void Update() =>
			W.Query<None<DeadTag>>()
			 .WriteBlock<VelocityComponent, UnitComponent>()
			 .Read<DataComponent, PositionComponent>()
			 .For(ref this);

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(
			uint count,
			World<StaticWorld>.EntityBlock entities,
			Block<VelocityComponent> velocities,
			Block<UnitComponent> units,
			BlockR<DataComponent> data,
			BlockR<PositionComponent> positions)
		{
			for (uint i = 0; i < count; i++)
				UpdateVelocitySystemForEach(
					ref velocities[i].Value,
					ref units[i].Value,
					in data[i].Value,
					in positions[i].Value);
		}
	}
}

}
