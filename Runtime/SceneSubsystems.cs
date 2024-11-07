using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pi.Subsystems
{
    public class SceneSubsystems : SubsystemLocator
    {
        //
        // Public Interface
        //
        public static T Find<T>() where T : SceneSubsystem
        {
            return Instance?.GetComponent<T>();
        }

        //
        // Unity Events
        //
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnApplicationQuit()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void Awake()
        {
            foreach (var subsystemType in SubsystemLocators.GetAllSubsystemTypes<SceneSubsystem>())
            {
                gameObject.AddComponent(subsystemType);
            }
        }
        private void OnDestroy()
        {
            if (Instance == this) { Instance = null; }
        }

        //
        // Internal Interface
        //
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!Instance) { Instance = SubsystemLocators.CreateInstance<SceneSubsystems>(); }
        }
        private static SceneSubsystems Instance { get; set; }
    }
}