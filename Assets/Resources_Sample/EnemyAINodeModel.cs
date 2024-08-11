using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist.Sample {

    public struct EnemyAINodeModel {

        public bool isRoot;
        public bool isAction;
        public EnemyAIConditionModel condition;
        public EnemyAIActionModel action;
        public BTNodeType nodeType;

    }

}