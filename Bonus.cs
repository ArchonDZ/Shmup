using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour, IMovable
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private Sprite[] spites = new Sprite[4];

    private bool levelUp;
    private AmmunitionType ammunition;
    private Rigidbody rb;
    private BoundsCheck bndCheck;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (0 == Random.Range(0, 3))
            levelUp = true;

        if (!levelUp)
        {
            int type = Random.Range(0, 4);
            ammunition = (AmmunitionType)type;
            spriteRenderer.sprite = spites[type];
        }
    }

    void Update()
    {
        if (bndCheck && bndCheck.type == BoundType.down)
            Destroy(gameObject);

        Move();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Armament armament))
            if (levelUp)
                armament.LevelUp();
            else
            {
                if (armament.GetAmmunitionType() == ammunition)
                    armament.LevelUp();
                else
                    armament.AbsorbPowerUp(ammunition);
            }

        Destroy(gameObject);
    }

    public void Move()
    {
        rb.MovePosition(transform.position + Vector3.down * speed * Time.deltaTime);
    }
}
