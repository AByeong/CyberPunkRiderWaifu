using UnityEngine;

public class EliteAI_Female : MonoBehaviour
{
    public Animator EliteAnimator;

    // Inspector에서 설정할 수 있는 private 변수입니다.
    // NotRunning 상태가 된 후, 다시 Running 상태가 가능해지기까지의 대기 시간(초)입니다.
    [SerializeField]
    private float runCooldownDuration = 3.0f; // 예시: 3초의 쿨다운 시간을 기본값으로 설정

    private bool isCoolingDown = false; // 현재 쿨다운 상태인지 여부를 나타내는 플래그
    private float currentCooldownTime = 0f; // 현재 남은 쿨다운 시간을 추적하는 변수

    void Update()
    {
        // isCoolingDown 플래그가 true일 때 (즉, NotRunning 상태가 호출된 후) 쿨다운 타이머를 작동시킵니다.
        if (isCoolingDown)
        {
            currentCooldownTime -= Time.deltaTime; // 매 프레임 경과 시간을 쿨다운 시간에서 차감합니다.
            if (currentCooldownTime <= 0)
            {
                isCoolingDown = false; // 쿨다운 시간이 모두 지나면, 쿨다운 상태를 해제합니다.
                currentCooldownTime = 0f; // 타이머를 0으로 리셋합니다.
            }
        }
    }

    public void Running()
    {
        // 쿨다운 중이 아닐 때만 Running 상태로 전환할 수 있습니다.
        if (!isCoolingDown)
        {
            EliteAnimator.SetBool("Running", true);
        }
       
    }

    public void NotRunning()
    {
        EliteAnimator.SetBool("Running", false); // 애니메이터의 Running 상태를 false로 설정합니다.
        isCoolingDown = true;                   // 쿨다운 상태를 활성화합니다.
        currentCooldownTime = runCooldownDuration; // 설정된 쿨다운 시간으로 현재 쿨다운 타이머를 초기화합니다.
    }
}