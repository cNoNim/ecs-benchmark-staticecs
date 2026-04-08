using System;
using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct SpawnSystem : ISystem
	{
		public void Update() =>
			W.Query<All<SpawnTag>>()
			 .For(static (World<StaticWorld>.Entity entity, ref UnitComponent unit, in DataComponent data) =>
			  {
				  var unitType = SpawnUnit(
					  in data.Value,
					  ref unit.Value,
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
			  });
	}
}

}
