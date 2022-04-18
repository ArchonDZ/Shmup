using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private GameObject[] sparks = new GameObject[2];
    [SerializeField] private Vector3 direction;
    [SerializeField] private float damagePerSecond = 4.0f;
    [SerializeField] private float damageStepSecond = 1.0f;
    [SerializeField] private float changeDamageLvl = 4.0f;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layerMask;

    [HideInInspector] public bool enable = true;
    private float damageCurrent;
    private float lastTimeProgression;
    private int level;
    private Vector3 hitPoint;
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        DamageReset();
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, maxDistance, layerMask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IDamageReceiver damageReceiver))
                damageReceiver.Receive(damageCurrent * Time.deltaTime);

            DamageProgression();
            hitPoint = hitInfo.point;
        }
        else
        {
            DamageReset();
            hitPoint = transform.position + direction * maxDistance;
        }


        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hitPoint);
        sparks[0].transform.position = transform.position + Vector3.up;
        sparks[1].transform.position = hitPoint;
    }

    void LateUpdate()
    {
        if (!enable)
            Destroy(gameObject);

        enable = false;
    }

    void DamageProgression()
    {
        if (Time.time - lastTimeProgression > 0.5f)
        {
            damageCurrent += damageStepSecond + changeDamageLvl * level;
            lastTimeProgression = Time.time;
        }
    }

    void DamageReset()
    {
        damageCurrent = damagePerSecond;
    }

    public void Init(int levelLaser)
    {
        level = levelLaser;
    }
}
