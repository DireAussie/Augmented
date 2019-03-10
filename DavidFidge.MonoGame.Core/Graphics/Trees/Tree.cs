﻿using System;
using System.Collections.Generic;
using System.Linq;

using Castle.Components.DictionaryAdapter.Xml;

using DavidFidge.MonoGame.Core.Graphics.Models;
using DavidFidge.MonoGame.Core.Interfaces.Components;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DavidFidge.MonoGame.Core.Graphics.Trees
{
    public class Tree : BaseModelTemplate
    {
        public Tree(IGameProvider gameProvider) : base(gameProvider)
        {
        }

        public void LoadContent(string woodTexture)
        {
            var trunk = new Trunk(_gameProvider, woodTexture)
            {
                Height = 30f,
                Radius = 10f,
                TrunkCircumferenceVertexCount = 8,
                TrunkHeightVertexCount = 2
            };

            var trunkModelMeshPart = trunk.CreateModelMeshPart(_gameProvider);

            var modelMesh = new ModelMesh(_gameProvider.Game.GraphicsDevice, new List<ModelMeshPart> { trunkModelMeshPart });

            var modelBone = new ModelBone
            {
                ModelTransform = Matrix.Identity,
                Transform = Matrix.Identity
            };

            modelMesh.ParentBone = new ModelBone();

            var model = new Model(
                _gameProvider.Game.GraphicsDevice,
                new List<ModelBone> { modelBone },
                new List<ModelMesh> { modelMesh }
            );

            trunk.CreateTrunkEffect(trunkModelMeshPart);

            LoadContent(model);
        }
    }
}
