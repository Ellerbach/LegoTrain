using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IoTCoreHelpers;
using System.Diagnostics;
using Microsoft.Azure.Devices.Client;
using LegoTrain.Models;
using Newtonsoft.Json;

namespace LegoTrain
{
    partial class TrainManagement
    {
        private const string strFileIoT = "iot.config";
        static private string strconn = "";
        //constant for the commands
        const string TrainCmd = "train";
        const string SignalCmd = "signal";
        const string SwitchCmd = "switch";
        const string GetStausCmd = "getstatus";
        const string GetTrainStatus = "gettrainstatus";
        const string GetSwitchStatus = "getswitchstatus";
        const string GetSignalStatus = "getsignalstatus";
        static public async Task InitIoTHub()
        {
            try
            {
                var fileToRead = new FileStream(await Helpers.GetFilePathAsync(strFileIoT), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                long fileLength = fileToRead.Length;

                byte[] buf = new byte[fileLength];
                //string mySetupString = "";

                // Reads the data.
                fileToRead.Read(buf, 0, (int)fileLength);
                //await str.ReadAsync(buf,  )
                // convert the read into a string

                strconn = new string(Encoding.UTF8.GetChars(buf));

                MessageIoT cmdmsg = new MessageIoT();
                cmdmsg.command = GetStausCmd;
                cmdmsg.message = "ok"; //JsonConvert.SerializeObject(myParamRail);
                SendDataToAzure(JsonConvert.SerializeObject(cmdmsg));
                ReceiveDataFromAzure();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing Azure Iot Hub connection string: {ex.Message}");
            }
        }

        static private async Task ReceiveDataFromAzure()
        {
            if (strconn == "")
                return;
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(strconn, TransportType.Http1);

            Message receivedMessage = null;
            string messageData;

            while (true)
            {
                try
                {
                    receivedMessage = await deviceClient.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error receiving from Azure Iot Hub: {ex.Message}");
                }


                if (receivedMessage != null)
                {
                    bool ballOK = true;
                    // {"command":"addprogram","message":"{\"DateTimeStart\":\"2016-06-02T03:04:05+00:00\",\"Duration\":\"00:02:05\",\"SprinklerNumber\":3}"}
                    //MessageIoT temp = new MessageIoT();
                    //temp.command = "test";
                    //temp.message = JsonConvert.SerializeObject(new SprinklerProgram(new DateTimeOffset(2016, 6, 2, 3, 4, 5, new TimeSpan(0, 0, 0)), new TimeSpan(0, 2, 5), 3));
                    //var ret = JsonConvert.SerializeObject(temp);
                    //SendDataToAzure(ret);
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    MessageIoT cmdmsg = null;
                    try
                    {
                        cmdmsg = JsonConvert.DeserializeObject<MessageIoT>(messageData);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            await deviceClient.RejectAsync(receivedMessage);
                            ballOK = false;
                        }
                        catch (Exception)
                        {
                            ballOK = false;
                        }

                    }
                    if (!ballOK)
                    { }
                    else
                    {

                        if (string.Compare(cmdmsg.command, TrainCmd, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            try
                            {
                                //{"command":"train","message":"{\"TrainNumber\":\"3\",\"Speed\":\"0\"}"}
                                var traincmd = JsonConvert.DeserializeObject<TrainCommand>(cmdmsg.message);
                                if ((traincmd.TrainNumber >= 0) && (traincmd.TrainNumber < myParamRail.NumberOfTrains))
                                {
                                    if (traincmd.Speed == 0)
                                        myLego.SingleOutputPWM(LegoPWM.BRK, (LegoPWMOutput)myParamRail.Trains[traincmd.TrainNumber].RedBlue, (LegoChannel)(myParamRail.Trains[traincmd.TrainNumber].Channel-1));
                                    else if ((traincmd.Speed >= 8) || (traincmd.Speed <= -8))
                                        myLego.SingleOutputPWM(traincmd.Speed > 0 ? LegoPWM.FWD7: LegoPWM.REV7, (LegoPWMOutput)myParamRail.Trains[traincmd.TrainNumber].RedBlue, (LegoChannel)(myParamRail.Trains[traincmd.TrainNumber].Channel-1));
                                    else
                                        myLego.SingleOutputCST(traincmd.Speed > 0 ? LegoCST.INC_PWM : LegoCST.DEC_PWM, (LegoPWMOutput)myParamRail.Trains[traincmd.TrainNumber].RedBlue, (LegoChannel)(myParamRail.Trains[traincmd.TrainNumber].Channel-1));
                                }
                                else
                                    ballOK = false;
                            }
                            catch (Exception)
                            {
                                ballOK = false;
                            }
                        }
                        else if (string.Compare(cmdmsg.command, SignalCmd, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            try
                            {
                                //{"command":"signal","message":"{\"SignaSwitchNumber\":\"1\",\"State\":\"false\"}"}
                                var sgcmd = JsonConvert.DeserializeObject<SignalSwitchCommand>(cmdmsg.message);
                                if ((sgcmd.SignaSwitchNumber >= 0) && (sgcmd.SignaSwitchNumber < mySignal.NumberOfSignals))
                                    mySignal.ChangeSignal(sgcmd.SignaSwitchNumber, sgcmd.State);
                                else
                                    ballOK = false;
                            }
                            catch (Exception)
                            {
                                ballOK = false;
                            }

                        }
                        else if (string.Compare(cmdmsg.command, SwitchCmd, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            try
                            {
                                //{ "command":"switch","message":"{\"SignaSwitchNumber\":\"1\",\"State\":\"false\"}"}
                                var sgcmd = JsonConvert.DeserializeObject<SignalSwitchCommand>(cmdmsg.message);
                                if ((sgcmd.SignaSwitchNumber >= 0) && (sgcmd.SignaSwitchNumber < mySwitch.NumberOfSwitch))
                                    mySwitch.ChangeSwitch(sgcmd.SignaSwitchNumber, sgcmd.State);
                                else
                                    ballOK = false;
                            }
                            catch (Exception)
                            {
                                ballOK = false;
                            }
                        }
                        else if (string.Compare(cmdmsg.command, GetStausCmd, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            try
                            {
                                cmdmsg.message = JsonConvert.SerializeObject(myParamRail);
                                SendDataToAzure(JsonConvert.SerializeObject(cmdmsg));
                            }
                            catch (Exception)
                            {
                                ballOK = false;
                            }
                        }
                        else if (string.Compare(cmdmsg.command, GetSignalStatus, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            try
                            {
                                SignalSwitchCommand[] sig = new SignalSwitchCommand[myParamRail.NumberOfSignals];
                                for (byte i=0; i<sig.Length; i++)
                                {
                                    sig[i].SignaSwitchNumber = i;
                                    sig[i].State = mySignal.GetSignal(i);
                                }
                                cmdmsg.message = JsonConvert.SerializeObject(sig);
                                SendDataToAzure(JsonConvert.SerializeObject(cmdmsg));
                            }
                            catch (Exception)
                            {
                                ballOK = false;
                            }
                        }
                        else if (string.Compare(cmdmsg.command, GetSwitchStatus, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            try
                            {
                                SignalSwitchCommand[] sig = new SignalSwitchCommand[myParamRail.NumberOfSwitchs];
                                for (byte i = 0; i < sig.Length; i++)
                                {
                                    sig[i].SignaSwitchNumber = i;
                                    sig[i].State = mySwitch.GetSwitch(i);
                                }
                                cmdmsg.message = JsonConvert.SerializeObject(sig);
                                SendDataToAzure(JsonConvert.SerializeObject(cmdmsg));
                            }
                            catch (Exception)
                            {
                                ballOK = false;
                            }
                        }
                        else if (string.Compare(cmdmsg.command, GetTrainStatus, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            try
                            {
                                TrainCommand[] tra = new TrainCommand[myParamRail.NumberOfTrains];
                                for(byte i = 0; i<tra.Length;i++)
                                {
                                    tra[i].TrainNumber = i;
                                    //TODO: need to implement capturing train speed
                                    //tra[i].Speed = 
                                } 
                                cmdmsg.message = JsonConvert.SerializeObject(tra);
                                SendDataToAzure(JsonConvert.SerializeObject(cmdmsg));
                            }
                            catch (Exception)
                            {
                                ballOK = false;
                            }
                        }


                    }

                    try
                    {
                        if (ballOK)
                            await deviceClient.CompleteAsync(receivedMessage);
                        else
                            await deviceClient.RejectAsync(receivedMessage);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            await deviceClient.RejectAsync(receivedMessage);
                        }
                        catch (Exception)
                        {

                        }
                        //throw;
                    }

                }
            }
        }

        static private async Task SendDataToAzure(string text)
        {
            if (strconn == "")
                return;
            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(strconn, TransportType.Http1);

                //var text = "{\"info\":\"RPI SerreManagment Working\"}";
                var msg = new Message(Encoding.UTF8.GetBytes(text));

                await deviceClient.SendEventAsync(msg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error posting on Azure Iot Hub: {ex.Message}");
            }

        }

    }
}
