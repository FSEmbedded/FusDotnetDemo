using System;
using System.Linq;
using System.Threading;
using Iot.Device.SocketCan;

namespace IoTLib_Test.Models
{
    internal class Can_Tests
    {
        private bool runCanTest;
        private CanId canId;

        public void CanStart()
        {
            runCanTest = true;
            canId = new CanId()
            {
                Standard = 0x1A
            };

            Thread writeThread = new Thread(() => { CanWrite(); });
            writeThread.Start();

            Thread readThread = new Thread(() => { CanRead(); });
            readThread.Start();
        }

        public void CanStop() 
        { 
            runCanTest = false;
        }

        public void CanRead() 
        {
            try
            {
                using (CanRaw can = new CanRaw("can1"))
                {
                    byte[] buffer = new byte[8];
                    // to scope to specific id
                    // can.Filter(id);

                    while (runCanTest)
                    {
                        try
                        {
                            if (can.TryReadFrame(buffer, out int frameLength, out CanId id))
                            {
                                Span<byte> data = new Span<byte>(buffer, 0, frameLength);
                                string type = id.ExtendedFrameFormat ? "EFF" : "SFF";
                                string dataAsHex = string.Join("", data.ToArray().Select((x) => x.ToString("X2")));
                                Console.WriteLine($"Id: 0x{id.Value:X2} [{type}]: {dataAsHex}");
                            }
                            else
                            {
                                Console.WriteLine($"Invalid frame received!");
                            }
                        }
                        catch(Exception ex)
                        {
                            //TODO
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO
            }
        }

        public void CanWrite()
        {
            try
            {
                using CanRaw can = new CanRaw();
                byte[][] buffers = new byte[][]
                {
                new byte[8] { 1, 2, 3, 40, 50, 60, 70, 80 },
                new byte[7] { 1, 2, 3, 40, 50, 60, 70 },
                new byte[0] { },
                new byte[1] { 254 },
                };

                if (!canId.IsValid)
                {
                    // This is more form of the self-test rather than actual part of the sample
                    throw new Exception("Id is invalid");
                }

                while (runCanTest)
                {
                    foreach (byte[] buffer in buffers)
                    {
                        can.WriteFrame(buffer, canId);
                        string dataAsHex = string.Join(string.Empty, buffer.Select((x) => x.ToString("X2")));
                        Console.WriteLine($"Sending: {dataAsHex}");
                        Thread.Sleep(1000);
                    }
                }
            }
            catch(Exception ex)
            {
                //TODO
            }
        }
    }
}