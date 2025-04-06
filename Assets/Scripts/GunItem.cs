using UnityEngine;

public class GunItem : Item
{
    public GameObject bulletPrefab;
    public float shootForce = 20f;
    public ParticleSystem muzzleEffect;

    void Awake()
    {
        itemName = "Gun";
        isStackable = false;
        isUsable = false;  // Gun uses different activation method
    }

    public override bool Use(GameObject player)
    {
        return false;  // Gun is handled differently through shooting input
    }

    public void Shoot(Transform shootPoint)
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
        if (muzzleEffect != null)
            muzzleEffect.Play();
    }
}
