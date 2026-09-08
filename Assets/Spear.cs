using UnityEngine;

public class Spear : MonoBehaviour
{
    public float lifeTime = 8f;
    public float bounceForce = 18f;
    public float bounceCooldown = 0.4f;

    private Rigidbody rb;
    private bool isStuck = false;
    private float nextBounceTime = 0f;
    private PlayerController owner;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    public void SetOwner(PlayerController player)
    {
        owner = player;
    }

    void OnDestroy()
    {
        if (owner != null)
        {
            owner.OnSpearDestroyed(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (isStuck == false && other.gameObject.tag != "Player")
        {
            StickInPlace();
        }
    }

    void StickInPlace()
    {
        isStuck = true;
        rb.isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        if (Time.time < nextBounceTime)
            return;

        Rigidbody playerRb = other.attachedRigidbody;

        if (playerRb == null)
            return;

        if (playerRb.linearVelocity.y < 0.5f && other.transform.position.y > transform.position.y)
        {
            Vector3 newVel = playerRb.linearVelocity;
            newVel.y = bounceForce;
            playerRb.linearVelocity = newVel;

            nextBounceTime = Time.time + bounceCooldown;
        }
    }
}