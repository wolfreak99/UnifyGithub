/*************************
 * Original url: http://wiki.unity3d.com/index.php/Finite_State_Machine
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/Finite_State_Machine.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
    Contents [hide] 
    1 Description 
    2 Components 
    3 C# - FSMSystem.cs 
    4 Example 
    
    Description This is a Deterministic Finite State Machine framework based on chapter 3.1 of Game Programming Gems 1 by Eric Dybsend. Therea are two classes and two enums. Include them in your project and follow the explanations to get the FSM working properly. There's also a complete example script at the end of this page. 
    Components Transition enum: This enum contains the labels to the transitions that can be fired by the system. Don't change the first label, NullTransition, as the FSMSystem class uses it. 
    StateID enum: This is the ID of the states the game may have. You could use references to the real States' classes but using enums makes the system less susceptible to have code having access to objects it is not supposed to. All the states' ids should be placed here. Don't change the first label, NullStateID, as the FSMSystem class uses it. 
    FSMState class: This class has a Dictionary with pairs (Transition-StateID) indicating which new state S2 the FSM should go to when a transition T is fired and the current state is S1. It has methods to add and delete pairs (Transition-StateID), a method to check which state to go to if a transition is passed to it. Two methods are used in the example given to check which transition should be fired (Reason()) and which action(s) (Act()) the GameObject that has the FSMState attached should do. You don't have to use this schema, but some kind of transition-action code must be used in your game. 
    FSMSystem: This is the Finite State Machine class that each NPC or GameObject in your game must have in order to use the framework. It stores the NPC's States in a List, has methods to add and delete a state and a method to change the current state based on a transition passed to it (PerformTransition()). You can call this method anywhere within your code, as in a collision test, or within Update() or FixedUpdate(). 
    C# - FSMSystem.csusing System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
     
    /**
    A Finite State Machine System based on Chapter 3.1 of Game Programming Gems 1 by Eric Dybsand
     
    Written by Roberto Cezar Bianchini, July 2010
     
     
    How to use:
    	1. Place the labels for the transitions and the states of the Finite State System
    	    in the corresponding enums.
     
    	2. Write new class(es) inheriting from FSMState and fill each one with pairs (transition-state).
    	    These pairs represent the state S2 the FSMSystem should be if while being on state S1, a
    	    transition T is fired and state S1 has a transition from it to S2. Remember this is a Deterministic FSM. 
    	    You can't have one transition leading to two different states.
     
    	   Method Reason is used to determine which transition should be fired.
    	   You can write the code to fire transitions in another place, and leave this method empty if you
    	   feel it's more appropriate to your project.
     
    	   Method Act has the code to perform the actions the NPC is supposed do if it's on this state.
    	   You can write the code for the actions in another place, and leave this method empty if you
    	   feel it's more appropriate to your project.
     
    	3. Create an instance of FSMSystem class and add the states to it.
     
    	4. Call Reason and Act (or whichever methods you have for firing transitions and making the NPCs
    	     behave in your game) from your Update or FixedUpdate methods.
     
    	Asynchronous transitions from Unity Engine, like OnTriggerEnter, SendMessage, can also be used, 
    	just call the Method PerformTransition from your FSMSystem instance with the correct Transition 
    	when the event occurs.
     
     
     
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
    INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE 
    AND NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
    */
     
     
    /// <summary>
    /// Place the labels for the Transitions in this enum.
    /// Don't change the first label, NullTransition as FSMSystem class uses it.
    /// </summary>
    public enum Transition
    {
        NullTransition = 0, // Use this transition to represent a non-existing transition in your system
    }
     
    /// <summary>
    /// Place the labels for the States in this enum.
    /// Don't change the first label, NullTransition as FSMSystem class uses it.
    /// </summary>
    public enum StateID
    {
        NullStateID = 0, // Use this ID to represent a non-existing State in your system	
    }
     
    /// <summary>
    /// This class represents the States in the Finite State System.
    /// Each state has a Dictionary with pairs (transition-state) showing
    /// which state the FSM should be if a transition is fired while this state
    /// is the current state.
    /// Method Reason is used to determine which transition should be fired .
    /// Method Act has the code to perform the actions the NPC is supposed do if it's on this state.
    /// </summary>
    public abstract class FSMState
    {
        protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
        protected StateID stateID;
        public StateID ID { get { return stateID; } }
     
        public void AddTransition(Transition trans, StateID id)
        {
            // Check if anyone of the args is invalid
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
                return;
            }
     
            if (id == StateID.NullStateID)
            {
                Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
                return;
            }
     
            // Since this is a Deterministic FSM,
            //   check if the current transition was already inside the map
            if (map.ContainsKey(trans))
            {
                Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() + 
                               "Impossible to assign to another state");
                return;
            }
     
            map.Add(trans, id);
        }
     
        /// <summary>
        /// This method deletes a pair transition-state from this state's map.
        /// If the transition was not inside the state's map, an ERROR message is printed.
        /// </summary>
        public void DeleteTransition(Transition trans)
        {
            // Check for NullTransition
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed");
                return;
            }
     
            // Check if the pair is inside the map before deleting
            if (map.ContainsKey(trans))
            {
                map.Remove(trans);
                return;
            }
            Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() + 
                           " was not on the state's transition list");
        }
     
        /// <summary>
        /// This method returns the new state the FSM should be if
        ///    this state receives a transition and 
        /// </summary>
        public StateID GetOutputState(Transition trans)
        {
            // Check if the map has this transition
            if (map.ContainsKey(trans))
            {
                return map[trans];
            }
            return StateID.NullStateID;
        }
     
        /// <summary>
        /// This method is used to set up the State condition before entering it.
        /// It is called automatically by the FSMSystem class before assigning it
        /// to the current state.
        /// </summary>
        public virtual void DoBeforeEntering() { }
     
        /// <summary>
        /// This method is used to make anything necessary, as reseting variables
        /// before the FSMSystem changes to another one. It is called automatically
        /// by the FSMSystem before changing to a new state.
        /// </summary>
        public virtual void DoBeforeLeaving() { } 
     
        /// <summary>
        /// This method decides if the state should transition to another on its list
        /// NPC is a reference to the object that is controlled by this class
        /// </summary>
        public abstract void Reason(GameObject player, GameObject npc);
     
        /// <summary>
        /// This method controls the behavior of the NPC in the game World.
        /// Every action, movement or communication the NPC does should be placed here
        /// NPC is a reference to the object that is controlled by this class
        /// </summary>
        public abstract void Act(GameObject player, GameObject npc);
     
    } // class FSMState
     
     
    /// <summary>
    /// FSMSystem class represents the Finite State Machine class.
    ///  It has a List with the States the NPC has and methods to add,
    ///  delete a state, and to change the current state the Machine is on.
    /// </summary>
    public class FSMSystem
    {
        private List<FSMState> states;
     
        // The only way one can change the state of the FSM is by performing a transition
        // Don't change the CurrentState directly
        private StateID currentStateID;
        public StateID CurrentStateID { get { return currentStateID; } }
        private FSMState currentState;
        public FSMState CurrentState { get { return currentState; } }
     
        public FSMSystem()
        {
            states = new List<FSMState>();
        }
     
        /// <summary>
        /// This method places new states inside the FSM,
        /// or prints an ERROR message if the state was already inside the List.
        /// First state added is also the initial state.
        /// </summary>
        public void AddState(FSMState s)
        {
            // Check for Null reference before deleting
            if (s == null)
            {
                Debug.LogError("FSM ERROR: Null reference is not allowed");
            }
     
            // First State inserted is also the Initial state,
            //   the state the machine is in when the simulation begins
            if (states.Count == 0)
            {
                states.Add(s);
                currentState = s;
                currentStateID = s.ID;
                return;
            }
     
            // Add the state to the List if it's not inside it
            foreach (FSMState state in states)
            {
                if (state.ID == s.ID)
                {
                    Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() + 
                                   " because state has already been added");
                    return;
                }
            }
            states.Add(s);
        }
     
        /// <summary>
        /// This method delete a state from the FSM List if it exists, 
        ///   or prints an ERROR message if the state was not on the List.
        /// </summary>
        public void DeleteState(StateID id)
        {
            // Check for NullState before deleting
            if (id == StateID.NullStateID)
            {
                Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
                return;
            }
     
            // Search the List and delete the state if it's inside it
            foreach (FSMState state in states)
            {
                if (state.ID == id)
                {
                    states.Remove(state);
                    return;
                }
            }
            Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() + 
                           ". It was not on the list of states");
        }
     
        /// <summary>
        /// This method tries to change the state the FSM is in based on
        /// the current state and the transition passed. If current state
        ///  doesn't have a target state for the transition passed, 
        /// an ERROR message is printed.
        /// </summary>
        public void PerformTransition(Transition trans)
        {
            // Check for NullTransition before changing the current state
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
                return;
            }
     
            // Check if the currentState has the transition passed as argument
            StateID id = currentState.GetOutputState(trans);
            if (id == StateID.NullStateID)
            {
                Debug.LogError("FSM ERROR: State " + currentStateID.ToString() +  " does not have a target state " + 
                               " for transition " + trans.ToString());
                return;
            }
     
            // Update the currentStateID and currentState		
            currentStateID = id;
            foreach (FSMState state in states)
            {
                if (state.ID == currentStateID)
                {
                    // Do the post processing of the state before setting the new one
                    currentState.DoBeforeLeaving();
     
                    currentState = state;
     
                    // Reset the state to its desired condition before it can reason or act
                    currentState.DoBeforeEntering();
                    break;
                }
            }
     
        } // PerformTransition()
     
    } //class FSMSystemExampleHere's an example that implements the above framework. The GameObject with this script follows a path of waypoints and starts chasing a target if it comes within a certain distance from it. Attach this class to your NPC. Besides the framework transition and stateid enums, you also have to setup a reference to the waypoints and to the target (player). I used FixedUpdate() in the MonoBehaviour because the NPC reasoning schema doesn't need to be called every frame, but you can change that and use Update(). 
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
     
    [RequireComponent(typeof(Rigidbody))]
    public class NPCControl : MonoBehaviour
    {
        public GameObject player;
        public Transform[] path;
        private FSMSystem fsm;
     
        public void SetTransition(Transition t) { fsm.PerformTransition(t); }
     
        public void Start()
        {
            MakeFSM();
        }
     
        public void FixedUpdate()
        {
            fsm.CurrentState.Reason(player, gameObject);
            fsm.CurrentState.Act(player, gameObject);
        }
     
    	// The NPC has two states: FollowPath and ChasePlayer
    	// If it's on the first state and SawPlayer transition is fired, it changes to ChasePlayer
    	// If it's on ChasePlayerState and LostPlayer transition is fired, it returns to FollowPath
        private void MakeFSM()
        {
            FollowPathState follow = new FollowPathState(path);
            follow.AddTransition(Transition.SawPlayer, StateID.ChasingPlayer);
     
            ChasePlayerState chase = new ChasePlayerState();
            chase.AddTransition(Transition.LostPlayer, StateID.FollowingPath);
     
            fsm = new FSMSystem();
            fsm.AddState(follow);
            fsm.AddState(chase);
        }
    }
     
    public class FollowPathState : FSMState
    {
        private int currentWayPoint;
        private Transform[] waypoints;
     
        public FollowPathState(Transform[] wp) 
        { 
            waypoints = wp;
            currentWayPoint = 0;
            stateID = StateID.FollowingPath;
        }
     
        public override void Reason(GameObject player, GameObject npc)
        {
            // If the Player passes less than 15 meters away in front of the NPC
            RaycastHit hit;
            if (Physics.Raycast(npc.transform.position, npc.transform.forward, out hit, 15F))
            {
                if (hit.transform.gameObject.tag == "Player")
                    npc.GetComponent<NPCControl>().SetTransition(Transition.SawPlayer);
            }
        }
     
        public override void Act(GameObject player, GameObject npc)
        {
            // Follow the path of waypoints
    		// Find the direction of the current way point 
            Vector3 vel = npc.rigidbody.velocity;
            Vector3 moveDir = waypoints[currentWayPoint].position - npc.transform.position;
     
            if (moveDir.magnitude < 1)
            {
                currentWayPoint++;
                if (currentWayPoint >= waypoints.Length)
                {
                    currentWayPoint = 0;
                }
            }
            else
            {
                vel = moveDir.normalized * 10;
     
                // Rotate towards the waypoint
                npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
                                                          Quaternion.LookRotation(moveDir),
                                                          5 * Time.deltaTime);
                npc.transform.eulerAngles = new Vector3(0, npc.transform.eulerAngles.y, 0);
     
            }
     
            // Apply the Velocity
            npc.rigidbody.velocity = vel;
        }
     
    } // FollowPathState
     
    public class ChasePlayerState : FSMState
    {
        public ChasePlayerState()
        {
            stateID = StateID.ChasingPlayer;
        }
     
        public override void Reason(GameObject player, GameObject npc)
        {
            // If the player has gone 30 meters away from the NPC, fire LostPlayer transition
            if (Vector3.Distance(npc.transform.position, player.transform.position) >= 30)
                npc.GetComponent<NPCControl>().SetTransition(Transition.LostPlayer);
        }
     
        public override void Act(GameObject player, GameObject npc)
        {
            // Follow the path of waypoints
    		// Find the direction of the player 		
            Vector3 vel = npc.rigidbody.velocity;
            Vector3 moveDir = player.transform.position - npc.transform.position;
     
            // Rotate towards the waypoint
            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
                                                      Quaternion.LookRotation(moveDir),
                                                      5 * Time.deltaTime);
            npc.transform.eulerAngles = new Vector3(0, npc.transform.eulerAngles.y, 0);
     
            vel = moveDir.normalized * 10;
     
            // Apply the new Velocity
            npc.rigidbody.velocity = vel;
        }
     
    } // ChasePlayerState
}
