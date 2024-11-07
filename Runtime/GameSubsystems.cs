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

        private static GameSubsystems Instance { get; set; }
    }
}