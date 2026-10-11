using System.Collections;
using Source.Scripts.General;
using Source.Scripts.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Scripts.EntryPoint
{
    public class BootsTrap : MonoBehaviour
    {
        private void Awake()
        {
            //YandexGamesSdk.CallbackLogging = true;
        }

        private IEnumerator Start()
        {
            string language = "en";
            yield return null;

            LocalizationInitializer localizationInitializer = new();
            localizationInitializer.ApplyLocalization(language);
            SceneManager.LoadScene(Scenes.Menu.ToString());
        }
    }
}