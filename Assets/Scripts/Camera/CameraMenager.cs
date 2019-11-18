using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class CameraMenager : MonoBehaviour
{
    [SerializeField]
    private float kWidth = 160f;
    [SerializeField]
    private float kThickHeight = 30f;
    [SerializeField]
    private Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
    [SerializeField]
    private float s_TextFontSize = 14;
    [SerializeField]
    private Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
    private string kStandardSpritePath = "UI/Skin/UISprite.psd";

    public Vector3 offset;
    Vector3 startoffset;
    public Vector3 offsetmove;

    private Vector2 s_ThickGUIElementSize;
    private GameObject parent;
    List<Object> cameras;
    List<GameObject> buttonRoots = new List<GameObject>();

    void Start()
    {
        s_ThickGUIElementSize  = new Vector2(kWidth, kThickHeight);
        parent = GetOrCreateCanvasGameObject();
        startoffset = offset;
    }

    private GameObject CreateUIElementRoot(string name, Vector2 size)
    {

        GameObject child = new GameObject(name);

        Undo.RegisterCreatedObjectUndo(child, "Create " + name);
        Undo.SetTransformParent(child.transform, parent.transform, "Parent " + child.name);
        GameObjectUtility.SetParentAndAlign(child, parent);

        RectTransform rectTransform = child.AddComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), rectTransform);
        Selection.activeGameObject = child;
        return child;
    }

     
  
    public void ButtonDisplayCams()
    {
        foreach (Camera go in Resources.FindObjectsOfTypeAll(typeof(Camera)))
        {
            /* if (!go.scene.isLoaded)
                 continue;*/
            //if (go.GetComponent<Camera>() != null)
           /* if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                continue;
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject))
                continue;*/
            cameras.Add(go);
        }

            for (int i = 0;i < cameras.Count; i++)
        {
            offset += offsetmove;
            GameObject buttonRoot = CreateUIElementRoot("Button", s_ThickGUIElementSize);
            GameObject childText = new GameObject(i.ToString());
            GameObjectUtility.SetParentAndAlign(childText, buttonRoot);

            Image image = buttonRoot.AddComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            Button bt = buttonRoot.AddComponent<Button>();
            SetDefaultColorTransitionValues(bt);
            bt.onClick.AddListener(() => TaskOnClick(int.Parse(childText.name)));

            TextMeshProUGUI text = childText.AddComponent<TextMeshProUGUI>();
            text.text = i.ToString();
            text.alignment = TextAlignmentOptions.Center;
            SetDefaultTextValues(text);

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            buttonRoots.Add(buttonRoot);
        }
        offset = startoffset;
    }
    public GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        return null;
    }
    private void SetDefaultTextValues(TextMeshProUGUI lbl)
    {
        lbl.color = s_TextColor;
        lbl.fontSize = s_TextFontSize;
    }
    private void SetDefaultColorTransitionValues(Selectable slider)
    {
        ColorBlock colors = slider.colors;
        colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
        colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
        colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
    }

    private void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
    {


        Vector2 localPlanePosition;
        Vector3 position = Vector3.zero;
        localPlanePosition.x = transform.position.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
        localPlanePosition.y = transform.position.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

            localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
            localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);


            position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
            position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

            Vector3 minLocalPosition;
            minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
            minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

            Vector3 maxLocalPosition;
            maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
            maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

            position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x) + offset.x;
            position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y) + offset.y;

        itemTransform.anchoredPosition = position;
        itemTransform.localRotation = Quaternion.identity;
        itemTransform.localScale = Vector3.one;
    }

    void TaskOnClick(int buttonOn)
    {
        foreach (Camera camera in cameras)
        {
            camera.enabled = false;
        }
        Camera cam = cameras[buttonOn] as Camera;
        cam.enabled = true;
        foreach (GameObject btn in buttonRoots)
        {
           Destroy(btn);
        }
        buttonRoots.Clear();
    }


}
