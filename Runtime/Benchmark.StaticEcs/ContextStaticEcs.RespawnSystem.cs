using System;
using System.Runtime.CompilerServices;
using Benchmark.Core.Components;
using Benchmark.Core.Hash;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct RespawnSystem : ISystem, W.IQueryBlock.Read<UnitComponent, DataComponent>
	{
		public void Update()
		{
			W.Query<All<DeadTag>>().ReadBlock<UnitComponent, DataComponent>().For(ref this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invoke(uint count, World<StaticWorld>.EntityBlock entities, BlockR<UnitComponent> unit,
						   BlockR<DataComponent> data)
		{
			for (uint i = 0; i < count; i++)
			{
				if (data[i].Value.Tick < unit[i].Value.RespawnTick)
					continue;

				var newEntity = W.NewEntity<UnitEntity>()
								 .Set(
									  new DataComponent { Value = data[i].Value },
									  new UnitComponent
									  {
										  Value = new Unit
										  {
											  Id   = unit[i].Value.Id | (uint)data[i].Value.Tick << 16,
											  Seed = StableHash32.Hash(unit[i].Value.Seed, unit[i].Value.Counter),
										  },
									  });
				newEntity.Set<SpawnTag>();

				entities[i].Destroy();
			}
		}
	}
}

}
