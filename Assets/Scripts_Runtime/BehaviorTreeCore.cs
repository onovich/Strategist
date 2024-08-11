using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class BehaviorTreeCore {

        public BTNode root;

        public BehaviorTreeCore(BTNode root) {
            this.root = root;
        }

        public void Tick() {
            if (root == null) {
                return;
            }
            var status = root.Execute();
            if (status == BTNodeStatus.Done) {
                root.Reset();
            }
        }

    }

}