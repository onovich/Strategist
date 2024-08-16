using System;
using System.Collections.Generic;

namespace MortiseFrame.Strategist {

    public class FSMCore {

        public FSMNode currentNode;
        SortedList<int, FSMNode> nodes;
        SortedList<int, FSMTranslation> translations;
        int nodeIndexRecord;
        int translationIndexRecord;

        public void Tick(float dt) {
            if (currentNode == null) {
                return;
            }
            currentNode.Tick(dt);
        }

        public int CreateNode(Action onEnter, Action onExit, Action<float> onTick) {
            currentNode = new FSMNode(onEnter, onExit, onTick);
            nodeIndexRecord++;
            nodes.Add(nodeIndexRecord, currentNode);
            return nodeIndexRecord;
        }

        public void RemoveNode(int nodeIndex) {
            var node = nodes[nodeIndex];
            nodes.Remove(nodeIndex);
            if (currentNode == node) {
                currentNode = null;
            }
        }

        public int AddTranslation(int fromIndex, int toIndex, Func<bool> condition) {
            var from = nodes[fromIndex];
            var to = nodes[toIndex];
            var translation = new FSMTranslation(from, to, condition);
            from.AddTranslation(translation);
            translationIndexRecord++;
            translations.Add(translationIndexRecord, translation);
            return translationIndexRecord;
        }

        public void RemoveTranslation(int translationIndex) {
            var translation = translations[translationIndex];
            translations.Remove(translationIndex);
            var from = translation.from;
            from.RemoveTranslation(translation);
        }

        public void SetCurrentNodeManully(int nodeIndex) {
            currentNode = nodes[nodeIndex];
        }

    }

}