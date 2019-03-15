using Meteor;

namespace TrainJam.Multiplayer
{
    public sealed class LobbyDocument : MongoDocument
    {
        public ConnectionReadyDocument[] connections;
        public int playerCount;
        public int readyCount;
        public string message;
    }
}