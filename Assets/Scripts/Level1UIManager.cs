using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class Level1UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject startPanel;
    public GameObject winPanel;
    public TMP_Text coinText;
    public TMP_Text storyText;
    public Button nextButton;
    public Button startButton;

    [Header("Story Pages")]
    [TextArea(3, 5)]
    public string[] storyPages;

    private int currentPage = 0;

    private void Start()
    {
        winPanel.SetActive(false);
        startPanel.SetActive(true);

        nextButton.onClick.AddListener(NextPage);
        startButton.onClick.AddListener(HideStartPanel);

        currentPage = 0;

        // ��ʼֻ��ʾ��һҳ��ť
        startButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);

        // ��֤ storyText ����
        if (storyText != null)
        {
            storyText.text = "";
            storyText.alpha = 0f;
        }

        UpdateStoryText();

        // 新增：刷新金币UI
        CoinManager coinManager = FindObjectOfType<CoinManager>();
        if (coinManager != null)
        {
            UpdateCoinText(0, coinManager.totalCoinsRequired);
        }
    }

    private void UpdateStoryText()
    {
        if (storyPages.Length > 0 && currentPage < storyPages.Length)
        {
            StopAllCoroutines(); // ֹͣ����δ��ɵ�Э��
            StartCoroutine(FadeInStoryText(storyPages[currentPage]));

            // �����һҳʱ��ʾ����ʼ��Ϸ����ť
            if (currentPage == storyPages.Length - 1)
            {
                nextButton.gameObject.SetActive(false);
                startButton.gameObject.SetActive(true);
            }
            else
            {
                nextButton.gameObject.SetActive(true);
                startButton.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator FadeInStoryText(string textToShow)
    {
        float duration = 1.0f;
        float timer = 0f;

        storyText.text = textToShow;
        storyText.alpha = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            storyText.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        storyText.alpha = 1f;
    }

    public void NextPage()
    {
        currentPage++;
        UpdateStoryText();
    }

    public void HideStartPanel()
    {
        startPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateCoinText(int current, int total)
    {
        if (coinText != null)
            coinText.text = $"���: {current} / {total}";
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }

    public void GoToNextLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Level2");
        }
    }
}
