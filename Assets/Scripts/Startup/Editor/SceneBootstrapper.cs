using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Startup.Editor
{
    [InitializeOnLoad]
    public class SceneBootstrapper
    {
        private const string PreviousSceneKey = "PreviousScene";
        private const string ShouldLoadBootstrapKey = "LoadBootstrapScene";

        private const string LoadBootstrapMenuKey = "Tools/Load Bootstrap Scene On Play";
        private const string DontLoadBootstrapMenuKey = "Tools/Don't Load Bootstrap Scene On Play";

        private static string BootstrapScene => EditorBuildSettings.scenes[0].path;

        private static string PreviousScene
        {
            get => EditorPrefs.GetString(PreviousSceneKey);
            set => EditorPrefs.SetString(PreviousSceneKey, value);
        }

        private static bool ShouldLoadBootstrapScene
        {
            get => EditorPrefs.GetBool(ShouldLoadBootstrapKey, true);
            set => EditorPrefs.SetBool(ShouldLoadBootstrapKey, value);
        }

        static SceneBootstrapper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (!ShouldLoadBootstrapScene) return;

            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                PreviousScene = SceneManager.GetActiveScene().path;
                
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() && IsSceneInBuildSettings(BootstrapScene))
                    EditorSceneManager.OpenScene(BootstrapScene);
            }
            else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                if (!string.IsNullOrEmpty(PreviousScene))
                    EditorSceneManager.OpenScene(PreviousScene);
            }
        }

        // These menu items toggle behavior.

        // This adds a menu item called "Load Bootstrap Scene On Play" to the GameSystems menu and
        // enables the behavior if selected.
        [MenuItem(LoadBootstrapMenuKey)]
        private static void EnableBootstrapper()
        {
            ShouldLoadBootstrapScene = true;
        }

        // Validates the above function and menu item, which grays out if ShouldLoadBootstrapScene is true.
        [MenuItem(LoadBootstrapMenuKey, true)]
        private static bool ValidateEnableBootstrapper()
        {
            return !ShouldLoadBootstrapScene;
        }

        // Adds a menu item called "Don't Load Bootstrap Scene On Play" to the GameSystems menu and
        // disables the behavior if selected.
        [MenuItem(DontLoadBootstrapMenuKey)]
        private static void DisableBootstrapper()
        {
            ShouldLoadBootstrapScene = false;
        }

        // Validates the above function and menu item, which grays out if ShouldLoadBootstrapScene is false.
        [MenuItem(DontLoadBootstrapMenuKey, true)]
        private static bool ValidateDisableBootstrapper()
        {
            return ShouldLoadBootstrapScene;
        }

        private static bool IsSceneInBuildSettings(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
                return false;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == scenePath)
                    return true;
            }

            return false;
        }
    }
}