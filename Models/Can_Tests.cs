using System;
using System.Threading;
using System.Diagnostics;
using Iot.Device.SocketCan;

namespace IoTLib_Test.Models
{
    internal class Can_Tests
    {
        private bool runCanTest;
        private bool testSuccess = false;
        private string? canDev;
        private string? bitrate;

        private CanId canIdSend;
        private CanId canIdReturn;

        /* Value to send over CAN */
        private readonly byte[] valueSend = [1, 2, 3, 40, 50, 60, 70, 80];
        private byte[]? valueRead;

        #region RunTest
        public bool RunCanTest(string _canDev, string _bitrate)
        {
            canDev = "can" + _canDev;
            bitrate = _bitrate;
            runCanTest = true;
            testSuccess = false;
            /* Reset compare values */
            canIdReturn = new CanId();
            valueRead = null;

            /* Check if CAN device is up */
            if (!IsCanDevUp())
            {
                /* Activate canDev */
                ActivateCanDev();
                /* Throw exception if canDev couldn't be activated */
                if (!IsCanDevUp())
                {
                    //TODO: Exception testen
                    throw new Exception($"Could not activate CAN device {canDev}");
                }
            }

            canIdSend = new CanId()
            {
                Standard = 0x1A
            };

            /* Start write processes */
            Thread writeThread = new(CanWrite);
            writeThread.Start();

            int maxTestRuns = 0;

            while (runCanTest)
            {
                // CanRead returns true, if anything was read
                if (CanRead())
                {
                    /* Compare CAN IDs & valueRead / valueSend */
                    if (canIdReturn.Value != canIdSend.Value && ByteArraysEqual(valueRead!, valueSend))
                    {
                        testSuccess = true;
                        runCanTest = false;
                    }
                    /* Automatically end CAN Test */
                    else if (maxTestRuns == 10)
                    {
                        testSuccess = false;
                        runCanTest = false;
                    }
                    maxTestRuns++;
                }
            }
            return testSuccess;
        }
        #endregion
        #region RW_Test
        public void CanWrite()
        {
            using CanRaw can = new(canDev!);
            Span<byte> bytes = new(valueSend);

            while(runCanTest)
            {
                /* Write data */
                can.WriteFrame(bytes, canIdSend);
                Thread.Sleep(1000);
            }
        }

        public bool CanRead()
        {
            using (CanRaw can = new CanRaw(canDev!))
            {
                /* buffer needs to be the same length as valueSend */
                byte[] buffer = new byte[valueSend.Length];

                if (can.TryReadFrame(buffer, out int frameLength, out canIdReturn))
                {
                    Span<byte> bytesRead = new Span<byte>(buffer, 0, frameLength);
                    valueRead = bytesRead.ToArray();
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region ActivateCanDev
        public bool IsCanDevUp()
        {
            /* Check if CAN device is up */
            /* Run shell command */
            string argument = $"-c \"ip link show {canDev} | grep -q 'state UP'\"";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = argument,
            };

            using (Process process = Process.Start(startInfo)!)
            {
                process.WaitForExit();
                /* ExitCode is 0 if canDev is up */
                if (process.ExitCode == 0)
                    return true;
            }
            return false;
        }

        public void ActivateCanDev()
        {
            /* Activate canDev and setup bitrate */
            string argument = $"-c \"ip link set {canDev} up type can bitrate {bitrate} && ifconfig {canDev} up\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = argument,
            };

            using (Process process = Process.Start(startInfo)!)
            {
                process.WaitForExit();
            }
        }
        #endregion
        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}

//TODO: try-catch - Gegenstelle CAN nicht aktiviert führt zu Absturz