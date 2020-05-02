using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public class SystemSorterTest
    {
        [EcsUpdateBefore(typeof(SystemC))]
        [EcsUpdateAfter(typeof(SystemD))]
        private class SystemA : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        [EcsUpdateBefore(typeof(SystemA))]
        [EcsUpdateAfter(typeof(SystemG))]
        private class SystemB : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        [EcsUpdateBefore(typeof(SystemC))]
        [EcsUpdateAfter(typeof(SystemC))]
        private class SystemC : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        [EcsUpdateAfter(typeof(SystemB))]
        private class SystemD : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        [EcsUpdateBefore(typeof(SystemC))]
        [EcsUpdateAfter(typeof(SystemA))]
        private class SystemE : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        [EcsUpdateBefore(typeof(SystemB))]
        [EcsUpdateBefore(typeof(SystemG))]
        private class SystemF : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        [EcsUpdateBefore(typeof(SystemF))]
        private class SystemG : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(SystemSorterException))]
        public void SorterTest()
        {
            SystemA systemA = new SystemA();
            SystemB systemB = new SystemB();
            SystemC systemC = new SystemC();
            SystemD systemD = new SystemD();
            SystemE systemE = new SystemE();
            SystemF systemF = new SystemF();
            List<IEcsSystem> systems = new List<IEcsSystem> {systemE, systemA, systemD, systemF, systemC, systemB};

            SystemSorter.Sort(systems);

            Assert.AreEqual(6, systems.Count);

            Assert.AreEqual(systemF, systems[0]);
            Assert.AreEqual(systemB, systems[1]);
            Assert.AreEqual(systemD, systems[2]);
            Assert.AreEqual(systemA, systems[3]);
            Assert.AreEqual(systemE, systems[4]);
            Assert.AreEqual(systemC, systems[5]);

            SystemSorter.Sort(new List<IEcsSystem> {new SystemF(), new SystemG()}); //ExpectedException
        }

    }
}