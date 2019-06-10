using UnityEngine;
using System.Collections;

public class Player : Character
{
    protected override void Awake()
    {
        base.Awake();

        facing = 1;
    }

    public IEnumerator MoveToNextTile()
    {
        float timer = .0f;
        float time = .5f;
        float start = transform.position.x;
        float end = start + 2.5f;

        StartCoroutine(MoveCamera());

        while(timer < time)
        {
            timer += Time.deltaTime;
            transform.position = new Vector3(Mathf.Lerp(start, end, timer / time), transform.position.y, transform.position.z);

            yield return 0;
        }

        timer = .0f;
        start = transform.position.x;
        end = start + 2.5f;
        float baseY = transform.position.y;

        while(timer < time)
        {
            timer += Time.deltaTime;
            float p = timer / time;

            transform.position = new Vector3(Mathf.Lerp(start, end, p), baseY + Mathf.Sin(p * Mathf.PI), transform.position.z);

            yield return 0;
        }

        transform.position = new Vector3(end, baseY, transform.position.z);
    }

    private IEnumerator MoveCamera()
    {
        float timer = .0f;

        float cameraStart = Camera.main.transform.position.x;
        float cameraEnd = cameraStart + 5.0f;

        while (timer < 1.0f)
        {
            timer += Time.deltaTime;

            Camera.main.transform.position = new Vector3(Mathf.Lerp(cameraStart, cameraEnd, timer), Camera.main.transform.position.y, Camera.main.transform.position.z);

            yield return 0;
        }
    }
}
