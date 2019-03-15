using Meteor;

namespace TrainJam.Multiplayer
{
    public sealed class MatchDocument : MongoDocument
    {
        public string[] players;
        public string stage;
        public int round;
        public int playerCount;
    }
}