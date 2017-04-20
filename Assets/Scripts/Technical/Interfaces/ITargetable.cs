using UnityEngine;
using System;

public interface ITargetable {

    Vector3 TargetPosition();

    void RegisterForOnDiedCallback(Action callback);
    void DeregisterForOnDiedCallback(Action callback);
}
