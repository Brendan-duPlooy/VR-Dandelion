using UnityEngine;
using UnityEngine.InputSystem;

public class DandelionKeyboardInput : MonoBehaviour
{
    [SerializeField]
    private DandelionBloom bloom;

    void Reset()
    {
        bloom = GetComponent<DandelionBloom>();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            bloom.BloomFlower();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            bloom.BlowFlower();
        }

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            bloom.ResetFlower();
        }
    }
}