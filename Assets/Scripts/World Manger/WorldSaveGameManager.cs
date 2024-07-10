using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EB
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;
        [SerializeField] int worldSceneindex = 1;

        private void Awake()
        {
            // There can only be one instance of this script at one time, if another exists, destroy it
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator LoadNewGame()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneindex);

            yield return null;
        }

        public int GetWorldSceneIndex()
        {
            return worldSceneindex;
        }
    }
}

