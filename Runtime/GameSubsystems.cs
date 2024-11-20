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

        private static GameSubsystems Instance { get; set; }
    }
}