using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct KillSystem : ISystem
	{
		public void Update() =>
			W.Query<None<DeadTag>>().For(static (World<StaticWorld>.Entity entity, ref UnitComponent unit,
												 in HealthComponent health, in DataComponent data) =>
			{
				if (health.Value.Hp > 0)
					return;

				unit.Value.RespawnTick = data.Value.Tick + RespawnTicks;
				entity.Set<DeadTag>();
			});
	}
}

}
