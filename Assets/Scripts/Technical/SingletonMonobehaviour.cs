//Credit Herman Tulleken:
//http://www.gamasutra.com/blogs/HermanTulleken/20160812/279100/50_Tips_and_Best_Practices_for_Unity_2016_Edition.php

using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) +
                       " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }
}
