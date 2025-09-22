using Ironcow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrisonState : CharacterState
{
    public override void OnStateEnter()
    {
        character.prizon.SetActive(true);
        rigidbody.linearVelocity = Vector3.zero;
    }

    public override void OnStateExit()
    {
        character.prizon.SetActive(false);
    }

    public override void OnStateUpdate()
    {
        
    }
}
