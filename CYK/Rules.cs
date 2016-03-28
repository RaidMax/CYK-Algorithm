using System;

namespace CSC2710_HW4
{  
    abstract class Rule
    {
        public Char Name { get; private set; }
        public Char[] EndPoints { get; protected set; }

        public Rule(Char Name)
        {
            this.Name = Name;
        }
    }

    class CaseOne : Rule
    {
        public CaseOne(Char Name, Char A, Char B) : base(Name)
        {
            EndPoints = new Char[] { A, B };
        }
    }

    class CaseTwo : Rule
    {
        public CaseTwo(Char Name, Char terminalCharacter) : base(Name)
        {
            EndPoints = new Char[] { terminalCharacter };
        }
    }

    class CaseThree : Rule
    {
        public CaseThree(Char Name) : base(Name)
        {
            EndPoints = new Char[] { '*' };   
        }
    }
}
