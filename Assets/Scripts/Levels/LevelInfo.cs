using System;
using Missions;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace Levels
{
    [CreateAssetMenu(fileName = "Level Info")]
    public class LevelInfo : ScriptableObject
    {
        [NonSerialized] public int Id;
        [SerializeField] public string LevelName;
        [SerializeField] public bool ShowPlanetInfo;
        [SerializeField] public bool OnlyShowInfo;
#if UNITY_EDITOR
        [SerializeField] public AudioClip LevelMusic;
#endif
        [SerializeField] public string LevelMusicPath;
        [SerializeField] public MissionData LevelMission;
        [SerializeField] public PlanetInfo Planet;
        [SerializeField] public float PlanetStartPosition;
        [SerializeField] [Range(0f, 1f)] public float PlanetHorizontalPosition;
        [SerializeField] public AnimationCurve PlanetSizeCurve;
        [SerializeField] public float PlanetLeaveTime;
        [SerializeField] public bool StopPlanetOnEnd;
        [SerializeField] public bool OverrideBackgroundImage;
        [SerializeField] public Sprite BackgroundImage;

#if UNITY_EDITOR
        [ContextMenu("Get music path")]
        public void GetMusicPath()
        {
            const string resourcesPath = "Resources/";
            if (LevelMusic == null)
            {
                Debug.LogError($"Level music {LevelName} is null!");
                return;
            }

            string fullPath = AssetDatabase.GetAssetPath(LevelMusic);
            int index = fullPath.IndexOf(resourcesPath);
            if (index > 0)
            {
                LevelMusicPath = fullPath.Substring(index + resourcesPath.Length);
                LevelMusicPath = Path.ChangeExtension(LevelMusicPath, null);
            }
            else
            {
                Debug.LogError($"{LevelName} - Cannot get music path from full path: {fullPath}! Check that it is in resources folder!");
            }
        }
#endif
    }
}