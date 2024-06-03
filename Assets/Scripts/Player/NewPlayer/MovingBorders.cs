using System;
using UnityEngine;

namespace Player.NewPlayer
{
    [CreateAssetMenu]
    public class MovingBorders : ScriptableObject
    {
        public float MinXOffset = 1.5f;
        public float MaxXOffset = 1.5f;
        public float MinYOffset = 1.5f;
        public float MaxYOffset = 1.5f;
        
        [NonSerialized] public float MinX;
        [NonSerialized] public float MaxX;
        [NonSerialized] public float MinY;
        [NonSerialized] public float MaxY;
    }
}