using Pharaoh.Gameplay.Components;
using Pharaoh.Gameplay.Sets;

namespace Pharaoh.AI
{
    public class EnemyPawn : Pawn
    {
        public EnemyPawnRuntimeSet enemyPawnRuntimeSet;
        
        private void OnEnable()
        {
            enemyPawnRuntimeSet?.Add(this);
        }

        private void OnDisable()
        {
            enemyPawnRuntimeSet?.Remove(this);
        }
    }
}