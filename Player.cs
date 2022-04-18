using UnityEngine;

public class Player : MonoBehaviour, IMovable, IDamageReceiver
{
    [HideInInspector] public static Player player;

    [SerializeField] private float health = 1f;
    [SerializeField] private float damageFromRam = 1f;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    [SerializeField] private float speed = 1f;
#endif

    public float Health { get => health; set { health = value; GUIGameUpdater.guiGameUpdater.SetHealth(Mathf.RoundToInt(health)); } }

#if UNITY_ANDROID
    private float deltaX, deltaY;
    private Rigidbody rb;
#endif

    [HideInInspector] public Armament armament;
    [HideInInspector] public BoundsCheck boundsCheck;

    void Awake()
    {
#if UNITY_ANDROID
        rb = GetComponent<Rigidbody>();
#endif

        armament = GetComponent<Armament>();
        boundsCheck = GetComponent<BoundsCheck>();
        player = this;
    }

    void Update()
    {
        if (!Level.level.EndGameStatus)
        {
            Move();
            Fire();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageReceiver damageReceiver))
            damageReceiver.Receive(damageFromRam);
    }

    public void Move()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos3D.z = transform.position.z;
        transform.position = Vector3.MoveTowards(transform.position, mousePos3D, speed * Time.deltaTime);
#endif
#if UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchPos.z = transform.position.z;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    deltaX = touchPos.x - transform.position.x;
                    deltaY = touchPos.y - transform.position.y;
                    break;

                case TouchPhase.Moved:
                    Vector3 targetPos = new Vector3(touchPos.x - deltaX, touchPos.y - deltaY, touchPos.z);
                    rb.MovePosition(targetPos);
                    break;
            }
        }
#endif
    }

    private void Fire()
    {
        armament.Fire();
    }

    public void SuperPower()
    {
        armament.SuperPower();
    }

    public void Receive(float damage)
    {
        if (!Level.level.EndGameStatus)
        {
            Health -= damage;
            Handheld.Vibrate();

            if (Health <= 0)
            {
                Health = 0;
                Death();
            }
            else
                ShowHit();
        }
    }

    private void ShowHit()
    {
        EffectController.effectController.ActivateEffectInPosition(EffectType.hit, gameObject.transform.position - new Vector3(0, +0.5f, -1));
    }

    private void Death()
    {
        EffectController.effectController.ActivateEffectInPosition(EffectType.explosion, gameObject.transform.position);
        AudioController.audioController.PlayClip(SoundType.explosion);

        Destroy(gameObject);
        Level.level.EndGame(false);
    }
}
