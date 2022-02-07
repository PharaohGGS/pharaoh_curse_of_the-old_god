using BehaviourTree.Runtime;
using BehaviourTree.Tools;
using Pharaoh.Tools;
using DesignPatterns;
using UnityEngine;

namespace Pharaoh.AI.Managers
{
    public class AiManager : MonoSingleton<AiManager>
    {
        [field: SerializeField] public GenericDictionary<int, BehaviourTreeRunner> RunnerIds { get; private set; }
        
    }
}