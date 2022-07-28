using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SameScene
{
    public class GameCanvas : SimulationBehaviour
    {
        public Toggle[] Toggles;

        public Text GamemodeText;

        private void Awake()
        {
            for (int i = 0; i < Toggles.Length; i++)
            {
                var index = i;
                Toggles[i].onValueChanged.AddListener((value) => ToggleValueChanged(value, index));
            }

            Launcher.OnCurrentGameModeChanged += CurrentGameModeChanged;
        }

        private void ToggleValueChanged(bool value, int setId)
        {
            BasicSpawner.CurrentInput.SetId = setId;
        }

        private void CurrentGameModeChanged(GameMode gamemode)
        {
            GamemodeText.text = gamemode.ToString();
        }
    }
}