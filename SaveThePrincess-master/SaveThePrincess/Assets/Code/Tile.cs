using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tile : MonoBehaviour
{
    public class BalanceInfo
    {
        public int monsterPrefab;
        public float potionChance;
    }

    private BalanceInfo tileInfo;

    public Player Player { get; set; }
    private Character npc;

    private void Awake()
    {
        
    }

    public void CreateNPC(GameObject prefab, Character.BalanceInfo balanceInfo, BalanceInfo tileInfo)
    {
        npc = Instantiate(prefab, transform).GetComponentInChildren<Character>();
        npc.InitBalancing(balanceInfo);
        npc.transform.parent.localPosition = new Vector3(.8f, 1.0f, -1.0f);

        this.tileInfo = tileInfo;
    }

    public IEnumerator DoBattleAsync()
    {
        List<Character> characters = new List<Character>();
        characters.Add(Player);
        characters.Add(npc);
        characters = characters.OrderByDescending(c => c.AttackSpeed).ToList();

        while(true)
        {
            for(int i = 0; i < characters.Count; ++i)
            {
                int other = (i + 1) % characters.Count;

                yield return StartCoroutine(characters[i].Interact(characters[other]));
                
                if(characters[other].IsDead)
                {
                    Vector3 pos = characters[other].transform.position;
                    Destroy(characters[other].gameObject);

                    if (!Player.IsDead)
                    {
                        if (Random.Range(.0f, 99.999f) < tileInfo.potionChance)
                        {
                            yield return StartCoroutine(FindPotion(pos));
                        }
                    }

                    yield break;
                }
            }
        }
    }

    private IEnumerator FindPotion(Vector3 position)
    {
        GameObject potion = Instantiate(Resources.Load<GameObject>("potion"), position, Quaternion.identity);

        yield return new WaitForSeconds(.5f);

        GameObject damageObject = Instantiate(Resources.Load<GameObject>("damage"), Player.healthBar.transform.parent.position + new Vector3(.0f, .45f, .0f), Quaternion.identity);
        damageObject.GetComponent<TextMesh>().text = $"+{Potion.effect}";

        Player.AddHealth(Potion.effect);

        yield return new WaitForSeconds(1.0f);

        Destroy(potion);
        Destroy(damageObject);
    }
}
