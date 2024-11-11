using UnityEngine;

namespace Modules
{
    /// <summary>
    ///     노치가 있는 모바일 기기를 위한 안전 영역 구현. 사용법:
    ///     (1) GUI 패널의 최상위 레벨에 이 컴포넌트를 추가하세요.
    ///     (2) 패널이 전체 화면 배경 이미지를 사용하는 경우, 즉시 자식을 생성하고 그 자식에 컴포넌트를 추가한 다음 모든 요소를 그 아래에 자식으로 배치하세요.
    ///     이렇게 하면 배경 이미지가 노치 뒤에 있는 화면의 전체 영역까지 확장되어 더 보기 좋습니다.
    ///     (3) 전체 수평 및 수직 배경 줄무늬를 혼합하여 사용하는 경우, 필요한 요소별로 X 및 Y 축의 조정을 사용하세요.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        [SerializeField] private bool ConformX = true; // X축에 대해 화면 안전 영역을 따를지 여부 (기본값 true)
        [SerializeField] private bool ConformY = true; // Y축에 대해 화면 안전 영역을 따를지 여부 (기본값 true)
        [SerializeField] private bool Logging; // 로그를 활성화할지 여부 (기본값 false)
        private ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;
        private Rect LastSafeArea = new(0, 0, 0, 0);
        private Vector2Int LastScreenSize = new(0, 0);

        private RectTransform Panel;

        private void Awake()
        {
            Panel = GetComponent<RectTransform>();

            if (Panel == null)
            {
                Debug.LogError("안전 영역을 적용할 수 없습니다 - " + name + "에 RectTransform이 없습니다.");
                Destroy(gameObject);
            }

            Refresh();
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea
                || Screen.width != LastScreenSize.x
                || Screen.height != LastScreenSize.y
                || Screen.orientation != LastOrientation)
            {
                // 자동 회전이 꺼져 있고 화면 방향을 수동으로 강제 설정한 경우를 수정합니다.
                LastScreenSize.x = Screen.width;
                LastScreenSize.y = Screen.height;
                LastOrientation = Screen.orientation;

                ApplySafeArea(safeArea);
            }
        }

        private Rect GetSafeArea()
        {
            Rect safeArea = Screen.safeArea;

            if (Application.isEditor && Sim != SimDevice.None)
            {
                var nsa = new Rect(0, 0, Screen.width, Screen.height);

                switch (Sim)
                {
                    case SimDevice.iPhoneX:
                        nsa = Screen.height > Screen.width ? NSA_iPhoneX[0] : NSA_iPhoneX[1];
                        break;
                    case SimDevice.iPhoneXsMax:
                        nsa = Screen.height > Screen.width ? NSA_iPhoneXsMax[0] : NSA_iPhoneXsMax[1];
                        break;
                    case SimDevice.Pixel3XL_LSL:
                        nsa = Screen.height > Screen.width ? NSA_Pixel3XL_LSL[0] : NSA_Pixel3XL_LSL[1];
                        break;
                    case SimDevice.Pixel3XL_LSR:
                        nsa = Screen.height > Screen.width ? NSA_Pixel3XL_LSR[0] : NSA_Pixel3XL_LSR[1];
                        break;
                }

                safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width,
                    Screen.height * nsa.height);
            }

            return safeArea;
        }

        private void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            if (!ConformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            if (!ConformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            // 일부 삼성 기기에서의 잘못된 화면 초기 상태 수정
            if (Screen.width > 0 && Screen.height > 0)
            {
                // 안전 영역 사각형을 절대 픽셀에서 정규화된 앵커 좌표로 변환
                Vector2 anchorMin = r.position;
                Vector2 anchorMax = r.position + r.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                // 일부 삼성 기기 (예: Note 10+, A71, S20)에서 Refresh가 두 번 호출되고 첫 번째 호출에서 NaN 앵커 좌표가 반환되는 문제 수정
                if (anchorMin is { x: >= 0, y: >= 0 } && anchorMax is { x: >= 0, y: >= 0 })
                {
                    Panel.anchorMin = anchorMin;
                    Panel.anchorMax = anchorMax;
                }
            }

            if (Logging)
                Debug.LogFormat("{0}에 새로운 안전 영역이 적용되었습니다: x={1}, y={2}, w={3}, h={4} (전체 영역: w={5}, h={6})",
                    name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }

        #region 시뮬레이션

        /// <summary>
        ///     물리적 노치나 소프트웨어 홈 바로 인해 안전 영역을 사용하는 시뮬레이션 기기. 에디터에서만 사용합니다.
        /// </summary>
        public enum SimDevice
        {
            /// <summary>
            ///     시뮬레이션된 안전 영역을 사용하지 않음 - GUI는 일반적으로 전체 화면을 사용합니다.
            /// </summary>
            None,

            /// <summary>
            ///     iPhone X 및 Xs를 시뮬레이션합니다 (안전 영역이 동일함).
            /// </summary>
            iPhoneX,

            /// <summary>
            ///     iPhone Xs Max 및 XR을 시뮬레이션합니다 (안전 영역이 동일함).
            /// </summary>
            iPhoneXsMax,

            /// <summary>
            ///     Google Pixel 3 XL의 왼쪽 가로 모드를 시뮬레이션합니다.
            /// </summary>
            Pixel3XL_LSL,

            /// <summary>
            ///     Google Pixel 3 XL의 오른쪽 가로 모드를 시뮬레이션합니다.
            /// </summary>
            Pixel3XL_LSR
        }

        /// <summary>
        ///     에디터에서만 사용하는 시뮬레이션 모드. 다른 안전 영역으로 전환하려면 런타임 시 수정할 수 있습니다.
        /// </summary>
        public const SimDevice Sim = SimDevice.None;

        /// <summary>
        ///     iPhone X에서 홈 인디케이터가 있는 경우의 정규화된 안전 영역 (Xs, 11 Pro와 동일한 비율). 절대 값:
        ///     PortraitU x=0, y=102, w=1125, h=2202 (전체 영역 w=1125, h=2436);
        ///     PortraitD x=0, y=102, w=1125, h=2202 (전체 영역 w=1125, h=2436, Portrait Up으로 유지);
        ///     LandscapeL x=132, y=63, w=2172, h=1062 (전체 영역 w=2436, h=1125);
        ///     LandscapeR x=132, y=63, w=2172, h=1062 (전체 영역 w=2436, h=1125).
        ///     종횡비: 약 19.5:9.
        /// </summary>
        private readonly Rect[] NSA_iPhoneX =
        {
            new(0f, 102f / 2436f, 1f, 2202f / 2436f), // 세로
            new(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f) // 가로
        };

        /// <summary>
        ///     iPhone Xs Max에서 홈 인디케이터가 있는 경우의 정규화된 안전 영역 (XR, 11, 11 Pro Max와 동일한 비율). 절대 값:
        ///     PortraitU x=0, y=102, w=1242, h=2454 (전체 영역 w=1242, h=2688);
        ///     PortraitD x=0, y=102, w=1242, h=2454 (전체 영역 w=1242, h=2688, Portrait Up으로 유지);
        ///     LandscapeL x=132, y=63, w=2424, h=1179 (전체 영역 w=2688, h=1242);
        ///     LandscapeR x=132, y=63, w=2424, h=1179 (전체 영역 w=2688, h=1242).
        ///     종횡비: 약 19.5:9.
        /// </summary>
        private readonly Rect[] NSA_iPhoneXsMax =
        {
            new(0f, 102f / 2688f, 1f, 2454f / 2688f), // 세로
            new(132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f) // 가로
        };

        /// <summary>
        ///     Pixel 3 XL의 왼쪽 가로 모드에 대한 정규화된 안전 영역. 절대 값:
        ///     PortraitU x=0, y=0, w=1440, h=2789 (전체 영역 w=1440, h=2960);
        ///     PortraitD x=0, y=0, w=1440, h=2789 (전체 영역 w=1440, h=2960);
        ///     LandscapeL x=171, y=0, w=2789, h=1440 (전체 영역 w=2960, h=1440);
        ///     LandscapeR x=0, y=0, w=2789, h=1440 (전체 영역 w=2960, h=1440).
        ///     종횡비: 18.5:9.
        /// </summary>
        private readonly Rect[] NSA_Pixel3XL_LSL =
        {
            new(0f, 0f, 1f, 2789f / 2960f), // 세로
            new(0f, 0f, 2789f / 2960f, 1f) // 가로
        };

        /// <summary>
        ///     Pixel 3 XL의 오른쪽 가로 모드에 대한 정규화된 안전 영역. 절대 값 및 종횡비는 위와 동일.
        /// </summary>
        private readonly Rect[] NSA_Pixel3XL_LSR =
        {
            new(0f, 0f, 1f, 2789f / 2960f), // 세로
            new(171f / 2960f, 0f, 2789f / 2960f, 1f) // 가로
        };

        #endregion
    }
}