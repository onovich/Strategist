using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class BTNode {

        BTNodeType nodeType;

        BTNodeStatus status;
        public void SetStatus(BTNodeStatus value) => status = value;

        List<BTNode> children;
        Func<BTNodeStatus> action;
        BTNode activeChild;
        Func<bool> condition;

        public BTNode(Func<BTNodeStatus> action, Func<bool> condition) {
            this.action = action;
            this.condition = condition;
            this.children = new List<BTNode>();
        }

        public void Reset() {
            status = BTNodeStatus.NotEntered;
            foreach (var child in children) {
                child.Reset();
            }
        }

        public void AddChild(BTNode child) {
            if (nodeType == BTNodeType.Action) {
                throw new Exception("Add Child Error: Action Node Can't Have Children");
            }
            children.Add(child);
        }

        public void SetAction(Func<BTNodeStatus> action, Func<bool> condition) {
            this.nodeType = BTNodeType.Action;
            this.condition = condition;
            this.action = action;
            this.status = BTNodeStatus.NotEntered;
            this.children = null;
        }

        public void SetContainer(BTNodeType nodeType, Func<bool> condition) {
            if (nodeType == BTNodeType.Action) {
                throw new Exception("Set Container Error: Action Node Can't Be Container");
            }
            this.nodeType = nodeType;
            this.condition = condition;
            this.status = BTNodeStatus.NotEntered;
            this.children = new List<BTNode>();
        }

        public void Execute() {
            switch (nodeType) {
                case BTNodeType.Sequence:
                    ExecuteSequence();
                    break;
                case BTNodeType.Selector:
                    ExecuteSelector();
                    break;
                case BTNodeType.ParallelAnd:
                    ExecuteParallelAnd();
                    break;
                case BTNodeType.ParallelOr:
                    ExecuteParallelOr();
                    break;
                case BTNodeType.Action:
                    ExecuteAction();
                    break;
            }
        }

        BTNodeStatus ExecuteAction() {
            if (!PreCondition()) {
                status = BTNodeStatus.Done;
            } else {
                status = Action_Tick();
            }
            return status;
        }

        BTNodeStatus Action_Tick() {
            return action.Invoke();
        }

        public bool PreCondition() {
            return condition.Invoke();
        }

        void ExecuteSequence() {
            // 遍历子节点
            var doneCount = 0;
            foreach (var child in children) {
                // 1. 如果任意节点不可执行, 则当前节点状态置为Done, 并阻断遍历
                if (!child.PreCondition()) {
                    status = BTNodeStatus.Done;
                    return;
                }
                // 2. 如果当前子节点状态为Running, 则执行当前子节点
                if (child.status == BTNodeStatus.Running) {
                    child.Execute();
                    return;
                }
                // 3. 如果当前子节点状态为Done, 则计数器+1
                if (child.status == BTNodeStatus.Done) {
                    doneCount++;
                }
                // 4. 如果计数器等于子节点数量, 则当前节点状态置为Done, 并阻断遍历
                if (doneCount == children.Count) {
                    status = BTNodeStatus.Done;
                    return;
                }
            }
        }

        void ExecuteSelector() {
            // 当前活跃节点不为空时, 执行当前活跃节点
            if (activeChild != null) {
                if (activeChild.status == BTNodeStatus.Running) {
                    activeChild.Execute();
                    return;
                }
                if (activeChild.status == BTNodeStatus.Done) {
                    status = BTNodeStatus.Done;
                    activeChild = null;
                    return;
                }
            }
            // 当前活跃节点为空时, 遍历子节点
            foreach (var child in children) {
                // 1. 如果当前子节点不可执行, 则继续遍历
                if (!child.PreCondition()) {
                    continue;
                }
                // 2. 如果当前子节点状态为Running, 则执行当前子节点
                if (child.status == BTNodeStatus.Running) {
                    activeChild = child;
                    activeChild.Execute();
                    return;
                }
                // 3. 如果当前子节点状态为Done, 则当前节点状态置为Done, 并阻断遍历
                if (child.status == BTNodeStatus.Done) {
                    status = BTNodeStatus.Done;
                    return;
                }
            }
        }

        void ExecuteParallelAnd() {
            // 遍历子节点
            var doneCount = 0;
            foreach (var child in children) {
                // 1. 如果当前子节点状态为Running, 则执行当前子节点
                if (child.status == BTNodeStatus.Running) {
                    child.Execute();
                }
                // 2. 如果当前子节点状态为Done, 则计数器+1
                if (child.status == BTNodeStatus.Done) {
                    doneCount++;
                }
                // 3. 如果计数器等于子节点数量, 则当前节点状态置为Done, 并阻断遍历
                if (doneCount == children.Count) {
                    status = BTNodeStatus.Done;
                    return;
                }
            }
        }

        void ExecuteParallelOr() {
            // 遍历子节点
            foreach (var child in children) {
                // 1. 如果当前子节点状态为Running, 则执行当前子节点
                if (child.status == BTNodeStatus.Running) {
                    child.Execute();
                }
                // 2. 如果当前子节点状态为Done, 则当前节点状态置为Done, 并阻断遍历
                if (child.status == BTNodeStatus.Done) {
                    status = BTNodeStatus.Done;
                    return;
                }
            }
        }

    }

}