namespace Lab8.Blue
{
    public class Task4
    {
        public abstract class Team
        {
            protected string _name;
            protected int[] _scores;
            protected int _matchCount;

            public string Name => _name;

            public int[] Scores
            {
                get
                {
                    if (_scores == null) return new int[0];
                    int[] copy = new int[_scores.Length];
                    Array.Copy(_scores, copy, _scores.Length);
                    return copy;
                }
            }
            public int TotalScore
            {
                get
                {
                    if (_scores == null) return 0;
                    int sum = 0;
                    for (int i = 0; i < _scores.Length; i++)
                    {
                        sum += _scores[i];
                    }
                    return sum;
                }
            }
            protected Team(string name)
            {
                _name = name;
                _scores = new int[0];
                _matchCount = 0;
            }
            public void PlayMatch(int result)
            {
                if (_scores == null) return;

                Array.Resize(ref _scores, _scores.Length + 1);
                _scores[_scores.Length - 1] = result;
            }
            public void Print()
            {
                Console.WriteLine($"{_name}: {TotalScore} очков");
            }
        }
        public class ManTeam : Team
        {
            public ManTeam(string name) : base(name) { }
        }
        public class WomanTeam : Team
        {
            public WomanTeam(string name) : base(name) { }
        }
        public class Group
        {
            private string _name;
            private ManTeam[] _manTeams;
            private WomanTeam[] _womanTeams;
            private int _manTeamCount;
            private int _womanTeamCount;

            public string Name => _name;

            public ManTeam[] ManTeams
            {
                get
                {
                    ManTeam[] result = new ManTeam[12];
                    if (_manTeams != null)
                    {
                        for (int i = 0; i < _manTeamCount && i < 12; i++)
                        {
                            result[i] = _manTeams[i];
                        }
                    }
                    return result;
                }
            }
            public WomanTeam[] WomanTeams
            {
                get
                {
                    WomanTeam[] result = new WomanTeam[12];
                    if (_womanTeams != null)
                    {
                        for (int i = 0; i < _womanTeamCount && i < 12; i++)
                        {
                            result[i] = _womanTeams[i];
                        }
                    }
                    return result;
                }
            }
            public Group(string name)
            {
                _name = name;
                _manTeams = new ManTeam[12];
                _womanTeams = new WomanTeam[12];
                _manTeamCount = 0;
                _womanTeamCount = 0;
            }
            public void Add(Team team)
            {
                if (team == null) return;

                if (team is ManTeam manTeam)
                {
                    if (_manTeamCount < 12)
                    {
                        _manTeams[_manTeamCount] = manTeam;
                        _manTeamCount++;
                    }
                }
                else if (team is WomanTeam womanTeam)
                {
                    if (_womanTeamCount < 12)
                    {
                        _womanTeams[_womanTeamCount] = womanTeam;
                        _womanTeamCount++;
                    }
                }
            }
            public void Add(Team[] teams)
            {
                if (teams == null) return;

                foreach (Team team in teams)
                {
                    Add(team);
                }
            }
            private static void SortTeams<T>(T[] teams, int count) where T : Team
            {
                if (teams == null || count <= 1) return;
                for (int i = 0; i < count - 1; i++)
                {
                    for (int j = i + 1; j < count; j++)
                    {
                        if (teams[i].TotalScore < teams[j].TotalScore)
                        {
                            T temp = teams[i];
                            teams[i] = teams[j];
                            teams[j] = temp;
                        }
                    }
                }
            }
            public void Sort()
            {
                SortTeams(_manTeams, _manTeamCount);
                SortTeams(_womanTeams, _womanTeamCount);
            }
            private static T[] MergeTeams<T>(T[] teams1, int count1, T[] teams2, int count2, int size) where T : Team
            {
                int teamsFromEach = size / 2;
                T[] candidates = new T[teamsFromEach * 2];
                int candidateCount = 0;
                for (int i = 0; i < teamsFromEach && i < count1; i++)
                {
                    if (teams1[i] != null)
                    {
                        candidates[candidateCount] = teams1[i];
                        candidateCount++;
                    }
                }
                for (int i = 0; i < teamsFromEach && i < count2; i++)
                {
                    if (teams2[i] != null)
                    {
                        candidates[candidateCount] = teams2[i];
                        candidateCount++;
                    }
                }
                for (int i = 0; i < candidateCount - 1; i++)
                {
                    for (int j = i + 1; j < candidateCount; j++)
                    {
                        if (candidates[i].TotalScore < candidates[j].TotalScore)
                        {
                            T temp = candidates[i];
                            candidates[i] = candidates[j];
                            candidates[j] = temp;
                        }
                    }
                }

                return candidates;
            }
            public static Group Merge(Group group1, Group group2, int size)
            {
                Group result = new Group("Финалисты");
                group1.Sort();
                group2.Sort();
                ManTeam[] mergedManTeams = MergeTeams(
                    group1._manTeams, group1._manTeamCount,
                    group2._manTeams, group2._manTeamCount,
                    size);
                WomanTeam[] mergedWomanTeams = MergeTeams(
                    group1._womanTeams, group1._womanTeamCount,
                    group2._womanTeams, group2._womanTeamCount,
                    size);
                foreach (ManTeam team in mergedManTeams)
                {
                    if (team != null)
                        result.Add(team);
                }
                foreach (WomanTeam team in mergedWomanTeams)
                {
                    if (team != null)
                        result.Add(team);
                }
                return result;
            }
            public void Print()
            {
                Console.WriteLine($"Группа {_name}:");
                Console.WriteLine("Мужские команды:");
                for (int i = 0; i < _manTeamCount; i++)
                {
                    if (_manTeams[i] != null)
                    {
                        _manTeams[i].Print();
                    }
                }
                Console.WriteLine("Женские команды:");
                for (int i = 0; i < _womanTeamCount; i++)
                {
                    if (_womanTeams[i] != null)
                    {
                        _womanTeams[i].Print();
                    }
                }
            }
        }
    }
}
