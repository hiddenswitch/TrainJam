using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private static NetworkManager m_Instance;

    public static NetworkManager Instance{
        get{
            if (m_Instance == null){
                m_Instance = FindObjectOfType<NetworkManager>();
            }

            return m_Instance;
        }
    }

    private void Start()
    {
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN.");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() was called by PUN.");
        var count = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN.");
        PhotonNetwork.CreateRoom("Main");
    }

    /// <summary>
    /// Calls RPCSendIngredient on all player clients
    /// </summary>
    public void CallSendIngredient(Ingredient ingredient, PlayerType player){
        //Convert Actions into Serializable Strings to be sent as RPC
        Action[] actions = ingredient.GetActions();
        string[] ingredientTypes = new string[actions.Length];
        int[] ingredientAmounts = new int[actions.Length];
        for (int i = 0; i < actions.Length; i++)
        {
            ingredientTypes[i] = actions[i].actionType.ToString();
            ingredientAmounts[i] = actions[i].amount;
        }

        photonView.RPC("RPCSendIngredient", RpcTarget.All, ingredient.ingredientType, ingredientTypes, ingredientAmounts, player);
    }

    [PunRPC]
    public void RPCSendIngredient(IngredientType ingredientType, string[] ingredientActionTypes, int[] ingredientAmounts, PlayerType player)
    {
        Debug.Log($"{ingredientType} TO {player.ToString()}");
        GameManager.Instance.SendIngredientCallback(ingredientType, ingredientActionTypes, ingredientAmounts, player);
    }

    public void CallBling(){
        EffectsManager.Instance.Bling();
        photonView.RPC("RPCBling", RpcTarget.Others);
    }

    [PunRPC]
    public void RPCBling()
    {
        EffectsManager.Instance.Bling();
    }
}
