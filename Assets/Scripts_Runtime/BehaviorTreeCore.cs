using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class BehaviorTreeCore {

        public int rootIndex;
        public SortedList<int, BTNode> nodes;

        int indexRecord;

        public BehaviorTreeCore() {
            nodes = new SortedList<int, BTNode>();
            indexRecord = 0;
        }

        public void AddNode(Func<BTNodeStatus> action) {
            nodes.Add(index, node);
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