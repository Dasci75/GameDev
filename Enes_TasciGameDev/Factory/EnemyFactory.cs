using Enes_TasciGameDev.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Enes_TasciGameDev.Factory
{
    public class EnemyFactory
    {
        public static object CreateEnemy(EnemyType type, Texture2D texture, Vector2 position, float speed, List<Vector2> patrolPoints = null)
        {
            switch (type)
            {
                case EnemyType.Goblin:
                    return new Goblin(texture, position, speed);

                case EnemyType.Skeleton:
                    if (patrolPoints == null) patrolPoints = new List<Vector2>();
                    return new Skeleton(texture, position, speed, patrolPoints);

                case EnemyType.Thief:
                    return new Thief(texture, position, speed);

                default:
                    throw new ArgumentException("Unknown enemy type: " + type);
            }
        }
    }

    public enum EnemyType
    {
        Goblin,
        Skeleton,
        Thief
    }
}