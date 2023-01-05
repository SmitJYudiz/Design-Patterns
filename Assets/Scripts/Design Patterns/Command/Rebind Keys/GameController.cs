using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CommandPattern.RebindKeys
{
    //command pattern: rebind keys example from the book "Game Prograqmming Patterns"
    //is also including undo, redo, and replay system
    public class GameController: MonoBehaviour
    {

        public MoveObject objectThatMoves;

        //the keys we have that are also connect with commands
        private Command buttonW;
        private Command buttonA;
        private Command buttonS;
        private Command buttonD;

        //Store the commands here to make undo, redo, replay easier
        //The book is using one List and an Index

        //private List<Command> oldCommands = new List<Command>();
        //start at -1 because in the beginning we haven't added any commands
        //private int currentCommnandIndex = -1;

        //But I think it is easier to use two stacks
        //when I replay, we convert the undo stack to an array

        private Stack<Command> undoCommands = new Stack<Command>();
        private Stack<Command> redoCommands = new Stack<Command>();

        private bool isReplaying = false;

        //To make replay work, we need to know where the object started
        private Vector3 startPos;

        //for reversed play:
        private Vector3 reversedPlayStartPos;

        //the time between each command execution when we replay so we can see what's going on
        private const float REPLAY_PAUSE_TIMER = 0.5f;

        void Start()
        {
            //Bind the keys to default commands
            buttonW = new MoveForwardCommand(objectThatMoves);

            buttonA = new TurnLeftCommand(objectThatMoves);

            buttonS = new MoveBackCommand(objectThatMoves);

            buttonD = new TurnRightCommand(objectThatMoves);

            startPos = objectThatMoves.transform.position;
        }

        private void Update()
        {
            //we can check for input while we are replying: i think he means to say: check for inputs only if we not replaying
            //and if replaying then return:

            if(isReplaying)
            {
                return;
            }

            //we will here jump in steps to make the undo System easier

            //if we were moving with speed*Time.deltaTime, the undo System would be more complicated to implement.

            // When we Undo, the Time.deltaTime may be different, so we end up at another position than we previously had

            //you could solve this by saving Time.deltaTime somewhere

            if(Input.GetKeyDown(KeyCode.W))
            {
                ExecuteNewCommand(buttonW);
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                ExecuteNewCommand(buttonA);
            }
            else if(Input.GetKeyDown(KeyCode.S))
            {
                ExecuteNewCommand(buttonS);
            }else if(Input.GetKeyDown(KeyCode.D))
            {
                ExecuteNewCommand(buttonD);
            }

            //undo with u
            else if(Input.GetKeyDown(KeyCode.U))
            {
                if(undoCommands.Count == 0)
                {
                    Debug.Log("Can't undo because we are back where we started");
                }
                else
                {
                    Command lastCommand = undoCommands.Pop();
                    lastCommand.Undo();

                    //Add this to Redo: if we want to Redo the undo
                    redoCommands.Push(lastCommand);
                }
            }

            //Redo with r
            else if(Input.GetKeyDown(KeyCode.R))
            {
                if(redoCommands.Count == 0)
                {
                    Debug.Log("Can't redo because we are at the end");
                }
                else
                {
                    Command nextCommand = redoCommands.Pop();
                    nextCommand.Execute();

                    //Add to undo if we want to undo the redo
                    undoCommands.Push(nextCommand);
                }
            }

            //rebind keys by just swapping A and D buttons
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SwapKeys(ref buttonA, ref buttonD);
            }

            //Start replay
            if(Input.GetKeyDown(KeyCode.Return))
            {
                if(isReplaying)
                {
                    return;
                }
                Time.timeScale = 2;
                StartCoroutine(Replay());
                isReplaying = true;
            }

            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
               if(isReplaying)
                {
                    return;
                }
                StartCoroutine(ReversedPlay());
                isReplaying = true;
            }
        }


        //Replay Coroutine
        private IEnumerator Replay()
        {
            //Move the object back to where it started
            objectThatMoves.transform.position = startPos;

            //Pause so we can see that it has started at the start position
            yield return new WaitForSeconds(REPLAY_PAUSE_TIMER);

            //convert the undo stack to an array:
            Command[] oldCommands = undoCommands.ToArray();

            //this array is inverted: see geeksforgeeks.com for reference
            for(int i = oldCommands.Length - 1; i>=0; i--)
            {
                Command currentCommand = oldCommands[i];
                currentCommand.Execute();
                yield return new WaitForSeconds(REPLAY_PAUSE_TIMER);
            }

            isReplaying = false;
        }

        //smit's add on:
        //it is opposite of reverse: like you made positions: 1,2,3,4,5: then here it will play as: 5,4,3,2,1
        private IEnumerator ReversedPlay()
        {
            objectThatMoves.transform.position = reversedPlayStartPos;
            yield return new WaitForSeconds(REPLAY_PAUSE_TIMER);

            Command[] oldCommands = undoCommands.ToArray();

            //the undoCommands.TOArray() provides reversed stack: what exactly we need: so no need to iterate reversed for loop like in Replay() coroutine
            for(int i=0; i<= oldCommands.Length-1; i++)
            {
                Command currentCommand = oldCommands[i];
                currentCommand.Undo();
                yield return new WaitForSeconds(REPLAY_PAUSE_TIMER);
            }
            isReplaying = false;
        }


        //will execute the command and do stuff to the list (stack here) to make the replay, undo, redo system work
        private void ExecuteNewCommand(Command commandButton)
        {
            commandButton.Execute();
            
            //add the new command to the last position in the list: here we are using the stack: so stack.puch: will add this command at the last position of the stack
            undoCommands.Push(commandButton);

            reversedPlayStartPos = objectThatMoves.transform.position;

            //doubt in below line
            //remove all redo commands because redo is not defined when we have add a new command
            redoCommands.Clear();
        }


        //swap the pointers to two commands
        private void SwapKeys(ref Command Key1, ref Command Key2)
        {
            Command temp = Key1;

            Key1 = Key2;
            Key2 = temp;
        }
    }

}

