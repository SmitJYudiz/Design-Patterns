using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommandPattern.RebindKeys
{
    //This class handles all methods that moves the object it's attached to
    public class MoveObject : MonoBehaviour
    {
        //speed of the object
        private const float MOVE_STEP_DISTANCE = 1f;

        //these methods will be executed by their own command
        public void MoveForward()
        {
            Move(Vector3.forward);
        }

        public void MoveBack()
        {
            Move(Vector3.back);
        }

        public void TurnLeft()
        {
            Move(Vector3.left);
        }

        public void TurnRight()
        {
            Move(Vector3.right);
        }



        //Help method to make it more general
        private void Move(Vector3 direction)
        {
            transform.Translate(direction * MOVE_STEP_DISTANCE);
        }
    }

}




