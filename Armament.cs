using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmunitionType
{
    missile,
    plasma,
    bomb,
    laser
}

[System.Serializable]
public class AmmunitionDefinition
{
    public AmmunitionType type;
    public GameObject prefab;
    public int maxLvl;
    public float delayBetweenShots = 0.2f;
    public float changeDelayLvl = 0.02f;

    public int countProjectiles = 100;
    [HideInInspector] public Queue<GameObject> activeObjects;
}

[System.Serializable]
public class WeaponDefinition
{
    public Weapon[] weapons;
}

public class Armament : MonoBehaviour
{
    [SerializeField] private AmmunitionType typeAmmunition;
    [SerializeField] public SuperPower superPower;
    [SerializeField] private WeaponDefinition[] definitionWeapon = new WeaponDefinition[5];
    [SerializeField] private AmmunitionDefinition[] definitionAmmunition = new AmmunitionDefinition[1];

    [SerializeField] private SafeInt level;
    private Dictionary<AmmunitionType, AmmunitionDefinition> dictionaryWeapon = new Dictionary<AmmunitionType, AmmunitionDefinition>();

    public int Level { get => level.GetValue(); set { level = new SafeInt(value); GUIGameUpdater.guiGameUpdater.SetArmor(level.GetValue() + 1); } }

    void Awake()
    {
        foreach (AmmunitionDefinition def in definitionAmmunition)
            if (def.prefab && !dictionaryWeapon.ContainsKey(def.type))
            {
                dictionaryWeapon.Add(def.type, def);

                def.activeObjects = new Queue<GameObject>(def.countProjectiles);
                for (int i = 0; i < def.countProjectiles; i++)
                {
                    GameObject gameObject = Instantiate(def.prefab);
                    gameObject.SetActive(false);
                    def.activeObjects.Enqueue(gameObject);
                }
            }
    }

    public void Fire()
    {
        if (!dictionaryWeapon.TryGetValue(typeAmmunition, out AmmunitionDefinition ammunition)) return;

        switch (typeAmmunition)
        {
            case AmmunitionType.laser:
                Irradiate(ammunition);
                break;
            default:
                Shoot(ammunition);
                break;
        }
    }

    public void SuperPower()
    {
        superPower.StartCoroutine(superPower.Init(typeAmmunition));
    }

    private void Irradiate(AmmunitionDefinition ammunition)
    {
        foreach (Weapon weapon in definitionWeapon[0].weapons)
            weapon?.Irradiate(ammunition, Level);
    }

    private void Shoot(AmmunitionDefinition ammunition)
    {
        foreach (Weapon weapon in definitionWeapon[Level].weapons)
            weapon?.Shoot(ammunition, Level);
    }

    public AmmunitionType GetAmmunitionType()
    {
        return typeAmmunition;
    }

    public void LevelUp()
    {
        if (dictionaryWeapon.TryGetValue(typeAmmunition, out AmmunitionDefinition ammunition))
            if (Level < ammunition.maxLvl - 1)
                Level++;
    }

    public void AbsorbPowerUp(AmmunitionType type)
    {
        typeAmmunition = type;
        Level = 0;
    }
}
