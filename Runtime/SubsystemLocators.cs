using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Pi.Subsystems
{
    /** Utilities for creating and querying subsystem locators */
    public static class SubsystemLocators
    {
        /**
         * <summary>Create a game object with a subsystem locator component</summary>
         * <returns>the new subsystem locator component</returns>
         * <remarks>The game object will have the name of the subsystem locator</remarks>
         */
        public static T CreateInstance<T>() where T : SubsystemLocator
        {
            var locatorType = typeof(T);
            var locatorObject = new GameObject(locatorType.Name, locatorType);
            return locatorObject.GetComponent<T>();
        }

        /** Get all concrete subtypes of a kind of subsystem */
        public static IEnumerable<Type> GetAllSubsystemTypes<T>() where T : SubsystemBase
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)));
        }

        /** Get the method for determining if a subsystem should be loaded */
        public static MethodInfo ShouldLoadMethod(Type type)
        {
            var shouldLoadMethod = type.GetMethod("ShouldLoad", BindingFlags.Static);
            if (shouldLoadMethod == null) { return null; }
            if (shouldLoadMethod.ReturnType != typeof(bool)) { return null; }
            return shouldLoadMethod;
        }
    }
}