using Meteor;

namespace TrainJam.Multiplayer
{
    public sealed class EntityDocument : MongoDocument
    {
        public string prefab;
        public int[] playerIds = new int[0];
        public float[] values = new float[0];
        public string[] texts = new string[0];
        public bool[] bools = new bool[0];
        public int playerId => playerIds.Length == 0 ? 0 : playerIds[0];
        public float value => values.Length == 0 ? 0 : values[0];
        public string text => texts.Length == 0 ? null : texts[0];
        public bool boolValue => bools.Length != 0 && bools[0];
    }
}