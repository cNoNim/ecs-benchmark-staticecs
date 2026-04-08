using Benchmark.Core.Components;
using Benchmark.Core.Hash;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct RespawnSystem : ISystem
	{
		public void Update()
		{
			W.Query<All<DeadTag>>().For(static (W.Entity entity, in UnitComponent unit, in DataComponent data) =>
			{
				if (data.Value.Tick < unit.Value.RespawnTick)
					return;

				var newEntity = W.NewEntity<Default>()
								 .Set(
									  new DataComponent { Value = data.Value },
									  new UnitComponent
									  {
										  Value = new Unit
										  {
											  Id   = unit.Value.Id | (uint)data.Value.Tick << 16,
											  Seed = StableHash32.Hash(unit.Value.Seed, unit.Value.Counter),
										  },
									  });
				newEntity.Set<SpawnTag>();

				entity.Destroy();
			});
		}
	}
}

}
