// Licensed to the Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.

using System.Net;

namespace nanoDiscovery
{
    public class DiscoveryMessage
    {
        public static readonly byte[] Header = new byte[] { (byte)'n', (byte)'D', (byte)'C' };
        public const int Version = 1;

        /// <summary>
        /// Creates a message using the protocol eelemnts.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="id">The ID of the element.</param>
        /// <param name="ipAddress">The IP address of the sender.</param>
        /// <param name="payload">An additional payload that is application specific.</param>
        /// <returns>The message to send.</returns>
        public static byte[] CreateMessage(DiscoveryMessageType messageType, sbyte id, IPAddress ipAddress, byte[] payload)
        {
            // Message looks like in bytes: n D C Version MessageType ID IP1 IP2 IP3 IP4 payload_bytes 
            int inc = 0;
            byte[] ret;
            byte[] ip = new byte[0];
            if (messageType == DiscoveryMessageType.Discovery)
            {
                ret = new byte[Header.Length + 1 + 1];
            }
            else
            {
                ip = ipAddress.GetAddressBytes();
                ret = new byte[Header.Length + 1 + 1 + 1 + ip.Length + (payload != null ? payload.Length : 0)];
            }

            Header.CopyTo(ret, 0);
            inc += Header.Length;
            ret[inc++] = Version;
            ret[inc++] = (byte)messageType;
            if (messageType != DiscoveryMessageType.Discovery)
            {
                ret[inc++] = (byte)id;
                ip.CopyTo(ret, inc);
                inc += ip.Length;
                if (payload != null)
                {
                    for (int i = 0; i < payload.Length; i++)
                    {
                        ret[inc++] = payload[i];
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Decodes a message with the protocol elements.
        /// </summary>
        /// <param name="message">The full message.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="id">The ID of the element.</param>
        /// <param name="ipAddress">The IP address of the sender.</param>
        /// <param name="payload">An additional payload that is application specific.</param>
        /// <returns>True if the message can be sucessfully decoded, false otherwise.</returns>
        public static bool DecodeMessage(byte[] message, out DiscoveryMessageType messageType, out sbyte id, out IPAddress ipAddress, out byte[] payload)
        {
            messageType = DiscoveryMessageType.None;
            id = -1;
            ipAddress = IPAddress.Any;
            payload = null;
            // Message looks like in bytes: n D C Version MessageType ID IP1 IP2 IP3 IP4 payload_bytes
            int inc = 0;
            // Check we have a minimum size of 5
            if (message.Length < 5)
            {
                return false;
            }

            for (int i = 0; i < Header.Length; i++)
            {
                if (message[i] != Header[i])
                {
                    return false;
                }

                inc++;
            }

            if (message[inc++] != Version)
            {
                return false;
            }

            messageType = (DiscoveryMessageType)message[inc++];

            if (messageType != DiscoveryMessageType.Discovery)
            {
                // Wee need at least ID + IP length
                var ipLength = ipAddress.GetAddressBytes().Length;
                if (message.Length < 6 + ipLength)
                {
                    return false;
                }

                id = (sbyte)message[inc++];
                var ipAddressBytes = new byte[ipLength];
                for (int i = 0; i < ipLength; i++)
                {
                    ipAddressBytes[i] = message[inc++];
                }

                ipAddress = new IPAddress(ipAddressBytes);

                // If we have anything more, then it's the payload
                payload = new byte[message.Length - 6 - ipLength];
                for (int i = 0; i < payload.Length; i++)
                {
                    payload[i] = message[inc++];
                }
            }

            return true;
        }
    }
}
