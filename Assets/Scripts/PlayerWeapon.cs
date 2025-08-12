using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerWeapon : MonoBehaviour
{
    public MMF_Player shootFeedbacks;

    public float inbetweenShotDelay = 0.5f;
    public int maxBullets = 6;

    public int currentBullets;
    public bool canShoot;

    private float cooldownTimer = 0f;

    void Start()
    {
        canShoot = true;
        currentBullets = maxBullets;
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnReloadInput += OnReloadInput;
        }
    }

    void Update()
    {
        if (!canShoot)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f && currentBullets > 0)
            {
                canShoot = true;
            }
        }
    }

    public void Shoot()
    {
        if (canShoot && currentBullets > 0)
        {
            canShoot = false;
            currentBullets--;
            cooldownTimer = inbetweenShotDelay;

            shootFeedbacks?.PlayFeedbacks();
        }
    }

    public void OnReloadInput()
    {
        currentBullets = maxBullets;
        Debug.Log("Reload");
    }
}
