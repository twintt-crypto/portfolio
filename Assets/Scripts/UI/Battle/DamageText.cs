using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DamageText : MonoBehaviour
{
    public TMP_Text text;

    public async void Play(int damage)
    {
        text.text = damage.ToString();

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * 80;

        var token = this.GetCancellationTokenOnDestroy();

        float time = 0;

        while (time < 1f)
        {
            token.ThrowIfCancellationRequested();

            time += Time.deltaTime;

            transform.position = Vector3.Lerp(start, end, time);

            await UniTask.Yield();
        }

        ResourceManager.Free(gameObject);
    }
}