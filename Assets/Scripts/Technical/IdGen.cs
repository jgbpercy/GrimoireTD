using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IdGen {

    private static int nextId = 0;

    public static int GetNextId()
    {
        nextId++;
        return nextId;
    }
}
