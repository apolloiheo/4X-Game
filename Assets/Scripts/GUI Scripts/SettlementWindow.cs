using TMPro;
using UnityEngine;

public class SettlementWindow : MonoBehaviour
{
    public GameObject settlementWindow;
    public RectTransform unitsContent;
    public RectTransform buildingsContent;
    public Director director;
    public TMP_Text settlementName;

    public void Start()
    {
        director = FindObjectOfType<Director>();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestoryWindow();
        }
    }
    
    private void DestoryWindow()
    {
        settlementWindow.transform.SetParent(null);
        director._zoomedIn = false;
        director.RestoreCameraState();
        Destroy(settlementWindow);
    }
}
