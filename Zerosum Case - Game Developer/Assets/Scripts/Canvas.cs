using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Canvas : MonoBehaviour
{
    [SerializeField] GameObject progressIndicator;
    [SerializeField] GameObject levelCompletePanel;
    public GameObject coin;
    GameObject levelFailPanel;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject collectCoinButton;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject levelBar;
    [SerializeField] GameObject level1;
    [SerializeField] GameObject level2;
    [SerializeField] GameObject level3;
    [SerializeField] GameObject level4;
    [SerializeField] GameObject level5;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] RectTransform coinRect;
    [SerializeField] RectTransform complateImage;
    [SerializeField] Text levelText;
    [SerializeField] Text coinCount;
    [SerializeField] Slider slider;
    [SerializeField] PlayerSettings settings;

    void Start()
    {
        if (settings.level == 5)
        {
            settings.level = 0;
        }

        settings.newCoin = settings.coin;

        levelCompletePanel.SetActive(false);
        levelBar.SetActive(true);
        levelText.text = "Level " + (settings.level + 1).ToString();
        nextButton.SetActive(false);
        collectCoinButton.SetActive(false);
        coin.SetActive(true);

        if (settings.level == 0)
        {
            level5.SetActive(false);
            level1.SetActive(true);
        }

        if (settings.level == 1)
        {
            level1.SetActive(false);
            level2.SetActive(true);
        }

        if (settings.level == 2)
        {
            level2.SetActive(false);
            level3.SetActive(true);
        }

        if (settings.level == 3)
        {
            level3.SetActive(false);
            level4.SetActive(true);
        }

        if (settings.level == 4)
        {
            level4.SetActive(false);
            level5.SetActive(true);
        }
    }

    void Update()
    {
        settings.sensitivity = slider.value;
    }

    public void Sensitivity()
    {
        transform.localEulerAngles = new Vector3(0, slider.value, 0);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        levelBar.SetActive(false);
        coin.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        levelBar.SetActive(true);
        coin.SetActive(true);
    }

    public void CollectCoinButton()
    {
        levelCompletePanel.SetActive(true);
        complateImage.DOAnchorPos(new Vector2(0, 0), 0.35f);
        StartCoroutine(NewCoin());
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Zerosum Case");
    }

    public void NextLevel()
    {
        settings.level++;
        SaveLoadPrefs();
        SceneManager.LoadScene("Zerosum Case");

    }

    public void SaveLoadPrefs()
    {
        PlayerPrefs.SetInt("level", settings.level);
        PlayerPrefs.SetInt("coin", settings.coin);
        PlayerPrefs.SetInt("newCoin", settings.newCoin);
        PlayerPrefs.SetFloat("sensitivity", settings.sensitivity);
    }

    public void levelComplete()
    {
        pauseButton.SetActive(false);
        collectCoinButton.SetActive(true);
        progressIndicator.SetActive(false);
        levelBar.SetActive(false);
        nextButton.SetActive(true);
    }

    public IEnumerator levelFail()
    {
        yield return new WaitForSeconds(0.2f);
        pauseButton.SetActive(false);
        progressIndicator.SetActive(false);
        levelFailPanel.SetActive(true);
        levelBar.SetActive(false);
    }

    public void CoinCollect()
    {
        GameObject obj = Instantiate(coinPrefab, transform);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(150, 300), Random.Range(-400, -250));
        obj.transform.DOMove(coinRect.transform.position, 0.5f).SetDelay(Random.Range(0.5f, 0.75f));
        obj.GetComponent<RectTransform>().DOScale(new Vector2(0.75f, 0.75f), 0.75f).SetDelay(Random.Range(0.5f, 0.75f));
        Destroy(obj, 1.2f);
    }

    public IEnumerator NewCoin()
    {
        yield return new WaitForSeconds(0.45f);
        while (settings.newCoin < settings.coin)
        {
            settings.newCoin += 5;
            if (settings.newCoin > settings.coin)
            {
                settings.newCoin = settings.coin;
            }
            coinCount.text = settings.newCoin.ToString();
            yield return new WaitForSeconds(0.04f);
        }

        yield return new WaitForSeconds(0.5f);
    }
}
