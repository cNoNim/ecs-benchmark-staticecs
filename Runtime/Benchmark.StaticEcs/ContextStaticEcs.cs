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
		 .Component<DataComponent>()
		 .Component<UnitComponent>()
		 .Component<HealthComponent>()
		 .Component<DamageComponent>()
		 .Component<SpriteComponent>()
		 .Component<PositionComponent>()
		 .Component<VelocityComponent>()
		 .Component<AttackComponent>()
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
			var entity = W.NewEntity<Default>()
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
