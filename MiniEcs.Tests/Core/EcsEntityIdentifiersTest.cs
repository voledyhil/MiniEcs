using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniEcs.Core;

namespace MiniEcs.Tests.Core
{
    [TestClass]
    public class EcsEntityIdentifiersTest
    {
        [TestMethod]
        public void GenerateTest()
        {
            const ushort entityId = 5;
            const ushort version = 3;

            uint entity = EcsEntityIdentifiers.Generate(entityId, version);

            Assert.AreEqual(entityId, EcsEntityIdentifiers.GetId(entity));
            Assert.AreEqual(version, EcsEntityIdentifiers.GetVersion(entity));
        }

        [TestMethod]
        public void NextTest()
        {
            EcsEntityIdentifiers identifiers = new EcsEntityIdentifiers();
            
            // allocate new id
            uint entity = identifiers.Next();
            ushort id = EcsEntityIdentifiers.GetId(entity);
            ushort version = EcsEntityIdentifiers.GetVersion(entity);
            
            Assert.AreEqual(1, id);
            Assert.AreEqual(0, version);
            
            // recycle entity
            identifiers.Recycle(entity);
            
            // inc version
            entity = identifiers.Next();
            id = EcsEntityIdentifiers.GetId(entity);
            version = EcsEntityIdentifiers.GetVersion(entity);
            
            Assert.AreEqual(1, id);
            Assert.AreEqual(1, version);
            
            // allocate new id
            entity = identifiers.Next();
            id = EcsEntityIdentifiers.GetId(entity);
            version = EcsEntityIdentifiers.GetVersion(entity);

            Assert.AreEqual(2, id);
            Assert.AreEqual(0, version);

            // enumerate all versions
            for (int i = 0; i < ushort.MaxValue - 1; i++)
            {
                identifiers.Recycle(entity);
                entity = identifiers.Next();
            }
           
            // recycle last entity version entity
            identifiers.Recycle(entity);
            
            // allocate new id
            entity = identifiers.Next();
            id = EcsEntityIdentifiers.GetId(entity);
            version = EcsEntityIdentifiers.GetVersion(entity);
            
            Assert.AreEqual(3, id);
            Assert.AreEqual(0, version);

        }
    }
}