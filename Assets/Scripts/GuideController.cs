using UnityEngine;

public class GuideController : MonoBehaviour
{
    public Transform player;
    public Transform guideParticles;
    public Transform targetPoint;

    [Header("Effect Objects")]
    public ParticleSystem guideParticleSystem;
    public GameObject arrivalSwirlParticles;
    public GameObject humanModel;

    [Header("Guide Placement")]
    public float hoverHeight = 1.4f;
    public float idealFrontDistance = 1.2f;
    public float maxDistanceFromPlayer = 2.0f;

    [Header("Guide Movement")]
    public float moveSpeed = 0.8f;
    public float followLerpSpeed = 3.0f;
    public float arriveDistance = 0.5f;

    [Header("Arrival")]
    public float playerArrivalDistance = 0.8f;
    public float swirlDuration = 2.0f;
    public float swirlRotationSpeed = 120f;

    private bool reachedTarget = false;
    private bool transformationStarted = false;
    private float arrivalTimer = 0f;

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

        if (humanModel != null)
            humanModel.SetActive(false);
    }

    void Update()
    {
        if (player == null || guideParticles == null || targetPoint == null) return;

        if (!reachedTarget)
        {
            UpdateGuideMovement();
            CheckArrival();
        }
        else
        {
            UpdateArrivalEffect();
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
        if (transformationStarted) return;

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

            if (humanModel != null)
            {
                humanModel.SetActive(true);
                humanModel.transform.position = targetPoint.position;
                humanModel.transform.rotation = Quaternion.LookRotation(
                    player.position - humanModel.transform.position,
                    Vector3.up
                );
            }
        }
    }
}