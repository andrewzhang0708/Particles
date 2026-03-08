using UnityEngine;

public class GuideController : MonoBehaviour
{
    public Transform player;
    public Transform guideParticles;
    public Transform targetPoint;

    [Header("Guide Placement")]
    public float hoverHeight = 1.4f;
    public float idealFrontDistance = 1.2f;
    public float maxDistanceFromPlayer = 2.0f;

    [Header("Guide Movement")]
    public float moveSpeed = 0.8f;
    public float followLerpSpeed = 3.0f;
    public float arriveDistance = 0.5f;

    private bool reachedTarget = false;

    void Start()
    {
        if (player != null && guideParticles != null)
        {
            Vector3 startPos = player.position + player.forward * idealFrontDistance;
            startPos.y = hoverHeight;
            guideParticles.position = startPos;
        }
    }

    void Update()
    {
        if (player == null || guideParticles == null || targetPoint == null) return;
        if (reachedTarget) return;

        Vector3 playerFlat = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 guideFlat = new Vector3(guideParticles.position.x, 0f, guideParticles.position.z);
        Vector3 targetFlat = new Vector3(targetPoint.position.x, 0f, targetPoint.position.z);

        float distGuideToTarget = Vector3.Distance(guideFlat, targetFlat);
        if (distGuideToTarget <= arriveDistance)
        {
            reachedTarget = true;
            return;
        }

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
}