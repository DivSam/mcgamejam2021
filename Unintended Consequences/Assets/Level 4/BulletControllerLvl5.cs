using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BulletControllerLvl5 : NetworkBehaviour
{
    public float waitTime = 0.1f;
    bool active = false;
    public float speed = 10f;
    private Rigidbody2D myRb;
    public GameObject dontDamage;
    public GameObject myParticleSystem;
    public AudioManagement myAudioManagement;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitThenSetActive());
        myRb = GetComponent<Rigidbody2D>();
        myAudioManagement = GameObject.FindGameObjectWithTag("audio").GetComponent<AudioManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        myRb.velocity = transform.right * speed;
    }
    public void OnTriggerEnter2D(Collider2D Enemy)
    {

        if (Enemy.gameObject.CompareTag("Collidable"))
        {
            Instantiate(myParticleSystem, transform.position, transform.rotation);
            myAudioManagement.PlayBallHit();
            Destroy(gameObject);
        }
        if (Enemy.gameObject.CompareTag("Player"))
        {
            // If bullet was spawned by current player, don't damage current player
            if (Enemy.gameObject.GetComponent<NetworkIdentity>().netId == dontDamage.GetComponent<NetworkIdentity>().netId) return;

            // Send message to player object to deal damage to itself
            Enemy.gameObject.GetComponent<CharacterHPLvl5>().onDamage(2.0f);
            myAudioManagement.PlaySplat();
            Instantiate(myParticleSystem, transform.position, transform.rotation);
            Destroy(gameObject);

        }
    }
    [Command(ignoreAuthority = true)]
    void CmdDamage(GameObject enemy)
    {
        enemy.SendMessage("onDamage", 2.0);
        Debug.Log("Damaging enemy");
    }

    IEnumerator waitThenSetActive()
    {
        yield return new WaitForSeconds(waitTime);
        active = true;
    }



}
