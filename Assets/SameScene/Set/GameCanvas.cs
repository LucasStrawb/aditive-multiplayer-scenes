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
        [Header("Game Status")]
        [SerializeField] private Text _gamemodeText;

        [Header("Set Management")]
        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private Button _newButton;
        [SerializeField] private Transform _parent;

        private readonly List<Button> _buttons = new();

        private void OnEnable()
        {
            Launcher.OnCurrentGameModeChanged += CurrentGameModeChanged;
        }

        private void OnDisable()
        {
            Launcher.OnCurrentGameModeChanged -= CurrentGameModeChanged;
        }

        private void Awake()
        {
            _newButton.onClick.AddListener(AddSet);
        }

        private void CurrentGameModeChanged(GameMode gamemode)
        {
            _gamemodeText.text = gamemode.ToString();
        }

        private void AddSet()
        {
            var setId = _buttons.Count;
            var button = Instantiate(_buttonPrefab, _parent);
            _buttons.Add(button);
            button.onClick.AddListener(() => FocusSet(setId));
            button.GetComponentInChildren<Text>().text = "" + setId;

            BasicSpawner.CurrentInput.CreateSetId = setId;

            _newButton.transform.SetAsLastSibling();
        }

        private void FocusSet(int setId)
        {
            BasicSpawner.CurrentInput.FocusSetId = setId;
        }

        [ContextMenu("Focus Set 0")]
        public void FocusSet0()
        {
            FocusSet(0);
        }

        [ContextMenu("Focus Set 1")]
        public void FocusSet1()
        {
            FocusSet(1);
        }
    }
}