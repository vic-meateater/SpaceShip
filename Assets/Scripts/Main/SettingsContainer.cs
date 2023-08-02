using Data;
using UnityEngine;

namespace Main
{
    public class SettingsContainer : Singleton<SettingsContainer>
    {
        public SpaceShipSettings SpaceShipSettings => spaceShipSettings;

        [SerializeField] private SpaceShipSettings spaceShipSettings;
    }
}
