using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemData currentItem;
    public ItemData currentItemCount;
    public bool isSelected = false;

    public Sprite normalSprite;
    public Sprite selectedSprite;

    private Image image;
    private void Start()
    {
        Image image = GetComponent<Image>();
    }
    private void Update()
    {
        if (isSelected)
        {
            image.sprite = selectedSprite;

        }
        else
        {
            image.sprite = normalSprite;
        }
    }
}
