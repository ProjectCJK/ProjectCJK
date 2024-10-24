using UnityEngine;

namespace Modules
{
    // TODO : 프레임 고정을 위해 이후 스플래쉬 이미지 띄우는 시점에 반드시 Initialize 호출해줄 것.
    public class FrameRateController : MonoBehaviour
    {
        public void Initialize()
        {
            // 프레임 고정
            Application.targetFrameRate = 60;
            // VSync 비활성화
            QualitySettings.vSyncCount = 0;
        }
    }
}