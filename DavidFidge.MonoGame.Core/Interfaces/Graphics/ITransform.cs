﻿using Microsoft.Xna.Framework;

namespace DavidFidge.MonoGame.Core.Interfaces.Graphics
{
    public interface ITransform
    {
        void ChangeTranslationRelative(Vector3 translationDelta);
        void ChangeTranslation(Vector3 translation);
        void ChangeScaleRelative(Vector3 scaleDelta);
        void ChangeScale(Vector3 scale);
        void ChangeRotationRelative(Quaternion quaternion);
        void ChangeRotation(Quaternion quaternion);

        Vector3 Translation { get; }
        Matrix TranslationMatrix { get; }

        Vector3 Scale { get; }
        Matrix ScaleMatrix { get; }

        Quaternion Rotation { get; }

        Matrix Transform { get; }
    }
}
