using UnityEngine;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        //replaces any field of a vector with a new value. If no new value, it stays the same. returns a new Vector3
        public static Vector3 ReplaceField(this Vector3 original, float? newX = null, float? newY = null, float? newZ = null)
        {
            return new Vector3(newX ?? original.x, newY ?? original.y, newZ ?? original.z);
        }

        //checks if a gameObject has the component given and returns true
        public static bool HasComponent<T>(this GameObject obj)
        where T : Component
        {
            return obj.GetComponent<T>() != null;
        }

        //returns the current time in a minutes and seconds format. requires any gameObject
        public static string GetPassedTime(this GameObject obj)
        {
            float timer = Time.timeSinceLevelLoad;
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer - minutes * 60);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
