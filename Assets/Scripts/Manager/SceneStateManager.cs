using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager instance;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �V�[���J��
    /// </summary>
    /// <param name="nextSceneName"></param>
    /// <param name="duration"></param>
    public void PrepareNextScene(SceneName nextSceneName, float duration = 1.0f) {
        StartCoroutine(NextScene(nextSceneName, duration));

        /// <summary>
        /// �V�[���J�ڂ̎�����
        /// </summary>
        /// <returns></returns>
        IEnumerator NextScene(SceneName nextSceneName, float duration) {

            yield return new WaitForSeconds(duration);

            SceneManager.LoadScene(nextSceneName.ToString());
        }
    }
}
