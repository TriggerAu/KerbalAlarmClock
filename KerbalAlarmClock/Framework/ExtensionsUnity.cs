/* Part of KSPPluginFramework
Version 1.2

Forum Thread:https://forum.kerbalspaceprogram.com/topic/60381-ksp-plugin-framework-plugin-examples-and-structure/
Author: TriggerAu, 2014
License: The MIT License (MIT)
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace KSPPluginFramework
{
    /// <summary>
    /// CLass containing some extension methods for Unity Objects
    /// </summary>
    public static class UnityExtensions
    {
        /// <summary>
        /// Ensure that the Rect remains within the screen bounds
        /// </summary>
        public static Rect ClampToScreen(this Rect r)
        {
            return r.ClampToScreen(new RectOffset(0, 0, 0, 0));
        }

        /// <summary>
        /// Ensure that the Rect remains within the screen bounds
        /// </summary>
        /// <param name="ScreenBorder">A Border to the screen bounds that the Rect will be clamped inside (can be negative)</param>
        public static Rect ClampToScreen(this Rect r, RectOffset ScreenBorder)
        {
            return r.ClampToScreen(ScreenBorder, 1f);
        }

        private static RectOffset zeroRectOffset;
        /// <summary>
        /// Ensure that the Rect remains within the screen bounds
        /// </summary>
        /// <param name="ScreenBorder">A Border to the screen bounds that the Rect will be clamped inside (can be negative)</param>
        /// <param name="scale">the UIScale to calc at</param>
        public static Rect ClampToScreen(this Rect r, RectOffset ScreenBorder, float scale)
        {
            if (ScreenBorder == null)
            {
                //catch a default if we need it
                if (zeroRectOffset == null)
                {
                    zeroRectOffset = new RectOffset(0, 0, 0, 0);
                }
                ScreenBorder = zeroRectOffset;
            }
            r.x = Mathf.Clamp(r.x * scale, ScreenBorder.left * scale, Screen.width - r.width * scale - ScreenBorder.right * scale) / scale;
            r.y = Mathf.Clamp(r.y * scale, ScreenBorder.top * scale, Screen.height - r.height * scale - ScreenBorder.bottom * scale) / scale;

            if (r.x < 0)
            {
                r.x = 0;
            }

            if (r.y < 0)
            {
                r.y = 0;
            }
            return r;
        }

        public static GUIStyle PaddingChange(this GUIStyle g, Int32 PaddingValue)
        {
            GUIStyle gReturn = new GUIStyle(g);
            gReturn.padding = new RectOffset(PaddingValue, PaddingValue, PaddingValue, PaddingValue);
            return gReturn;
        }
        public static GUIStyle PaddingChangeBottom(this GUIStyle g, Int32 PaddingValue)
        {
            GUIStyle gReturn = new GUIStyle(g);
            gReturn.padding.bottom = PaddingValue;
            return gReturn;
        }
        public static GUIStyle PaddingChangeLeft(this GUIStyle g, Int32 PaddingValue)
        {
            GUIStyle gReturn = new GUIStyle(g);
            gReturn.padding.left = PaddingValue;
            return gReturn;
        }
    }
}