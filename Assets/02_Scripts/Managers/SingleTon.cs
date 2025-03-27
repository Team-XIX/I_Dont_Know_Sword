using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //FindObject Type T
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    //if instance == null Create Instance as SingleTon
                    string name = typeof(T).ToString();
                    var singletonObj = new GameObject(name);
                    _instance = singletonObj.AddComponent<T>();
                    DontDestroyOnLoad(singletonObj);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        //gameObject to rootObject
        if (transform.root != null || transform.parent != null)
            DontDestroyOnLoad(transform.root);
        else if (_instance != null)
            Destroy(this.gameObject);
    }

}