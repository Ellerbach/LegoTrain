using IoTCoreHelpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

/*
 * Copyright 2015 Laurent Ellerbach (http://blogs.msdn.com/laurelle)
 *
 * Please refer to the details here:
 *
 *     http://opensource.org/licenses/ms-pl
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Please note that most of this code is ported from an old .Net Microframework
 * development. It may be far from what you can do using most recent C# features
 */

namespace LegoTrain
{
    #region enums
    //mode
    public enum LegoMode
    {
        COMBO_DIRECT_MODE = 0x1,
        SINGLE_PIN_CONTINUOUS = 0x2,
        SINGLE_PIN_TIMEOUT = 0x3,
        SINGLE_OUTPUT_PWM = 0x4,
        PWM = 0x05,
        SINGLE_OUTPUT_CST = 0x6,
    };

    //Mode Single Output CST
    public enum LegoCST
    {
        CLEARC1_CLEARC2 = 0x0,
        SETC1_SETC2 = 0x1,
        CLEARC1_SETC2 = 0x2,
        SETC1_CLEARC2 = 0x3,
        INC_PWM = 0x4,
        DEC_PWM = 0x5,
        FULLFWD = 0x6,
        FULLBKD = 0x7,
        TOGGLE_FWD_BKD = 0x8,
    };

    //PWM speed steps
    public enum LegoPWM
    {
        FLT = 0x0,
        FWD1 = 0x1,
        FWD2 = 0x2,
        FWD3 = 0x3,
        FWD4 = 0x4,
        FWD5 = 0x5,
        FWD6 = 0x6,
        FWD7 = 0x7,
        BRK = 0x8,
        REV7 = 0x9,
        REV6 = 0xA,
        REV5 = 0xB,
        REV4 = 0xC,
        REV3 = 0xD,
        REV2 = 0xE,
        REV1 = 0xf
    };

    //speed
    public enum LegoSpeed
    {
        RED_FLT = 0x0,
        RED_FWD = 0x1,
        RED_REV = 0x2,
        RED_BRK = 0x3,
        BLUE_FLT = 0x0,
        BLUE_FWD = 0x4,
        BLUE_REV = 0x8,
        BLUE_BRK = 0xC
    };

    //channel
    public enum LegoChannel
    {
        CH1 = 0x0,
        CH2 = 0x1,
        CH3 = 0x2,
        CH4 = 0x3
    };

    //output
    public enum LegoOutput
    {
        RED_PINC1 = 0x0,
        RED_PINC2 = 0x1,
        BLUE_PINC1 = 0x2,
        BLUE_PINC2 = 0x3
    };
    //LegoPWMOutput
    public enum LegoPWMOutput
    {
        RED = 0x0,
        BLUE = 0x1,
    }

    //Continuous and timeout mode
    public enum LegoFunction
    {
        NO_CHANGE = 0x0,
        CLEAR = 0x1,
        SET = 0x2,
        TOGGLE = 0x3
    }
    #endregion

    public sealed class LegoInfrared
    {
        
        private uint[] toggle = new uint[] { 0, 0, 0, 0 };
        // IR = 6 time 1 and 0 = 101010101010 
        // ZE = 2 time 0 = 00 = 0
        private const ushort _high = 0xFE00; //MOSI outpout high bit always first 10 = ushort
        private const ushort _low = 0x0000;
        private const Int32 SPI_CHIP_SELECT_LINE = 0;       /* Line 0 maps to physical pin number 24 on the Rpi2        */
        private SpiDevice MySerial;
        private GpioController IoController;
        private GpioPin DataCommandPin;
        private const Int32 DATA_COMMAND_PIN = 22;          /* We use GPIO 22 since it's conveniently near the SPI pins */
        //private OutputPort MyInfraredSelect = new OutputPort(Pins.GPIO_PIN_D9,false);
        //private SPI.Configuration SPIConfInfrared;
        private Helpers.Waiting myCounter;
        public LegoInfrared()
        {
            myCounter = new Helpers.Waiting();
            InitPins();
            InitSPI();
        }

        private async void InitSPI()
        {
            try
            {
                //Frequency is 38KHz in the protocol
                float t_carrier = 1 / 38.0f;
                //Reality is that there is a 2us difference in the output as there is always a 2us bit on on SPI using MOSI
                float t_ushort = t_carrier - 2e-3f;
                //Calulate the outpout frenquency. Here = 16/(1/38 -2^-3) = 658KHz
                int freq = (int)(16.0f * 1000 / t_ushort);

                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                settings.ClockFrequency = freq;
                settings.SharingMode = SpiSharingMode.Shared;
                settings.Mode = SpiMode.Mode3;
                string aqs = SpiDevice.GetDeviceSelector();                     /* Get a selector string that will return all SPI controllers on the system */
                var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the SPI bus controller devices with our selector string             */
                if (dis.Count == 0)
                {
                    Debug.WriteLine("SPI does not exist on the current system.");
                    return;
                }
                MySerial = await SpiDevice.FromIdAsync(dis[0].Id, settings);    /* Create an SpiDevice with our bus controller and SPI settings             */
                if (MySerial == null)
                {
                    Debug.WriteLine("SPI does not exist on the current system.");
                    return;
                }
                
                //SPIConfInfrared = new SPI.Configuration(
                //Pins.GPIO_NONE, // SS-pin
                //true,             // SS-pin active state
                //0,                 // The setup time for the SS port
                //0,                 // The hold time for the SS port
                //true,              // The idle state of the clock
                //true,             // The sampling clock edge
                //freq,              // The SPI clock rate in KHz
                //SPI_Devices.SPI1);   // The used SPI bus (refers to a MOSI MISO and SCLK pinset)

                //MySerial = new MultiSPI(SPIConfInfrared);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error iitializing SPI: " + e.Message);
                return;
            }
        }

        private void InitPins()
        {
            try
            {
                IoController = GpioController.GetDefault(); /* Get the default GPIO controller on the system */
                if (IoController == null)
                {
                    Debug.WriteLine("GPIO does not exist on the current system.");
                    return;
                }

                /* Initialize a pin as output for the Data/Command line on the display  */
                DataCommandPin = IoController.OpenPin(DATA_COMMAND_PIN);
                DataCommandPin.Write(GpioPinValue.Low);
                DataCommandPin.SetDriveMode(GpioPinDriveMode.Output);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Error iitializing GPIO: " + e.Message);
            }
        }

        private int FillBit(ushort[] uBuff, int iStart, int lBit, int sBit)
        {
            int inc;
            int i = iStart;
            //startstop bit
            for (inc = 0; inc < lBit; inc++)
            {
                uBuff[i] = _high;
                i++;
            }
            for (inc = 0; inc < sBit; inc++)
            {
                uBuff[i] = _low;
                i++;
            }
            return i;

        }

        private int FillStartStop(ushort[] uBuff, int iStart)
        {
            //Bit Start/stop = 6 x IR + 39 x ZE
            return FillBit(uBuff, iStart, 6, 39);
        }

        private int FillHigh(ushort[] uBuff, int iStart)
        {
            //Bit high = 6 x IR + 21 x ZE
            return FillBit(uBuff, iStart, 6, 21);
        }

        private int FillLow(ushort[] uBuff, int iStart)
        {
            //Bit low = 6 x IR + 10 x ZE
            return FillBit(uBuff, iStart, 6, 10);
        }

        private bool spi_send(ushort code)
        {
            bool isvalid = true;
            try
            {
                ushort[] tosend = new ushort[522]; // 522 is the max size of the message to be send
                ushort x = 0x8000;
                int i = 0;

                //Start bit
                i = FillStartStop(tosend, i);

                //encoding the 2 codes
                while (x != 0)
                {
                    if ((code & x) != 0)
                        i = FillHigh(tosend, i);
                    else
                        i = FillLow(tosend, i);
                    x >>= 1;  //next bit
                }
                //stop bit
                i = FillStartStop(tosend, i);
                if (DataCommandPin!=null)
                    DataCommandPin.Write(GpioPinValue.High);
                if (MySerial != null)
                    MySerial.Write(Helpers.UshortToByte(tosend));
                else
                    Debug.WriteLine("No SPI or PC mode");
                if(DataCommandPin!=null)
                    DataCommandPin.Write(GpioPinValue.Low);
            }
            catch (Exception e)
            {
                //Debug.Print("error spi send: " + e.Message);
                isvalid = false;
            }
            return isvalid;
        }

        

        private bool sendAllMessage(ushort[] nib2, ushort[] nib3, ushort[] nib4, ushort[] nib1)
        {
            bool isvalid = true;
            ushort[] code = new ushort[4];
            for (int i = 0; i < 4; i++)
                code[i] = (ushort)((nib1[i] << 12) | (nib2[i] << 8) | (nib3[i] << 4) | nib4[i]);
            // send 3 times each channel, starting with number 4
            for (uint j = 0; j < 3; j++)
            {
                for (uint i = 0; i < 4; i++)
                {
                    isvalid = spi_send(code[3 - i]);
                    if (!isvalid)
                        return isvalid;
                }
                //System.Threading.Thread.Sleep(16);
                //await Task.Delay(TimeSpan.FromMilliseconds(16));
                myCounter.Wait(16);
            }
            // wait 8 * 16ms, we already wait 16 ms in the past one
            //System.Threading.Thread.Sleep(112);
            //await Task.Delay(TimeSpan.FromMilliseconds(112));
            myCounter.Wait(112);
            // send another time all channels
            for (uint i = 0; i < 4; i++)
            {
                isvalid = spi_send(code[i]);
                if (!isvalid)
                    return isvalid;
            }
            // and finally send the last part of the message
            // wait a bit
            //System.Threading.Thread.Sleep(80);
            //await Task.Delay(TimeSpan.FromMilliseconds(80));
            myCounter.Wait(80);
            for (uint i = 0; i < 4; i++)
            {
                isvalid = spi_send(code[i]);
                if (!isvalid)
                    return isvalid;
                //System.Threading.Thread.Sleep(32);
                //await Task.Delay(TimeSpan.FromMilliseconds(32));
                myCounter.Wait(32);
            }
            // Toggle every channel
            for (uint i = 0; i < 4; i++)
            {
                if (toggle[(int)i] == 0)
                    toggle[(int)i] = 8;
                else
                    toggle[(int)i] = 0;
            }
            return isvalid;

        }

        private bool sendMessage(ushort nib1, ushort nib2, ushort nib3, ushort nib4, uint channel)
        {
            bool isvalid = true;
            ushort code = (ushort)((nib1 << 12) | (nib2 << 8) | (nib3 << 4) | nib4);
            for (uint i = 0; i < 6; i++)
            {
                message_pause(channel, i);

                isvalid = spi_send(code);
                if (!isvalid)
                    return isvalid;
            }
            if (toggle[(int)channel] == 0)
                toggle[(int)channel] = 8;
            else
                toggle[(int)channel] = 0;
            return isvalid;
        }

        public bool ComboModeAll([ReadOnlyArray()]LegoSpeed[] blue_speed, [ReadOnlyArray()]LegoSpeed[] red_speed)
        {
            //prepare all channels
            ushort[] nib1 = new ushort[4], nib2 = new ushort[4], nib3 = new ushort[4], nib4 = new ushort[4];

            //set nibs
            for (int i = 0; i < 4; i++)
            {
                nib1[i] = (ushort)(toggle[i] | (uint)i);
                //nib1 = (uint)channel;
                nib2[i] = (ushort)LegoMode.COMBO_DIRECT_MODE;
                nib3[i] = (ushort)((uint)blue_speed[i] | (uint)red_speed[i]);
                nib4[i] = (ushort)(0xf ^ nib1[i] ^ nib2[i] ^ nib3[i]);
            }
            return (sendAllMessage(nib2, nib3, nib4, nib1));
        }

        public bool ComboMode(LegoSpeed blue_speed, LegoSpeed red_speed, LegoChannel channel)
        {
            uint nib1, nib2, nib3, nib4;

            //set nibs
            nib1 = toggle[(uint)channel] | (uint)channel;
            //nib1 = (uint)channel;
            nib2 = (uint)LegoMode.COMBO_DIRECT_MODE;
            nib3 = (uint)blue_speed | (uint)red_speed;
            nib4 = 0xf ^ nib1 ^ nib2 ^ nib3;

            return (sendMessage((ushort)nib1, (ushort)nib2, (ushort)nib3, (ushort)nib4, (uint)channel));
        }

        public bool SingleOutputPWMAll([ReadOnlyArray()]LegoPWM[] pwm, [ReadOnlyArray()]LegoPWMOutput[] output)
        {
            //prepare all channels
            ushort[] nib1 = new ushort[4], nib2 = new ushort[4], nib3 = new ushort[4], nib4 = new ushort[4];

            //set nibs
            for (int i = 0; i < 4; i++)
            {
                nib1[i] = (ushort)(toggle[i] | (uint)i);
                //nib1 = (uint)channel;
                nib2[i] = (ushort)((ushort)LegoMode.SINGLE_OUTPUT_PWM | (ushort)output[i]);
                nib3[i] = (ushort)(pwm[i]);
                nib4[i] = (ushort)(0xf ^ nib1[i] ^ nib2[i] ^ nib3[i]);
            }
            return (sendAllMessage(nib2, nib3, nib4, nib1));
        }

        public bool SingleOutputPWM(LegoPWM pwm, LegoPWMOutput output, LegoChannel channel)
        {
            uint nib1, nib2, nib3, nib4;

            //set nibs
            nib1 = toggle[(uint)channel] | (uint)channel;
            nib2 = (uint)LegoMode.SINGLE_OUTPUT_PWM | (uint)output;
            nib3 = (uint)pwm;
            nib4 = 0xf ^ nib1 ^ nib2 ^ nib3;

            return (sendMessage((ushort)nib1, (ushort)nib2, (ushort)nib3, (ushort)nib4, (uint)channel));
        }

        public bool SingleOutputCST(LegoCST pwm, LegoPWMOutput output, LegoChannel channel)
        {
            uint nib1, nib2, nib3, nib4;

            //set nibs
            nib1 = toggle[(uint)channel] | (uint)channel;
            nib2 = (uint)LegoMode.SINGLE_OUTPUT_CST | (uint)output;
            nib3 = (uint)pwm;
            nib4 = 0xf ^ nib1 ^ nib2 ^ nib3;

            return (sendMessage((ushort)nib1, (ushort)nib2, (ushort)nib3, (ushort)nib4, (uint)channel));
        }

        public bool ComboPWM(LegoPWM pwm1, LegoPWM pwm2, LegoChannel channel)
        {
            uint nib1, nib2, nib3, nib4;

            //set nibs
            nib1 = 1 << 2 | (uint)channel;
            nib2 = (uint)pwm1;
            nib3 = (uint)pwm2;
            nib4 = 0xf ^ nib1 ^ nib2 ^ nib3;

            return (sendMessage((ushort)nib1, (ushort)nib2, (ushort)nib3, (ushort)nib4, (uint)channel));
        }

        public bool SingleOutputContinuousAll([ReadOnlyArray()]LegoFunction[] lfunction, [ReadOnlyArray()]LegoOutput[] output)
        {
            //prepare all channels
            ushort[] nib1 = new ushort[4], nib2 = new ushort[4], nib3 = new ushort[4], nib4 = new ushort[4];

            //set nibs
            for (int i = 0; i < 4; i++)
            {
                nib1[i] = (ushort)(toggle[i] | (uint)i);
                //nib1 = (uint)channel;
                nib2[i] = (ushort)LegoMode.SINGLE_PIN_CONTINUOUS;
                nib3[i] = (ushort)((uint)output[i] << 2 | (uint)lfunction[i]);
                nib4[i] = (ushort)(0xf ^ nib1[i] ^ nib2[i] ^ nib3[i]);
            }
            return (sendAllMessage(nib2, nib3, nib4, nib1));
        }

        public bool SingleOutputContinuous(LegoFunction lfunction, LegoOutput output, LegoChannel channel)
        {
            uint nib1, nib2, nib3, nib4;

            //set nibs
            nib1 = toggle[(uint)channel] | (uint)channel;
            nib2 = (uint)LegoMode.SINGLE_PIN_CONTINUOUS;
            nib3 = (uint)output << 2 | (uint)lfunction;
            nib4 = 0xf ^ nib1 ^ nib2 ^ nib3;

            return (sendMessage((ushort)nib1, (ushort)nib2, (ushort)nib3, (ushort)nib4, (uint)channel));
        }

        public bool SingleOutputTimeout(LegoFunction lfunction, LegoOutput output, LegoChannel channel)
        {
            uint nib1, nib2, nib3, nib4;

            //set nibs
            nib1 = toggle[(uint)channel] | (uint)channel;
            nib2 = (uint)LegoMode.SINGLE_PIN_TIMEOUT;
            nib3 = (uint)output << 2 | (uint)lfunction;
            nib4 = 0xf ^ nib1 ^ nib2 ^ nib3;

            return (sendMessage((ushort)nib1, (ushort)nib2, (ushort)nib3, (ushort)nib4, (uint)channel));
        }


        private void message_pause(uint channel, uint count)
        {

            int a = 0;
            // delay for first message
            // (4 - Ch) * Tm
            if (count == 0)
                a = 4 - (int)channel + 1;
            // next 2 messages
            // 5 * Tm
            else if (count == 1 || count == 2)
                a = 5;
            // last 2 messages
            // (6+2*Ch) * Tm
            else if (count == 3 || count == 4)
                a = 5 + ((int)channel + 1) * 2;

            // Tm = 16 ms (in theory 13.7 ms)
            //System.Threading.Thread.Sleep(a * 16);
            //await Task.Delay(TimeSpan.FromMilliseconds(a * 16));
            myCounter.Wait(a * 16);
        }


    }
}
