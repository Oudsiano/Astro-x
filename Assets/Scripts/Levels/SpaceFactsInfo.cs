using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "Space Facts Info")]
    public class SpaceFactsInfo : ScriptableObject
    {
        [SerializeField] public string[] Facts;
    }
}