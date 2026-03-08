using UnityEngine;

public class KyleWave : MonoBehaviour
{
    private Animator animator;
    private Transform rightUpperArm;
    private Transform rightLowerArm;

    private Quaternion upperArmStartRot;
    private Quaternion lowerArmStartRot;

    private bool initialized = false;
    private bool isWaving = false;

    private float waveTimer = 0f;
    private float raiseDuration = 0.35f;
    private float holdDuration = 1.5f;
    private float lowerDuration = 0.5f;

    private Quaternion upperRaised;
    private Quaternion lowerRaised;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null && animator.isHuman)
        {
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);

            if (rightUpperArm != null && rightLowerArm != null)
            {
                upperArmStartRot = rightUpperArm.localRotation;
                lowerArmStartRot = rightLowerArm.localRotation;

                // 这组角度先用更明显一点的
                upperRaised = upperArmStartRot * Quaternion.Euler(-65f, 0f, -15f);
                lowerRaised = lowerArmStartRot * Quaternion.Euler(-35f, 0f, 0f);

                initialized = true;
            }
        }
    }

    public void StartWave()
    {
        Debug.Log("KyleWave.StartWave entered");

        if (!initialized || isWaving) return;

        waveTimer = 0f;
        isWaving = true;
    }

    void LateUpdate()
    {
        if (!isWaving || !initialized) return;

        waveTimer += Time.deltaTime;

        // 1. 抬手
        if (waveTimer < raiseDuration)
        {
            float t = waveTimer / raiseDuration;
            rightUpperArm.localRotation = Quaternion.Slerp(upperArmStartRot, upperRaised, t);
            rightLowerArm.localRotation = Quaternion.Slerp(lowerArmStartRot, lowerRaised, t);
        }
        // 2. 挥手
        else if (waveTimer < raiseDuration + holdDuration)
        {
            float localT = waveTimer - raiseDuration;
            float angle = Mathf.Sin(localT * 10f) * 35f;

            rightUpperArm.localRotation = upperRaised;
            rightLowerArm.localRotation = lowerRaised * Quaternion.Euler(0f, angle, 0f);
        }
        // 3. 放下手
        else if (waveTimer < raiseDuration + holdDuration + lowerDuration)
        {
            float t = (waveTimer - raiseDuration - holdDuration) / lowerDuration;
            rightUpperArm.localRotation = Quaternion.Slerp(upperRaised, upperArmStartRot, t);
            rightLowerArm.localRotation = Quaternion.Slerp(lowerRaised, lowerArmStartRot, t);
        }
        else
        {
            rightUpperArm.localRotation = upperArmStartRot;
            rightLowerArm.localRotation = lowerArmStartRot;
            isWaving = false;
        }
    }
}