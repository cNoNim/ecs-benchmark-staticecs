using Benchmark.Core;
using Benchmark.Core.Components;
using FFS.Libraries.StaticEcs;

namespace Benchmark.StaticEcs
{

public sealed partial class ContextStaticEcs : ContextBase
{
	public ContextStaticEcs()
		: base("Static Ecs") { }

	protected override void DoSetup()
	{
		W.Create(WorldConfig.Default());
		W.Types()
		 .EntityType<UnitEntity>(1)
		 .EntityType<EventEntity>(2)
		 .Component(new ComponentTypeConfig<DataComponent>(noDataLifecycle: true))
		 .Component(new ComponentTypeConfig<UnitComponent>(noDataLifecycle: true))
		 .Component(new ComponentTypeConfig<HealthComponent>(noDataLifecycle: true))
		 .Component(new ComponentTypeConfig<DamageComponent>(noDataLifecycle: true))
		 .Component(new ComponentTypeConfig<SpriteComponent>(noDataLifecycle: true))
		 .Component(new ComponentTypeConfig<PositionComponent>(noDataLifecycle: true))
		 .Component(new ComponentTypeConfig<VelocityComponent>(noDataLifecycle: true))
		 .Component(new ComponentTypeConfig<AttackComponent>(noDataLifecycle: true))
		 .Tag<SpawnTag>()
		 .Tag<DeadTag>()
		 .Tag<UnitNpcTag>()
		 .Tag<UnitHeroTag>()
		 .Tag<UnitMonsterTag>();
		W.Initialize((uint)EntityCount * 2);

		Systems.Create();
		Systems.Add(new SpawnSystem())
			   .Add(new RespawnSystem())
			   .Add(new KillSystem())
			   .Add(new RenderSystem(Framebuffer))
			   .Add(new StateSpriteSystem<SpawnTag>(SpriteMask.Spawn))
			   .Add(new StateSpriteSystem<DeadTag>(SpriteMask.Grave))
			   .Add(new UnitSpriteSystem<UnitNpcTag>(SpriteMask.NPC))
			   .Add(new UnitSpriteSystem<UnitHeroTag>(SpriteMask.Hero))
			   .Add(new UnitSpriteSystem<UnitMonsterTag>(SpriteMask.Monster))
			   .Add(new DamageSystem())
			   .Add(new AttackSystem(EntityCount))
			   .Add(new MovementSystem())
			   .Add(new UpdateVelocitySystem())
			   .Add(new UpdateDataSystem());
		Systems.Initialize();

		for (var i = 0; i < EntityCount; i++)
		{
			var entity = W.NewEntity<UnitEntity>()
						  .Set(
							   new DataComponent(),
							   new UnitComponent
							   {
								   Value = new Unit
								   {
									   Id   = (uint)i,
									   Seed = (uint)i,
								   },
							   });
			entity.Set<SpawnTag>();
		}
	}

	protected override void DoRun(int tick)
	{
		Systems.Update();
		W.Tick();
	}

	protected override void DoCleanup()
	{
		Systems.Destroy();
		W.Destroy();
	}
}

}
