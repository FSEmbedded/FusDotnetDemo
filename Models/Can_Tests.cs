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
        private byte[] valueSend = [];
        private byte[] valueRead = [];

        private bool runRwProcesses;
        private readonly int maxRunCount = 10; // counter for repeating Read task

        public (byte[], CanId) StartCanRWTest(string _canDev, string _bitrate, CanId _canIdWrite, byte[] _valueSend)
        {
            canDev = "can" + _canDev;
            bitrate = _bitrate;
            canIdWrite = _canIdWrite;
            valueSend = _valueSend;
            runRwProcesses = true;
            int runCount = 0;
            /* Reset canIdRead values */
            canIdRead = new CanId();
            valueRead = [];
            
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

            try
            {
                /* Start single write processes to check if receiving device is working */
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
                        if (canIdRead.Value != canIdWrite.Value)
                        {
                            runRwProcesses = false;
                        }
                    }
                }
                runCount++;
            }
            runRwProcesses = false;
            return (valueRead, canIdRead);
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
                    Span<byte> bytesRead = new(buffer, 0, frameLength);
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
            ProcessStartInfo startInfo = new()
            {
                FileName = "/bin/bash",
                Arguments = argument,
            };

            using Process process = Process.Start(startInfo)!;
            process.WaitForExit();
            /* ExitCode is 0 if canDev is up */
            if (process.ExitCode == 0)
            {
                return true;
            }
            return false;
        }

        public void ActivateCanDev()
        {
            /* Activate canDev and setup bitrate */
            string argument = $"-c \"ip link set {canDev} up type can bitrate {bitrate} && ifconfig {canDev} up\"";

            ProcessStartInfo startInfo = new()
            {
                FileName = "/bin/bash",
                Arguments = argument,
            };

            using Process process = Process.Start(startInfo)!;
            process.WaitForExit();
        }
        #endregion
    }
}
