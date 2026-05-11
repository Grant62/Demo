using Features.Town.UI;
using JKFrame;
using UnityEngine;

namespace Main.Runtime
{
    public class TownSceneContext : MonoBehaviour
    {
        private void Start()
        {
            UISystem.Show<TownWin>();
        }
    }
}