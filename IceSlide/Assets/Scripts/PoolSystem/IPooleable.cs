using UnityEngine;
public interface IPooleable
{
    public GameObject GameObject { get; }
    Pool Pool { get; set; }
}
