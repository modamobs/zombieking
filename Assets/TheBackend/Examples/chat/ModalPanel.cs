using UnityEngine;
using UnityEngine.UI;

//  This script will be updated in Part 2 of this 2 part series.
public class ModalPanel : MonoBehaviour
{
    public Text alert;
    public Button button;
    public GameObject alertModalPanelObject;

    private static ModalPanel modalPanel;
    public ContentSizeFitter _ContentSizeFitter;

    public static ModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogWarning("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }

    public void AlertShow(string text)
    {
        alert.text = text;

        //_ContentSizeFitter.enabled = true;
        //_ContentSizeFitter.SetLayoutVertical();

        alertModalPanelObject.SetActive(true);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(CloseAlertPanel);
        button.gameObject.SetActive(true);
    }

    internal void CloseAlertPanel()
    {
        alertModalPanelObject.SetActive(false);
    }
}