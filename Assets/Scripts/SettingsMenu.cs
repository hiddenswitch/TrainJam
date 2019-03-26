using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsMenuObject;
    public TextMeshProUGUI connectedText;
    public Button reconnectButton;

    public void Open()
    {
        settingsMenuObject.SetActive(true);
    }

    public void Close()
    {
        settingsMenuObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Open();
        }

        connectedText.text = PhotonNetwork.InRoom ? "Connected" : "Not Connected";
        reconnectButton.interactable = !PhotonNetwork.InRoom;
    }
}
