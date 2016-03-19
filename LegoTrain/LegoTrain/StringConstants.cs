using System;

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
        #region All const string
        // security key
        static string MySecurityKey = "Key1234";
        const string paramSecurityKey = "sec";
        static string securityKey = "";
        // parameters
        static private string strFileProgram = "ParamTrain.txt";
        private const char ParamSeparator = '&';
        private const char ParamStart = '?';
        private const char ParamEqual = '=';
        private const string paramNumberSignal = "nsi";
        private const string paramNumberSwitch = "nai";
        private const string paramNameSwitch = "ain";
        private const string paramNumberTrain = "ntr";
        private const string paramTrainName = "tn";
        private const string paramParamChannel = "tc";
        private const string paramRedBlue = "tr";
        private const string paramSecurity = "sec";
        private const string paramSerial = "dbg";
        private const string paramCantonOrigin = "co";
        private const string paramCantonEnd = "cf";
        private const string paramSwitchOrigin = "ao";
        private const string paramSwitchStraight = "ad";
        private const string paramSwitchTurn = "ac";
        private const string paramNumberCantons = "nca";
        private const string paramGoto = "gt";
        private const string paramDetecteur = "d";
        private const string paramTrainSpeed = "tv";
        private const string paramWeb = "wb";
        private const string paramtop = "pt";
        private const string paramleft = "pl";
        private const string paramSwitchMinDur = "smi";
        private const string paramSwitchMaxDur = "sma";
        private const string paramSwitchMinAng = "ami";
        private const string paramSwitchMaxAng = "ama";
        private const string paramSwitchAngle = "sa";
        // Strings to be used for the page names
        const string pageDefault = "default.aspx";
        const string pageCombo = "combo.aspx";
        const string pageComboAll = "comboall.aspx";
        const string pageUtil = "util.aspx";
        const string pageSingleOutput = "singlepwm.aspx";
        const string pageSingleOutputAll = "singlepwmall.aspx";
        const string pageSingleCST = "singlecst.aspx";
        const string pagePWM = "combopwm.aspx";
        const string pageContinuous = "continuous.aspx";
        const string pageContinuousAll = "continuousall.aspx";
        const string pageSingleTimeout = "timeout.aspx";
        const string pageSignal = "signal.aspx";
        const string pageSwitch = "switch.aspx";
        const string pageAll = "all.aspx";
        const string pageCircuit = "circ.aspx";
        const string pageCSS = "train.css";
        // Strings to be used for the param names
        const string paramComboBlue = "bl";
        const string paramComboRed = "rd";
        const string paramChannel = "ch";
        const string paramSinglePWM = "pw";
        const string paramSingleOutput = "op";
        const string paramComboPWM1 = "p1";
        const string paramComboPWM2 = "p2";
        const string paramContinuousFct = "fc";
        const string paramMode = "md";
        const string paramNoUI = "no";
        const string paramILSNum = "il";
        const string paramSignalSwitch = "si";
        // Strings to be used for separators and returns
        //HTTP/1.1 200 OK\r\nContent-Type: text/html; charset=utf-8\r\nCache-Control: no-cache\r\nConnection: close\r\n\r\n
        const string strOK = "OK";
        const string strProblem = "Problem";
        const char strEndFile = '\r';

        #endregion All const string

    }
}
