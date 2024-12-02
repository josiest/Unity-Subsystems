using UnityEngine;

namespace Pi.Subsystems
{
    public class GameSubsystems : SubsystemLocator
    {
        //
        // Public Interface
        //
        public static T Find<T>() where T : GameSubsystem
        {
            return Instance?.GetComponent<T>();
        }
        
        //
        // Unity Events
        //
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            Instance = SubsystemLocators.CreateInstance<GameSubsystems>();
        }
        private void Awake()
        {
            DontDestroyOnLoad(this);
            foreach (var subsystemType in SubsystemLocators.GetAllSubsystemTypes<GameSubsystem>())
            {
                bool shouldLoad = true;
                var shouldLoadMethod = SubsystemLocators.ShouldLoadMethod(subsystemType);
                if (shouldLoadMethod != null && shouldLoadMethod.GetParameters().Length == 0)
                {
                    shouldLoad = shouldLoadMethod.Invoke(null, null) is true;
                }
                if (shouldLoad)
                {
                    Debug.Log($"Loading Game Subsystem {subsystemType.FullName}");
                    gameObject.AddComponent(subsystemType);
                }
                else
                {
                    Debug.Log($"Found Game Subsystem {subsystemType.FullName} but it's been specified not to load");
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