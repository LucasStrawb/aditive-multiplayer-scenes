using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Test
{
    public class Menu : MonoBehaviour
    {
        public static string SessionName = "Test";
        public static bool AllowClientSideManagement = true;

        [SerializeField] Button _loginButton;

        private void Awake()
        {
            _loginButton.onClick.AddListener(LoadScene);
        }

        public void LoadScene()
        {
            SceneManager.LoadScene("Game");
        }
    }
}