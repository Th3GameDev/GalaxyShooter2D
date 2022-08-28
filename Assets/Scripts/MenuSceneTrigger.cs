using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneTrigger : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    private MainMenu _mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        _mainMenu = GameObject.Find("MenuCanvas").GetComponent<MainMenu>();

        if (_mainMenu == null)
        {
            Debug.LogWarning("MainMenu Script is Null");
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("VC_Camera"))
        {
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = Color.black;
            _camera.cullingMask = 0;

            if (_mainMenu != null)
            {
                _mainMenu.StartLoading();
            }

            //SceneManager.LoadScene(1, LoadSceneMode.Single);
            //Debug.Log("Load Game");
        }
    }
}
