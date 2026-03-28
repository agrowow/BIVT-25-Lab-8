using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab8Test.Blue
{
    [TestClass]
    public sealed class Task3
    {
        record InputRow(string Name, string Surname, int[] Penalties);
        record OutputRow(string Name, string Surname, int Total, bool IsExpelled);

        private InputRow[] _input;
        private OutputRow[] _output;
        private Lab8.Blue.Task3.Participant[] _participants;

        [TestInitialize]
        public void LoadData()
        {
            var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            folder = Path.Combine(folder, "Lab8Test", "Blue");

            var input = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "input.json")))!;
            var output = JsonSerializer.Deserialize<JsonElement>(
                File.ReadAllText(Path.Combine(folder, "output.json")))!;

            _input = input.GetProperty("Task3").Deserialize<InputRow[]>()!;
            _output = output.GetProperty("Task3").Deserialize<OutputRow[]>()!;
            _participants = new Lab8.Blue.Task3.Participant[_input.Length];

            // Reset HockeyPlayer static fields
            var hpType = typeof(Lab8.Blue.Task3.HockeyPlayer);
            var sumFi = hpType.GetField("_summaryPenalties", BindingFlags.Static | BindingFlags.NonPublic);
            sumFi?.SetValue(null, 0);
            var playFi = hpType.GetField("_players", BindingFlags.Static | BindingFlags.NonPublic);
            playFi?.SetValue(null, 0);
        }

        [TestMethod]
        public void Test_00_OOP()
        {
            var type = typeof(Lab8.Blue.Task3.Participant);
            Assert.IsFalse(type.IsValueType, "Participant должен быть классом");
            Assert.IsTrue(type.IsClass);
            Assert.AreEqual(0, type.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);

            Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false, "Нет свойства Name");
            Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false, "Нет свойства Surname");
            Assert.IsTrue(type.GetProperty("Penalties")?.CanRead ?? false, "Нет свойства Penalties");
            Assert.IsTrue(type.GetProperty("Total")?.CanRead ?? false, "Нет свойства Total");
            Assert.IsTrue(type.GetProperty("IsExpelled")?.CanRead ?? false, "Нет свойства IsExpelled");
            Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? false, "Свойство Name должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? false, "Свойство Surname должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("Penalties")?.CanWrite ?? false, "Свойство Penalties должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("Total")?.CanWrite ?? false, "Свойство Total должно быть только для чтения");
            Assert.IsFalse(type.GetProperty("IsExpelled")?.CanWrite ?? false, "Свойство IsExpelled должно быть только для чтения");

            var isExpProp = type.GetProperty("IsExpelled");
            Assert.IsTrue(isExpProp?.GetGetMethod()?.IsVirtual ?? false, "Свойство IsExpelled должно быть виртуальным");

            Assert.IsNotNull(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string), typeof(string) }, null), "Нет публичного конструктора Participant(string name, string surname)");

            var playMethod = type.GetMethod("PlayMatch", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int) }, null);
            Assert.IsNotNull(playMethod, "Нет публичного метода PlayMatch(int time)");
            Assert.IsTrue(playMethod?.IsVirtual ?? false, "Метод PlayMatch должен быть виртуальным");

            Assert.IsNotNull(type.GetMethod("Sort", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(Lab8.Blue.Task3.Participant[]) }, null), "Нет публичного статического метода Sort(Participant[] array)");
            Assert.IsNotNull(type.GetMethod("Print", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null), "Нет публичного метода Print()");

            Assert.AreEqual(0, type.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(5, type.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, type.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(12, type.GetMethods().Count(f => f.IsPublic));

            // BasketballPlayer
            var typeB = typeof(Lab8.Blue.Task3.BasketballPlayer);
            Assert.IsTrue(typeB.IsClass);
            Assert.AreEqual(type, typeB.BaseType);
            Assert.IsNotNull(typeB.GetConstructor(new[] { typeof(string), typeof(string) }));
            var playMB = typeB.GetMethod("PlayMatch", new[] { typeof(int) });
            Assert.AreEqual(typeB, playMB?.DeclaringType);
            var isExpB = typeB.GetProperty("IsExpelled")?.GetGetMethod();
            Assert.AreEqual(typeB, isExpB?.DeclaringType);
            Assert.AreEqual(0, typeB.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(5, typeB.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, typeB.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(11, typeB.GetMethods().Count(f => f.IsPublic));

            // HockeyPlayer
            var typeH = typeof(Lab8.Blue.Task3.HockeyPlayer);
            Assert.IsTrue(typeH.IsClass);
            Assert.AreEqual(type, typeH.BaseType);
            Assert.IsNotNull(typeH.GetConstructor(new[] { typeof(string), typeof(string) }));
            var playMH = typeH.GetMethod("PlayMatch", new[] { typeof(int) });
            Assert.AreEqual(typeH, playMH?.DeclaringType);
            var isExpH = typeH.GetProperty("IsExpelled")?.GetGetMethod();
            Assert.AreEqual(typeH, isExpH?.DeclaringType);
            Assert.AreEqual(0, typeH.GetFields().Count(f => f.IsPublic));
            Assert.AreEqual(5, typeH.GetProperties().Count(f => f.PropertyType.IsPublic));
            Assert.AreEqual(1, typeH.GetConstructors().Count(f => f.IsPublic));
            Assert.AreEqual(11, typeH.GetMethods().Count(f => f.IsPublic));
        }

        [TestMethod]
        public void Test_01_Create()
        {
            Init();
            Check(matchesExpected: false);
        }

        [TestMethod]
        public void Test_03_PlayMatches()
        {
            Init();
            PlayMatches();
            Check(matchesExpected: true);
        }

        [TestMethod]
        public void Test_04_Sort()
        {
            Init();
            PlayMatches();

            Lab8.Blue.Task3.Participant.Sort(_participants);

            Assert.AreEqual(_output.Length, _participants.Length);
            for (int i = 0; i < _participants.Length; i++)
            {
                Assert.AreEqual(_output[i].Name, _participants[i].Name);
                Assert.AreEqual(_output[i].Surname, _participants[i].Surname);
                Assert.AreEqual(_output[i].Total, _participants[i].Total);
                Assert.AreEqual(_output[i].IsExpelled, _participants[i].IsExpelled);
            }
        }

        [TestMethod]
        public void Test_05_ArrayLinq()
        {
            Init();
            PlayMatches();
            ArrayLinq();
            Check(matchesExpected: true);
        }

        [TestMethod]
        public void Test_06_BaseParticipant()
        {
            var p = new Lab8.Blue.Task3.Participant("Test", "Test");
            CollectionAssert.AreEqual(new int[0], p.Penalties);
            Assert.AreEqual(0, p.Total);
            Assert.IsFalse(p.IsExpelled);

            p.PlayMatch(3);
            p.PlayMatch(10);
            p.PlayMatch(4);
            CollectionAssert.AreEqual(new[] { 3, 10, 4 }, p.Penalties);
            Assert.AreEqual(17, p.Total);
            Assert.IsTrue(p.IsExpelled);
        }

        [TestMethod]
        public void Test_07_BasketballPlayer()
        {
            var p = new Lab8.Blue.Task3.BasketballPlayer("Test", "Test");
            p.PlayMatch(3);
            p.PlayMatch(6);
            p.PlayMatch(-1);
            p.PlayMatch(4);
            p.PlayMatch(5);
            CollectionAssert.AreEqual(new[] { 3, 4, 5 }, p.Penalties);
            Assert.AreEqual(12, p.Total);
            // count5=1, length=3, 1 > 0.3 true; total12 > 6 true
            Assert.IsTrue(p.IsExpelled);

            var p2 = new Lab8.Blue.Task3.BasketballPlayer("Test2", "Test2");
            for (int k = 0; k < 10; k++)
            {
                p2.PlayMatch(1);
            }
            Assert.AreEqual(10, p2.Total);
            Assert.AreEqual(10, p2.Penalties.Length);
            // count5=0, total10 <=20
            Assert.IsFalse(p2.IsExpelled);

            var p3 = new Lab8.Blue.Task3.BasketballPlayer("Test3", "Test3");
            for (int k = 0; k < 9; k++)
            {
                p3.PlayMatch(1);
            }
            p3.PlayMatch(5);
            Assert.AreEqual(14, p3.Total);
            Assert.AreEqual(10, p3.Penalties.Length);
            // count5=1 <=1 (1 >1 false), total14 <=20
            Assert.IsFalse(p3.IsExpelled);

            var p4 = new Lab8.Blue.Task3.BasketballPlayer("Test4", "Test4");
            for (int k = 0; k < 8; k++)
            {
                p4.PlayMatch(1);
            }
            p4.PlayMatch(5);
            p4.PlayMatch(5);
            Assert.AreEqual(18, p4.Total);
            Assert.AreEqual(10, p4.Penalties.Length);
            // count5=2 >1 true
            Assert.IsTrue(p4.IsExpelled);
        }

        [TestMethod]
        public void Test_08_HockeyPlayer()
        {
            // Reset statics
            var hpType = typeof(Lab8.Blue.Task3.HockeyPlayer);
            var sumFi = hpType.GetField("_totalTime", BindingFlags.Static | BindingFlags.NonPublic);
            sumFi?.SetValue(null, 0);
            var playFi = hpType.GetField("_players", BindingFlags.Static | BindingFlags.NonPublic);
            playFi?.SetValue(null, 0);

            var p1 = new Lab8.Blue.Task3.HockeyPlayer("Test1", "Test1");
            p1.PlayMatch(20);
            p1.PlayMatch(30);
            Assert.AreEqual(50, p1.Total);
            Assert.IsTrue(p1.IsExpelled);

            // New set
            sumFi?.SetValue(null, 0);
            playFi?.SetValue(null, 0);

            var p2 = new Lab8.Blue.Task3.HockeyPlayer("Test2", "Test2");
            p2.PlayMatch(1);
            p2.PlayMatch(2);
            Assert.AreEqual(3, p2.Total);

            Assert.IsTrue(p2.IsExpelled);

            var p3 = new Lab8.Blue.Task3.HockeyPlayer("Test3", "Test3");
            p3.PlayMatch(0);

            Assert.IsTrue(p2.IsExpelled);
            Assert.IsFalse(p3.IsExpelled);
        }

        private void Init()
        {
            for (int i = 0; i < _input.Length; i++)
            {
                _participants[i] = new Lab8.Blue.Task3.Participant(_input[i].Name, _input[i].Surname);
            }
        }

        private void PlayMatches()
        {
            for (int i = 0; i < _input.Length; i++)
            {
                foreach (var time in _input[i].Penalties)
                {
                    _participants[i].PlayMatch(time);
                }
            }
        }

        private void ArrayLinq()
        {
            for (int i = 0; i < _participants.Length; i++)
            {
                var times = _participants[i].Penalties;
                if (times == null) continue;
                for (int j = 0; j < times.Length; j++)
                    times[j] = -1;
            }
        }

        private void Check(bool matchesExpected)
        {
            Assert.AreEqual(_input.Length, _participants.Length);

            for (int i = 0; i < _input.Length; i++)
            {
                Assert.AreEqual(_input[i].Name, _participants[i].Name);
                Assert.AreEqual(_input[i].Surname, _participants[i].Surname);

                if (matchesExpected)
                {
                    Assert.IsNotNull(_participants[i].Penalties, "Penalties должны быть инициализированы после PlayMatch");
                    Assert.AreEqual(_input[i].Penalties.Length, _participants[i].Penalties.Length);

                    int sum = 0;
                    bool expelled = false;
                    for (int j = 0; j < _input[i].Penalties.Length; j++)
                    {
                        Assert.AreEqual(_input[i].Penalties[j], _participants[i].Penalties[j]);
                        sum += _input[i].Penalties[j];
                        if (_input[i].Penalties[j] >= 10) expelled = true;
                    }

                    Assert.AreEqual(sum, _participants[i].Total);
                    Assert.AreEqual(expelled, _participants[i].IsExpelled);
                }
                else
                {
                    if (_participants[i].Penalties == null || _participants[i].Penalties.Length == 0)
                    {
                        // ok
                    }
                    else
                    {
                        for (int j = 0; j < _participants[i].Penalties.Length; j++)
                            Assert.AreEqual(0, _participants[i].Penalties[j]);
                    }

                    Assert.AreEqual(0, _participants[i].Total);
                    Assert.AreEqual(false, _participants[i].IsExpelled);
                }
            }
        }
    }
}
