namespace Lab8.Blue
{
    public class Task2
    {
        public class Participant
        {
            private string _name;
            private string _surname;
            private int[,] _marks;
            private int _jumpCounter;

            public string Name => _name;
            public string Surname => _surname;

            public int[,] Marks
            {
                get
                {
                    if (_marks == null) return new int[0, 0];

                    int[,] copy = new int[_marks.GetLength(0), _marks.GetLength(1)];
                    Array.Copy(_marks, copy, _marks.Length);
                    return copy;
                }
            }
            public int TotalScore
            {
                get
                {
                    if (_marks == null) return 0;

                    int sum = 0;
                    foreach (int mark in _marks)
                    {
                        sum += mark;
                    }
                    return sum;
                }
            }
            public Participant(string name, string surname)
            {
                _name = name;
                _surname = surname;
                _marks = new int[2, 5];
                _jumpCounter = 0;
            }
            public void Jump(int[] result)
            {
                if (result == null || result.Length != 5 || _jumpCounter >= 2) return;

                for (int i = 0; i < 5; i++)
                {
                    _marks[_jumpCounter, i] = result[i];
                }
                _jumpCounter++;
            }
            public static void Sort(Participant[] array)
            {
                if (array == null || array.Length <= 1) return;

                for (int i = 0; i < array.Length - 1; i++)
                {
                    for (int j = 0; j < array.Length - 1 - i; j++)
                    {
                        if (array[j].TotalScore < array[j + 1].TotalScore)
                        {
                            Participant temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                        }
                    }
                }
            }
            public void Print()
            {
                Console.Write($"{Name} {Surname} {TotalScore} ");

                if (_marks != null)
                {
                    for (int i = 0; i < _marks.GetLength(0); i++)
                    {
                        for (int j = 0; j < _marks.GetLength(1); j++)
                        {
                            Console.Write($"{_marks[i, j]} ");
                        }
                    }
                }
                Console.WriteLine();
            }
        }
        public abstract class WaterJump
        {
            private string _name;
            private int _bank;
            
            protected List<Participant> _participants;
            
            public string Name => _name;
            public int Bank => _bank;
            public Participant[] Participants => _participants.ToArray();
            
            public abstract double[] Prize { get; }
            
            protected WaterJump(string name, int bank)
            {
                _name = name;
                _bank = bank;
                _participants = new List<Participant>();
            }
            public void Add(Participant participant)
            {
                if (participant != null)
                {
                    _participants.Add(participant);
                }
            }
            public void Add(Participant[] participants)
            {
                if (participants != null)
                {
                    _participants.AddRange(participants);
                }
            }
        }
        public class WaterJump3m : WaterJump
        {
            public WaterJump3m(string name, int bank) : base(name, bank) { }

            public override double[] Prize
            {
                get
                {
                    if (_participants.Count < 3)
                        return new double[0];

                    double[] prizes = new double[3];
                    prizes[0] = Bank * 0.5;
                    prizes[1] = Bank * 0.3;
                    prizes[2] = Bank * 0.2;
                    return prizes;
                }
            }
        }
        public class WaterJump5m : WaterJump
        {
            public WaterJump5m(string name, int bank) : base(name, bank) { }

            public override double[] Prize
            {
                get
                {
                    int participantCount = _participants.Count;
                    if (participantCount < 3)
                        return new double[0];
                    int countAboveMiddle = participantCount / 2;
                    if (countAboveMiddle < 3) countAboveMiddle = 3;
                    if (countAboveMiddle > 10) countAboveMiddle = 10;
                    double n = 20.0 / countAboveMiddle;
                    double[] prizes = new double[participantCount];
                    for (int i = 0; i < countAboveMiddle; i++)
                    {
                        prizes[i] = Bank * (n / 100.0);
                    }
                    if (participantCount >= 1)
                        prizes[0] += Bank * 0.4;
                    if (participantCount >= 2)
                        prizes[1] += Bank * 0.25;
                    if (participantCount >= 3)
                        prizes[2] += Bank * 0.15;
                    return prizes;
                }
            }
        }
    }
}
