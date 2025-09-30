using System;
using System.Collections.Generic;

namespace LazyCoder.Core
{
    /// <summary>
    /// A lightweight state machine.
    /// </summary>
    /// <remarks>
    /// 	<para>To use it: </para>
    /// 	<list type="bullet">
    /// 		<item>
    /// 			<description>Define your own label. Enums are probably the best
    /// choice.</description>
    /// 		</item>
    /// 		<item>
    /// 			<description>Construct a new state machine, typically in a
    /// MonoBehaviour's Start method.</description>
    /// 		</item>
    /// 		<item>
    /// 			<description>Add the various states with the appropriate delegates.
    /// </description>
    /// 		</item>
    /// 		<item>
    /// 			<description>Call the state machine's Update method from the
    /// MonoBehaviour's Update method.</description>
    /// 		</item>
    /// 		<item>
    /// 			<description>Set the CurrentState property on the state machine to transition.
    /// (You can either set it爁rom one of the state delegates, or from anywhere else.
    /// </description>
    /// 		</item>
    /// 	</list>
    /// 	<para>When a state is changed, the OnStop on existing state is called, then the
    /// OnStart of the new state, and from there on OnUpdate of the new state each time
    /// the update is called.</para>
    /// </remarks>
    /// <typeparam name="TLabel">The label type of this state machine. Enums are common,
    /// but strings or int are other possibilities.</typeparam>
    public class StateMachine<TLabel>
    {
        #region Types

        private class State
        {
            #region Public Fields

            public readonly TLabel Label;
            public readonly Action OnStart;
            public readonly Action OnStop;
            public readonly Action OnUpdate;

            #endregion

            #region Constructors

            public State(TLabel label, Action onStart, Action onUpdate, Action onStop)
            {
                this.OnStart = onStart;
                this.OnUpdate = onUpdate;
                this.OnStop = onStop;
                this.Label = label;
            }

            #endregion
        }

        #endregion

        #region Private Fields

        private readonly Dictionary<TLabel, State> _stateDictionary;
        private State _currentState;
        private State _previousState;

        #endregion

        #region Properties

        public TLabel PreviousState => _previousState == null ? _currentState.Label : _previousState.Label;

        /// <summary>
        /// Returns the label of the current state.
        /// </summary>
        public TLabel CurrentState
        {
            get => _currentState.Label;

            set => ChangeState(value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new StateMachine.
        /// </summary>
        public StateMachine()
        {
            _stateDictionary = new Dictionary<TLabel, State>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method should be called every frame.
        /// </summary>
        public void Update()
        {
            if (_currentState != null && _currentState.OnUpdate != null)
            {
                _currentState.OnUpdate();
            }
        }

        public void AddState(TLabel label)
        {
            _stateDictionary[label] = new State(label, null, null, null);
        }
        
        public void AddState(TLabel label, IStateMachine stateMachine)
        {
            stateMachine.Init();

            _stateDictionary[label] = new State(label, stateMachine.OnStart, stateMachine.OnUpdate, stateMachine.OnStop);
        }

        /// <summary>
        /// Adds a state, and the delegates that should run 
        /// when the state starts, stops, 
        /// and when the state machine is updated.
        /// 
        /// Any delegate can be null, and wont be executed.
        /// </summary>
        /// <param name="label">The name of the state to add.</param>
        /// <param name="onStart">The action performed when the state is entered.</param>
        /// <param name="onUpdate">The action performed when the state machine is updated in the given state.</param>
        /// <param name="onStop">The action performed when the state machine is left.</param>
        public void AddState(TLabel label,  Action onStart ,  Action onUpdate ,  Action onStop)
        {
            _stateDictionary[label] = new State(label, onStart, onUpdate, onStop);
        }

        /// <summary>
        /// Adds a sub-state machine for the given state.
        ///
        /// The sub-state machine need not be updated, as long as this state machine
        /// is being updated.
        /// </summary>
        /// <typeparam name="TSubStateLabel">The type of the sub-machine.</typeparam>
        /// <param name="label">The name of the state to add.</param>
        /// <param name="subMachine">The sub-machine that will run during the given state.</param>
        /// <param name="subMachineStartState">The starting state of the sub-machine.</param>
        public void AddState<TSubStateLabel>(TLabel label, StateMachine<TSubStateLabel> subMachine,TSubStateLabel subMachineStartState)
        {
            AddState(label,() => subMachine.ChangeState(subMachineStartState),subMachine.Update, null);
        }

        /// <summary>
        /// Returns the current state name
        /// </summary>
        public override string ToString()
        {
            return CurrentState.ToString();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Changes the state from the existing one to the state with the given label.
        /// 
        /// It is legal (and useful) to transition to the same state, in which case the 
        /// current state's onStop action is called, the onStart action is called, and the
        /// state keeps on updating as before. The behaviour is exactly the same as switching to
        /// a new state.
        /// </summary>
        private void ChangeState(TLabel newState)
        {
            _previousState = _currentState;

            if (_currentState != null && _currentState.OnStop != null)
            {
                _currentState.OnStop();
            }

            _currentState = _stateDictionary[newState];

            _currentState.OnStart?.Invoke();

            EventStateChanged?.Invoke(this);
        }

        #endregion

        #region Callback Events

        public event Action<StateMachine<TLabel>> EventStateChanged;

        #endregion
    }
}