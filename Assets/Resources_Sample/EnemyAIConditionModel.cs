using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist.Sample {

    public struct EnemyAIConditionModel {

        public EnemyAIConditionType conditionType;
        public float distanceFromPlayer;
        public bool isPlayerInPowerMode;

    }

}