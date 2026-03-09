using UnityEngine;

public class GuideController : MonoBehaviour
{
    public Transform player;
    public Transform guideParticles;
    public Transform targetPoint;

    public KyleWave kyleWave;

    public KyleSpawner kyleSpawner;

    [Header("Effect Objects")]
    public ParticleSystem guideParticleSystem;
    public GameObject arrivalSwirlParticles;
    public Transform humanAnchor;
    public Transform humanModel;
    [Header("Guide Placement")]
    public float hoverHeight = 1.4f;
    public float idealFrontDistance = 1.2f;
    public float maxDistanceFromPlayer = 2.0f;

    [Header("Guide Movement")]
    public float moveSpeed = 0.8f;
    public float followLerpSpeed = 3.0f;

    [Header("Arrival")]
    public float playerArrivalDistance = 0.8f;
    public float swirlDuration = 2.0f;
    public float swirlRotationSpeed = 120f;

    [Header("Human Reveal")]
    public float humanHeightOffset = 0.0f;
    public float humanGrowDuration = 1.2f;
    public Vector3 humanFinalScale = Vector3.one;

    private bool reachedTarget = false;
    private bool transformationStarted = false;
    private bool humanGrowing = false;

    private float arrivalTimer = 0f;
    private float growTimer = 0f;

    void Start()
    {
        if (player != null && guideParticles != null)
        {
            Vector3 startPos = player.position + player.forward * idealFrontDistance;
            startPos.y = hoverHeight;
            guideParticles.position = startPos;
        }

        if (arrivalSwirlParticles != null)
            arrivalSwirlParticles.SetActive(false);

        if (humanAnchor != null)
            humanAnchor.gameObject.SetActive(false);

        if (humanModel != null)
            humanModel.localScale = Vector3.zero;
    }

    void Update()
    {
        if (player == null || guideParticles == null || targetPoint == null) return;

        if (!reachedTarget)
        {
            UpdateGuideMovement();
            CheckArrival();
        }
        else if (!transformationStarted)
        {
            UpdateArrivalEffect();
        }
        else if (humanGrowing)
        {
            UpdateHumanGrowth();
        }
    }

    void UpdateGuideMovement()
    {
        Vector3 playerFlat = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 guideFlat = new Vector3(guideParticles.position.x, 0f, guideParticles.position.z);
        Vector3 targetFlat = new Vector3(targetPoint.position.x, 0f, targetPoint.position.z);

        Vector3 toTarget = (targetFlat - guideFlat).normalized;
        Vector3 candidatePos = guideParticles.position + new Vector3(toTarget.x, 0f, toTarget.z) * moveSpeed * Time.deltaTime;
        candidatePos.y = hoverHeight;

        float candidateDistToPlayer = Vector3.Distance(
            new Vector3(candidatePos.x, 0f, candidatePos.z),
            playerFlat
        );

        if (candidateDistToPlayer <= maxDistanceFromPlayer)
        {
            guideParticles.position = candidatePos;
        }
        else
        {
            Vector3 waitPos = player.position + player.forward * idealFrontDistance;
            waitPos.y = hoverHeight;

            guideParticles.position = Vector3.Lerp(
                guideParticles.position,
                waitPos,
                followLerpSpeed * Time.deltaTime
            );
        }
    }

    void CheckArrival()
    {
        float distPlayerToTarget = Vector3.Distance(
            new Vector3(player.position.x, 0f, player.position.z),
            new Vector3(targetPoint.position.x, 0f, targetPoint.position.z)
        );

        if (distPlayerToTarget <= playerArrivalDistance)
        {
            reachedTarget = true;

            if (guideParticleSystem != null)
                guideParticleSystem.Stop();

            if (arrivalSwirlParticles != null)
            {
                arrivalSwirlParticles.SetActive(true);
                arrivalSwirlParticles.transform.position = targetPoint.position + Vector3.up * 1.2f;
            }
        }
    }

    void UpdateArrivalEffect()
    {
        arrivalTimer += Time.deltaTime;

        if (arrivalSwirlParticles != null)
        {
            arrivalSwirlParticles.transform.Rotate(Vector3.up, swirlRotationSpeed * Time.deltaTime);
        }

        if (arrivalTimer >= swirlDuration)
        {
            transformationStarted = true;

            if (arrivalSwirlParticles != null)
                arrivalSwirlParticles.SetActive(false);

            // if (humanAnchor != null)
            // {
            //     humanAnchor.gameObject.SetActive(true);
            //     humanAnchor.position = targetPoint.position + Vector3.up * humanHeightOffset;

            //     Vector3 lookDir = player.position - humanAnchor.position;
            //     lookDir.y = 0f;
            //     if (lookDir.sqrMagnitude > 0.001f)
            //         humanAnchor.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            // }

            // if (humanModel != null)
            //     humanModel.localScale = Vector3.zero;

            // humanGrowing = true;
            // growTimer = 0f;
            if(kyleSpawner != null)
            {
                kyleSpawner.SpawnKyles();
            }
        }
    }

    void UpdateHumanGrowth()
    {
        if (humanModel == null) return;

        growTimer += Time.deltaTime;
        float t = Mathf.Clamp01(growTimer / humanGrowDuration);

        // 用一个更有“长出来”感觉的 easing
        float eased = 1f - Mathf.Pow(1f - t, 3f);

        humanModel.localScale = Vector3.Lerp(Vector3.zero, humanFinalScale, eased);

        if (t >= 1f)
{
        humanGrowing = false;
        humanModel.localScale = humanFinalScale;

        if (kyleWave != null)
        {
            Debug.Log("StartWave called");
            kyleWave.StartWave();
        }
    }
    }
}