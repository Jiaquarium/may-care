using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Script_SnapShowButton : MonoBehaviour
{
    [SerializeField] private RectTransform scrollContainer;
    [SerializeField] private RectTransform maskContainer;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private float startingY;
    [SerializeField] private RectTransform thisButton;

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            if (!GetBottomYAdjustment())    GetTopYAdjustment();
        }
    }

    /// <summary>
    /// When scrolling down, snaps so bottom button is always completely visible
    /// </summary>
    public bool GetBottomYAdjustment()
    {
        float bottomOfButton = thisButton.anchoredPosition.y
            - (thisButton.rect.height / 2)
            - verticalLayoutGroup.padding.top;

        float anchorTopMargin = scrollContainer.anchoredPosition.y;

        // distance from bottom of scroll container to maskContainer
        float distanceFromContainerTop = Mathf.Abs(bottomOfButton - (startingY - anchorTopMargin));
        
        if (distanceFromContainerTop > maskContainer.rect.height)
        {
            float adjustmentY = distanceFromContainerTop - maskContainer.rect.height;

            scrollContainer.anchoredPosition = new Vector2(
                scrollContainer.anchoredPosition.x,
                scrollContainer.anchoredPosition.y + adjustmentY
            );

            return true;
        }

        return false;
    }

    public void GetTopYAdjustment()
    {
        float topOfButton = thisButton.anchoredPosition.y
            + (thisButton.rect.height / 2)
            + verticalLayoutGroup.padding.top;

        float anchorTopMargin = scrollContainer.anchoredPosition.y;

        float distanceFromContainerTop = Mathf.Abs(startingY - anchorTopMargin);

        if (topOfButton + distanceFromContainerTop > 0)
        {
            float adjustmentY = distanceFromContainerTop + topOfButton;

            scrollContainer.anchoredPosition = new Vector2(
                scrollContainer.anchoredPosition.x,
                scrollContainer.anchoredPosition.y - adjustmentY
            );
        }
    }

    public void Setup(
        RectTransform _scrollContainer,
        RectTransform _maskContainer     
    )
    {
        scrollContainer = _scrollContainer;
        maskContainer = _maskContainer;
        thisButton = GetComponent<RectTransform>();
        startingY = scrollContainer.anchoredPosition.y;
        verticalLayoutGroup = scrollContainer.GetComponent<VerticalLayoutGroup>();
    }
}
