using System.Collections;
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
                initialized = true;
            }
        }
    }

    public void StartWave()
    {
        Debug.Log("KyleWave.StartWave entered");
        if (!initialized || isWaving) return;
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        isWaving = true;

        // 先把手臂抬起来
        float raiseDuration = 0.4f;
        float t = 0f;

        Quaternion upperRaised = upperArmStartRot * Quaternion.Euler(-35f, 0f, -25f);
        Quaternion lowerRaised = lowerArmStartRot * Quaternion.Euler(-20f, 0f, 0f);

        while (t < raiseDuration)
        {
            t += Time.deltaTime;
            float k = t / raiseDuration;

            rightUpperArm.localRotation = Quaternion.Slerp(upperArmStartRot, upperRaised, k);
            rightLowerArm.localRotation = Quaternion.Slerp(lowerArmStartRot, lowerRaised, k);

            yield return null;
        }

        // 挥手：前臂来回摆几次
        int waveCount = 15;
        float waveSpeed = 5f;

        for (int i = 0; i < waveCount; i++)
        {
            float timer = 0f;
            while (timer < Mathf.PI)
            {
                timer += Time.deltaTime * waveSpeed;

                float angle = Mathf.Sin(timer) * 25f;
                rightLowerArm.localRotation = lowerRaised * Quaternion.Euler(0f, angle, 0f);

                yield return null;
            }
        }

        // 停回“举手结束”的姿势
        rightUpperArm.localRotation = upperRaised;
        rightLowerArm.localRotation = lowerRaised;

        isWaving = false;
    }
}