using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class BTNode {

        public BTNodeType nodeType;
        public BTNodeStatus status;
        public List<BTNode> children;
        BTNode activeChild;
        public Func<BTNodeStatus> action_enter;
        public Func<BTNodeStatus> action_tick;

        public Func<bool> condition;

        public BTNode(Func<BTNodeStatus> action, Func<bool> condition) {
            this.action_enter = action;
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

        public void SetAction(Func<BTNodeStatus> action_enter, Func<BTNodeStatus> action_tick, Func<bool> condition) {
            this.nodeType = BTNodeType.Action;
            this.condition = condition;
            this.action_enter = action_enter;
            this.action_tick = action_tick;
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

        public BTNodeStatus Execute() {
            switch (nodeType) {
                case BTNodeType.Sequence:
                    return ExecuteSequence();
                case BTNodeType.Selector:
                    return ExecuteSelector();
                case BTNodeType.ParallelAnd:
                    return ExecuteParallelAnd();
                case BTNodeType.ParallelOr:
                    return ExecuteParallelOr();
                case BTNodeType.Action:
                    return ExecuteAction();
            }
            return status;
        }

        BTNodeStatus ExecuteAction() {
            if (status == BTNodeStatus.NotEntered) {
                if (!condition.Invoke()) {
                    status = BTNodeStatus.Done;
                } else {
                    status = Action_Enter();
                }
            } else {
                if (status == BTNodeStatus.Running) {
                    status = Action_Tick();
                }
            }
            return status;
        }

        BTNodeStatus Action_Enter() {
            return action_enter.Invoke();
        }

        BTNodeStatus Action_Tick() {
            return action_tick.Invoke();
        }

        BTNodeStatus ExecuteSequence() {
            if (status == BTNodeStatus.NotEntered) {
                if (!condition.Invoke()) {
                    return BTNodeStatus.Done;
                } else {
                    status = BTNodeStatus.Running;
                }
            } else {
                if (status == BTNodeStatus.Running) {
                    var doneCount = 0;
                    foreach (var child in children) {
                        if (child.status != BTNodeStatus.Done) {
                            var _ = child.Execute();
                            break;
                        } else {
                            doneCount++;
                        }
                    }
                    if (doneCount == children.Count) {
                        status = BTNodeStatus.Done;
                    }
                }
            }
            return status;
        }

        BTNodeStatus ExecuteSelector() {
            if (status == BTNodeStatus.NotEntered) {
                if (!condition.Invoke()) {
                    return BTNodeStatus.Done;
                } else {
                    status = BTNodeStatus.Running;
                }
            } else {
                if (status == BTNodeStatus.Running) {
                    if (activeChild != null) {
                        var _ = activeChild.Execute();
                        if (activeChild.status == BTNodeStatus.Done) {
                            status = BTNodeStatus.Done;
                            activeChild = null;
                        }
                    } else {
                        foreach (var child in children) {
                            var _ = child.Execute();
                            if (child.status == BTNodeStatus.Done) {
                                status = BTNodeStatus.Done;
                                break;
                            } else if (child.status == BTNodeStatus.Running) {
                                activeChild = child;
                                break;
                            }
                        }
                    }
                }
            }
            return status;
        }

        BTNodeStatus ExecuteParallelAnd() {
            if (status == BTNodeStatus.NotEntered) {
                if (!condition.Invoke()) {
                    return BTNodeStatus.Done;
                } else {
                    status = BTNodeStatus.Running;
                }
            } else {
                if (status == BTNodeStatus.Running) {
                    var doneCount = 0;
                    foreach (var child in children) {
                        var _ = child.Execute();
                        if (child.status == BTNodeStatus.Done) {
                            doneCount++;
                        }
                    }
                    if (doneCount == children.Count) {
                        status = BTNodeStatus.Done;
                    }
                }
            }
            return status;
        }

        BTNodeStatus ExecuteParallelOr() {
            if (status == BTNodeStatus.NotEntered) {
                if (!condition.Invoke()) {
                    return BTNodeStatus.Done;
                } else {
                    status = BTNodeStatus.Running;
                }
            } else {
                if (status == BTNodeStatus.Running) {
                    var doneCount = 0;
                    foreach (var child in children) {
                        var _ = child.Execute();
                        if (child.status == BTNodeStatus.Done) {
                            doneCount++;
                        }
                    }
                    if (doneCount > 0) {
                        status = BTNodeStatus.Done;
                    }
                }
            }
            return status;
        }

    }

}