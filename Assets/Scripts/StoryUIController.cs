using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryUIController : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public Button nextPageButton;
    public Button startGameButton;

    [TextArea(3, 10)]
    public string[] pages;  // ÿһҳ�Ĺ����ı�

    private int currentPageIndex = 0;
    private float fadeDuration = 1.0f;

    void Start()
    {
        nextPageButton.onClick.AddListener(NextPage);
        startGameButton.onClick.AddListener(StartGame);
        startGameButton.gameObject.SetActive(false);
        StartCoroutine(ShowTextWithFade(pages[currentPageIndex]));
    }

    void NextPage()
    {
        currentPageIndex++;

        if (currentPageIndex < pages.Length)
        {
            StartCoroutine(ShowTextWithFade(pages[currentPageIndex]));
        }
        else
        {
            storyText.text = "";
            nextPageButton.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(true);
        }
    }

    IEnumerator ShowTextWithFade(string newText)
    {
        // ����
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            storyText.alpha = alpha;
            yield return null;
        }

        storyText.text = newText;

        // ����
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            storyText.alpha = alpha;
            yield return null;
        }
    }

    void StartGame()
    {
        // ���� UI��������ҿ��ƻ������ʽ����
        gameObject.SetActive(false);
        // �����������������ʼ��Ϸ����������������͵ȣ����Ե��ã�
        // GameManager.Instance.StartLevel();
    }
}
