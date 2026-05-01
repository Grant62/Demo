using Features.Town.UI;
using JKFrame;
using UnityEngine;

namespace Main
{
    public class GameFlowController : MonoBehaviour
    {
        private bool _loaded;

        private void Start()
        {
            SceneSystem.LoadSceneAsync("Town", OnTownLoadProgress);
        }

        private void OnTownLoadProgress(float progress)
        {
            if (progress >= 1 && !_loaded)
            {
                _loaded = true;
                ResSystem.InstantiateGameObject("TownSceneObj");
                UISystem.Show<TownButtonsWin>();
            }
        }
    }
}