using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public int maxDistance = 70;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateEvent(int eventID)
    {
        if (eventID == 1)
        {
            SceneManager.LoadScene("AR mode");
        }
        else if (eventID == 2)
        {
            SceneManager.LoadScene("AR mode");
        }
        else if (eventID == 3)
        {
            SceneManager.LoadScene("AR mode");
        }
        else if (eventID == 4)
        {
            SceneManager.LoadScene("AR mode");
        }
        else if (eventID == 5)
        {
            SceneManager.LoadScene("AR mode");
        }
        else if (eventID == 6)
        {
            SceneManager.LoadScene("AR mode");
        }
    }
}
