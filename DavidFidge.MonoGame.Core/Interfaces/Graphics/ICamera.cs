﻿using Microsoft.Xna.Framework;

namespace DavidFidge.MonoGame.Core.Interfaces.Graphics
{
    public interface ICamera : ITransform
    {
        void Reset();
        Matrix View { get; }
        Matrix Projection { get; }
        void Update();
        void Initialise();
    }
}