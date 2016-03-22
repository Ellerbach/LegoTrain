using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using IoTCoreHelpers;
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
    public sealed class Switch
    {
        private byte mNumberSwitch;
        private bool[] mSwitchStatus;
        private const byte nUMBER_SWITCH_MAX = 16;
        private int numOutput = 3;
        private uint mMinAngle;
        private uint mMaxAngle;
        private GpioPin[] mOut;
        // create a new servo
        // Rotational Range: 203° 
        // Pulse Cycle: 20 ms 
        // Pulse Width: 540-2470 µs 
        private ServoMotor mSwitch; 
        private const int GPIO_PIN_D6 = 16;
        private const int GPIO_PIN_D7 = 20;
        private const int GPIO_PIN_D8 = 21;
        private const int GPIO_PIN_D9 = 19;
        private const int GPIO_PIN_D5 = 13;
        private int[] mPinTable = new int[4] { GPIO_PIN_D6, 
            GPIO_PIN_D7, GPIO_PIN_D8, GPIO_PIN_D9 };

        public Switch(byte NumberSwitch, uint MinDur, uint MaxDur, uint MinAng, uint MaxAng, uint ServoAngle)
        {
            mSwitch = new ServoMotor(GPIO_PIN_D5, new ServoMotorDefinition(MinDur, MaxDur, (uint)SerMotorPeriod.DefaultPeriod, ServoAngle, 5000));
            mMinAngle = MinAng;
            mMaxAngle = MaxAng;
            mNumberSwitch = NumberSwitch;
            if ((mNumberSwitch <= 0) && (mNumberSwitch > NUMBER_SWITCH_MAX))
                new Exception("Not correct number of Signals");
            mSwitchStatus = new bool[mNumberSwitch];
            if (mNumberSwitch <= 2)
                numOutput = 1;
            else if (mNumberSwitch <= 4)
                numOutput = 2;
            else if (mNumberSwitch <= 8)
                numOutput = 3;
            else if (mNumberSwitch <= 16)
                numOutput = 4;
            else
                new Exception("Too many Signals");
            //initialise the outputs based on the number of signals
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                Debug.WriteLine("No GPIO detected on your system");
                return;
            }

            //initialize all multiplexer output to Low
            mOut = new GpioPin[mPinTable.Length];
            for (int i = 0; i < mPinTable.Length; i++)
            {
                mOut[i] = gpio.OpenPin(mPinTable[i]);
                mOut[i].Write(GpioPinValue.Low);
                mOut[i].SetDriveMode(GpioPinDriveMode.Output);
            }
            Debug.Write("All GPIO initialised in Switch");
            //initialise all signals to "false"
            for (byte i = 0; i < mNumberSwitch; i++)
                ChangeSwitch(i, false);

        }

        public byte NumberOfSwitch
        { get { return mNumberSwitch; } }

        public static byte NUMBER_SWITCH_MAX
        {
            get
            {
                return nUMBER_SWITCH_MAX;
            }
        }

        public void ChangeSwitch(byte NumSignal, bool value)
        {
            if ((NumSignal <= 0) && (NumSignal > mNumberSwitch))
                new Exception("Not correct number of Signals");
            //need to convert to select the right Signal
            mSwitchStatus[NumSignal] = value;
            for (int j = 0; j < numOutput; j++)
                if (((NumSignal >> j) & 1) == 1)
                {
                    if (mOut != null)
                        mOut[j].Write(GpioPinValue.High);
                    Debug.WriteLine("Selecting switch: {0} to value {1}", j, GpioPinValue.High);
                }
                else { 
                    if (mOut!=null)
                        mOut[j].Write(GpioPinValue.Low);
                    Debug.WriteLine("Selecting switch: {0} to value {1}", j, GpioPinValue.Low);
                }
            // here, need to be smart!! probably not this
            Debug.WriteLine("Changing switch: {0} to value {1}", NumSignal, value);
            if (value)
            {
                mSwitch.Angle = mMaxAngle;
            }
            else
            {
                mSwitch.Angle = mMinAngle;
            }

        }

        public bool GetSwitch(byte NumSignal)
        {
            if ((NumSignal <= 0) && (NumSignal > mNumberSwitch))
                new Exception("Not correct number of Signals");
            return mSwitchStatus[NumSignal];
        }
    }
}