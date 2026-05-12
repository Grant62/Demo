using Features.Action.UI;
using Features.Battle.Domain;
using JKFrame;
using UnityEngine;

namespace Features.Battle.Runtime
{
    public class CombatSceneContext : MonoBehaviour
    {
        private void Awake()
        {
            EnergySystem.Initialize();
            MpSystem.Initialize();
        }

        private void Start()
        {
            UISystem.Show<ActionWin>();
        }
    }
}