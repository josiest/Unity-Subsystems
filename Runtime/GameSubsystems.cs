using UnityEngine;

namespace Pi.Subsystems
{
    public class GameSubsystems : SubsystemLocator
    {
        //
        // Public Interface
        //
        public static T FindOrRegister<T>() where T : GameSubsystem
        {
            var subsystem = Instance?.GetComponent<T>();
            return subsystem? subsystem : Instance?.gameObject.AddComponent<T>();
        }
        public static T Find<T>() where T : GameSubsystem
        {
            return Instance?.GetComponent<T>();
        }
        
        //
        // Unity Events
        //
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            Debug.Log("[Pi.Subsystems.Game] Creating Game Subsystem Locator");
            Instance = SubsystemLocators.CreateInstance<GameSubsystems>();
        }
        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(this); return; }
            Instance = this;

            DontDestroyOnLoad(this);
            foreach (var subsystemType in SubsystemLocators.GetAllSubsystemTypes<GameSubsystem>())
            {
                bool shouldLoad = true;
                var shouldLoadMethod = SubsystemLocators.ShouldLoadMethod(subsystemType);
                if (shouldLoadMethod != null && shouldLoadMethod.GetParameters().Length == 0)
                {
                    shouldLoad = shouldLoadMethod.Invoke(null, null) is true;
                }
                if (shouldLoad && !GetComponent(subsystemType))
                {
                    Debug.Log($"[Pi.Subsystems.Game] Loading Game Subsystem {subsystemType.FullName}");
                    gameObject.AddComponent(subsystemType);
                }
                else if (!shouldLoad)
                {
                    Debug.Log($"[Pi.Subsystems.Game] Found Game Subsystem {subsystemType.FullName} " +
                               "but it's been specified not to load");
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

        private static GameSubsystems Instance { get; set; }
    }
}