using Features.Dungeon.Application;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.Dungeon.Infrastructure
{
    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                ResetStage();
        }

        private void ResetStage()
        {
            DungeonService.ResetGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
