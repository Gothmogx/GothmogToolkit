using UnityEngine;

namespace GothmogToolkit.Tools.Helpers.Extensions
{
    public static class ComponentsExtensions
    {
        public static void SetActiveSafe(this Component component, bool active)
        {
            if (component)
                component.gameObject.SetActive(active);
        }

        public static void SetActiveSafe(this GameObject gameObject, bool active)
        {
            if (gameObject)
                gameObject.SetActive(active);
        }
        
        public static void DestroyGameObjectSafe(this Component component)
        {
            if (component) 
                component.gameObject.DestroyGameObjectSafe();
        }

        public static void DestroyGameObjectSafe(this GameObject gameObject)
        {
            if (gameObject)
                Object.Destroy(gameObject);
        }
    }
}