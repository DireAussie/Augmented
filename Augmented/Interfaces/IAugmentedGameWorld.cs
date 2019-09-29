﻿using Microsoft.Xna.Framework;

namespace Augmented.Interfaces
{
    public interface IAugmentedGameWorld
    {
        void Draw(Matrix view, Matrix projection);
        void LoadContent();
        void RecreateHeightMap();
        void Pick(Ray ray);
        void Action(Ray ray);
    }
}