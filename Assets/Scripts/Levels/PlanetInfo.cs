using System.Collections.Generic;
using LevelObjects;
using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "Planet Info")]
    public class PlanetInfo : ScriptableObject
    {
        [Header("Game info")]
        [SerializeField] public float Size;
        [SerializeField] public float RotationSpeed;
        [SerializeField] public Texture2D Texture;
        [SerializeField] public Planet Prefab;

        [Header("Preview info")]
        [SerializeField] public List<PlanetInfoSlide> InfoSlides;
    }
}