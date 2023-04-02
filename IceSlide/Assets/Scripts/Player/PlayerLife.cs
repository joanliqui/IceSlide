using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] GameObject deadParticlePrefab;
    public UnityEvent onPlayerDead;

    #region Component References
    private PlayerMovement1 player;
    private SpriteRenderer sr;
    private Collider2D col;
    private Rigidbody2D rb;
    [SerializeField] GameObject playerArrow;
    #endregion


    private void Start()
    {
        player = GetComponent<PlayerMovement1>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        onPlayerDead.AddListener(CameraHandler.Instance.CameraShake);
    }
    public void PlayerDead()
    {
        sr.enabled = false;
        col.enabled = false;
        rb.isKinematic = true;
        playerArrow.SetActive(false);

        GameObject deadParticle = Instantiate(deadParticlePrefab, transform.position,Quaternion.identity);
        ParticleSystem ps = deadParticle.GetComponent<ParticleSystem>();
        ps.Play();

        onPlayerDead?.Invoke();
        StartCoroutine(ReloadScene());
    }

    IEnumerator ReloadScene()
    {
        player.Inputs.Disable();
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
