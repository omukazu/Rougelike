using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelike
{
    public class Action
    {
        protected ActionPattern pattern;
        protected Coordinates sourceP;

        public ActionPattern P
        {
            get { return pattern; }
        }
        public virtual GameObject S
        {
            get { return null; }
        }
        public virtual Coordinates Sc
        {
            get { return sourceP; }
        }
        public virtual GameObject T
        {
            get { return null; }
        }
        public virtual Coordinates Tc
        {
            get { return sourceP; }
        }
        public virtual string Message
        {
            get { return null; }
        }
    }

    public class Step : Action
    {
        GameObject source;
        Coordinates targetP;

        public Step(ActionPattern pattern, GameObject source, Coordinates sourceP, Coordinates targetP)
        {
            this.pattern = pattern;
            this.source = source;
            this.sourceP = sourceP;
            this.targetP = targetP;
        }

        public override GameObject S
        {
            get { return source; }
        }
        public override Coordinates Sc
        {
            get { return sourceP; }
        }
        public override Coordinates Tc
        {
            get { return targetP; }
        }
    }

    public class Swap : Action
    {
        GameObject source;
        GameObject target;
        Coordinates targetP;

        public Swap(ActionPattern pattern, GameObject source, Coordinates sourceP, GameObject target, Coordinates targetP)
        {
            this.pattern = pattern;
            this.source = source;
            this.sourceP = sourceP;
            this.target = target;
            this.targetP = targetP;
        }

        public override GameObject S
        {
            get { return source; }
        }
        public override Coordinates Sc
        {
            get { return sourceP; }
        }
        public override GameObject T
        {
            get { return target; }
        }
        public override Coordinates Tc
        {
            get { return targetP; }
        }
    }

    public class Attack : Action
    {
        string message;

        public Attack(ActionPattern pattern, Coordinates sourceP, string message)
        {
            this.pattern = pattern;
            this.sourceP = sourceP;
            this.message = message;
        }

        public override Coordinates Sc
        {
            get { return sourceP; }
        }
        public override string Message
        {
            get { return message; }
        }
    }

}
