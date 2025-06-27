using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FullpitImage : MonoBehaviour
{
    public Image image; // Preserved Aspect가 켜진 이미지
    public RectTransform fitPanel; // 이 패널이 '실제 이미지 영역'을 커버

    void Start()
    {
        UpdateFitPanel();
    }

    void UpdateFitPanel()
    {
        RectTransform imgRect = image.rectTransform;
        Rect spriteRect = image.sprite.rect;
        float spriteAspect = spriteRect.width / spriteRect.height;

        float parentWidth = imgRect.rect.width;
        float parentHeight = imgRect.rect.height;
        float parentAspect = parentWidth / parentHeight;

        float fitWidth, fitHeight;
        if (spriteAspect > parentAspect)
        {
            // 좌우 기준으로 맞춤, 위아래 여백 발생
            fitWidth = parentWidth;
            fitHeight = parentWidth / spriteAspect;
        }
        else
        {
            // 위아래 기준으로 맞춤, 좌우 여백 발생
            fitHeight = parentHeight;
            fitWidth = parentHeight * spriteAspect;
        }

        // fitPanel을 "실제 Sprite 표시 크기"로 설정
        fitPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fitWidth);
        fitPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fitHeight);
        fitPanel.anchoredPosition = Vector2.zero; // 이미지 중앙
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(FullpitImage))]
    public class FullpitImageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FullpitImage script = (FullpitImage)target;

            if (GUILayout.Button("이미지 크기에 맞춰 패널 조정"))
            {
                script.UpdateFitPanel();
                // SceneView 갱신
                EditorUtility.SetDirty(script);
            }
        }
    }
#endif
}