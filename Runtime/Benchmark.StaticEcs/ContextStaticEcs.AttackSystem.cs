using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Benchmark.Core;
using Benchmark.Core.Algorithms;
using Benchmark.Core.Components;
using Benchmark.Core.Random;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs
{
	private struct AttackSystem
		: ISystem, World<StaticWorld>.IQueryBlock.Read<UnitComponent, PositionComponent>,
		  World<StaticWorld>.IQueryBlock.Write<UnitComponent>.Read<PositionComponent, DamageComponent, DataComponent>
	{
		private          State _state;
		private readonly int   _capacity;

		private struct State
		{
			public uint[]   Keys;
			public Target[] Targets;
			public int[]    Indirection;
			public int      Count;
		}

		public AttackSystem(int capacity)
		{
			_state    = default;
			_capacity = capacity;
		}

		public void Update()
		{
			var keys        = ArrayPool<uint>.Shared.Rent(_capacity);
			var indirection = ArrayPool<int>.Shared.Rent(_capacity);
			var targets     = ArrayPool<Target>.Shared.Rent(_capacity);
			try
			{
				_state = new State
				{
					Keys        = keys,
					Targets     = targets,
					Indirection = indirection,
					Count       = 0,
				};

				// FillTargets
				W.Query<None<SpawnTag, DeadTag>>()
				 .ReadBlock<UnitComponent, PositionComponent>()
				 .For(ref this);
				var count = _state.Count;
				if (count <= 0)
					return;

				RadixSort.SortWithIndirection(keys.AsSpan(0, count), indirection.AsSpan(0, count), count);
				// CreateAttacks
				W.Query<None<SpawnTag, DeadTag>>()
				 .WriteBlock<UnitComponent>()
				 .Read<PositionComponent, DamageComponent, DataComponent>()
				 .For(ref this);
			}
			finally
			{
				ArrayPool<uint>.Shared.Return(keys);
				ArrayPool<int>.Shared.Return(indirection);
				ArrayPool<Target>.Shared.Return(targets);
			}
		}

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(
			uint count,
			World<StaticWorld>.EntityBlock entities,
			BlockR<UnitComponent> units,
			BlockR<PositionComponent> positions)
		{
			for (uint i = 0; i < count; i++)
			{
				_state.Keys[_state.Count]    = units[i].Value.Id;
				_state.Targets[_state.Count] = new Target(entities[i].GID, positions[i].Value);
				_state.Count++;
			}
		}

#if ECS_BENCHMARK_FORCE_INLINING
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
		public void Invoke(
			uint count,
			World<StaticWorld>.EntityBlock entities,
			Block<UnitComponent> units,
			BlockR<PositionComponent> positions,
			BlockR<DamageComponent> damages,
			BlockR<DataComponent> data)
		{
			for (uint i = 0; i < count; i++)
			{
				var damage = damages[i];
				if (damages[i].Value.Cooldown                                               <= 0
				 || (data[i].Value.Tick - units[i].Value.SpawnTick) % damage.Value.Cooldown != 0)
					continue;

				var generator = new RandomGenerator(units[i].Value.Seed);
				var index     = generator.Random(ref units[i].Value.Counter, _state.Count);
				var target    = _state.Targets[_state.Indirection[index]];

				W.NewEntity<EventEntity>()
				 .Set(
					  new AttackComponent
					  {
						  Value = new Attack<EntityGID>
						  {
							  Target = target.Entity,
							  Damage = damage.Value.Attack,
							  Ticks  = Common.AttackTicks(positions[i].Value.V, target.Position.V),
						  },
					  });
			}
		}
	}
}

}
