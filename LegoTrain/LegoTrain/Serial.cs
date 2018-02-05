using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace LegoTrain
{
    public class Serial
    {
        static private SerialDevice serialPort = null;
        static private DataReader dataReaderObject = null;
        static private DataWriter dataWriteObject = null;
        static private CancellationTokenSource ReadCancellationTokenSource;
        const int timeout = 1000;
        const int timeoutEx = timeout + 500;

        public async Task SelectAndInitSerial(SerialDevice myserial)
        {
            try
            {
                if (myserial == null)
                {
                    string aqs = SerialDevice.GetDeviceSelector();
                    var dis = await DeviceInformation.FindAllAsync(aqs);

                    for (int i = 0; i < dis.Count; i++)
                    {
                        Debug.WriteLine(string.Format("Serial device found: {0}", dis[i].Id));
                        if (dis[i].Id.IndexOf("UART0") != -1)
                        {
                            serialPort = await SerialDevice.FromIdAsync(dis[i].Id);
                        }
                    }
                }
                else
                    serialPort = myserial;
                if (serialPort != null)
                {
                    serialPort.BaudRate = 9600; 
                    //initializing the defaults timeout and other serial data
                    serialPort.WriteTimeout = TimeSpan.FromMilliseconds(timeout);
                    serialPort.ReadTimeout = TimeSpan.FromMilliseconds(timeout);
                    serialPort.Parity = SerialParity.None;
                    serialPort.StopBits = SerialStopBitCount.One;
                    serialPort.DataBits = 8;
                    serialPort.Handshake = SerialHandshake.None;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception initializing serrail port: {0}", ex.Message));
            }
        }

        public async void WriteSerial(byte[] txBuff)
        {
            try
            {
                if (serialPort != null)
                {
                    Task<UInt32> storeAsyncTask;
                    //Launch the WriteAsync task to perform the write
                    if (dataWriteObject == null)
                        dataWriteObject = new DataWriter(serialPort.OutputStream);
                    dataWriteObject.WriteBytes(txBuff);
                    storeAsyncTask = dataWriteObject.StoreAsync().AsTask();
                    UInt32 bytesWritten = await storeAsyncTask;
                    if (bytesWritten > 0)
                    {
                        //Debug.WriteLine(string.Format("Bytes written successfully: {0}", bytesWritten));
                    }
                    else
                    {
                        Debug.WriteLine("Error sending data");
                    }
                }
                else
                {
                    Debug.WriteLine("No serial port initialized");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Eror sending data: {0}", ex.Message));
                // Cleanup once complete
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }

        public async Task<byte[]> ReadSerial()
        {
            // clean token
            if (ReadCancellationTokenSource == null)
                ReadCancellationTokenSource = new CancellationTokenSource();
            ReadCancellationTokenSource.CancelAfter(timeoutEx);
            //read from the serial port
            byte[] rx_buffer = await ReadAsync(timeout, ReadCancellationTokenSource.Token);
            if (ReadCancellationTokenSource != null)
            {
                ReadCancellationTokenSource.Dispose();
                ReadCancellationTokenSource = null;
            }
            // check if data are valids
            return rx_buffer;
        }

        private async Task<byte[]> ReadAsync(int timeout, CancellationToken cancellationToken)
        {
            try
            {
                Task<UInt32> loadAsyncTask;

                uint ReadBufferLength = 1024;

                // If task cancellation was requested, comply
                //cancellationToken.ThrowIfCancellationRequested();
                if (dataReaderObject == null)
                    dataReaderObject = new DataReader(serialPort.InputStream);
                // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
                //dataReaderObject.InputStreamOptions = InputStreamOptions.ReadAhead;
                dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
                // set serial timeout for reading. initialize the timout reading function
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(timeout);
                //await Task.Delay(timeout);
                // Create a task object to wait for data on the serialPort.InputStream
                //loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask();
                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                byte[] retval = null;
                if (bytesRead > 0)
                {
                    retval = new byte[bytesRead];
                    dataReaderObject.ReadBytes(retval); //return the bytes read
                    //Debug.WriteLine(String.Format("Bytes received successfully: {0}", bytesRead));
                }
                else
                    Debug.WriteLine(String.Format("No bytes received successfully"));
                return retval;
            }
            catch (Exception)
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
                Debug.WriteLine("Exception in reading");
                return null;
            }

        }

        public void CloseDevice()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;

        }
    }
}
