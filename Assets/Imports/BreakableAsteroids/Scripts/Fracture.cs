using UnityEngine;

namespace Imports.BreakableAsteroids.Scripts
{
    public class Fracture : MonoBehaviour
    {
        [Tooltip("\"Fractured\" is the object that this will break into")]
        public GameObject fractured;

        public void FractureObject()
        {
            var spawnedFractured = Instantiate(fractured, transform.position, transform.rotation); //Spawn in the broken version
            spawnedFractured.transform.localScale = transform.lossyScale;
            Destroy(gameObject); //Destroy the object to stop it getting in the way
        }
    }
}
