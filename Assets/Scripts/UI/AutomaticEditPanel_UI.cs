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

    static InputField inputField;
    static GameObject editPanelContent;

    void Start()
    {
        inputField = GameObject.Find("Canvas/InputFieldExample").GetComponent<InputField>();
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
        foreach (MotorController motor in Simulation.robotSelected.motors)
        {
            GameObject parent = new GameObject();
            parent.transform.SetParent(editPanelContent.transform);
            parent.transform.localPosition = Vector3.zero;
            parent.AddComponent<VerticalLayoutGroup>();
            parent.AddComponent<LayoutElement>();
            parent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(180, 40 + 3 * 30);
            AddText(motor.name, parent);
            GameObject parentButton = new GameObject();
            parentButton.transform.SetParent(parent.transform);
            parentButton.transform.localPosition = Vector3.zero;
            var element = parentButton.AddComponent<LayoutElement>();
            element.minHeight = 20;
            parentButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
            Draw.Instance.AddCircle(motor);
            AddButtonDelete(parentButton).onClick.AddListener(() => ButtonActionDelete(null, motor, parent));
            AddButtonSelect(parentButton).onClick.AddListener(() => ButtonActionSelect(motor.gameObject));
            InputField textInputFieldRPM = AddInputField("Maximum revolutions per minute...", parent);
            textInputFieldRPM.text = motor.maxWheelRpm.ToString();
            textInputFieldRPM.onValueChanged.AddListener(delegate { ChangeValueMotorRPM(motor, textInputFieldRPM.text); });

            InputField textInputFieldTorque = AddInputField("Maximum torque...", parent);
            textInputFieldTorque.text = motor.maxWheelTorque.ToString();
            textInputFieldTorque.onValueChanged.AddListener(delegate { ChangeValueMotorTorque(motor, textInputFieldTorque.text); });

            InputField textInputFieldRadius = AddInputField("Wheel radius[m]...", parent);
            if (motor.wheelCollider != null)
                textInputFieldRadius.text = motor.wheelCollider.radius.ToString();
            else
                Simulation.robotSelected.reload = true;
            textInputFieldRadius.onValueChanged.AddListener(delegate { ChangeValueMotorRadius(motor, textInputFieldRadius.text); });

        }
        if (Simulation.robotSelected.sensors.Length > 0)
            AddText("Sensors", editPanelContent);
        foreach (DistanceSensorController sensor in Simulation.robotSelected.sensors)
        {
            GameObject parent = new GameObject();
            parent.transform.SetParent(editPanelContent.transform);
            parent.transform.localPosition = Vector3.zero;
            parent.AddComponent<VerticalLayoutGroup>();
            parent.AddComponent<LayoutElement>();
            parent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(180, 40 + 3 * 30);
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

            InputField textInputFieldRange = AddInputField("Maximum range[m]...", parent);
            textInputFieldRange.text = sensor.maxRange.ToString();
            textInputFieldRange.onValueChanged.AddListener(delegate { ChangeValueSensorRange(sensor, textInputFieldRange.text); });

            InputField textInputFieldUpdateDt = AddInputField("Frequency...", parent);
            textInputFieldUpdateDt.text = sensor.updateDt.ToString();
            textInputFieldUpdateDt.onValueChanged.AddListener(delegate { ChangeValueSensorUpdateDT(sensor, textInputFieldUpdateDt.text); });

            InputField textInputFieldFOV = AddInputField("View angle...", parent);
            textInputFieldFOV.text = sensor.FOV.ToString();
            textInputFieldFOV.onValueChanged.AddListener(delegate { ChangeValueSensorFov(sensor, textInputFieldFOV.text); });
        }
    }


    static void AutomaticPosition(RectTransform rectTransform)
    {
        RectTransformExtensions.SetSize(rectTransform, new Vector2(180, 30));
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

    static void ButtonActionDelete(DistanceSensorController sensor, MotorController motor, GameObject parent)
    {
        if (sensor != null)
        {
            Destroy(sensor.transform.GetChild(0).gameObject);
            DestroyImmediate(sensor);
            Simulation.robotSelected.UpdateEquipment();
            DrawSensorUpdate();
        }
        else
        {
            motor.GetComponent<MeshCollider>().enabled = true;
            Destroy(motor.wheelCollider);
            DestroyImmediate(motor);
            Simulation.robotSelected.UpdateEquipment();
            DrawMotorUpdate();
        }
        Destroy(parent);
    }

    static Button AddButtonSelect(GameObject parent)
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

    static InputField AddInputField(string text, GameObject parent)
    {
        InputField textInputField = inputField.Spawn();
        var element = textInputField.gameObject.AddComponent<LayoutElement>();
        element.minHeight = 25;
        textInputField.transform.SetParent(parent.transform);
        AutomaticPosition(textInputField.GetComponent<RectTransform>());
        Text placeholder = textInputField.placeholder.GetComponent<Text>();
        placeholder.text = text;
        placeholder.fontSize = 10;
        textInputField.textComponent.fontSize = 10;
        return textInputField;
    }

    static void ChangeValueMotorRPM(MotorController motorToWheel, string value)
    {
        value = value.Replace(".", ",");
        try
        {
            motorToWheel.maxWheelRpm = float.Parse(value);
        }
        catch (Exception e) { }
        DrawMotorUpdate();
    }

    static void ChangeValueMotorTorque(MotorController motorToWheel, string value)
    {
        value = value.Replace(".", ",");
        try
        {
            motorToWheel.maxWheelTorque = float.Parse(value);
        }
        catch (Exception e) { }
        DrawMotorUpdate();
    }

    private static void ChangeValueMotorRadius(MotorController motor, string text)
    {
        text = text.Replace(".", ",");
        try
        {
            motor.wheelCollider.radius = float.Parse(text);
        }
        catch (Exception e) { }
        DrawMotorUpdate();
    }

    static void ChangeValueSensorRange(DistanceSensorController sensor, string value)
    {
        value = value.Replace(".", ",");
        try
        {
            sensor.maxRange = float.Parse(value);
        }
        catch (Exception e) { }
        DrawSensorUpdate();
    }

    static void ChangeValueSensorUpdateDT(DistanceSensorController sensor, string value)
    {
        value = value.Replace(".", ",");
        try
        {
            sensor.updateDt = float.Parse(value);
        }
        catch (Exception e) { }
        DrawSensorUpdate();
    }
    static void ChangeValueSensorFov(DistanceSensorController sensor, string value)
    {
        value = value.Replace(".", ",");
        try
        {
            sensor.FOV = float.Parse(value);
        }
        catch (Exception e) { }
        DrawSensorUpdate();
    }
    static void DrawSensorUpdate()
    {
        Draw.Instance.ClearLine();
        foreach (DistanceSensorController sensor in Simulation.robotSelected.sensors)
        {
            DrawSensor(sensor);
        }
    }
    static void DrawSensor(DistanceSensorController sensor)
    {
        for (float a = -sensor.FOV; a < sensor.FOV; a += 2f)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0, a, 0));
            Vector3 direction = rotation * sensor.startLight.forward;
            Draw.Instance.Line(sensor.startLight.position, sensor.startLight.position + direction * sensor.maxRange, Color.green);
        }
    }

    static void DrawMotorUpdate()
    {
        Draw.Instance.ClearCircle();
        foreach (MotorController motor in Simulation.robotSelected.motors)
        {
            Draw.Instance.AddCircle(motor);
        }
    }
    
    void Update()
    {
        if(Simulation.state == Simulation.State.edit)
        {
            DrawMotorUpdate();
            DrawSensorUpdate();
        }
    }

}
