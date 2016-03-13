using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class menuButton : MonoBehaviour
{
    public Image targetGraphic;
    public Sprite newGraphic;
    public int ID;
    void OnMouseEnter()
    {
        GetComponent<Text>().color = Color.grey;
        targetGraphic.sprite = newGraphic;
    }

    void OnMouseDown()
    {
        switch (ID)
        {
            case 0:
                Application.LoadLevel(2);
                break;
            case 1:
                Application.LoadLevel(1);
                break;
            case 2:
                Application.LoadLevel(3);
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void OnMouseExit()
    {
        GetComponent<Text>().color = Color.white;
    }
}
