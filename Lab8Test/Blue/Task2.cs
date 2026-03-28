using System.Reflection;
using System.Text.Json;

namespace Lab8Test.Blue
{
    [TestClass]
    public sealed class Task2
    {
        record InputRow(string Name, string Surname, int[][] Marks);
        record OutputRow(string Name, string Surname, int TotalScore);

        private InputRow[] _input;
        private OutputRow[] _output;
        private OutputRow[] _outputSorted;

        private Lab8.Blue.Task2.Participant[] _participants;
        private Lab8.Blue.Task2.WaterJump[] _tournaments;

        [TestInitialize]
        public void LoadData()
        {
            var folder = Directory.GetParent(Directory.GetCurrentDirectory())
                .Parent.Parent.Parent.FullName;

            folder = Path.Combine(folder, "Lab8Test", "Blue");

            var input = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "input.json")))!;

            var output = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "output.json")))!;

            _input = input.GetProperty("Task2").Deserialize<InputRow[]>()!;
            _output = output.GetProperty("Task2").GetProperty("Base").Deserialize<OutputRow[]>()!;
            _outputSorted = output.GetProperty("Task2").GetProperty("Sorted").Deserialize<OutputRow[]>()!;

            _participants = new Lab8.Blue.Task2.Participant[_input.Length];
            _tournaments = new Lab8.Blue.Task2.WaterJump[2];
        }

        [TestMethod]
        public void Test_00_OOP()
        {
            var pType = typeof(Lab8.Blue.Task2.Participant);
            var baseType = typeof(Lab8.Blue.Task2.WaterJump);
            var t3 = typeof(Lab8.Blue.Task2.WaterJump3m);
            var t5 = typeof(Lab8.Blue.Task2.WaterJump5m);

            Assert.IsTrue(pType.IsValueType);
            Assert.AreEqual(0, pType.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);

            Assert.IsTrue(pType.GetProperty("Name")?.CanRead ?? false);
            Assert.IsTrue(pType.GetProperty("Surname")?.CanRead ?? false);
            Assert.IsTrue(pType.GetProperty("Marks")?.CanRead ?? false);
            Assert.IsTrue(pType.GetProperty("TotalScore")?.CanRead ?? false);

            Assert.IsNotNull(pType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(string), typeof(string) },
                null));

            Assert.IsNotNull(pType.GetMethod("Jump"));
            Assert.IsNotNull(pType.GetMethod("Sort"));
            Assert.IsNotNull(pType.GetMethod("Print"));

            Assert.AreEqual(0, pType.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(4, pType.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, pType.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(10, pType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length);

            Assert.IsTrue(baseType.IsClass);
            Assert.IsTrue(baseType.IsAbstract);

            Assert.IsTrue(t3.IsSubclassOf(baseType));
            Assert.IsTrue(t5.IsSubclassOf(baseType));

            Assert.IsTrue(baseType.GetProperty("Name")?.CanRead ?? false);
            Assert.IsTrue(baseType.GetProperty("Bank")?.CanRead ?? false);
            Assert.IsTrue(baseType.GetProperty("Participants")?.CanRead ?? false);

            var prizeProp = baseType.GetProperty("Prize");
            Assert.IsNotNull(prizeProp);
            Assert.IsTrue(prizeProp.GetMethod.IsAbstract);

            Assert.IsNotNull(baseType.GetMethod("Add", new[] { pType }));
            Assert.IsNotNull(baseType.GetMethod("Add", new[] { pType.MakeArrayType() }));

            Assert.AreEqual(0, baseType.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(4, baseType.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(0, baseType.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(10, baseType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length);

            Assert.AreEqual(0, t3.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(4, t3.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, t3.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(10, t3.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length);

            Assert.AreEqual(0, t5.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(4, t5.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, t5.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(10, t5.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length);
        }

        [TestMethod]
        public void Test_01_CreateParticipants()
        {
            InitParticipants();

            for (int i = 0; i < _participants.Length; i++)
            {
                Assert.AreEqual(_input[i].Name, _participants[i].Name);
                Assert.AreEqual(_input[i].Surname, _participants[i].Surname);
                Assert.AreEqual(0, _participants[i].TotalScore);
            }
        }

        [TestMethod]
        public void Test_02_Jumps()
        {
            InitParticipants();
            Jump();

            for (int i = 0; i < _participants.Length; i++)
            {
                Assert.AreEqual(_output[i].TotalScore, _participants[i].TotalScore);
            }
        }

        [TestMethod]
        public void Test_03_SortParticipants()
        {
            InitParticipants();
            Jump();

            Lab8.Blue.Task2.Participant.Sort(_participants);

            for (int i = 0; i < _participants.Length; i++)
            {
                Assert.AreEqual(_outputSorted[i].TotalScore, _participants[i].TotalScore);
            }
        }

        [TestMethod]
        public void Test_04_AddParticipants()
        {
            InitParticipants();

            InitTournaments();

            _tournaments[0].Add(_participants[0]);
            _tournaments[0].Add(_participants.Skip(1).ToArray());

            Assert.AreEqual(_participants.Length,
                _tournaments[0].Participants.Length);
        }

        [TestMethod]
        public void Test_05_Prize3m()
        {
            InitParticipants();
            Jump();
            InitTournaments();

            foreach (var p in _participants)
                _tournaments[0].Add(p);

            var prize = _tournaments[0].Prize;

            if (_participants.Length < 3)
                Assert.IsNull(prize);
            else
            {
                Assert.AreEqual(3, prize.Length);
                Assert.AreEqual(500, prize[0], 0.0001);
                Assert.AreEqual(300, prize[1], 0.0001);
                Assert.AreEqual(200, prize[2], 0.0001);
            }
        }

        [TestMethod]
        public void Test_06_Prize5m()
        {
            InitParticipants();
            Jump();
            InitTournaments();

            foreach (var p in _participants)
                _tournaments[1].Add(p);

            var prize = _tournaments[1].Prize;

            if (_participants.Length < 3)
            {
                Assert.IsNull(prize);
                return;
            }

            Assert.IsNotNull(prize);
            Assert.IsTrue(prize.Length == _participants.Length / 2);

            Assert.AreEqual(440, prize[0], 0.0001);
            Assert.AreEqual(290, prize[1], 0.0001);
            Assert.AreEqual(190, prize[2], 0.0001);
            Assert.AreEqual(40, prize[3], 0.0001);
            Assert.AreEqual(40, prize[4], 0.0001);
        }

        private void InitParticipants()
        {
            for (int i = 0; i < _input.Length; i++)
            {
                _participants[i] =
                    new Lab8.Blue.Task2.Participant(
                        _input[i].Name,
                        _input[i].Surname);
            }
        }

        private void Jump()
        {
            for (int i = 0; i < _participants.Length; i++)
            {
                foreach (var jump in _input[i].Marks)
                    _participants[i].Jump(jump);
            }
        }

        private void InitTournaments()
        {
            _tournaments[0] =
                new Lab8.Blue.Task2.WaterJump3m("A", 1000);

            _tournaments[1] =
                new Lab8.Blue.Task2.WaterJump5m("B", 1000);
        }
    }
}
