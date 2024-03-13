using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSwitchScript : MonoBehaviour
{
    public void OnButtonClick()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
