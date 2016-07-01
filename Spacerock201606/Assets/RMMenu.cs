using UnityEngine;
using System.Collections;

public class RMMenu : MonoBehaviour {

	RMMenuItem[] items;
	int highlightedItem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetMenuItems(RMMenuItem[] newItems)
	{
        int currentIndex = 0;
        foreach (RMMenuItem item in newItems)
        {
            item.index = currentIndex;
            currentIndex++;
        }
		items = newItems;
		highlightedItem = 0;
		items[highlightedItem].SetHighlighted(true);
	}

	public void HighlightNextItem(bool down)
	{
        items[highlightedItem].SetHighlighted(false);

        int indexOfNextItem = (highlightedItem + (down ? +1 : -1)) % items.Length;
        if (indexOfNextItem < 0)
        {
            indexOfNextItem = indexOfNextItem + items.Length;
        }

        highlightedItem = indexOfNextItem;
		items[highlightedItem].SetHighlighted(true);
	}

    public void SelectHighlightedItem()
    {
        items[highlightedItem].SetSelected();
    }
}
