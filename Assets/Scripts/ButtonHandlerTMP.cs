using UnityEngine;
using TMPro;
using System.Collections;

public class ButtonHandlerTMP : MonoBehaviour
{
    public TMP_Text resultText;
    public GameObject door;

    public void OnCorrectAnswer()
    {
        if (resultText != null)
        {
            resultText.text = "Correct!";
            StartCoroutine(ClearTextAfterDelay(3f));
        }
        else
        {
            Debug.LogWarning("ResultText 未绑定！");
        }

        if (door != null)
            door.SetActive(true);
        else
            Debug.LogWarning("Door 未绑定！");
    }

    public void OnWrongAnswer()
    {
        if (resultText != null)
        {
            resultText.text = "Wrong!";
            StartCoroutine(ClearTextAfterDelay(3f));
        }
        else
        {
            Debug.LogWarning("ResultText 未绑定！");
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resultText.text = "";  // 清空文本
    }
}