using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    private float fireDistance = 50f;
    public int weaponPower = 5;
    public int maxBullet = 12;
    public static int remainBullet;

    public float delayFire = 0.12f;
    public float reloadTime = 1.8f;
    private float lastFireTime;

    public GameObject bulletEffect;
    public GameObject[] eff_Flash;

    ParticleSystem ps;
    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        ps = bulletEffect.GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (anim.GetFloat("MoveMotion") == 0)
            {
                anim.SetTrigger("Attack");
            }
            
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitinfo = new RaycastHit();

            if (Time.time >= lastFireTime + delayFire && remainBullet > 0)
            {
                if (Physics.Raycast(ray, out hitinfo, fireDistance))
                {
                    // 레이캐스트에 부딪힌 대상이 enemy라면 데미지 함수 실행
                    if (hitinfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        EnemyFSM eFSM = hitinfo.transform.GetComponent<EnemyFSM>();
                        eFSM.HitEnemy(weaponPower);
                    }

                    else
                    {
                        bulletEffect.transform.position = hitinfo.point;
                        // 이펙트의 forward 방향을 레이캐스트가 부딪힌 지점의 벡터와 일치
                        bulletEffect.transform.forward = hitinfo.normal;
                        ps.Play();
                    }
                }
                StartCoroutine(ShootEffectOn(0.05f));

                remainBullet--;
                if (remainBullet <= 0)
                {
                    StartCoroutine(ReloadBullet());
                }
            }
        }
    }

    IEnumerator ShootEffectOn(float duration)
    {
        int num = Random.Range(0, eff_Flash.Length);
        eff_Flash[num].SetActive(true);
        yield return new WaitForSeconds(duration);
        eff_Flash[num].SetActive(false);
    }

    IEnumerator ReloadBullet()
    {
        yield return new WaitForSeconds(reloadTime);
        remainBullet = maxBullet;
    }

    private void OnEnable()
    {
        remainBullet = maxBullet;
    }
}
