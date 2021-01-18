using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadMain() {
        SceneManager.LoadScene("main");
    }
    public void LoadBridges() {
        SceneManager.LoadScene("bridges");
    }
    public void LoadDesert() {
        SceneManager.LoadScene("desert");
    }
    public void LoadCity() {
        SceneManager.LoadScene("city");
    }
}