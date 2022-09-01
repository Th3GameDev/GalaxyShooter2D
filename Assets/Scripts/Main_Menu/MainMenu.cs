using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField]
    private float _loadSpeed = 1f;

    private float _loadPercentage;

    //[SerializeField]
    //private TextMeshProUGUI _LoadingText;

    [SerializeField]
    private TextMeshProUGUI _percentText;


    [SerializeField]
    private float sceneTransitionTime = 1f;

    [SerializeField]
    private Image crossfadeEffect;

    private Animator anim;

    private bool isLoaded;


    [SerializeField]
    private Slider _loadingBar;

    [Header("Menu UI")]
    [SerializeField]
    private GameObject[] _uiElements = new GameObject[6];


    void Start()
    {
        anim = GetComponent<Animator>();
        _loadingBar.gameObject.SetActive(false);
        _percentText.gameObject.SetActive(false);
        _uiElements[0].SetActive(true);
        _uiElements[1].SetActive(true);
        _uiElements[2].SetActive(false);
        _uiElements[3].SetActive(false);
        _uiElements[4].SetActive(false);

    }

    void Update()
    {
        _percentText.text = Mathf.RoundToInt(_loadPercentage) + "%";

        if (_loadPercentage >= 100)
        {
            _loadPercentage = 100;

            StartCoroutine(SceneTransition());
        }
    }

    public void StartLoading()
    {
        _loadingBar.gameObject.SetActive(true);
        _percentText.gameObject.SetActive(true);

        _uiElements[0].SetActive(false);
        _uiElements[1].SetActive(false);
        _uiElements[2].SetActive(true);
        _uiElements[3].SetActive(true);
        _uiElements[4].SetActive(true);

        StartCoroutine(LoadingBar());
    }


    IEnumerator LoadingBar()
    {
        while (_loadPercentage != 100)
        {
            if (_loadPercentage < 50)
            {
                yield return new WaitForSeconds(0.3f);

                _loadPercentage = _loadingBar.value += 50;
            }
            else if (_loadPercentage >= 50 && _loadPercentage < 100)
            {
                yield return new WaitForSeconds(0.1f);

                _loadPercentage = _loadingBar.value += 10 * _loadSpeed * Time.deltaTime;
            }
            else if (_loadPercentage >= 100)
            {
                isLoaded = true;
                break;
            }
        }
    }

    IEnumerator SceneTransition()
    {

        anim.SetTrigger("FadeIn");

        yield return new WaitForSeconds(sceneTransitionTime);

        SceneManager.LoadScene(1);
    }


    /*
    //Old Menu
    public void LoadGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    */
}
