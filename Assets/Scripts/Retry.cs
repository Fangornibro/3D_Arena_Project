using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    [SerializeField] private Player player;
    private void Start()
    {
        Close();
    }
    public void Open()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        Pause.SetPause(true);
    }

    public void MenuOpen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).Find("MenuText").GetComponent<TextMeshProUGUI>().SetText("");
        Pause.SetPause(true);
    }

    public void Close()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        if (player.curHealth > 0)
        {
            Pause.SetPause(false);
        }
    }

    public void RetryFunc()
    {
        SceneManager.LoadScene("MainScene");
    }
}
