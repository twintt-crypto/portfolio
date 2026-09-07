using UnityEngine;

namespace Crystal
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel. 
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        #region Simulations
        /// <summary>
        /// Simulation device that uses safe area due to a physical notch or software home bar. For use in Editor only.
        /// </summary>
        public enum SimDevice
        {
            /// <summary>
            /// Don't use a simulated safe area - GUI will be full screen as normal.
            /// </summary>
            None,
            /// <summary>
            /// Simulate the iPhone X and Xs (identical safe areas).
            /// </summary>
            iPhoneX,
            /// <summary>
            /// Simulate the iPhone Xs Max and XR (identical safe areas).
            /// </summary>
            iPhoneXsMax,
            /// <summary>
            /// Simulate the Google Pixel 3 XL using landscape left.
            /// </summary>
            Pixel3XL_LSL,
            /// <summary>
            /// Simulate the Google Pixel 3 XL using landscape right.
            /// </summary>
            Pixel3XL_LSR
        }

        /// <summary>
        /// Simulation mode for use in editor only. This can be edited at runtime to toggle between different safe areas.
        /// </summary>
        public static SimDevice Sim = SimDevice.None;

        /// <summary>
        /// Normalised safe areas for iPhone X with Home indicator (ratios are identical to Xs, 11 Pro). Absolute values:
        ///  PortraitU x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436;
        ///  PortraitD x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125;
        ///  LandscapeR x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        Rect[] NSA_iPhoneX = new Rect[]
        {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
        };

        /// <summary>
        /// Normalised safe areas for iPhone Xs Max with Home indicator (ratios are identical to XR, 11, 11 Pro Max). Absolute values:
        ///  PortraitU x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688;
        ///  PortraitD x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242;
        ///  LandscapeR x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        Rect[] NSA_iPhoneXsMax = new Rect[]
        {
            new Rect (0f, 102f / 2688f, 1f, 2454f / 2688f),  // Portrait
            new Rect (132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // Landscape
        };

        /// <summary>
        /// Normalised safe areas for Pixel 3 XL using landscape left. Absolute values:
        ///  PortraitU x=0, y=0, w=1440, h=2789 on full extents w=1440, h=2960;
        ///  PortraitD x=0, y=0, w=1440, h=2789 on full extents w=1440, h=2960;
        ///  LandscapeL x=171, y=0, w=2789, h=1440 on full extents w=2960, h=1440;
        ///  LandscapeR x=0, y=0, w=2789, h=1440 on full extents w=2960, h=1440.
        ///  Aspect Ratio: 18.5:9.
        /// </summary>
        Rect[] NSA_Pixel3XL_LSL = new Rect[]
        {
            new Rect (0f, 0f, 1f, 2789f / 2960f),  // Portrait
            new Rect (0f, 0f, 2789f / 2960f, 1f)  // Landscape
        };

        /// <summary>
        /// Normalised safe areas for Pixel 3 XL using landscape right. Absolute values and aspect ratio same as above.
        /// </summary>
        Rect[] NSA_Pixel3XL_LSR = new Rect[]
        {
            new Rect (0f, 0f, 1f, 2789f / 2960f),  // Portrait
            new Rect (171f / 2960f, 0f, 2789f / 2960f, 1f)  // Landscape
        };
        #endregion

        RectTransform Panel;
        Rect LastSafeArea = new Rect (0, 0, 0, 0);
        Vector2Int LastScreenSize = new Vector2Int (0, 0);
        ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;
        [SerializeField] bool ConformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField] bool ConformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        [SerializeField] bool Logging = false;  // Conform to screen safe area on Y-axis (default true, disable to ignore)

        void Awake ()
        {
            Panel = GetComponent<RectTransform> ();

            if (Panel == null)
            {
                Debug.LogError ("Cannot apply safe area - no RectTransform found on " + name);
                Destroy (gameObject);
            }

            Refresh ();
        }

        void Update ()
        {
            Refresh ();
        }

        void Refresh ()
        {
            Rect safeArea = GetSafeArea ();

            if (safeArea != LastSafeArea
                || Screen.width != LastScreenSize.x
                || Screen.height != LastScreenSize.y
                || Screen.orientation != LastOrientation)
            {
                // Fix for having auto-rotate off and manually forcing a screen orientation.
                // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
                LastScreenSize.x = Screen.width;
                LastScreenSize.y = Screen.height;
                LastOrientation = Screen.orientation;

                ApplySafeArea (safeArea);
            }
        }

        Rect GetSafeArea ()
        {
            Rect safeArea = Screen.safeArea;

            if (Application.isEditor && Sim != SimDevice.None)
            {
                Rect nsa = new Rect (0, 0, Screen.width, Screen.height);

                switch (Sim)
                {
                    case SimDevice.iPhoneX:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_iPhoneX[0];
                        else  // Landscape
                            nsa = NSA_iPhoneX[1];
                        break;
                    case SimDevice.iPhoneXsMax:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_iPhoneXsMax[0];
                        else  // Landscape
                            nsa = NSA_iPhoneXsMax[1];
                        break;
                    case SimDevice.Pixel3XL_LSL:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_Pixel3XL_LSL[0];
                        else  // Landscape
                            nsa = NSA_Pixel3XL_LSL[1];
                        break;
                    case SimDevice.Pixel3XL_LSR:
                        if (Screen.height > Screen.width)  // Portrait
                            nsa = NSA_Pixel3XL_LSR[0];
                        else  // Landscape
                            nsa = NSA_Pixel3XL_LSR[1];
                        break;
                    default:
                        break;
                }

                safeArea = new Rect (Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
            }

            return safeArea;
        }

        void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;
#if !UNITY_EDITOR && UNITY_ANDROID
            // if (SystemManager.Instance.AndroidVersionCheck() >= 35)
            // {
            //     if(SystemManager.Instance.IsGestureMode() == false)
            //     {
            //         // 세로(포트레이트) 모드: 기존 로직 유지 (하단 제스처 바 보정)
            //         if (Screen.height >= Screen.width)
            //         {
            //             // 하단 제외 영역 계산 (SafeArea 기준으로 실제 제외된 하단 높이 + 보정값)
            //             float bottomPadding = Screen.height - (r.y + r.height) + 25f;
            //
            //             if (bottomPadding > 0f)
            //             {
            //                 r.y += bottomPadding;
            //                 r.height = Mathf.Max(0f, r.height - bottomPadding);
            //             }
            //         }
            //         // 가로(랜드스케이프) 모드: 좌/우 제스처 바 보정
            //         else
            //         {
            //             float padding = Mathf.Max(0f, r.x) + 25;
            //
            //             bool isLandscapeLeft =
            //                 (Screen.width > Screen.height) &&
            //                 (
            //                     Screen.orientation == ScreenOrientation.LandscapeLeft ||
            //                     (Screen.orientation == ScreenOrientation.AutoRotation &&
            //                      Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
            //                 );
            //
            //             if (isLandscapeLeft)
            //             {
            //                 r.width = Mathf.Max(0f, r.width - padding);
            //             }
            //         }
            //     }                                   
            // }           
#endif

//#if !UNITY_EDITOR && UNITY_IOS
                    // 세로(포트레이트) 모드
                    /*if (Screen.height >= Screen.width)
                    {
                        r.height = Screen.height - r.y;

                    }
                    // 가로(랜드스케이프) 모드
                    else
                    {
                        bool isLandscapeLeft =
                            (Screen.width > Screen.height) &&
                            (
                                Screen.orientation == ScreenOrientation.LandscapeLeft ||
                                (Screen.orientation == ScreenOrientation.AutoRotation &&
                                 Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
                            );

                        if (isLandscapeLeft)
                        {
                            r.width = Screen.width - r.x;
                        }
                    }*/
//#endif

            // Ignore x-axis?
            if (!ConformX)
            {
                r.x = 0;
                r.width = Screen.width;
            }

            // Ignore y-axis?
            if (!ConformY)
            {
                r.y = 0;
                r.height = Screen.height;
            }

            // Check for invalid screen startup state on some Samsung devices (see below)
            if (Screen.width > 0 && Screen.height > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalized anchor coordinates
                Vector2 anchorMin = r.position;
                Vector2 anchorMax = r.position + r.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                // Avoid NaN issue
                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    Panel.anchorMin = anchorMin;
                    Panel.anchorMax = anchorMax;
                }
            }

            if (Logging)
            {
                Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
            }
        }
    }
}
