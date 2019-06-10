using UnityEngine;
using System.Collections;

public class Princess : Character
{
    public TextMesh textMesh;

    protected override void Start()
    {
        base.Start();

        balanceInfo = new BalanceInfoFixed();
        balanceInfo.attackSpeed = int.MaxValue;
    }

    public override IEnumerator Interact(Character other)
    {
        other.hitPoints = 0;
        other.healthBar.transform.localScale = new Vector3(.0f, .0f, 1.0f);

        yield return StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
        string[] texts = { "Welcome back my child", "..", "\n...", "your mission is done." };
        
        for(int i = 0; i < texts.Length; ++i)
        {
            for(int c = 0; c < texts[i].Length; ++c)
            {
                textMesh.text += texts[i][c];

                yield return new WaitForSeconds(.05f);
            }

            yield return new WaitForSeconds(.5f);
        }
    }
}
