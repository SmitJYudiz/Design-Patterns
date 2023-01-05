using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern.RebindKeys
{
    //the point of this command is to do nothing

    // Is used instead of setting a command to null, so it is called Null object, which is another programming pattern
    public class DoNothingCommand:Command
    {
        public override void Execute()
        {
            
        }

        public override void Undo()
        {
            
        }
    }
}
