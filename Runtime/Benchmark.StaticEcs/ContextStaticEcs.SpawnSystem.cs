using System;
using System.Runtime.CompilerServices;
using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct SpawnSystem : ISystem, W.IQueryBlock.Write<UnitComponent>.Read<DataComponent>
	{
		public void Update() =>
			W.Query<All<SpawnTag>>()
			 .WriteBlock<UnitComponent>()
			 .Read<DataComponent>().For(ref this);

		[MethodImpl(AggressiveInlining)]
		public void Invoke(uint count, W.EntityBlock entities, Block<UnitComponent> unit,
						   BlockR<DataComponent> data)
		{
			for (uint i = 0; i < count; i++)
			{
				var entity = entities[i];
				var unitType = SpawnUnit(
					in data[i].Value,
					ref unit[i].Value,
					out var health,
					out var damage,
					out var sprite,
					out var position,
					out var velocity);

				entity.Delete<SpawnTag>();
				entity.Set(
					new HealthComponent { Value   = health },
					new DamageComponent { Value   = damage },
					new SpriteComponent { Value   = sprite },
					new PositionComponent { Value = position },
					new VelocityComponent { Value = velocity });

				switch (unitType)
				{
				case UnitType.NPC:
					entity.Set<UnitNpcTag>();
					break;
				case UnitType.Hero:
					entity.Set<UnitHeroTag>();
					break;
				case UnitType.Monster:
					entity.Set<UnitMonsterTag>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}

}
