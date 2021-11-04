using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    /// <summary>
    /// Whether or not an ObjectPool should consider this object ready for use;
    /// </summary>
    bool ReadyForPoolToUse { get; set; }
}
