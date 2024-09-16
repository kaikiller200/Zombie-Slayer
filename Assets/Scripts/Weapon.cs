using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public bool isActiveWeapon;

    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    public float shootingDelay = 0.5f;
    bool allowReset = true;

    [Header("Burst")]
    public int bulletsPerBurst = 3;
    int burstBulletsLeft;

    [Header("Spread")]
    public float spreadIntensity;
    public float HipSpreadIntensity;
    public float ADSSpreadIntensity;


    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("Loading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    [Header("Spawn")]
    public Vector3 SpawnPosition;
    public Vector3 SpawnRotation;

    bool IsADS;

    
    public enum WeaponModel
    {
        M1911,
        M4_8
    }
    public WeaponModel thisWeaponModel;
    public enum ShootingMode 
    { 
        Single, 
        Burst, 
        Auto 
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        spreadIntensity = HipSpreadIntensity;
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }

            if (Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }



            if (bulletsLeft == 0 && isShooting)
            {
                SoundManager.Instance.EmptyMagSound.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
                isShooting = Input.GetKey(KeyCode.Mouse0);
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (Input.GetKey(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }

            // automatic reload
            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0 && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                //Reload();
            }

           
        }

    }

    private void EnterADS()
    {
        animator.SetTrigger("EnterADS");
        IsADS = true;
        HUDManager.Instance.MiddleDot.SetActive(false);
        spreadIntensity = ADSSpreadIntensity;
    }
    private void ExitADS()
    {
        animator.SetTrigger("ExitADS");
        IsADS = false;
        HUDManager.Instance.MiddleDot.SetActive(true);
        spreadIntensity = HipSpreadIntensity;
    }
    

    private void FireWeapon()
    {

        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        if (IsADS)
        {
            animator.SetTrigger("ADS_Recoil");
        }
        else
        {
            animator.SetTrigger("Recoil");
        }

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;
        StartCoroutine(ResetShotAfterDelay(shootingDelay));

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.transform.forward = shootingDirection;

        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.velocity = shootingDirection * bulletVelocity;

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }


    private void Reload()
    {
        SoundManager.Instance.PlayReloadingSound(thisWeaponModel);
        animator.SetTrigger("Reload");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        if(WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize) 
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }

        isReloading = false;
    }

    private IEnumerator ResetShotAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        readyToShoot = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(100);

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);
        float z = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
