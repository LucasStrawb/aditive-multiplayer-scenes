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
        private uint[] _renderersLM;

        private Light[] _light;
        private int[] _lightLM;

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
            _renderersLM = _renderers.Select(o => o.renderingLayerMask).ToArray();

            _light = GetComponentsInChildren<Light>();
            _lightLM = _light.Select(o => o.cullingMask).ToArray();

            _currentSceneName = SetManager.CurrentScene.name;
        }

        private void OnSceneChanged(Scene scene)
        {
            var active = _currentSceneName == scene.name;

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].renderingLayerMask = active ? _renderersLM[i] : 0;

            for (int i = 0; i < _light.Length; i++)
                _light[i].cullingMask = active ? _lightLM[i] : 0;
        }
    }
}