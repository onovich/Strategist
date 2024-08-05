using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class BehaviorTreeCore {

        public BTNode root;

        public BehaviorTreeCore(BTNode root) {
            this.root = root;
        }

        public void Execute() {
            root.Execute();
        }

        public void Clear() {
            root.Clear();
        }

    }

}