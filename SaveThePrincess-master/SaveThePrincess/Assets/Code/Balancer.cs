using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Linq;

public class Balancer : MonoBehaviour
{
    public Character.BalanceInfo PlayerInfo { get; private set; }

    public GameObject[] monsterPrefabs;

    public Character.BalanceInfo[] BalanceInfos { get; private set; }

    public Tile.BalanceInfo[] TileInfos { get; private set; }

    private Vector2 scrollPos;

    private bool random;

    private bool shouldLoad;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        BalanceInfos = new Character.BalanceInfo[monsterPrefabs.Length];

        TileInfos = new Tile.BalanceInfo[9];
        for(int i = 0; i < TileInfos.Length; ++i)
        {
            TileInfos[i] = new Tile.BalanceInfo();
        }

        InitFixed();
    }

    private void InitFixed()
    {
        PlayerInfo = new Character.BalanceInfoFixed();

        for(int i = 0; i < BalanceInfos.Length; ++i)
        {
            BalanceInfos[i] = new Character.BalanceInfoFixed();
        }
    }

    private void InitRandom()
    {
        PlayerInfo = new Character.BalanceInfoRandom();

        for(int i = 0; i < BalanceInfos.Length; ++i)
        {
            BalanceInfos[i] = new Character.BalanceInfoRandom();
        }
    }

    private void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        GUILayout.BeginVertical(GUILayout.Width(500.0f));
        
        bool newRandom = GUILayout.Toggle(random, "Stats random range");
        if(newRandom != random)
        {
            random = newRandom;
            if (random) InitRandom();
            else InitFixed();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Potion effect");

        int newPotionEffect;
        if(int.TryParse(GUILayout.TextField(Potion.effect.ToString()), out newPotionEffect))
        {
            Potion.effect = newPotionEffect;
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20.0f);

        RenderBalanceInfo("Player", PlayerInfo);

        for(int i = 0; i < BalanceInfos.Length; ++i)
        {
            RenderBalanceInfo(monsterPrefabs[i].name, BalanceInfos[i]);
        }

        string[] content = (from p in monsterPrefabs select p.name).ToArray();
        for(int i = 0; i < TileInfos.Length; ++i)
        {
            GUILayout.Label($"Tile {i + 1}");

            TileInfos[i].monsterPrefab = GUILayout.SelectionGrid(TileInfos[i].monsterPrefab, content, content.Length);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Potion appear % chance");

            int chance;
            if(int.TryParse(GUILayout.TextField(TileInfos[i].potionChance.ToString()), out chance))
            {
                TileInfos[i].potionChance = chance;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20.0f);
        }

        GUILayout.EndVertical();

        GUILayout.EndScrollView();

        if(GUI.Button(new Rect(Screen.width - 110.0f, Screen.height - 30.0f, 100.0f, 20.0f), "Start"))
        {
            shouldLoad = true;
        }
    }

    private void RenderBalanceInfo(string name, object characterInfo)
    {
        GUILayout.Label(name);

        foreach(FieldInfo info in characterInfo.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(info.Name);
            string newValueS = GUILayout.TextField(info.GetValue(characterInfo).ToString());

            int newValue;
            if(int.TryParse(newValueS, out newValue))
            {
                info.SetValue(characterInfo, newValue);
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20.0f);
    }

    private void Update()
    {
        if(shouldLoad)
        {
            enabled = false;

            SceneManager.LoadScene("Scenes/BattleScene");
        }
    }
}
