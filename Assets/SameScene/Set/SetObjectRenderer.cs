using Fusion;
using System.Linq;
using UnityEngine;

namespace SameScene
{
    public class SetObjectRenderer : MonoBehaviour
    {
        private Renderer[] _renderers;
        private uint[] _renderersValues;

        private Light[] _light;
        private int[] _lightValues;

        private AudioSource[] _audioSources;
        private bool[] _audioSourcesValues;

        private int _setId;

        private void OnEnable()
        {
            //SetManager.OnSetChanged += SetChanged;
            Player.Local.OnSetChanged += SetChanged;
        }

        private void OnDisable()
        {
            //SetManager.OnSetChanged -= SetChanged;
            Player.Local.OnSetChanged -= SetChanged;
        }

        private void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _renderersValues = _renderers.Select(o => o.renderingLayerMask).ToArray();

            _light = GetComponentsInChildren<Light>();
            _lightValues = _light.Select(o => o.cullingMask).ToArray();

            _audioSources = GetComponentsInChildren<AudioSource>();
            _audioSourcesValues = _audioSources.Select(o => o.mute).ToArray();

            SetChanged(Player.Local.FocusSet);
        }

        public void SetSetId(int setId)
        {
            _setId = setId;
        }

        private void SetChanged(int setId)
        {
            var active = _setId == setId;

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].renderingLayerMask = active ? _renderersValues[i] : 0;

            for (int i = 0; i < _light.Length; i++)
                _light[i].cullingMask = active ? _lightValues[i] : 0;

            for (int i = 0; i < _audioSources.Length; i++)
                _audioSources[i].mute = active ? _audioSourcesValues[i] : true;
        }
    }
}