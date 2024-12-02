using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pi.Subsystems
{
    /**
     * A singleton whose lifetime is managed by the lifetime of a scene.
     *
     * By default, all scene subsystems will always be loaded before the scene starts.
     * You can change this behavior for a scene subsystem by defining its static ShouldLoad method.
     *
     * ShouldLoad has two signatures that you can choose to define:
     *
     * 1. static bool ShouldLoad(Scene scene)
     * 2. static bool ShouldLoad()
     *
     * You should only define one method, but ShouldLoad(Scene) will take precedence over ShouldLoad() -
     * meaning that if you define both, only ShouldLoad(Scene) will be called to determine if the scene
     * subsystem should be loaded.
     *
     * If ShouldLoad is not defined, or not defined with one of these two signatures, it will be ignored and
     * the subsystem will always load.
     */
    public class SceneSubsystems : SubsystemLocator
    {
        //
        // Public Interface
        //
        public static T FindOrRegister<T>() where T : SceneSubsystem
        {
            var subsystem = Instance?.GetComponent<T>();
            return subsystem? subsystem : Instance?.gameObject.AddComponent<T>();
        }
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
            if (Instance != this) { Destroy(this); return; }
            foreach (var subsystemType in SubsystemLocators.GetAllSubsystemTypes<SceneSubsystem>())
            {
                bool shouldLoad = true; 
                var shouldLoadMethod = SubsystemLocators.ShouldLoadMethod(subsystemType);
                if (shouldLoadMethod != null)
                {
                    var methodParams = shouldLoadMethod.GetParameters();
                    if (methodParams.Length == 1 && methodParams[0].ParameterType.IsSubclassOf(typeof(Scene)))
                    {
                        shouldLoad = shouldLoadMethod.Invoke(null, new[] { (object)gameObject.scene }) is true;
                    }
                    else if (methodParams.Length == 0)
                    {
                        shouldLoad = shouldLoadMethod.Invoke(null, null) is true;
                    }
                }
                if (shouldLoad)
                {
                    gameObject.AddComponent(subsystemType);
                }
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