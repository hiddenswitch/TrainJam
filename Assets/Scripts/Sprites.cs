using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrainJam
{
    public static class Sprites
    {
        private static Dictionary<string, Sprite> m_Sprites = new Dictionary<string, Sprite>();

        public static IReadOnlyDictionary<string, Sprite> sprites => m_Sprites;

        [RuntimeInitializeOnLoadMethod]
        static void LoadSprites()
        {
            m_Sprites = Resources.FindObjectsOfTypeAll<Sprite>().ToDictionary(sp => sp.name, sp => sp);
        }
    }
}