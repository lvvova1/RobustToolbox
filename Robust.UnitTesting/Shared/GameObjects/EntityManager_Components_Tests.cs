using System.Linq;
using NUnit.Framework;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.UnitTesting.Server;

namespace Robust.UnitTesting.Shared.GameObjects
{
    [TestFixture, Parallelizable ,TestOf(typeof(EntityManager))]
    public class EntityManager_Components_Tests
    {
        private static readonly EntityCoordinates DefaultCoords = new(new EntityUid(1), Vector2.Zero);

        [Test]
        public void AddComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = new DummyComponent()
            {
                Owner = entity
            };

            // Act
            entMan.AddComponent(entity, component);

            // Assert
            var result = entMan.GetComponent<DummyComponent>(entity.Uid);
            Assert.That(result, Is.EqualTo(component));
        }

        [Test]
        public void AddComponentOverwriteTest()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = new DummyComponent()
            {
                Owner = entity
            };

            // Act
            entMan.AddComponent(entity, component, true);

            // Assert
            var result = entMan.GetComponent<DummyComponent>(entity.Uid);
            Assert.That(result, Is.EqualTo(component));
        }

        [Test]
        public void AddComponent_ExistingDeleted()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var firstComp = new DummyComponent {Owner = entity};
            entMan.AddComponent(entity, firstComp);
            entMan.RemoveComponent<DummyComponent>(entity.Uid);
            var secondComp = new DummyComponent { Owner = entity };

            // Act
            entMan.AddComponent(entity, secondComp);

            // Assert
            var result = entMan.GetComponent<DummyComponent>(entity.Uid);
            Assert.That(result, Is.EqualTo(secondComp));
        }

        [Test]
        public void HasComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.HasComponent<DummyComponent>(entity.Uid);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasNetComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();

            var factory = sim.Resolve<IComponentFactory>();
            var netId = factory.GetRegistration<DummyComponent>().NetID!;

            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.HasComponent(entity.Uid, netId.Value);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetNetComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();

            var factory = sim.Resolve<IComponentFactory>();
            var netId = factory.GetRegistration<DummyComponent>().NetID!;

            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.GetComponent(entity.Uid, netId.Value);

            // Assert
            Assert.That(result, Is.EqualTo(component));
        }

        [Test]
        public void TryGetComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.TryGetComponent<DummyComponent>(entity.Uid, out var comp);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(comp, Is.EqualTo(component));
        }

        [Test]
        public void TryGetNetComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();

            var factory = sim.Resolve<IComponentFactory>();
            var netId = factory.GetRegistration<DummyComponent>().NetID!;

            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.TryGetComponent(entity.Uid, netId.Value, out var comp);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(comp, Is.EqualTo(component));
        }

        [Test]
        public void RemoveComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            entMan.RemoveComponent<DummyComponent>(entity.Uid);
            entMan.CullRemovedComponents();

            // Assert
            Assert.That(entMan.HasComponent(entity.Uid, component.GetType()), Is.False);
        }

        [Test]
        public void RemoveNetComponentTest()
        {
            // Arrange
            var sim = SimulationFactory();

            var factory = sim.Resolve<IComponentFactory>();
            var netId = factory.GetRegistration<DummyComponent>().NetID!;

            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            entMan.RemoveComponent(entity.Uid, netId.Value);
            entMan.CullRemovedComponents();

            // Assert
            Assert.That(entMan.HasComponent(entity.Uid, component.GetType()), Is.False);
        }

        [Test]
        public void GetComponentsTest()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.GetComponents<DummyComponent>(entity.Uid);

            // Assert
            var list = result.ToList();
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(component));
        }

        [Test]
        public void GetAllComponentsTest()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.EntityQuery<DummyComponent>(true);

            // Assert
            var list = result.ToList();
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(component));
        }

        [Test]
        public void GetAllComponentInstances()
        {
            // Arrange
            var sim = SimulationFactory();
            var entMan = sim.Resolve<IEntityManager>();
            var entity = entMan.SpawnEntity(null, DefaultCoords);
            var component = entity.AddComponent<DummyComponent>();

            // Act
            var result = entMan.GetComponents(entity.Uid);

            // Assert
            var list = result.Where(c=>c.Name == "Dummy").ToList();
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(component));
        }

        private static ISimulation SimulationFactory()
        {
            var sim = RobustServerSimulation
                .NewSimulation()
                .RegisterComponents(factory => factory.RegisterClass<DummyComponent>())
                .InitializeInstance();

            // Adds the map with id 1, and spawns entity 1 as the map entity.
            sim.AddMap(1);

            return sim;
        }

        [NetworkedComponent()]
        private class DummyComponent : Component, ICompType1, ICompType2
        {
            public override string Name => "Dummy";
        }

        private interface ICompType1 { }

        private interface ICompType2 { }
    }
}
