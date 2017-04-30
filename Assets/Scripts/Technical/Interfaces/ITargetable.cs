using UnityEngine;
using System;

public interface ITargetable {

    string GetId();

    string GetName();

    Vector3 TargetPosition();

    void RegisterForOnDiedCallback(Action callback);
    void DeregisterForOnDiedCallback(Action callback);
}
