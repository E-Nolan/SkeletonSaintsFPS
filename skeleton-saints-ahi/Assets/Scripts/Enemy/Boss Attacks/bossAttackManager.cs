using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossAttackManager : MonoBehaviour
{
    [Header("----- GameObjects -----")]
    [SerializeField] GameObject energyWaveObject;
    [SerializeField] GameObject returnToGameObject;

    [Header("----- Scripts -----")]
    [SerializeField] Enemy enemyScript;
    [SerializeField] EnemyAI enemyAiScript;

    [Header("----- Energy Wave Vars -----")] 
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip energyWaveAudio;
    [SerializeField] float waveCooldown;
    public bool CanSendWave;
    [SerializeField] bool waveSent;
    [SerializeField] bool atWaveLocation;
    public bool goingToWaveLocation;

    [Header("----- Misc Vars -----")]
    [SerializeField] bool invincible;

    private float _waveTimer;
    private UnityEngine.AI.NavMeshAgent agent;

    void Awake()
    {
        enemyAiScript = GetComponent<EnemyAI>();
        energyWaveObject = GetComponentInChildren<energyWave>().gameObject;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        energyWaveObject.SetActive(false);
        waveSent = false;
        CanSendWave = false;
        atWaveLocation = false;
        goingToWaveLocation = false;
        invincible = false;

    }

    void Update()
    {

        _waveTimer += Time.deltaTime;

        if (_waveTimer >= waveCooldown)
        {
            _waveTimer = 0f;
            CanSendWave = true;
        }

        if (waveSent)
            CanSendWave = false;

        atWaveLocation = Mathf.Abs((transform.position - returnToGameObject.transform.position).magnitude) < 0.1f;
        goingToWaveLocation = Mathf.Abs((agent.destination - returnToGameObject.transform.position).magnitude) < 0.1f;

        if ((CanSendWave && !goingToWaveLocation && agent.isActiveAndEnabled) ||
            (CanSendWave && Mathf.Abs((gameManager.instance.playerInstance.transform.position - agent.destination).magnitude) < 0.5f))
        {
            agent.destination = returnToGameObject.transform.position;
        }

        if (goingToWaveLocation)
            Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new 
                Vector3(returnToGameObject.transform.position.x, transform.position.y, returnToGameObject.transform.position.z)), Time.deltaTime);

        if(!goingToWaveLocation && invincible)
            TakeInvincibility();

        if (atWaveLocation)
            goingToWaveLocation = false;

        if (CanSendWave && !waveSent)
        {
            if (atWaveLocation)
            {
                goingToWaveLocation = false;

                if(invincible)
                    TakeInvincibility();

                _waveTimer = 0f;
                StartCoroutine(WaveDelay());
            }

            if (goingToWaveLocation && !invincible)
            {
                GiveInvincibility();
            }
        }
    }

    private void GiveInvincibility()
    {
        invincible = true;

        foreach(SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>()) 
            rend.material = enemyScript._flashMaterial;

        foreach(Collider col in GetComponents<Collider>())
            col.enabled = false;

        foreach(Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    private void TakeInvincibility()
    {
        invincible = false;
        foreach(SkinnedMeshRenderer rend in GetComponentsInChildren<SkinnedMeshRenderer>()) 
            rend.material = enemyScript._material;

        foreach(Collider col in GetComponents<Collider>())
            col.enabled = true;

        foreach(Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;
    }

    private IEnumerator WaveDelay()
    {
        CanSendWave = false;
        waveSent = true;
        agent.enabled = false;
        enemyScript.isAttacking = false;

        GetComponent<Animator>().SetTrigger("EnergyAttack");

        yield return new WaitForSeconds(0.75f);

        energyWaveObject.SetActive(true);

        GetComponent<Animator>().SetFloat("yVelocity", 0f);
        GetComponent<Animator>().SetFloat("xVelocity", 0f);

        yield return new WaitForSeconds(2f);

        agent.enabled = true;
        energyWaveObject.SetActive(false);

        yield return new WaitForSeconds(energyWaveObject.GetComponent<ParticleSystem>().main.duration);

        waveSent = false;
        _waveTimer = 0f;
    }

    private void PlayEnergyWaveScreech()
    {
        audioSource.PlayOneShot(energyWaveAudio, .5f);
    }
}
