using TrainJam.Multiplayer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    public class PortalToPlayerId : UIBehaviour
    {
        [SerializeField] private int m_ToPlayerId;
//        [SerializeField] priv

        public int toPlayerId => m_ToPlayerId;

        private void Awake()
        {
            
//            GameController.instance.Instantiate(toPlayerId, prefabToInstantiate);
        }
    }

}