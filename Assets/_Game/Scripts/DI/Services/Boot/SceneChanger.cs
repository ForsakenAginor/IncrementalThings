using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Source.Scripts.DI.Services.Boot
{
    public interface ISceneChanger
    {
        public void FadeOut();

        public void LoadScene(string sceneName);

        public void LoadSceneIgnoreTimeScale(string sceneName);
    }

    public class SceneChanger : MonoBehaviour, ISceneChanger
    {
        [SerializeField] private Image _blackScreenImage;
        [SerializeField] private float _animationDuration;

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                throw new ArgumentNullException(nameof(sceneName));

            _blackScreenImage.DOFade(1f, _animationDuration).OnComplete(() => StartCoroutine(LoadAsyncScene(sceneName)));
        }

        public void FadeOut()
        {
            _blackScreenImage.DOFade(0f, _animationDuration);
        }

        public void LoadSceneIgnoreTimeScale(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                throw new ArgumentNullException(nameof(sceneName));

            _blackScreenImage.DOFade(1f, _animationDuration).SetUpdate(true).OnComplete(() => StartCoroutine(LoadAsyncScene(sceneName)));
        }

        private IEnumerator LoadAsyncScene(string scene)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

            while (asyncLoad.isDone == false)
                yield return null;
        }
    }
}
