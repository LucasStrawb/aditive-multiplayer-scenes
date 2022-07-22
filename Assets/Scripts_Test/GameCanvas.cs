using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Test
{
    public class GameCanvas : MonoBehaviour
    {
        public Button[] ButtonsLoad;
        public Button[] ButtonsCurrent;

        public Text GamemodeText;

        private void Awake()
        {
            for (int i = 0; i < ButtonsLoad.Length; i++)
            {
                var index = i;
                ButtonsLoad[i].onClick.AddListener(() => SetManager.Instance.ToggleSet(index));
            }

            for (int i = 0; i < ButtonsCurrent.Length; i++)
            {
                var index = i;
                ButtonsCurrent[i].onClick.AddListener(() => SetManager.Instance.SetCurrentScene(index));
            }

            Launcher.OnCurrentGameModeChanged += CurrentGameModeChanged;
        }

        private void CurrentGameModeChanged(GameMode gamemode)
        {
            GamemodeText.text = gamemode.ToString();
        }
    }
}