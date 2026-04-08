using System.Runtime.CompilerServices;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct KillSystem
		: ISystem, World<StaticWorld>.IQueryBlock.Write<UnitComponent>.Read<HealthComponent, DataComponent>
	{
		public void Update() =>
			W.Query<None<DeadTag>>()
			 .WriteBlock<UnitComponent>()
			 .Read<HealthComponent, DataComponent>()
			 .For(ref this);

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(
			uint count,
			World<StaticWorld>.EntityBlock entities,
			Block<UnitComponent> units,
			BlockR<HealthComponent> healhs,
			BlockR<DataComponent> data)
		{
			for (uint i = 0; i < count; i++)
			{
				if (healhs[i].Value.Hp > 0)
					continue;

				units[i].Value.RespawnTick = data[i].Value.Tick + RespawnTicks;
				entities[i]
				   .Set<DeadTag>();
			}
		}
	}
}

}
