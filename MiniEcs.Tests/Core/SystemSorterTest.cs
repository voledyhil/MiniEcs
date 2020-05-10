using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;
using MiniEcs.Core.Systems;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public class SystemSorterTest
    {        
        [EcsUpdateInGroup(typeof(SystemGroupB))]
        private class SystemGroupA : EcsSystemGroup
        {
        }
        
        private class SystemGroupB : EcsSystemGroup
        {
        }
        
        
        [EcsUpdateInGroup(typeof(SystemGroupA))]
        [EcsUpdateBefore(typeof(SystemC))]
        [EcsUpdateAfter(typeof(SystemD))]
        private class SystemA : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        
        [EcsUpdateInGroup(typeof(SystemGroupA))]
        [EcsUpdateBefore(typeof(SystemA))]
        [EcsUpdateAfter(typeof(SystemG))]
        private class SystemB : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        
        [EcsUpdateInGroup(typeof(SystemGroupA))]
        [EcsUpdateBefore(typeof(SystemC))]
        [EcsUpdateAfter(typeof(SystemC))]
        private class SystemC : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        
        [EcsUpdateInGroup(typeof(SystemGroupA))]
        [EcsUpdateAfter(typeof(SystemB))]
        private class SystemD : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        
        [EcsUpdateInGroup(typeof(SystemGroupA))]
        [EcsUpdateBefore(typeof(SystemC))]
        [EcsUpdateAfter(typeof(SystemA))]
        private class SystemE : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }

        
        [EcsUpdateInGroup(typeof(SystemGroupA))]
        [EcsUpdateBefore(typeof(SystemB))]
        [EcsUpdateBefore(typeof(SystemG))]
        private class SystemF : IEcsSystem
        {
            public void Update(float deltaTime, EcsWorld world)
            {
            }
        }
        

        [EcsUpdateInGroup(typeof(SystemGroupB))]
        [EcsUpdateBefore(typeof(SystemGroupA))]
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
            SystemG systemG = new SystemG();
            
            EcsSystemGroup root = new EcsSystemGroup();
            root.AddSystem(systemE);
            root.AddSystem(systemA);
            root.AddSystem(systemD);
            root.AddSystem(systemF);
            root.AddSystem(systemC);
            root.AddSystem(systemB);
            root.AddSystem(systemG);
            
            root.Update(0, new EcsWorld(0));

            List<IEcsSystem> systems = new List<IEcsSystem>(root.Systems);
            Assert.AreEqual(1, systems.Count);
            
            systems = new List<IEcsSystem>(((EcsSystemGroup)systems[0]).Systems);
            Assert.AreEqual(2, systems.Count);
            Assert.AreEqual(systemG, systems[0]);
     
            systems = new List<IEcsSystem>(((EcsSystemGroup) systems[1]).Systems);
            Assert.AreEqual(systemF, systems[0]);
            Assert.AreEqual(systemB, systems[1]);
            Assert.AreEqual(systemD, systems[2]);
            Assert.AreEqual(systemA, systems[3]);
            Assert.AreEqual(systemE, systems[4]);
            Assert.AreEqual(systemC, systems[5]);

            EcsSystemSorter.Sort(new List<IEcsSystem> {new SystemF(), new SystemG()}); //ExpectedException
        }

    }
}