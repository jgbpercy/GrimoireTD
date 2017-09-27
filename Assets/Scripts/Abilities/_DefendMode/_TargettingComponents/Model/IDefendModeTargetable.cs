using UnityEngine;
using System;

namespace GrimoireTD.Abilities.DefendMode
{
    public interface IDefendModeTargetable
    {
        string Id { get; }

        string NameInGame { get; }

        Vector3 TargetPosition();

        event EventHandler OnDied;
    }
}