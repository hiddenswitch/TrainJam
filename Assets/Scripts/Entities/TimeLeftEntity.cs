using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TrainJam.Multiplayer.Entities
{
    /// <summary>
    /// Represents the game timer.
    ///
    /// The entity's first value is the duration in seconds.
    /// </summary>
    public class TimeLeftEntity : TimedEntity
    {
        [SerializeField] private Text m_TimerText;
        [SerializeField] private string m_TimeFormat = "mm\\:ss";

        protected override void OnTick(TimeSpan remaining)
        {
            m_TimerText.text = remaining.ToString(m_TimeFormat);
        }
    }
}