using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GamePage : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Button leftArrowBtn;
    [SerializeField] private Button rightArrowBtn;

    [Header("Scroll View")]
    [SerializeField] private float offset = 850f;
    [SerializeField] private RectTransform scrollView;

    [Header("Transition")]
    [SerializeField] private float speed = 0.25f;

    private void Start()
    {
        leftArrowBtn.onClick.AddListener(() => 
        {
            scrollView.DOMoveX(scrollView.localPosition.x - offset, speed);
        });
        
        rightArrowBtn.onClick.AddListener(() => 
        { 
            scrollView.DOMoveX(scrollView.localPosition.x + offset, speed); 
        });
    }
}
