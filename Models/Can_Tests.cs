using System;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using Iot.Device.SocketCan;

namespace IoTLib_Test.Models
{
    internal class Can_Tests
    {
        private string? canDev;
        private string? bitrate;
        private CanId canIdWrite;
        private CanId canIdRead;

        /* Value to send over CAN */
        private readonly byte[] valueSend = [1, 2, 3, 40, 50, 60, 70, 80];
        private byte[]? valueRead;

        private bool runRwProcesses;
        private bool canRwSuccess = false;
        private readonly int maxRunCount = 10;

        public bool StartCanRWTest(string _canDev, string _bitrate)
        {
            canDev = "can" + _canDev;
            bitrate = _bitrate;
            runRwProcesses = true;
            canRwSuccess = false;
            /* Reset compare values */
            canIdRead = new CanId();
            valueRead = null;

            /* Check if CAN device is up */
            if (!IsCanDevUp())
            {
                /* Activate canDev */
                ActivateCanDev();
                /* Throw exception if canDev couldn't be activated */
                if (!IsCanDevUp())
                {
                    runRwProcesses = false;
                    throw new Exception($"Exception: Could not activate CAN device {canDev}");
                }
            }

            canIdWrite = new CanId()
            {
                Standard = 0x1A
            };

            try
            {
                /* Start single write processes to check if it's working */
                CanWrite(1);
            }
            catch (Exception ex)
            {
                runRwProcesses = false;
                throw new Exception(ex.Message);
            }

            /* Start write thread */
            Thread writeThread = new(() => CanWrite(maxRunCount));
            writeThread.Start();

            int runCount = 0;

            while (runRwProcesses && runCount < maxRunCount)
            {
                /* Let CanRead run as Task, otherwise it won't timeout if it there is nothing to read */
                var readTask = Task.Run(CanRead);
                if (readTask.Wait(TimeSpan.FromSeconds(1)))
                {
                    /* CanRead returns true, if anything was read */
                    if (readTask.Result == true)
                    {
                        /* Compare CAN IDs & valueRead / valueSend */
                        if (canIdRead.Value != canIdWrite.Value && ByteArraysEqual(valueRead!, valueSend))
                        {
                            canRwSuccess = true;
                            runRwProcesses = false;
                        }
                    }
                }
                runCount++;
            }
            runRwProcesses = false;
            return canRwSuccess;
        }

        #region RW_Test
        public void CanWrite(int maxWriteCount)
        {
            using CanRaw can = new(canDev!);
            Span<byte> bytes = new(valueSend);
            int runCount = 0;

            while (runRwProcesses && runCount < maxWriteCount)
            {
                try
                {
                    /* Write data */
                    can.WriteFrame(bytes, canIdWrite);
                    runCount++;
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    runRwProcesses = false;
                    throw new Exception("CAN Write Exception: " + ex.Message);
                }
            }
        }

        public bool CanRead()
        {
            using CanRaw can = new(canDev!);
            /* buffer needs to be the same length as valueSend */
            byte[] buffer = new byte[valueSend.Length];

            try
            {
                if (can.TryReadFrame(buffer, out int frameLength, out canIdRead))
                {
                    Span<byte> bytesRead = new Span<byte>(buffer, 0, frameLength);
                    valueRead = bytesRead.ToArray();
                    return true;
                }
            }
            catch (Exception ex)
            {
                runRwProcesses = false;
                throw new Exception("CAN Read Exception: " + ex.Message);
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
            /* Compare byte arrays, return true if equal */
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
