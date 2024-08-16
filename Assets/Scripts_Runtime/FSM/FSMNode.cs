using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MortiseFrame.Strategist {

    public class FSMNode {

        List<FSMTranslation> translations;

        Action onEnter;
        public Action OnENter => onEnter;
        bool isEntering;

        Action onExit;
        public Action OnExit => onExit;
        bool isExiting;

        Action<float> onTick;

        public FSMNode(Action onEnter, Action onExit, Action<float> onTick) {
            this.onEnter = onEnter;
            this.onExit = onExit;
            this.onTick = onTick;
        }

        public void Enter() {
            isEntering = true;
        }

        public void AddTranslation(FSMTranslation translation) {
            if (translations == null) {
                translations = new List<FSMTranslation>();
            }
            translations.Add(translation);
        }

        public void RemoveTranslation(FSMTranslation translation) {
            translations.Remove(translation);
        }

        public void Tick(float dt) {
            if (isEntering) {
                isEntering = false;
                onEnter?.Invoke();
            }

            onTick?.Invoke(dt);

            foreach (var translation in translations) {
                if (translation.condition()) {
                    onExit?.Invoke();
                    translation.to.Enter();
                    return;
                }
            }
        }

    }

}