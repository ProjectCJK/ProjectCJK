using UnityEngine;

namespace Modules
{
    /// <summary>
    /// 노치가 있는 모바일 기기를 위한 안전 영역 구현.
    /// 사용법:
    /// 1. 이 컴포넌트를 GUI 패널의 최상위 레벨에 추가하세요.
    /// 2. 패널이 전체 화면 배경 이미지를 사용하는 경우, 즉시 자식을 생성하고 이 컴포넌트를 추가한 다음 모든 요소를 자식으로 배치하세요.
    /// 3. 전체 수평 및 수직 배경을 혼합하여 사용하는 경우, 필요한 요소별로 X 및 Y 축 조정을 사용하세요.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        #region 시뮬레이션 기기 정의

        /// <summary>
        /// 시뮬레이션에 사용할 디바이스. 에디터 환경에서만 사용됩니다.
        /// </summary>
        public enum SimDevice
        {
            None,           // 시뮬레이션 사용 안 함 (기본값)
            iPhoneX,        // iPhone X 및 iPhone Xs
            iPhoneXsMax,    // iPhone Xs Max 및 XR
            Pixel3XL_LSL,   // Google Pixel 3 XL (Landscape Left)
            Pixel3XL_LSR    // Google Pixel 3 XL (Landscape Right)
        }

        /// <summary>
        /// 현재 시뮬레이션 모드. 에디터에서 변경 가능.
        /// </summary>
        [SerializeField] private SimDevice Sim = SimDevice.None;

        #endregion

        #region 시뮬레이션 기기 안전 영역 데이터

        private readonly Rect[] NSA_iPhoneX = 
        {
            new Rect(0f, 102f / 2436f, 1f, 2202f / 2436f),  // 세로
            new Rect(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // 가로
        };

        private readonly Rect[] NSA_iPhoneXsMax = 
        {
            new Rect(0f, 102f / 2688f, 1f, 2454f / 2688f),  // 세로
            new Rect(132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // 가로
        };

        private readonly Rect[] NSA_Pixel3XL_LSL = 
        {
            new Rect(0f, 0f, 1f, 2789f / 2960f),  // 세로
            new Rect(0f, 0f, 2789f / 2960f, 1f)   // 가로
        };

        private readonly Rect[] NSA_Pixel3XL_LSR = 
        {
            new Rect(0f, 0f, 1f, 2789f / 2960f),  // 세로
            new Rect(171f / 2960f, 0f, 2789f / 2960f, 1f)  // 가로
        };

        #endregion

        private RectTransform Panel;
        private Rect LastSafeArea = new Rect(0, 0, 0, 0);
        private Vector2Int LastScreenSize = new Vector2Int(0, 0);
        private ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;

        [SerializeField] private bool ConformX = true;  // X축 안전 영역 반영 여부
        [SerializeField] private bool ConformY = true;  // Y축 안전 영역 반영 여부
        [SerializeField] private bool Logging = false; // 디버깅 로그 출력 여부

        /// <summary>
        /// 초기화. RectTransform을 설정하고 안전 영역을 즉시 갱신합니다.
        /// </summary>
        private void Awake()
        {
            Panel = GetComponent<RectTransform>();
            if (Panel == null)
            {
                Debug.LogError("RectTransform을 찾을 수 없습니다. SafeArea가 제거됩니다.");
                Destroy(gameObject);
                return;
            }

            Refresh();
        }

        /// <summary>
        /// 매 프레임마다 안전 영역 갱신 필요 여부를 확인합니다.
        /// </summary>
        private void Update()
        {
            Refresh();
        }

        /// <summary>
        /// 안전 영역을 계산하고 적용합니다.
        /// </summary>
        private void Refresh()
        {
            Rect safeArea = GetSafeArea();
            if (safeArea != LastSafeArea || Screen.width != LastScreenSize.x || Screen.height != LastScreenSize.y || Screen.orientation != LastOrientation)
            {
                LastScreenSize = new Vector2Int(Screen.width, Screen.height);
                LastOrientation = Screen.orientation;
                ApplySafeArea(safeArea);
            }
        }

        /// <summary>
        /// 안전 영역 정보를 가져옵니다. 에디터 환경에서는 시뮬레이션 데이터를 사용합니다.
        /// </summary>
        /// <returns>현재 안전 영역(Rect)</returns>
        private Rect GetSafeArea()
        {
            Rect safeArea = Screen.safeArea;

            if (Application.isEditor && Sim != SimDevice.None)
            {
                Rect simulatedArea = new Rect(0, 0, Screen.width, Screen.height);
                switch (Sim)
                {
                    case SimDevice.iPhoneX:
                        simulatedArea = Screen.height > Screen.width ? NSA_iPhoneX[0] : NSA_iPhoneX[1];
                        break;
                    case SimDevice.iPhoneXsMax:
                        simulatedArea = Screen.height > Screen.width ? NSA_iPhoneXsMax[0] : NSA_iPhoneXsMax[1];
                        break;
                    case SimDevice.Pixel3XL_LSL:
                        simulatedArea = Screen.height > Screen.width ? NSA_Pixel3XL_LSL[0] : NSA_Pixel3XL_LSL[1];
                        break;
                    case SimDevice.Pixel3XL_LSR:
                        simulatedArea = Screen.height > Screen.width ? NSA_Pixel3XL_LSR[0] : NSA_Pixel3XL_LSR[1];
                        break;
                }

                safeArea = new Rect(Screen.width * simulatedArea.x, Screen.height * simulatedArea.y, Screen.width * simulatedArea.width, Screen.height * simulatedArea.height);
            }

            return safeArea;
        }

        /// <summary>
        /// 안전 영역을 적용하여 패널의 앵커를 조정합니다.
        /// </summary>
        /// <param name="safeArea">적용할 안전 영역(Rect)</param>
        private void ApplySafeArea(Rect safeArea)
        {
            LastSafeArea = safeArea;

            if (!ConformX)
            {
                safeArea.x = 0;
                safeArea.width = Screen.width;
            }

            if (!ConformY)
            {
                safeArea.y = 0;
                safeArea.height = Screen.height;
            }

            if (Screen.width > 0 && Screen.height > 0)
            {
                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size;

                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    Panel.anchorMin = anchorMin;
                    Panel.anchorMax = anchorMax;
                }
            }

            if (Logging)
            {
                Debug.LogFormat("SafeArea 적용: x={0}, y={1}, w={2}, h={3} (화면 크기: w={4}, h={5})", safeArea.x, safeArea.y, safeArea.width, safeArea.height, Screen.width, Screen.height);
            }
        }
    }
}
