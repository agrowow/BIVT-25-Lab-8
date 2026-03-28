using System.Reflection;
using System.Text.Json;

namespace Lab8Test.Blue
{
    [TestClass]
    public sealed class Task1
    {
        record InputRow(string Name, string Surname);
        record OutputRow(string Name, string Surname, int Votes);

        private InputRow[] _input;
        private OutputRow[] _output;
        private Lab8.Blue.Task1.Response[] _responses;

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

            _input = input.GetProperty("Task1").Deserialize<InputRow[]>()!;
            _output = output.GetProperty("Task1").Deserialize<OutputRow[]>()!;

            _responses = new Lab8.Blue.Task1.Response[_input.Length];
        }

        [TestMethod]
        public void Test_00_OOP()
        {
            var baseType = typeof(Lab8.Blue.Task1.Response);
            var humanType = typeof(Lab8.Blue.Task1.HumanResponse);

            Assert.IsTrue(baseType.IsClass);
            Assert.IsTrue(humanType.IsSubclassOf(baseType));

            Assert.AreEqual(0,
                baseType.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);

            Assert.AreEqual(0,
                humanType.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);

            Assert.IsTrue(baseType.GetProperty("Name")?.CanRead ?? false);
            Assert.IsTrue(baseType.GetProperty("Votes")?.CanRead ?? false);

            Assert.IsFalse(baseType.GetProperty("Name")?.CanWrite ?? true);
            Assert.IsFalse(baseType.GetProperty("Votes")?.CanWrite ?? true);

            Assert.IsTrue(humanType.GetProperty("Surname")?.CanRead ?? false);
            Assert.IsFalse(humanType.GetProperty("Surname")?.CanWrite ?? true);

            Assert.IsNotNull(baseType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(string) },
                null));

            Assert.IsNotNull(humanType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(string), typeof(string) },
                null));

            var countVotes = baseType.GetMethod("CountVotes",
                BindingFlags.Instance | BindingFlags.Public);

            var print = baseType.GetMethod("Print",
                BindingFlags.Instance | BindingFlags.Public);

            Assert.IsNotNull(countVotes);
            Assert.IsNotNull(print);

            Assert.IsTrue(countVotes.IsVirtual);
            Assert.IsTrue(print.IsVirtual);

            var overrideMethod = humanType.GetMethod("CountVotes");
            Assert.AreNotEqual(
                overrideMethod.GetBaseDefinition().DeclaringType,
                overrideMethod.DeclaringType);

            overrideMethod = humanType.GetMethod("Print");
            Assert.AreNotEqual(
                overrideMethod.GetBaseDefinition().DeclaringType,
                overrideMethod.DeclaringType);

            Assert.AreEqual(0, baseType.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(2, baseType.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, baseType.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(8, baseType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length);

            Assert.AreEqual(0, humanType.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(3, humanType.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, humanType.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(9, humanType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length);
        }

        [TestMethod]
        public void Test_01_Create()
        {
            Init();
            Check(initial: true);
        }

        [TestMethod]
        public void Test_02_CountVotes()
        {
            Init();
            Run();
            Check(initial: false);
        }

        [TestMethod]
        public void Test_03_CountVotesHalf()
        {
            Init();

            for (int i = 0; i < _responses.Length - 1; i++)
                _responses[i].CountVotes(_responses);

            for (int i = 0; i < _responses.Length; i++)
                Assert.AreEqual(_output[i].Votes, _responses[i].Votes);
        }

        private void Init()
        {
            for (int i = 0; i < _input.Length; i++)
            {
                _responses[i] =
                    new Lab8.Blue.Task1.HumanResponse(
                        _input[i].Name,
                        _input[i].Surname);
            }
        }

        private void Run()
        {
            for (int i = 0; i < _responses.Length; i++)
                _responses[i].CountVotes(_responses);
        }

        private void Check(bool initial)
        {
            for (int i = 0; i < _responses.Length; i++)
            {
                var human = (Lab8.Blue.Task1.HumanResponse)_responses[i];

                Assert.AreEqual(_output[i].Name, human.Name);
                Assert.AreEqual(_output[i].Surname, human.Surname);

                if (initial)
                {
                    Assert.AreEqual(0, human.Votes);
                }
                else
                {
                    Assert.AreEqual(_output[i].Votes, human.Votes);
                }
            }
        }
    }
}
