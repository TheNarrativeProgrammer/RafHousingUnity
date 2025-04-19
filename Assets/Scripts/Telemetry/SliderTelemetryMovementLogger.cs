using UnityEngine;
using UnityEngine.UI;

public class SliderTelemetryMovementLogger : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private string eventTimeDate;
    [SerializeField] private float secondsSinceGameLaunch = 0.0f;


    private void Start()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChange);
        }
    }

    private void OnSliderValueChange(float value)
    {
        string EventName = "UserInput";
        eventTimeDate = System.DateTime.UtcNow.ToString("o");

        secondsSinceGameLaunch = Time.time - SessionTracker.Instance.GetStartTime();

        TelemetryManager.Instance.LogEvent(EventName, eventTimeDate, new System.Collections.Generic.Dictionary<string, object>
        {
            {"Heading", "SliderChange" }, //this is the contents of the Dictionary param
            {"TimeSinceLaunch", secondsSinceGameLaunch},
            {"SliderValue", slider.value },
            {"HousesOnBaord", GameManager.Instance.GetNumberOfHouses()}
        });
    }
}
