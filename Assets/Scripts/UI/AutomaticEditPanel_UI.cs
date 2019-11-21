using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;

public class AutomaticEditPanel_UI : MonoBehaviour
{
    static Vector2 rectTransformlast;
    static CultureInfo ci = new CultureInfo("pl-PL");

    static TMP_InputField inputField;
    static GameObject editPanelContent;

    void Start()
    {
        inputField = GameObject.Find("Canvas/InputFieldExample").GetComponent<TMP_InputField>();
        editPanelContent = this.gameObject;
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        IndividualEdit.editPanel.SetActive(false);
    }

    public static void Start_UI()
    {
       Draw.Instance.Clear();
        foreach (Transform child in editPanelContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (Simulation.robotSelected.motors.Length > 0)
            AddText("Motors", editPanelContent);
        foreach (MotorToWheel motor in Simulation.robotSelected.motors)
            {
                GameObject parent = new GameObject();
                parent.transform.SetParent(editPanelContent.transform);
            parent.transform.localPosition = Vector3.zero;
            parent.AddComponent<VerticalLayoutGroup>();
            parent.AddComponent<LayoutElement>();
            parent.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(180, 40 + 3 * 30);
            AddText(motor.name,parent);
            GameObject parentButton = new GameObject();
            parentButton.transform.SetParent(parent.transform);
            parentButton.transform.localPosition = Vector3.zero;
            var element =parentButton.AddComponent<LayoutElement>();
            element.minHeight = 20;
            parentButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
            Draw.Instance.AddCircle(motor);
            AddButtonDelete(parentButton).onClick.AddListener(() => ButtonActionDelete(null, motor, parent));
                AddButtonSelect(parentButton).onClick.AddListener(() => ButtonActionSelect(motor.gameObject));
                TMP_InputField textInputFieldRPM = AddInputField("Maksymalne obroty na minute...", parent);
                textInputFieldRPM.text = motor.maxWheelRpm.ToString();
                textInputFieldRPM.onValueChanged.AddListener(delegate { ChangeValueMotorRPM(motor, textInputFieldRPM.text); });

            TMP_InputField textInputFieldTorque = AddInputField("Maksymalny moment obrotowy...", parent);
                textInputFieldTorque.text = motor.maxWheelTorque.ToString();
                textInputFieldTorque.onValueChanged.AddListener(delegate { ChangeValueMotorTorque(motor, textInputFieldTorque.text); });

            TMP_InputField textInputFieldRadius = AddInputField("Promień koła...", parent);
            if (motor.wheelCollider != null)
                textInputFieldRadius.text = motor.wheelCollider.radius.ToString();
            else
                Simulation.robotSelected.reload = true;
            textInputFieldRadius.onValueChanged.AddListener(delegate { ChangeValueMotorRadius(motor, textInputFieldRadius.text); });

        }
        if (Simulation.robotSelected.sensors.Length > 0)
            AddText("Sensors", editPanelContent);
        foreach (ParamSensor sensor in Simulation.robotSelected.sensors)
            {
            GameObject parent = new GameObject();
            parent.transform.SetParent(editPanelContent.transform);
            parent.transform.localPosition = Vector3.zero;
            parent.AddComponent<VerticalLayoutGroup>();
            parent.AddComponent<LayoutElement>();
            parent.GetComponent<RectTransform>().sizeDelta = 
                new Vector2(180, 40 + 3*30);
            AddText(sensor.name, parent);
            GameObject parentButton = new GameObject();
            parentButton.transform.SetParent(parent.transform);
            parentButton.transform.localPosition = Vector3.zero;
            var element = parentButton.AddComponent<LayoutElement>();
            element.minHeight = 20;
            parentButton.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 30);
            DrawSensor(sensor);
            AddButtonDelete(parentButton).onClick.AddListener(() => ButtonActionDelete(sensor, null, parent));
            AddButtonSelect(parentButton).onClick.AddListener(() => ButtonActionSelect(sensor.gameObject));

            TMP_InputField textInputFieldRange = AddInputField("Maksymalny zasięg...", parent);
                textInputFieldRange.text = sensor.maxRange.ToString();
                textInputFieldRange.onValueChanged.AddListener(delegate { ChangeValueSensorRange(sensor, textInputFieldRange.text); });

                TMP_InputField textInputFieldUpdateDt = AddInputField("Częstotliwość...", parent);
                textInputFieldUpdateDt.text = sensor.updateDt.ToString();
                textInputFieldUpdateDt.onValueChanged.AddListener(delegate { ChangeValueSensorUpdateDT(sensor, textInputFieldUpdateDt.text); });

                TMP_InputField textInputFieldFOV = AddInputField("Kąt widzenia...",parent);
                textInputFieldFOV.text = sensor.FOV.ToString();
                textInputFieldFOV.onValueChanged.AddListener(delegate { ChangeValueSensorFov(sensor, textInputFieldFOV.text); });
            }
    }


    static void AutomaticPosition(RectTransform rectTransform)
    {
        RectTransformExtensions.SetSize(rectTransform,new Vector2(180,30));
        RectTransformExtensions.SetPositionOfPivot(rectTransform, rectTransformlast);
    }

    static void AutomaticPositionButton(RectTransform rectTransform)
    {
        RectTransformExtensions.SetSize(rectTransform, new Vector2(20, 20));
        RectTransformExtensions.SetPositionOfPivot(rectTransform, rectTransformlast);
    }

    static void AddText(string text, GameObject parent)
    {
        GameObject gameObjectMotors = new GameObject();
        gameObjectMotors.transform.SetParent(parent.transform);
        var element = gameObjectMotors.AddComponent<LayoutElement>();
        element.minHeight = 25;
        TextMeshProUGUI textMeshProUGUI = gameObjectMotors.AddComponent<TextMeshProUGUI>();
        textMeshProUGUI.enableAutoSizing = true;
        textMeshProUGUI.fontSizeMin = 6;
        AutomaticPosition(textMeshProUGUI.GetComponent<RectTransform>());
        textMeshProUGUI.text = text;
    }

    static Button AddButtonDelete(GameObject parent)
    {
        GameObject gameObjectMotors = new GameObject();
        gameObjectMotors.transform.SetParent(parent.transform);
        gameObjectMotors.transform.localPosition = Vector3.zero;
        Button button = gameObjectMotors.AddComponent<Button>();
        var image = gameObjectMotors.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Icon/Delete");
        button.targetGraphic = image;
        RectTransformExtensions.SetSize(button.GetComponent<RectTransform>(), new Vector2(20, 20));
        rectTransformlast = new Vector2(0, 0);
        AutomaticPositionButton(button.GetComponent<RectTransform>());
        rectTransformlast = new Vector2(rectTransformlast.x + 30, rectTransformlast.y);
        return button;
    }

    static void ButtonActionDelete(ParamSensor sensor, MotorToWheel motor, GameObject parent)
    {
        if (sensor != null)
        {
            Destroy(sensor);
            Destroy(sensor.transform.GetChild(0).gameObject);
            DrawSensorUpdate();
        }
        else
        {
            Destroy(motor);
            DrawMotorUpdate();
        }
        Destroy(parent);
    }

    static Button AddButtonSelect( GameObject parent)
    { 
        GameObject gameObjectMotors = new GameObject();
        gameObjectMotors.transform.SetParent(parent.transform);
        gameObjectMotors.transform.localPosition = Vector3.zero;
        Button button = gameObjectMotors.AddComponent<Button>();
        var image = gameObjectMotors.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Icon/Select");
        button.targetGraphic = image;
        RectTransformExtensions.SetSize(button.GetComponent<RectTransform>(), new Vector2(20, 20));
     AutomaticPositionButton(button.GetComponent<RectTransform>());
        return button;
    }

    static void ButtonActionSelect(GameObject select)
    {
        IndividualEdit.ResetSelect();
        IndividualEdit.SelectObject(select);
    }

    static TMP_InputField AddInputField(string text, GameObject parent)
    {
        TMP_InputField textInputField = inputField.Spawn();
        var element = textInputField.gameObject.AddComponent<LayoutElement>();
        element.minHeight = 25;
        textInputField.transform.SetParent(parent.transform);
      AutomaticPosition(textInputField.GetComponent<RectTransform>());
        TextMeshProUGUI placeholder = textInputField.placeholder.GetComponent<TextMeshProUGUI>();
        placeholder.text = text;
        placeholder.enableAutoSizing = true;
        placeholder.fontSizeMin = 6;
        placeholder.maxVisibleLines = 1;
        textInputField.textComponent.maxVisibleLines = 1;
        textInputField.textViewport.gameObject.AddComponent<VerticalLayoutGroup>();
        return textInputField;
    }

    static void ChangeValueMotorRPM(MotorToWheel motorToWheel, string value)
    {
        try
        {
            motorToWheel.maxWheelRpm = float.Parse(value, ci);
        }
        catch (Exception e) { }
        DrawMotorUpdate();
    }

    static void ChangeValueMotorTorque(MotorToWheel motorToWheel, string value)
    {
        try
        {
            motorToWheel.maxWheelTorque = float.Parse(value, ci);
        }
        catch (Exception e) { }
        DrawMotorUpdate();
    }

    private static void ChangeValueMotorRadius(MotorToWheel motor, string text)
    {
        try
        {
            motor.wheelCollider.radius = float.Parse(text, ci);
        }
        catch (Exception e) { }
        DrawMotorUpdate();
    }

    static void ChangeValueSensorRange(ParamSensor sensor, string value)
    {
        try
        {
            sensor.maxRange = float.Parse(value, ci);
        }
        catch (Exception e) { }
        DrawSensorUpdate();
    }

    static void ChangeValueSensorUpdateDT(ParamSensor sensor, string value)
    {
        try
        {
            sensor.updateDt = float.Parse(value, ci);
        }
        catch (Exception e) { }
        DrawSensorUpdate();
    }
    static void ChangeValueSensorFov(ParamSensor sensor, string value)
    {
        try
        {
            sensor.FOV = float.Parse(value, ci);
        }
        catch (Exception e) { }
        DrawSensorUpdate();
    }
    static void DrawSensorUpdate()
    {
        Draw.Instance.ClearLine();
        foreach (ParamSensor sensor in Simulation.robotSelected.sensors)
        {
            DrawSensor(sensor);
        }
    }
    static void DrawSensor(ParamSensor sensor)
    {
        for (float a = -sensor.FOV; a < sensor.FOV; a += 2f)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0,  a, 0));
            Vector3 direction = rotation * sensor.startLight.forward;
            Draw.Instance.Line(sensor.startLight.position, sensor.startLight.position + direction * sensor.maxRange, Color.green);
        }
    }

    static void DrawMotorUpdate()
    {
        Draw.Instance.ClearCircle();
        foreach (MotorToWheel motor in Simulation.robotSelected.motors)
        {
            Draw.Instance.AddCircle(motor);
        }
    }

}
