using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test
{
    public class RendererSetController : MonoBehaviour
    {
        private Renderer[] _renderers;
        private uint[] _renderersValues;

        private Light[] _light;
        private int[] _lightValues;

        private AudioSource[] _audioSources;
        private bool[] _audioSourcesValues;

        private string _currentSceneName;

        private void OnEnable()
        {
            SetManager.OnSceneChanged += OnSceneChanged;
        }

        private void OnDisable()
        {
            SetManager.OnSceneChanged -= OnSceneChanged;
        }

        private void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _renderersValues = _renderers.Select(o => o.renderingLayerMask).ToArray();

            _light = GetComponentsInChildren<Light>();
            _lightValues = _light.Select(o => o.cullingMask).ToArray();

            _audioSources = GetComponentsInChildren<AudioSource>();
            _audioSourcesValues = _audioSources.Select(o => o.mute).ToArray();

            _currentSceneName = SetManager.CurrentSceneName;
        }

        private void OnSceneChanged(string sceneName)
        {
            var active = _currentSceneName == sceneName;

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].renderingLayerMask = active ? _renderersValues[i] : 0;

            for (int i = 0; i < _light.Length; i++)
                _light[i].cullingMask = active ? _lightValues[i] : 0;

            for (int i = 0; i < _audioSources.Length; i++)
                _audioSources[i].mute = active ? _audioSourcesValues[i] : true;
        }
    }
}