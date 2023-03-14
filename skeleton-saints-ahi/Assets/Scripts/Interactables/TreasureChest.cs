using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : interactableButton
{
    public GameObject[] Drops;
    [Range(0.1f, 10f)]public float DestroyTimer;

    private GameObject _dropChosen;
    private int _index;

    void Awake()
    {
        if (Drops.Length <= 0)
            enabled = false;

        _index = Random.Range(0, Drops.Length);
        _dropChosen = Instantiate(Drops[_index], transform.position + new Vector3(0f, 1f, 0f), Drops[_index].transform.rotation);
        _dropChosen.SetActive(false);
        _dropChosen.transform.localScale *= .5f;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(Interacted)
            Interact();

        if (InteractedOnce && _dropChosen == null)
            ShrinkAway();
    }

    public override void Interact()
    {
        base.Interact();
        GetComponent<Animator>().SetTrigger("Activate");
        StartCoroutine(PickupDelay(1f));
    }

    private void EnablePickup()
    {
        _dropChosen.SetActive(true);
        ChangeColor();
    }

    private void ShrinkAway()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime);
        if(transform.localScale.x <= 0f)
            Destroy(gameObject);
    }

    private IEnumerator PickupDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnablePickup();
    }

    private IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(DestroyTimer);

    }
}
