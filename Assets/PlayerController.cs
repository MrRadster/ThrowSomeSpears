using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float turnSpeed = 12f;

    public GameObject spearPrefab;
    public Transform throwPoint;
    public GameObject heldSpear;
    public float throwForce = 25f;
    public float throwCooldown = 0.5f;
    public int maxSpears = 3;

    private Rigidbody rb;
    private float nextThrowTime = 0f;
    private Transform cam;
    private List<GameObject> spearsInWorld = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = (camForward * vertical + camRight * horizontal).normalized;

        Vector3 newVelocity = moveDirection * moveSpeed;
        newVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = newVelocity;

        Vector3 lookDirection = cam.forward;
        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 jumpVelocity = rb.linearVelocity;
            jumpVelocity.y = jumpForce;
            rb.linearVelocity = jumpVelocity;
        }

        if (Input.GetButtonDown("Fire1") && Time.time > nextThrowTime)
        {
            ThrowSpear();
            nextThrowTime = Time.time + throwCooldown;
        }
    }

    void ThrowSpear()
    {
        if (spearPrefab == null || throwPoint == null)
            return;

        Vector3 throwDirection = cam.forward;

        if (spearsInWorld.Count >= maxSpears)
        {
            if (spearsInWorld[0] != null)
            {
                Destroy(spearsInWorld[0]);
            }
            spearsInWorld.RemoveAt(0);
        }

        GameObject newSpear = Instantiate(spearPrefab, throwPoint.position, Quaternion.LookRotation(throwDirection));
        Rigidbody spearRb = newSpear.GetComponent<Rigidbody>();
        spearRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);

        spearsInWorld.Add(newSpear);

        Spear spearScript = newSpear.GetComponent<Spear>();
        if (spearScript != null)
        {
            spearScript.SetOwner(this);
        }

        if (heldSpear != null)
        {
            heldSpear.SetActive(false);
            Invoke("ShowHeldSpear", 0.4f);
        }
    }

    void ShowHeldSpear()
    {
        if (heldSpear != null)
        {
            heldSpear.SetActive(true);
        }
    }

    public void OnSpearDestroyed(GameObject spear)
    {
        spearsInWorld.Remove(spear);
    }
}