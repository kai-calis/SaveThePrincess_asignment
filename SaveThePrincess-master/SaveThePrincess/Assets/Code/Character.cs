using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public abstract class BalanceInfo
    {
        public int attackSpeed = 1;

        public abstract int GetHP();
        public abstract int GetAttackPower();
    }

    public class BalanceInfoFixed : BalanceInfo
    {
        public int hp = 10;
        public int attackPower = 1;

        public override int GetHP()
        {
            return hp;
        }

        public override int GetAttackPower()
        {
            return attackPower;
        }
    }

    public class BalanceInfoRandom : BalanceInfo
    {
        public int hpMin = 8;
        public int hpMax = 12;

        public int attackPowerMin = 1;
        public int attackPowerMax = 2;

        public override int GetHP()
        {
            return Random.Range(hpMin, hpMax + 1);
        }

        public override int GetAttackPower()
        {
            return Random.Range(attackPowerMin, attackPowerMax + 1);
        }
    }

    protected BalanceInfo balanceInfo;

    public int maxHP;
    public int hitPoints;

    public bool IsDead => hitPoints <= 0;

    public int AttackSpeed => balanceInfo.attackSpeed;

    protected int facing = -1;
    
    public GameObject healthBar;
    private float healthWidth;

    public GameObject attackAnimationPrefab;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        if (balanceInfo != null)
        {
            hitPoints = maxHP = balanceInfo.GetHP();
            healthBar.transform.parent.GetComponentInChildren<TextMesh>().text = $"{hitPoints}/{maxHP}";
        }

        healthWidth = healthBar.transform.localScale.x;
    }

    public void InitBalancing(BalanceInfo info)
    {
        balanceInfo = info;
    }

    public void AddHealth(int health)
    {
        hitPoints = Mathf.Min(hitPoints + health, maxHP);
        healthBar.transform.parent.GetComponentInChildren<TextMesh>().text = $"{hitPoints}/{maxHP}";

        float targetWidth = ((float)hitPoints / maxHP) * healthWidth;

        healthBar.transform.localScale = new Vector3(targetWidth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    public virtual IEnumerator Interact(Character other)
    {
        yield return StartCoroutine(AttackAsync());

        yield return StartCoroutine(other.DamageAsync(balanceInfo.GetAttackPower(), attackAnimationPrefab));
    }

    private IEnumerator AttackAsync()
    {
        float timer = .0f;
        const float speed = 5.0f;
        const float attackDistance = .5f;
        float baseX = transform.position.x;

        while(timer < 1.0f)
        {
            timer += Time.deltaTime * speed;

            transform.position = new Vector3(baseX + (Mathf.Sin(timer * Mathf.PI) * facing) * attackDistance, transform.position.y, transform.position.z);

            yield return 0;
        }

        transform.position = new Vector3(baseX, transform.position.y, transform.position.z);
    }

    private IEnumerator DamageAsync(int damage, GameObject animationPrefab)
    {
        GameObject animation = Instantiate(animationPrefab, transform.Find("middle").position + new Vector3(.0f, .0f, -1.0f), Quaternion.identity);

        yield return new WaitForSeconds(animation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        Destroy(animation);

        hitPoints = Mathf.Max(0, hitPoints - damage);

        healthBar.transform.parent.GetComponentInChildren<TextMesh>().text = $"{hitPoints}/{maxHP}";

        GameObject damageObject = Instantiate(Resources.Load<GameObject>("damage"), healthBar.transform.parent.position + new Vector3(.0f, .45f, .0f), Quaternion.identity);
        damageObject.GetComponent<TextMesh>().text = $"-{damage}";

        float timer = .0f;
        float startWidth = healthBar.transform.localScale.x;
        float targetWidth = ((float)hitPoints / maxHP) * healthWidth;

        const float speed = 2.0f;

        while(timer < 1.0f)
        {
            timer += Time.deltaTime * speed;

            healthBar.transform.localScale = new Vector3(Mathf.Lerp(startWidth, targetWidth, timer), healthBar.transform.localScale.y, healthBar.transform.localScale.z);

            yield return 0;
        }

        Destroy(damageObject);
    }
}
