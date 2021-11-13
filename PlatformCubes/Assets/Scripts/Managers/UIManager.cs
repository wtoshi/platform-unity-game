using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Singleton
    public static UIManager instance;

    [System.Serializable]
    public class Panels
    {
        public Transform PanelHolder;
        public GameObject MenuPanel;
        public GameObject GamePanel;
        public GameObject PassedPanel;
        public GameObject FailedPanel;
    }

    public enum PanelTypes
    {
        Menu, Game, Passed,Failed,Empty
    }

    [Header("UI Panels")]
    public Panels panels = new Panels();

    PanelTypes currentPanel = PanelTypes.Empty;

    // CubeSize buttons
    public GameObject[] ButtonList;
    Animator selectedButtonAnim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        ShowPanel( PanelTypes.Menu);
    }

    public void StartGame()
    {
        GameManager.instance.StartGame(); 
    }

    public void SetGamePanel()
    {
        ShowPanel(PanelTypes.Game);
        SetSelectedButton((int)GameManager.instance.CubeLenght-2);
    }

    public void NextLevel()
    {
        GameManager.instance.NextLevel();
    }

    public void RestartGame()
    {
        GameManager.instance.RestartGame();
    }

    public void ShowPanel(PanelTypes _panel)
    {
        if (currentPanel == _panel)
            return;

        currentPanel = _panel;

        for (int i = 0; i < panels.PanelHolder.childCount; i++)
        {
            panels.PanelHolder.GetChild(i).gameObject.SetActive(false);
        }

        switch (_panel)
        {
            case PanelTypes.Menu:
                panels.MenuPanel.SetActive(true);
                break;
            case PanelTypes.Game:
                panels.GamePanel.SetActive(true);
                break;
            case PanelTypes.Passed:
                panels.PassedPanel.SetActive(true);
                break;
            case PanelTypes.Failed:
                panels.FailedPanel.SetActive(true);
                break;
        }

    }

    public void OnClickSizeButton(Button _button)
    {
        int buttonIndex = _button.transform.GetSiblingIndex();
        GameManager.instance.CubeLenght = buttonIndex + 2;

        SetSelectedButton(buttonIndex);
    }

    void SetSelectedButton(int _buttonIndex)
    {
        if (selectedButtonAnim != null)
        {
            selectedButtonAnim.SetBool("Selected", false);
        }

        selectedButtonAnim = ButtonList[_buttonIndex].GetComponent<Animator>();
        selectedButtonAnim.SetBool("Selected", true);

        /*
        for (int i = 0; i < ButtonList.Length; i++)
        {
            Vector2 oldSize = ButtonList[i].GetComponent<RectTransform>().sizeDelta;
            ButtonList[i].GetComponent<RectTransform>().sizeDelta = new Vector2(90f,oldSize.y);
            ButtonList[i].GetComponent<Image>().color = Color.blue;
        }

        Vector2 size = ButtonList[_buttonIndex].GetComponent<RectTransform>().sizeDelta;
        ButtonList[_buttonIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(110f, size.y);
        ButtonList[_buttonIndex].GetComponent<Image>().color = Color.white;
        */
    }

}
