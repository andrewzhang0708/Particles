using UnityEngine;

public class GuideController : MonoBehaviour
{
    public Transform player;
    public Transform guideParticles;

    public float height = 1.4f;
    public float frontDistance = 1.2f;
    public float followSpeed = 2.0f;

    void Update()
    {
        if (player == null || guideParticles == null) return;

        Vector3 targetPos = player.position + player.forward * frontDistance;
        targetPos.y = height;

        guideParticles.position = Vector3.Lerp(
            guideParticles.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }
}