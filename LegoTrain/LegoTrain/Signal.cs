using IoTCoreHelpers;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
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
    public sealed class Signal
    {
        private byte mNumberSignal;
        private bool[] mSignalStatus;
        private const byte nUMBER_SIGNAL_MAX = 16;
        private const Int32 SPI_CHIP_SELECT_LINE = 0;       /* Line 0 maps to physical pin number 24 on the Rpi2        */

        private SpiDevice MySignal;

        public Signal(byte NumberOfSignal)
        {
            mNumberSignal = NumberOfSignal;
            if ((mNumberSignal <= 0) && (mNumberSignal > NUMBER_SIGNAL_MAX))
                new Exception("Not correct number of Signals");
            mSignalStatus = new bool[mNumberSignal];
            // open a SPI
            //MySignal = new MultiSPI(new SPI.Configuration(
            //    Pins.GPIO_PIN_D10, // SS-pin
            //    false,             // SS-pin active state
            //    0,                 // The setup time for the SS port
            //    0,                 // The hold time for the SS port
            //    false,              // The idle state of the clock
            //    true,             // The sampling clock edge
            //    1000,              // The SPI clock rate in KHz
            //    SPI_Devices.SPI1));   // The used SPI bus (refers to a MOSI MISO and SCLK pinset)
            InitSPI();

        }

        private async void InitSPI()
        {
            var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
            settings.ClockFrequency = 650000;
            settings.SharingMode = SpiSharingMode.Shared;
            settings.Mode = SpiMode.Mode2; // should be either 1 or 2
            string aqs = SpiDevice.GetDeviceSelector();                     /* Get a selector string that will return all SPI controllers on the system */
            var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the SPI bus controller devices with our selector string             */
            if (dis.Count == 0)
            {
                Debug.WriteLine("SPI does not exist on the current system.");
                return;
            }

            MySignal = await SpiDevice.FromIdAsync(dis[0].Id, settings);    /* Create an SpiDevice with our bus controller and SPI settings             */
            if (MySignal == null)
            {
                Debug.WriteLine("No SPI device present on the system");
                return;
            }
            //initialise all signals to "false"
            for (byte i = 0; i < mNumberSignal; i++)
                ChangeSignal(i, true);

        }

        public byte NumberOfSignals
        { get { return mNumberSignal; } }

        public static byte NUMBER_SIGNAL_MAX
        {
            get
            {
                return nUMBER_SIGNAL_MAX;
            }
        }

        public void ChangeSignal(byte NumSignal, bool value)
        {
            if ((NumSignal <= 0) && (NumSignal > mNumberSignal))
                new Exception("Not correct number of Signals");
            //need to convert to select the right Signal
            mSignalStatus[NumSignal] = value;
            // fill the buffer to be sent
            ushort[] mySign = new ushort[1] { 0 };
            for (ushort i = 0; i < mNumberSignal; i++)
                if (mSignalStatus[i])
                    mySign[0] = (ushort)(mySign[0] | (ushort)(1 << i));
            //send the bytes
            if (MySignal!= null)
                MySignal.Write(Helpers.UshortToByte(mySign));
            

        }

        public bool GetSignal(byte NumSignal)
        {
            if ((NumSignal <= 0) && (NumSignal > mNumberSignal))
                new Exception("Not correct number of Signals");
            // need to convert to BCD
            return mSignalStatus[NumSignal];
        }
    }
}