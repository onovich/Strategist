using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class BehaviorTreeCore {

        internal SortedList<int, BTNode> nodes;
        BTNode root;

        int indexRecord;

        public BehaviorTreeCore() {
            nodes = new SortedList<int, BTNode>();
            indexRecord = 0;
        }

        public int CreateContainerNode(Func<bool> condition, BTNodeType nodeType) {
            var node = new BTNode();
            node.SetContainer(nodeType, condition);
            var index = indexRecord++;
            nodes.Add(index, node);
            return index;
        }

        public int CreateAction(Func<BTNodeStatus> action, Func<bool> condition) {
            var node = new BTNode();
            node.SetAction(action, condition);
            var index = indexRecord++;
            nodes.Add(index, node);
            return index;
        }

        public void AddChild(int parentIndex, int childIndex) {
            var parent = nodes[parentIndex];
            var child = nodes[childIndex];
            parent.AddChild(child);
        }

        public void Tick() {
            if (root == null) {
                return;
            }
            root.Execute();
            var status = root.Status;
            if (status == BTNodeStatus.Done) {
                root.Reset();
            }
        }

    }

}