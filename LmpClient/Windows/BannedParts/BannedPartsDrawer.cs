﻿using LmpClient.Localization;
using UnityEngine;

namespace LmpClient.Windows.BannedParts
{
    public partial class BannedPartsWindow
    {
        public override void DrawWindowContent(int windowId)
        {
            GUILayout.BeginVertical();
            GUI.DragWindow(MoveRect);
            GUILayout.Label($"{_vesselName} ({_vesselId}) {LocalizationContainer.BannedPartsWindowText.Text}", BoldRedLabelStyle);
            GUILayout.Space(5);

            GUILayout.BeginVertical();
            ScrollPos = GUILayout.BeginScrollView(ScrollPos);
            foreach (var bannedPart in _bannedParts)
            {
                GUILayout.Label(bannedPart);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }
    }
}
