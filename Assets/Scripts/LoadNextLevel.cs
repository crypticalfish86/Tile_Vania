using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{

    [SerializeField] int levelToLoad = 1;
    private string levelToLoadStringReference;


    void Start()
    {
        levelToLoadStringReference = $"Level_{levelToLoad}";
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            StartCoroutine(LoadLevel());
        }
    }
        private IEnumerator LoadLevel(){
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(levelToLoadStringReference);
        }
}
