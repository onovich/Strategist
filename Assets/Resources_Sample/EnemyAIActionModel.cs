using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist.Sample {

    public struct EnemyAIActionModel {

        public EnemyAIActionType actionType;

        // Chase
        public float moveSpeed;

        // Attack
        public float damage;

        // Flee
        public float fleeSpeed;

        // Warn
        public float warnDuration;

    }

}