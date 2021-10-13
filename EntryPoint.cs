using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] Unit unit;

    private void Start()
    {
        unit.ReceiveHealing();
    }
}
