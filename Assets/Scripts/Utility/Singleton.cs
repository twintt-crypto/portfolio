using UnityEngine;
using System.Collections;

public class SingletonInMemory<T> where T : new()
{
    protected static T m_instance = default(T);

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new T();
            }

            return m_instance;
        }
    }
}

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T m_instance = null;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<T>();

                if (m_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    m_instance = obj.AddComponent<T>();
                }
            }

            return m_instance;
        }
    }

    public static bool IsDestroy()
    {
        return m_instance == null;
    }
}