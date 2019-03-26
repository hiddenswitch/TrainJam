using UnityEngine;
using UnityEngine.UI;

public class Grill : Tool
{
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    private void Start()
    {
        OnButton1();
    }

    public void OnButton1(){
        this.action.amount = 1;
        button1.interactable = false;
        button2.interactable = true;
        button3.interactable = true;
    }

    public void OnButton2()
    {
        this.action.amount = 2;

        button1.interactable = true;
        button2.interactable = false;
        button3.interactable = true;
    }

    public void OnButton3()
    {
        this.action.amount = 3;

        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = false;
    }
}
