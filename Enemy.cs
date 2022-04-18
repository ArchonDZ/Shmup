using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IMovable, IDamageReceiver, IObserver
{
    public int score = 1;
    [SerializeField] private float superPowerScore = 0.01f;
    [SerializeField] private float health = 1f;
    [SerializeField] private float damageFromRam = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private BoundType boundType = BoundType.down;
    [SerializeField] private Vector3 movement = Vector3.down;
    [SerializeField] private Bonus bonus;
    [SerializeField, Range(0, 1)] private float percentageDropBonus = 0.5f;
    [SerializeField] private bool movable;

    [HideInInspector] public Wave wave;
    [HideInInspector] public bool withoutBonus;

    public IObservable Observable { get => wave; set => wave = value as Wave; }
    public float Health { get => health; set => health = value; }

    private Rigidbody rb;
    private BoundsCheck bndCheck;
    private float receiveDamage;
    private float damageTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
    }

    void Start()
    {
        Health += Level.level.enemyHealthGain;
    }

    void Update()
    {
        if (bndCheck && bndCheck.type == boundType)
            Destroy(gameObject);

        if (!Level.level.EndGameStatus)
            if (movable)
                Move();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
            player.Receive(damageFromRam);
    }

    public void Init(Vector3 movement, Quaternion rotation, float speed, BoundType boundType, bool movable)
    {
        if (movement != Vector3.zero)
            this.movement = movement;

        gameObject.transform.rotation = rotation;

        if (speed != 0f)
            this.speed = speed;

        this.boundType = boundType;

        this.movable = movable;
    }

    public void Move()
    {
        rb.MovePosition(transform.position + movement * speed / 50f);
    }

    public void Receive(float damage)
    {
        if (!Level.level.EndGameStatus)
        {
            if (Health > 0)
            {
                Health -= damage;
                receiveDamage += damage;

                if (Health <= 0)
                {
                    if (damage >= 0.05)
                        ShowDamage();

                    Death();
                }
                else if ((damageTime == 0) || ((Time.time - damageTime) >= 0.1f))
                {
                    ShowHit();
                    if (damage >= 0.05)
                        ShowDamage();

                    damageTime = Time.time;
                }
            }
        }
    }

    private void ShowHit()
    {
        EffectController.effectController.ActivateEffectInPosition(EffectType.hit, gameObject.transform.position - new Vector3(0, -0.5f, -1));
    }

    private void ShowDamage()
    {
        GameObject damageGameObject = EffectController.effectController.GetObjectEffect(EffectType.damage);
        if (damageGameObject)
        {
            damageGameObject.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position) + new Vector3(1, 1, 0);
            if (damageGameObject.TryGetComponent<Text>(out Text text))
                text.text = "-" + ((receiveDamage % 1 != 0) ? receiveDamage.ToString("F1") : ((int)receiveDamage).ToString());

            receiveDamage = 0;
            damageGameObject.SetActive(true);
        }
    }

    private void Death()
    {
        EffectController.effectController.ActivateEffectInPosition(EffectType.explosion, gameObject.transform.position);
        AudioController.audioController.PlayClip(SoundType.explosion);

        if (TryGetComponent(out Dying dying))
            dying.Death();

        if (!withoutBonus && bonus && (percentageDropBonus > Random.value))
            Instantiate(bonus, transform.position, Quaternion.identity);

        Level.level.AddScore(score, superPowerScore);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        Observable?.RemoveObserver(this);
    }
}
