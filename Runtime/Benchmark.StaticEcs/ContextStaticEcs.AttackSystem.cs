using System;
using System.Buffers;
using Benchmark.Core;
using Benchmark.Core.Algorithms;
using Benchmark.Core.Components;
using Benchmark.Core.Random;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private readonly struct AttackSystem : ISystem
	{
		private readonly int _capacity;

		private struct FillState
		{
			public uint[]   Keys;
			public Target[] Targets;
			public int      Count;
		}

		private struct CreateState
		{
			public int      Count;
			public int[]    Indirection;
			public Target[] Targets;
		}

		public AttackSystem(int capacity) => _capacity = capacity;

		public void Update()
		{
			var keys        = ArrayPool<uint>.Shared.Rent(_capacity);
			var indirection = ArrayPool<int>.Shared.Rent(_capacity);
			var targets     = ArrayPool<Target>.Shared.Rent(_capacity);
			try
			{
				var count = FillTargets(keys, targets);
				if (count <= 0)
					return;

				RadixSort.SortWithIndirection(keys.AsSpan(0, count), indirection.AsSpan(0, count), count);
				CreateAttacks(count, indirection, targets);
			}
			finally
			{
				ArrayPool<uint>.Shared.Return(keys);
				ArrayPool<int>.Shared.Return(indirection);
				ArrayPool<Target>.Shared.Return(targets);
			}
		}

		private static int FillTargets(uint[] keys, Target[] targets)
		{
			var fill = new FillState
			{
				Keys    = keys,
				Targets = targets,
				Count   = 0,
			};
			W.Query<None<SpawnTag, DeadTag>>().For(
				ref fill,
				static (ref FillState state, World<StaticWorld>.Entity entity, in UnitComponent unit,
						in PositionComponent position) =>
				{
					state.Keys[state.Count]    = unit.Value.Id;
					state.Targets[state.Count] = new Target(entity.GID, position.Value);
					state.Count++;
				});

			return fill.Count;
		}

		private static void CreateAttacks(int count, int[] indirection, Target[] targets)
		{
			var create = new CreateState
			{
				Count       = count,
				Indirection = indirection,
				Targets     = targets,
			};
			W.Query<None<SpawnTag, DeadTag>>().For(
				ref create,
				static (ref CreateState state, ref UnitComponent unit, in PositionComponent position,
						in DamageComponent damage, in DataComponent data) =>
				{
					if (damage.Value.Cooldown <= 0)
						return;

					var tick = data.Value.Tick - unit.Value.SpawnTick;
					if (tick % damage.Value.Cooldown != 0)
						return;

					var generator = new RandomGenerator(unit.Value.Seed);
					var index     = generator.Random(ref unit.Value.Counter, state.Count);
					var target    = state.Targets[state.Indirection[index]];

					CreateAttack(target, position.Value, damage.Value.Attack);
				});
		}

		private static void CreateAttack(in Target target, in Position position, int damage)
		{
			W.NewEntity<Default>().Set(
				new AttackComponent
				{
					Value = new Attack<EntityGID>
					{
						Target = target.Entity,
						Damage = damage,
						Ticks  = Common.AttackTicks(position.V, target.Position.V),
					},
				});
		}
	}
}

}
