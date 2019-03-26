using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Camera camera;
    [SerializeField] private Color[] backgroundColors;
    [SerializeField] private GameObject blueScreen;
	[SerializeField] private GameObject orangeScreen;
    [SerializeField] private GameObject greenScreen;
    [SerializeField] private Ticket ticket;

    private int playerId;
    private PlayerType playerType;

    private void UpdateCameraBackgroundColor(){
        camera.backgroundColor = GetColorForPlayerType(this.playerType);
    }

    private void UpdateScreen(){
        switch(playerType){
            case PlayerType.Blue:
                blueScreen.SetActive(true);
                greenScreen.SetActive(false);
                orangeScreen.SetActive(false);
                break;
            case PlayerType.Orange:
                blueScreen.SetActive(false);
                greenScreen.SetActive(false);
                orangeScreen.SetActive(true);
                break;
            case PlayerType.Green:
                blueScreen.SetActive(false);
                greenScreen.SetActive(true);
                orangeScreen.SetActive(false);
                break;
        }
    }

    private PlayerType GetPlayerTypeForId(int id)
    {
        switch(id){
            case 1:
                return PlayerType.Blue;
            case 2:
                return PlayerType.Orange;
            case 3:
                return PlayerType.Green;
        }

        return PlayerType.Blue;
    }

    private Color GetColorForPlayerType(PlayerType playerType){
        if (playerType == PlayerType.Blue){
            return backgroundColors[0];
        }

        if (playerType == PlayerType.Orange)
        {
            return backgroundColors[1];
        }

        if (playerType == PlayerType.Green)
        {
            return backgroundColors[2];
        }

        return backgroundColors[3];
    }

    public void SendIngredientCallback(IngredientType ingredientType, string[] ingredientActionTypes, int[] ingredientActionAmounts, PlayerType player)
    {
        List<Action> actionList = new List<Action>();

        //Convert back sent strings to Actions 
        for (int i = 0; i < ingredientActionTypes.Length; i++){
            Action action = new Action();
            action.actionType = (ActionType)Enum.Parse(typeof(ActionType),ingredientActionTypes[i]);
            action.amount = ingredientActionAmounts[i];
            actionList.Add(action);
        }

        if (this.playerType == player){
            SpawnManager.Instance.SpawnIngredient(ingredientType, actionList.ToArray());
        }
    }

    public void ForcePlayerType(PlayerType playerType){
        this.playerType = playerType;
        UpdateCameraBackgroundColor();
        UpdateScreen();
    }

    public void ForceOrange(){
        ForcePlayerType(PlayerType.Orange);
    }

    public void ForceBlue()
    {
        ForcePlayerType(PlayerType.Blue);
    }

    public void ForceGreen()
    {
        ForcePlayerType(PlayerType.Green);
    }

}
