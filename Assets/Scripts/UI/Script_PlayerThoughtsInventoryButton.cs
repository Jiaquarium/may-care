using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Script_PlayerThoughtsInventoryButton : MonoBehaviour
{
    public Text text;
    public RectTransform scrollContainer;
    public RectTransform maskContainer;
    public Image image;
    private VerticalLayoutGroup verticalLayoutGroup;
    private float startingY;
    public bool isSelected = false;
    private RectTransform thisButton;
    
    void Start()
    {
        thisButton = GetComponent<RectTransform>();
        startingY = scrollContainer.anchoredPosition.y;
        verticalLayoutGroup = scrollContainer.GetComponent<VerticalLayoutGroup>();
        image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            image.enabled = true;
            if (!GetBottomYAdjustment())    GetTopYAdjustment();
            isSelected = true;
        }
        else
        {
            image.enabled = false;
            isSelected = false;
        }
    }

    public bool GetBottomYAdjustment()
    {
        float bottomOfButton = thisButton.anchoredPosition.y
            - (thisButton.rect.height / 2)
            - verticalLayoutGroup.padding.top;

        float anchorTopMargin = scrollContainer.anchoredPosition.y;

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
}
