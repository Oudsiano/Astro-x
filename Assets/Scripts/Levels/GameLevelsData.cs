using UnityEngine;

namespace Levels
{
    [CreateAssetMenu(fileName = "Game Levels Data")]
    public class GameLevelsData : ScriptableObject
    {
        [SerializeField] public LevelInfo[] Levels;

        public void Initialize()
        {
            for (int i = 0; i < Levels.Length; i++)
            {
                Levels[i].Id = i;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Calculate scores")]
        private void CalculateAllScores()
        {
            int totalScore = 0;
            foreach (var level in Levels)
            {
                if (level.LevelMission == null) continue;

                totalScore += level.LevelMission.RewardScoresForFullMission;
                foreach (var stage in level.LevelMission.Stages)
                {
                    if (stage.SpawnObjectData == null) continue;
                    if (stage.SpawnObjectData.SpawnObjects == null || stage.SpawnObjectData.SpawnObjects.Length == 0) continue;

                    totalScore += stage.SpawnObjectData.ScoresForDestroy * stage.ObjectsCount;
                }
            }
            
            Debug.Log($"Total scores for all missions: {totalScore}");
        }
#endif
    }
}