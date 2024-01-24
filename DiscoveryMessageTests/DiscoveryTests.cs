using nanoDiscovery;
using System.Net;

namespace DiscoveryMessageTests
{
    [TestClass]
    public class DiscoveryTests
    {
        [TestMethod]
        public void TestEncodeDiscoveryMessage()
        {
            // Arrange
            byte[] messageOK = new byte[] { (byte)'n', (byte)'D', (byte)'C', 1, (byte)DiscoveryMessageType.Discovery };

            // Act
            var message = DiscoveryMessage.CreateMessage(DiscoveryMessageType.Discovery, 0, null!, null!);

            // Assert
            CollectionAssert.AreEqual(messageOK, message);
        } 
        
        [TestMethod]
        public void TestDecodeDiscoveryMessage()
        {
            // Arrange            
            byte[] messageOK = new byte[] { (byte)'n', (byte)'D', (byte)'C', 1, (byte)DiscoveryMessageType.Discovery };
            DiscoveryMessageType messageType;
            sbyte id;
            IPAddress ip;
            byte[] payload;

            // Act
            bool res = DiscoveryMessage.DecodeMessage(messageOK, out messageType, out id, out ip, out payload);

            // Assert
            Assert.IsTrue(res);
            Assert.AreEqual(messageType, DiscoveryMessageType.Discovery);
        }

        [TestMethod]
        public void TestEncodeNormalMessage()
        {
            // Arrange
            var ipBytes = IPAddress.Parse("192.168.1.42").GetAddressBytes();
            byte[] messageOK = new byte[] { (byte)'n', (byte)'D', (byte)'C', 1, (byte)DiscoveryMessageType.Capabilities, 128, ipBytes[0], ipBytes[1], ipBytes[2], ipBytes[3], 1, 2, 3, 4 };

            // Act
            var message = DiscoveryMessage.CreateMessage(DiscoveryMessageType.Capabilities, -128, IPAddress.Parse("192.168.1.42"), new byte[] {1, 2, 3 ,4});

            // Assert
            CollectionAssert.AreEqual(messageOK, message);
        }


        [TestMethod]
        public void TestDecodeNormalMessage()
        {
            // Arrange
            var ipBytes = IPAddress.Parse("192.168.1.42").GetAddressBytes();
            byte[] messageOK = new byte[] { (byte)'n', (byte)'D', (byte)'C', 1, (byte)DiscoveryMessageType.Capabilities, 128, ipBytes[0], ipBytes[1], ipBytes[2], ipBytes[3], 1, 2, 3, 4 };

            DiscoveryMessageType messageType;
            sbyte id;
            IPAddress ip;
            byte[] payload;

            // Act
            bool res = DiscoveryMessage.DecodeMessage(messageOK, out messageType, out id, out ip, out payload);

            // Assert
            Assert.IsTrue(res);
            Assert.AreEqual(messageType, DiscoveryMessageType.Capabilities);
            Assert.AreEqual(-128, id);
            CollectionAssert.AreEqual(ipBytes, ip.GetAddressBytes());
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3,4}, payload);
        }
    }
}