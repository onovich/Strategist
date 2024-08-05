using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class BTNode {

        public BTNodeType nodeType;
        public List<BTNode> children;
        public Func<BTNodeStatus> action;
        public Func<bool> condition;

        public BTNode(Func<BTNodeStatus> action, Func<bool> condition) {
            this.action = action;
            this.condition = condition;
            this.children = new List<BTNode>();
        }

        public BTNodeStatus Execute() {
            return condition.Invoke() ? ExucuteAction() : BTNodeStatus.Failure;
        }

        BTNodeStatus ExucuteAction() {
            if (action != null) {
                return action.Invoke();
            }
            switch (nodeType) {
                case BTNodeType.Sequence:
                    return ExecuteSequence();
                case BTNodeType.Selector:
                    return ExecuteSelector();
                case BTNodeType.ParallelAnd:
                    return ExecuteParallelAnd();
                case BTNodeType.ParallelOr:
                    return ExecuteParallelOr();
                default:
                    return BTNodeStatus.Failure;
            }
        }

        BTNodeStatus ExecuteSequence() {
            foreach (var child in children) {
                var status = child.Execute();
                if (status == BTNodeStatus.Failure) {
                    return status;
                }
                if (status == BTNodeStatus.Running) {
                    return status;
                }
            }
            return BTNodeStatus.Success;
        }

        BTNodeStatus ExecuteSelector() {
            foreach (var child in children) {
                var status = child.Execute();
                if (status == BTNodeStatus.Success) {
                    return status;
                }
                if (status == BTNodeStatus.Running) {
                    return status;
                }
            }
            return BTNodeStatus.Failure;
        }

        BTNodeStatus ExecuteParallelAnd() {
            var successCount = 0;
            var failureCount = 0;
            foreach (var child in children) {
                var status = child.Execute();
                if (status == BTNodeStatus.Success) {
                    successCount++;
                }
                if (status == BTNodeStatus.Failure) {
                    failureCount++;
                }
            }
            if (successCount == children.Count) {
                return BTNodeStatus.Success;
            }
            if (failureCount > 0) {
                return BTNodeStatus.Failure;
            }
            return BTNodeStatus.Running;
        }

        BTNodeStatus ExecuteParallelOr() {
            var successCount = 0;
            var failureCount = 0;
            foreach (var child in children) {
                var status = child.Execute();
                if (status == BTNodeStatus.Success) {
                    successCount++;
                }
                if (status == BTNodeStatus.Failure) {
                    failureCount++;
                }
            }
            if (failureCount == children.Count) {
                return BTNodeStatus.Failure;
            }
            if (successCount > 0) {
                return BTNodeStatus.Failure;
            }
            return BTNodeStatus.Running;
        }

        public void AddChild(BTNode child) {
            children.Add(child);
        }

        public void Clear() {
            children.ForEach(child => child.Clear());
            children.Clear();
            action = null;
            condition = null;
        }

    }

}