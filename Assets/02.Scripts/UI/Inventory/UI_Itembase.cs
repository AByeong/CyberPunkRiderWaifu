using UnityEngine;
using UnityEngine.UI;
public class UI_Itembase : MonoBehaviour
{
    public Image Icon;

    public void Start()
    {
        Icon = GetComponent<Image>();
    }

    public void Show(Item item)
    {
        gameObject.SetActive(true);
        Icon.sprite = item.Icon;
    }
}
