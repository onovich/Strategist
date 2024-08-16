using System;

namespace MortiseFrame.Strategist {

    public class FSMTranslation {

        public FSMNode from;
        public FSMNode to;
        public Func<bool> condition;

        public FSMTranslation(FSMNode from, FSMNode to, Func<bool> condition) {
            this.from = from;
            this.to = to;
            this.condition = condition;
        }

    }

}