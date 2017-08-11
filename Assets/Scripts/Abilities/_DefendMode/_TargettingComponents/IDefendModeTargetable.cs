using UnityEngine;
using System;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetable
    {
        string GetId();

        string GetName();

        Vector3 TargetPosition();

        void RegisterForOnDiedCallback(Action callback);
        void DeregisterForOnDiedCallback(Action callback);
    }
}