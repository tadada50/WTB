using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] Slider leftPlayerHomeSlider;
    [SerializeField] Slider rightPlayerHomeSlider;

    [SerializeField] RawImage[] leftPlayerLifes;
    [SerializeField] RawImage[] rightPlayerLifes;
    [SerializeField] public float destructionThreshold = 0.75f;
    int leftPlayerLifesCount = 3;
    int rightPlayerLifesCount = 3;  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int LeftPlayerLifesCount
    {
        get {return leftPlayerLifesCount;}
        set {
            if (leftPlayerLifesCount == value) return;
            leftPlayerLifesCount = value;
            UpdateLeftPlayerLifes();
            if (OnLeftPlayerLifesChange != null)
                OnLeftPlayerLifesChange(leftPlayerLifesCount);
        }
    }   
    void UpdateLeftPlayerLifes()
    {
        for (int i = 0; i < leftPlayerLifes.Length; i++)
        {
            if (i < leftPlayerLifesCount)
            {
                leftPlayerLifes[i].enabled = true;
            }
            else
            {
                leftPlayerLifes[i].enabled = false;
            }
        }
    }
    public delegate void OnLeftPlayerLifesChangeDelegate(int newVal);
    public event OnLeftPlayerLifesChangeDelegate OnLeftPlayerLifesChange;

    public int RightPlayerLifesCount
    {
        get {return rightPlayerLifesCount;}
        set {
            if (rightPlayerLifesCount == value) return;
            rightPlayerLifesCount = value;
            UpdateRightPlayerLifes();
            if (OnRightPlayerLifesChange != null)
                OnRightPlayerLifesChange(rightPlayerLifesCount);
        }
    }
    void UpdateRightPlayerLifes()
    {
        for (int i = 0; i < rightPlayerLifes.Length; i++)
        {
            if (i < rightPlayerLifesCount)
            {
                rightPlayerLifes[i].enabled = true;
            }
            else
            {
                rightPlayerLifes[i].enabled = false;
            }
        }
    }
    public delegate void OnRightPlayerLifesChangeDelegate(int newVal);
    public event OnRightPlayerLifesChangeDelegate OnRightPlayerLifesChange;

    public float LeftPlayerHomeSliderValue
    {
        get {return leftPlayerHomeSlider.value;}
        set {
            if (leftPlayerHomeSlider.value == value) return;
            leftPlayerHomeSlider.value = value;
            if (OnLeftPlayerHomeSliderValueChange != null)
                OnLeftPlayerHomeSliderValueChange(leftPlayerHomeSlider.value);
            Debug.Log($"Left player home slider value: {leftPlayerHomeSlider.value}");
        }
    }
    public delegate void OnLeftPlayerHomeSliderValueChangeDelegate(float newVal);
    public event OnLeftPlayerHomeSliderValueChangeDelegate OnLeftPlayerHomeSliderValueChange;

    public float RightPlayerHomeSliderValue
    {
        get {return rightPlayerHomeSlider.value;}
        set {
            if (rightPlayerHomeSlider.value == value) return;
            rightPlayerHomeSlider.value = value;
            if (OnRightPlayerHomeSliderValueChange != null)
                OnRightPlayerHomeSliderValueChange(rightPlayerHomeSlider.value);
        }
    }
    public delegate void OnRightPlayerHomeSliderValueChangeDelegate(float newVal);
    public event OnRightPlayerHomeSliderValueChangeDelegate OnRightPlayerHomeSliderValueChange;


    void Start()
    {
        leftPlayerHomeSlider.minValue = destructionThreshold;
        rightPlayerHomeSlider.minValue = destructionThreshold;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
