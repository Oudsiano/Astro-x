using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DI;
using Services.DI;
using Startup.GameStateMachine.GameStates;
using UnityEngine;

namespace Startup.GameStateMachine
{
    public enum GameStateType
    {
        MainMenu,
        Play
    }
    
    public class GameStateMachine
    {
        public GameStateType CurrentStateType { get; private set; }

        private readonly Dictionary<GameStateType, IGameState> _states = new()
        {
            { GameStateType.MainMenu, GameContainer.Create<MainMenuGameState>() },
            { GameStateType.Play, GameContainer.Create<PlayGameState>() },
        };

        private IGameState _currentState;

        public async UniTask SwitchToState(GameStateType stateType, bool force = false)
        {
            Debug.Log($"Switching to state {stateType}");
            if (CurrentStateType == stateType && !force)
            {
                Debug.Log($"Already in state {stateType}!");
                return;
            }

            if (_currentState != null)
                await _currentState.OnExit();

            CurrentStateType = stateType;
            _currentState = _states[stateType];
            await _currentState.OnEnter();
        }
    }
}