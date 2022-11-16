using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<T>();
/*            if (instance == null)
                Debug.Log("Singleton<" + typeof(T) + "> instance has been not found.");*/
            return instance;
        }
    }

    protected void Awake()
    {
        if (instance == null)
            instance = this as T;
        else if (instance != this)
            DestroySelf();
    }

    public void DestroySelf()
    {
        print("Destroying singleton" + this.name);

        if (Application.isPlaying)
            Destroy(this);
        else
            DestroyImmediate(this);
    }

    private void OnDestroy()
    {
        instance = null;
    }
}