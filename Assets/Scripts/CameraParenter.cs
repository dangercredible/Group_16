using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPSStarter
{
    /// <summary>
    /// Automatically builds a first-person player with camera in every gameplay
    /// scene, exactly like the Framework's FpsStarterBootstrap.CreatePlayer().
    ///
    /// Uses [RuntimeInitializeOnLoadMethod] so it runs without needing any
    /// component in the scene — just having this script in the project is enough.
    ///
    /// Skips menu scenes ("Main Menu", "PauseMenu", "LoadingScene") so they
    /// keep their own cameras.
    /// </summary>
    public static class PlayerBuilder
    {
        private static readonly string[] SkipScenes = { "Main Menu", "PauseMenu", "LoadingScene" };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            foreach (string skip in SkipScenes)
                if (sceneName == skip) return;

            // Don't double-build if a player already exists.
            if (GameObject.FindWithTag("Player") != null) return;
            if (Object.FindFirstObjectByType<FirstPersonController>() != null) return;

            Build();
        }

        private static void Build()
        {
            // Disable all existing cameras (the orphan "Main Camera" etc.).
            foreach (Camera cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
                cam.gameObject.SetActive(false);

            // --- Player root with capsule ---
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1f, -7f);

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.radius = 0.35f;
            cc.height = 1.8f;

            // --- View Camera child at eye height ---
            GameObject viewObj = new GameObject("View Camera");
            viewObj.transform.SetParent(player.transform);
            viewObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            viewObj.transform.localRotation = Quaternion.identity;
            viewObj.tag = "MainCamera";

            Camera camera = viewObj.AddComponent<Camera>();
            camera.nearClipPlane = 0.03f;
            camera.backgroundColor = new Color(0.035f, 0.035f, 0.035f);

            viewObj.AddComponent<AudioListener>();

            // Add FPS components AFTER the camera child exists
            // so FirstPersonController.Awake() finds it via GetComponentInChildren<Camera>().
            player.AddComponent<FirstPersonController>();
            player.AddComponent<PlayerInteractor>();
        }
    }
}
