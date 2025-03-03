﻿using UnityEngine;

namespace Imports.SpaceShooterTemplate.Scripts
{
    /// <summary>
    /// This script moves the attached object along the Y-axis with the defined speed
    /// </summary>
    public class DirectMoving : MonoBehaviour {

        [Tooltip("Moving speed on Y axis in local space")]
        public float speed;

        private void Update()
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime)); 
        }
    }
}
