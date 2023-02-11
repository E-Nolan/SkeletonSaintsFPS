using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void shootForward();
    void shoot(Vector3 fireDirection);
}
