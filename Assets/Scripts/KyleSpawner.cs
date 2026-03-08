using UnityEngine;

public class KyleSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject kylePrefab;
    public Transform[] spawnPoints;

    public float waveDelaySpread = 1.5f;

    void Start()
    {
        SpawnKyles();
    }

    void SpawnKyles()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[i];

            GameObject kyle = Instantiate(
                kylePrefab,
                point.position,
                Quaternion.identity
            );

            // 面向玩家
            Vector3 lookDir = player.position - kyle.transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                kyle.transform.rotation =
                    Quaternion.LookRotation(lookDir);
            }

            // 挥手时间错开
            KyleWave wave = kyle.GetComponentInChildren<KyleWave>();

            if (wave != null)
            {
                float delay = Random.Range(0f, waveDelaySpread);
                StartCoroutine(StartWaveDelayed(wave, delay));
            }
        }
    }

    System.Collections.IEnumerator StartWaveDelayed(
        KyleWave wave,
        float delay
    )
    {
        yield return new WaitForSeconds(delay);
        wave.StartWave();
    }
}