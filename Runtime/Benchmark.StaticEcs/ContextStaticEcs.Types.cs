using System;
using System.Runtime.CompilerServices;
using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

internal struct StaticWorld : IWorldType { }
internal struct StaticSystems : ISystemsType { }
internal abstract class W : World<StaticWorld> { }
internal abstract class Systems : World<StaticWorld>.Systems<StaticSystems> { }

public sealed partial class ContextStaticEcs
{
	internal struct EventEntity : IEntityType {}
	internal struct UnitEntity : IEntityType {}

	private struct DataComponent : IComponent
	{
		public Data Value;
	}

	private struct UnitComponent : IComponent
	{
		public Unit Value;
	}

	private struct HealthComponent : IComponent
	{
		public Health Value;
	}

	private struct DamageComponent : IComponent
	{
		public Damage Value;
	}

	private struct SpriteComponent : IComponent
	{
		public Sprite Value;
	}

	private struct PositionComponent : IComponent
	{
		public Position Value;
	}

	private struct VelocityComponent : IComponent
	{
		public Velocity Value;
	}

	private struct AttackComponent : IComponent
	{
		public Attack<EntityGID> Value;

		public override bool Equals(object? obj) =>
			obj is AttackComponent other
		 && Value.Damage == other.Value.Damage
		 && Value.Ticks  == other.Value.Ticks
		 && Value.Target == other.Value.Target;

		public override int GetHashCode() => HashCode.Combine(Value.Target, Value.Damage, Value.Ticks);
	}

	private struct SpawnTag : ITag { }
	private struct DeadTag : ITag { }
	private struct UnitNpcTag : ITag { }
	private struct UnitHeroTag : ITag { }
	private struct UnitMonsterTag : ITag { }

	private struct Target
	{
		public readonly EntityGID Entity;
		public readonly Position  Position;

		public Target(EntityGID entity, Position position)
		{
			Entity   = entity;
			Position = position;
		}
	}
}

}
