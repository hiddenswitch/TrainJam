using System;
using UnityEngine;

namespace TrainJam
{
    [Serializable]
    public struct StagePlayersPlayerIdTuple : IEquatable<StagePlayersPlayerIdTuple>,
        IComparable<StagePlayersPlayerIdTuple>
    {
        [SerializeField] private string m_ActiveForStage;
        [SerializeField] private int m_ActiveForPlayerId;
        [SerializeField] private int m_NumberOfPlayers;

        public string ActiveForStage
        {
            get => m_ActiveForStage;
            set => m_ActiveForStage = value;
        }

        public int ActiveForPlayerId
        {
            get => m_ActiveForPlayerId;
            set => m_ActiveForPlayerId = value;
        }

        public int NumberOfPlayers
        {
            get => m_NumberOfPlayers;
            set => m_NumberOfPlayers = value;
        }

        public bool Equals(StagePlayersPlayerIdTuple other)
        {
            return string.Equals(m_ActiveForStage, other.m_ActiveForStage) &&
                   m_ActiveForPlayerId == other.m_ActiveForPlayerId && m_NumberOfPlayers == other.m_NumberOfPlayers;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is StagePlayersPlayerIdTuple other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (m_ActiveForStage != null ? m_ActiveForStage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ m_ActiveForPlayerId;
                hashCode = (hashCode * 397) ^ m_NumberOfPlayers;
                return hashCode;
            }
        }

        public static bool operator ==(StagePlayersPlayerIdTuple left, StagePlayersPlayerIdTuple right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StagePlayersPlayerIdTuple left, StagePlayersPlayerIdTuple right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(StagePlayersPlayerIdTuple other)
        {
            var activeForStageComparison =
                string.Compare(m_ActiveForStage, other.m_ActiveForStage, StringComparison.Ordinal);
            if (activeForStageComparison != 0) return activeForStageComparison;
            var activeForPlayerIdComparison = m_ActiveForPlayerId.CompareTo(other.m_ActiveForPlayerId);
            if (activeForPlayerIdComparison != 0) return activeForPlayerIdComparison;
            return m_NumberOfPlayers.CompareTo(other.m_NumberOfPlayers);
        }
    }
}