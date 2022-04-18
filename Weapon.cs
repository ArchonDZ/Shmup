using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Laser laser;
    private float lastShotTime;

    public void Shoot(AmmunitionDefinition ammunition, int level)
    {
        float delayBetweenShots = ammunition.delayBetweenShots - ammunition.changeDelayLvl * level;

        if (Time.time - lastShotTime < delayBetweenShots) return;

        GameObject gameObject = ammunition.activeObjects.Dequeue();
        if (gameObject != null)
        {
            gameObject.transform.position = transform.position;
            gameObject.transform.rotation = transform.rotation;
            if (gameObject.TryGetComponent(out Projectile projectile))
            {
                projectile.Init(level);
                lastShotTime = Time.time;
            }

            gameObject.SetActive(true);
            ammunition.activeObjects.Enqueue(gameObject);
        }
    }

    public void Irradiate(AmmunitionDefinition ammunition, int level)
    {
        if (laser)
            laser.enable = true;
        else if (Instantiate(ammunition.prefab, transform.position, transform.rotation, transform).TryGetComponent(out Laser newLaser))
        {
            newLaser.Init(level);
            laser = newLaser;
        }
    }

}
