// TPPlatformUtils.cs
using UnityEngine;
using TraversalPro;

public static class TPPlatformUtils
{
    public static bool TryGetMotor(Collider other, out CharacterMotor motor)
    {
        motor = other.GetComponentInParent<CharacterMotor>();
        return motor != null;
    }
}
