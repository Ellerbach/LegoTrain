using Devkoes.Restup.WebServer.Attributes;
using Devkoes.Restup.WebServer.Models.Schemas;
using IoTCoreHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;

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

    partial class TrainManagement
    {
        async static Task<string> GetFilePath(string filename)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                //var files = await localFolder.GetFilesAsync();
                //StorageFile file = files.FirstOrDefault(x => x.Name == filename);
                var file = await localFolder.GetFileAsync(filename);
                if (file != null)
                    return file.Path;

            }
            catch (Exception)
            {
            }
            return "";
        }

        static private async Task<ParamRail> LoadParamRail()
        {
            FileStream fileToRead = null;
            ParamRail myParamRail = new ParamRail();
            try
            {
                //fileToRead = new FileStream(strDefaultDir + "\\" + strFileProgram, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileToRead = new FileStream(await GetFilePath(strFileProgram), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                long fileLength = fileToRead.Length;

                byte[] buf = new byte[fileLength];
                //string mySetupString = "";

                // Reads the data.
                fileToRead.Read(buf, 0, (int)fileLength);
                //await str.ReadAsync(buf,  )
                // convert the read into a string

                List<Param> Params = Param.decryptParam(new String(Encoding.UTF8.GetChars(buf)));
                byte mSignal = byte.MaxValue;
                byte mSwitch = byte.MaxValue;
                byte mTrains = byte.MaxValue;
                if (Params == null)
                    return myParamRail;
                if (Params.Count == 0)
                    return myParamRail;

                mSignal = Param.CheckConvertByte(Params, paramNumberSignal);
                if ((mSignal <= 0) || (mSignal > LegoTrain.Signal.NUMBER_SIGNAL_MAX))
                    mSignal = LegoTrain.Signal.NUMBER_SIGNAL_MAX;
                mSwitch = Param.CheckConvertByte(Params, paramNumberSwitch);
                if ((mSwitch <= 0) || (mSwitch > LegoTrain.Switch.NUMBER_SWITCH_MAX))
                    mSwitch = LegoTrain.Switch.NUMBER_SWITCH_MAX;
                mTrains = Param.CheckConvertByte(Params, paramNumberTrain);
                if ((mTrains <= 0) || (mTrains > ParamTrain.NUMBER_TRAIN_MAX))
                    mTrains = ParamTrain.NUMBER_TRAIN_MAX;

                myParamRail.Serial = Param.CheckConvertBool(Params, paramSerial);
                myParamRail.WebServer = Param.CheckConvertBool(Params, paramWeb);
                myParamRail.SecurityKey = Param.CheckConvertString(Params, paramSecurity);
                myParamRail.MinDuration = Param.CheckConvertUInt16(Params, paramSwitchMinDur);
                myParamRail.MaxDuration = Param.CheckConvertUInt16(Params, paramSwitchMaxDur);
                myParamRail.MinAngle = Param.CheckConvertUInt16(Params, paramSwitchMinAng);
                myParamRail.MaxAngle = Param.CheckConvertUInt16(Params, paramSwitchMaxAng);
                myParamRail.ServoAngle = Param.CheckConvertUInt16(Params, paramSwitchAngle);

                //now load the params for the trains
                if (mTrains != 255)
                {
                    myParamRail.NumberOfTrains = mTrains;
                    myParamRail.Trains = new ParamTrain[mTrains];
                    for (byte a = 1; a <= mTrains; a++)
                    {
                        myParamRail.Trains[a - 1] = new ParamTrain();
                        byte mChannel = Param.CheckConvertByte(Params, paramParamChannel + a.ToString());
                        if (mChannel > 4)
                            mChannel = 4;
                        byte mRedBlue = Param.CheckConvertByte(Params, paramRedBlue + a.ToString());
                        if (mRedBlue > 1)
                            mRedBlue = 0;
                        byte mSpeed = Param.CheckConvertByte(Params, paramTrainSpeed + a.ToString());
                        if (mSpeed > 7)
                            mSpeed = 0;
                        myParamRail.Trains[a - 1].TrainName = Param.CheckConvertString(Params, paramTrainName + a.ToString());

                        myParamRail.Trains[a - 1].Channel = mChannel;
                        myParamRail.Trains[a - 1].RedBlue = (ParamTrainRedBlue)mRedBlue;
                        myParamRail.Trains[a - 1].Speed = mSpeed;
                    }
                }

                if (mSignal != 255)
                    myParamRail.NumberOfSignals = mSignal;
                if (mSwitch != 255)
                {
                    myParamRail.NumberOfSwitchs = mSwitch;
                    myParamRail.Switchs = new ParamSwitchs[mSwitch];
                    for (byte a = 1; a <= mSwitch; a++)
                    {
                        myParamRail.Switchs[a - 1] = new ParamSwitchs();
                        string mName = Param.CheckConvertString(Params, paramNameSwitch + a.ToString());
                        if (mName == "")
                            mName = "Switch " + a.ToString();
                        myParamRail.Switchs[a - 1].Name = mName;
                        myParamRail.Switchs[a - 1].Left = Param.CheckConvertInt32(Params, paramleft + a.ToString());
                        myParamRail.Switchs[a - 1].Top = Param.CheckConvertInt32(Params, paramtop + a.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if (fileToRead != null)
                {
                    fileToRead.Dispose();
                }
            }
            return myParamRail;
        }
    }
}
