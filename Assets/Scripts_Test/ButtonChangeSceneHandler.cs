using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class ButtonChangeSceneHandler : SimulationBehaviour
    {
        public Button[] Buttons;
        public Button ReloadButton;

        public void Awake()
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                var index = i;
                var button = Buttons[i];
                button.onClick.AddListener(() => ToggleScene(index + 1));
            }

            ReloadButton.onClick.AddListener(ReloadScenesButton);
        }

        public void ToggleScene(int sceneIndex)
        {
            //SetManager.Instance.ToggleScene(sceneIndex);
        }

        public void ReloadScenesButton()
        {
            //SetManager.Instance.ReloadScenesButton();
        }

        private void FixedUpdate()
        {
            if (!Runner.SceneManager())
                return;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].interactable = Runner.IsRunning;
                if (Runner.SceneManager().LoadedScenes.Contains(i + 1))
                    Buttons[i].image.color = Color.red;
                else
                    Buttons[i].image.color = Color.white;
            }
        }
    }
}