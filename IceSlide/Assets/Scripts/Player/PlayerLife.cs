using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    PlayerMovement1 player;
    SpriteRenderer sr;

    private void Start()
    {
        player = GetComponent<PlayerMovement1>();
        sr = GetComponent<SpriteRenderer>();
    }
    public void PlayerDead()
    {
        sr.color = Color.black;
        StartCoroutine(ReloadScene());
    }

    IEnumerator ReloadScene()
    {
        player.Inputs.Disable();
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
