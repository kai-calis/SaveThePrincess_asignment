using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject playerPrefab;

    public GameObject[] npcPrefabs;

    private List<GameObject> tiles;

    private Player player;

    private void Awake()
    {
        tiles = new List<GameObject>();

        var balancer = FindObjectOfType<Balancer>();
        for(int i = 0; i < balancer.TileInfos.Length; ++i)
        {
            npcPrefabs[i] = balancer.monsterPrefabs[balancer.TileInfos[i].monsterPrefab];
        }

        for (int i = 0; i < 10; ++i)
        {
            tiles.Add(Instantiate(tilePrefab, new Vector3(5.0f * i, -2.5f, .0f), Quaternion.identity));

            var npcPrefab = npcPrefabs[i];

            var balanceInfo = i < 9 ? balancer.BalanceInfos[balancer.TileInfos[i].monsterPrefab] : null;

            tiles[i].GetComponent<Tile>().CreateNPC(npcPrefab, balanceInfo, i < 9 ? balancer.TileInfos[i] : null);
        }

        player = Instantiate(playerPrefab, new Vector3(-.8f, -.8f, -1.0f), Quaternion.identity).GetComponent<Player>();
        player.InitBalancing(balancer.PlayerInfo);

        Destroy(balancer.gameObject);

        StartCoroutine(Game());
    }

    private IEnumerator Game()
    {
        for(int i = 0; i < tiles.Count; ++i)
        {
            tiles[i].GetComponent<Tile>().Player = player;

            yield return StartCoroutine(ProcessTile(tiles[i].GetComponent<Tile>()));

            if(player.IsDead || i == tiles.Count - 1)
            {
                break;
            }

            yield return StartCoroutine(player.MoveToNextTile());

            tiles[i].GetComponent<Tile>().Player = null;
        }

        yield return new WaitForSeconds(3.0f);
        
        SceneManager.LoadScene("Scenes/BalanceScene");
    }

    private IEnumerator ProcessTile(Tile tile)
    {
        yield return StartCoroutine(tile.DoBattleAsync());
    }
}
