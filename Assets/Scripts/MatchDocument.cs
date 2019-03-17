using System;
using Meteor;

namespace TrainJam.Multiplayer
{
    public sealed class MatchDocument : MongoDocument, IEquatable<MatchDocument>, IComparable<MatchDocument>
    {
        public string[] players;
        public string stage;
        public int round;
        public int playerCount;

        public bool Equals(MatchDocument other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(players, other.players) && string.Equals(stage, other.stage) && round == other.round && playerCount == other.playerCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is MatchDocument other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (players != null ? players.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (stage != null ? stage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ round;
                hashCode = (hashCode * 397) ^ playerCount;
                return hashCode;
            }
        }

        public static bool operator ==(MatchDocument left, MatchDocument right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MatchDocument left, MatchDocument right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(MatchDocument other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var stageComparison = string.Compare(stage, other.stage, StringComparison.Ordinal);
            if (stageComparison != 0) return stageComparison;
            var roundComparison = round.CompareTo(other.round);
            if (roundComparison != 0) return roundComparison;
            return playerCount.CompareTo(other.playerCount);
        }
    }
}