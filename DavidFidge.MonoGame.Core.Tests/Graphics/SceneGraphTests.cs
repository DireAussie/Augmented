﻿using System.Linq;

using DavidFidge.MonoGame.Core.Components;
using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Tests.Services;
using DavidFidge.TestInfrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

using IDrawable = DavidFidge.MonoGame.Core.Graphics.IDrawable;

namespace DavidFidge.MonoGame.Core.Tests.Graphics
{
    [TestClass]
    public class SceneGraphTests : BaseTest
    {
        private SceneGraph _sceneGraph;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _sceneGraph = new SceneGraph();
        }

        [TestMethod]
        public void Should_Call_LoadContent_Using_BreadthFirst()
        {
            // Arrange
            var testEntityRoot = new TestEntity();
            var testEntityChild1 = new TestEntity();
            var testEntityChild2 = new TestEntity();

            testEntityRoot.NodeAfter = testEntityChild1;
            testEntityChild1.NodeBefore = testEntityRoot;
            testEntityChild1.NodeAfter = testEntityChild2;
            testEntityChild2.NodeBefore = testEntityChild1;

            _sceneGraph.Initialise(testEntityRoot);
            _sceneGraph.Add(testEntityChild1, testEntityRoot);
            _sceneGraph.Add(testEntityChild2, testEntityRoot);

            // Act
            _sceneGraph.LoadContent();

            // Assert
            Assert.IsTrue(testEntityRoot.HasLoadContentBeenCalled);
            Assert.IsTrue(testEntityChild1.HasLoadContentBeenCalled);
            Assert.IsTrue(testEntityChild2.HasLoadContentBeenCalled);
        }

        [TestMethod]
        public void Should_Call_Draw_With_Correct_Parameters()
        {
            // Arrange
            var testEntity = new TestEntity
            {
                WorldTransform = new EntityTransform()
            };

            _sceneGraph.Initialise(testEntity);

            // Act
            _sceneGraph.Draw(Matrix.Identity, Matrix.Identity * 2);

            // Assert
            Assert.IsTrue(testEntity.HasDrawBeenCalled);
            Assert.AreEqual(Matrix.Identity,testEntity.View);
            Assert.AreEqual(Matrix.Identity * 2,testEntity.Projection);
        }

        [TestMethod]
        public void Should_Call_Draw_Using_BreadthFirst()
        {
            // Arrange
            var testEntityRoot = new TestEntity();
            var testEntityChild1 = new TestEntity();
            var testEntityChild2 = new TestEntity();

            testEntityRoot.NodeAfter = testEntityChild1;
            testEntityChild1.NodeBefore = testEntityRoot;
            testEntityChild1.NodeAfter = testEntityChild2;
            testEntityChild2.NodeBefore = testEntityChild1;

            _sceneGraph.Initialise(testEntityRoot);
            _sceneGraph.Add(testEntityChild1, testEntityRoot);
            _sceneGraph.Add(testEntityChild2, testEntityRoot);

            // Act
            _sceneGraph.Draw(Matrix.Identity, Matrix.Identity);

            // Assert
            Assert.IsTrue(testEntityRoot.HasDrawBeenCalled);
            Assert.IsTrue(testEntityChild1.HasDrawBeenCalled);
            Assert.IsTrue(testEntityChild2.HasDrawBeenCalled);
        }

        [TestMethod]
        public void NonIntersecting_Entity_Should_Be_Deselected()
        {
            // Arrange
            var nonIntersectingEntity = new TestSelectEntity();

            _sceneGraph.Initialise(nonIntersectingEntity);

            // Act
            var result = _sceneGraph.Select(new Ray(Vector3.Zero, Vector3.Zero));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Intersecting_Entities_Should_Return_Entity_With_Shortest_Intersecting_Distance()
        {
            // Arrange
            var entity1 = new TestSelectEntity { IntersectsReturnValue = 0.5f };
            var entity2 = new TestSelectEntity { IntersectsReturnValue = 0.4f };

            _sceneGraph.Initialise(entity1);
            _sceneGraph.Add(entity2, entity1);

            // Act
            var result = _sceneGraph.Select(new Ray(Vector3.Zero, Vector3.Zero));

            // Assert
            Assert.AreEqual(entity2, result);
        }

        [TestMethod]
        public void Remove_Should_Remove_Node_From_SceneGraph()
        {
            // Arrange
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();
            var entity3 = new TestEntity();
            var entity4 = new TestEntity();

            _sceneGraph.Initialise(entity1);
            _sceneGraph.Add(entity2, entity1);
            _sceneGraph.Add(entity3, entity2);
            _sceneGraph.Add(entity4, entity1);

            // Act
            _sceneGraph.Remove(entity2);

            // Assert
            var entitiesInGraph = _sceneGraph.GetEntitiesByBreadthFirstSearch();
            Assert.AreEqual(2, entitiesInGraph.Count);
            Assert.AreEqual(entity1, entitiesInGraph[0]);
            Assert.AreEqual(entity4, entitiesInGraph[1]);
        }

        [TestMethod]
        public void GetWorldTransform_Should_Get_Transforms_From_All_Parents()
        {
            // Arrange
            var entity1 = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX))
            };

            var entity2 = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX))
            };

            var entity3 = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX))
            };

            var entityNonAncestor = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX))
            };

            _sceneGraph.Initialise(entity1);
            _sceneGraph.Add(entity2, entity1);
            _sceneGraph.Add(entity3, entity2);
            _sceneGraph.Add(entityNonAncestor, entity1);

            // Act
            var result = _sceneGraph.GetWorldTransform(entity3);

            // Assert
            var expectedTransform = Matrix.CreateTranslation(new Vector3(3, 0, 0));

            Assert.That.AreEquivalent(expectedTransform, result);
        }

        [TestMethod]
        public void GetWorldTransformWithLocalTransform_Should_Get_Transforms_From_All_Parents_With_LocalTransform_Only_From_Called_Entity()
        {
            // Arrange
            var entity1 = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX)),
                LocalTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitZ))
            };

            var entity2 = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX))
            };

            var entity3 = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX)),
                LocalTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitY))
            };

            var entityNonAncestor = new TestEntity
            {
                WorldTransform = new EntityTransform(Matrix.CreateTranslation(Vector3.UnitX))
            };

            _sceneGraph.Initialise(entity1);
            _sceneGraph.Add(entity2, entity1);
            _sceneGraph.Add(entity3, entity2);
            _sceneGraph.Add(entityNonAncestor, entity1);

            // Act
            var result = _sceneGraph.GetWorldTransformWithLocalTransform(entity3);

            // Assert
            var expectedTransform = Matrix.CreateTranslation(new Vector3(3, 1, 0));

            Assert.That.AreEquivalent(expectedTransform, result);
        }

        public class TestSelectEntity : Entity, ISelectable
        {
            public bool IsSelected { get; set; }
            public float? IntersectsReturnValue { get; set; }

            public float? Intersects(Ray ray, Matrix worldTransform)
            {
                return IntersectsReturnValue;
            }
        }

        public class TestEntity : Entity, ILoadContent, IDrawable
        {
            public Matrix View { get; set; }
            public Matrix Projection { get; set; }
            public Matrix World { get; set; }
            public TestEntity NodeBefore { get; set; }
            public TestEntity NodeAfter { get; set; }

            public bool HasLoadContentBeenCalled { get; private set; }
            public bool HasDrawBeenCalled { get; private set; }

            public void LoadContent()
            {
                HasLoadContentBeenCalled = true;

                if (NodeBefore != null)
                    Assert.IsTrue(NodeBefore.HasLoadContentBeenCalled);

                if (NodeAfter != null)
                    Assert.IsFalse(NodeAfter.HasLoadContentBeenCalled);
            }

            public void Draw(Matrix view, Matrix projection, Matrix world)
            {
                Projection = projection;
                View = view;
                World = world;
                HasDrawBeenCalled = true;

                if (NodeBefore != null)
                    Assert.IsTrue(NodeBefore.HasDrawBeenCalled);

                if (NodeAfter != null)
                    Assert.IsFalse(NodeAfter.HasDrawBeenCalled);
            }
        }
   }
}