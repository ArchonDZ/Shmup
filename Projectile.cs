using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IMovable
{
    [SerializeField] protected float speed = 50f;
    [SerializeField] protected Vector3 movement = Vector3.up;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float changeDamageLvl = 1f;

    protected Rigidbody rb;
    protected float level;
    private BoundsCheck bndCheck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
    }

    void Update()
    {
        if (bndCheck && bndCheck.type != BoundType.none)
            gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        Move();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageReceiver damageReceiver))
            damageReceiver.Receive(damage + changeDamageLvl * level);

        gameObject.SetActive(false);
    }

    public virtual void Move()
    {
        rb.AddRelativeForce(movement * speed * Time.deltaTime, ForceMode.Impulse);
    }

    public void Init(float levelProjectile)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        level = levelProjectile;
    }
}
