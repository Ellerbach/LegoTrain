using Devkoes.Restup.WebServer.Models.Schemas;
using IoTCoreHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoTrain
{
    partial class TrainManagement
    {
        private GetResponse ProcessUtil(string rawURL)
        {
            // decode params
            List<Param> Params = Param.decryptParam(rawURL);
            if (Params != null)
            {
                //Debug.Print("some params");
            }

            string strResp = strOK;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        static private string DecryptSwitch(string rawURL)
        {
            // decode params
            List<Param> Params = Param.decryptParam(rawURL);
            string strResp = "";
            //check if Params contains anything and is valid
            if (Params == null)
                return strResp;
            if (Params.Count == 0)
                return strResp;

            bool bNoUI = Param.CheckConvertBool(Params, paramNoUI);
            byte mSignalSwitch = Param.CheckConvertByte(Params, paramSignalSwitch);
            if (mSignalSwitch >= mySwitch.NumberOfSwitch)
                mSignalSwitch = 255;
            bool bValue = Param.CheckConvertBool(Params, paramMode);

            if (!bNoUI)
            {
                strResp = "<HTML><BODY>Status of Switch<p>";
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySwitch.NumberOfSwitch; i++)
                    {
                        strResp += "Switch " + i + ": " + mySwitch.GetSwitch(i) + "</br>";
                    }
                else
                {
                    //change the value
                    mySwitch.ChangeSwitch(mSignalSwitch, bValue);
                    strResp += "Switch " + mSignalSwitch + ": " + bValue + "</br>";
                }
                strResp += "</p></BODY></HTML>";
            }
            else
            {
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySwitch.NumberOfSwitch; i++)
                    {
                        if (mySwitch.GetSwitch(i))
                            strResp += "1";
                        else
                            strResp += "0";
                    }
                else
                {
                    //change status
                    mySwitch.ChangeSwitch(mSignalSwitch, bValue);
                    strResp += strOK;
                }

            }

            return strResp;
        }

        private GetResponse ProcessSwitch(string rawURL)
        {
            return new GetResponse(GetResponse.ResponseStatus.OK, DecryptSwitch(rawURL));
        }

        static private string DecryptSignal(string rawURL)
        {
            // decode params
            List<Param> Params = Param.decryptParam(rawURL);
            string strResp = "";
            //check if Params contains anything and is valid
            if (Params == null)
                return strResp;
            if (Params.Count == 0)
                return strResp;

            bool bNoUI = Param.CheckConvertBool(Params, paramNoUI);
            byte mSignalSwitch = Param.CheckConvertByte(Params, paramSignalSwitch);
            if (mSignalSwitch >= mySignal.NumberOfSignals)
                mSignalSwitch = 255;
            bool bValue = Param.CheckConvertBool(Params, paramMode);

            if (!bNoUI)
            {
                strResp = "<HTML><BODY>Status of Signal<p>";
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySignal.NumberOfSignals; i++)
                    {
                        strResp += "Switch " + i + ": " + mySignal.GetSignal(i) + "</br>";
                    }
                else
                {
                    //change the value
                    mySignal.ChangeSignal(mSignalSwitch, bValue);
                    strResp += "Switch " + mSignalSwitch + ": " + bValue + "</br>";
                }
                strResp += "</p></BODY></HTML>";
            }
            else
            {
                if (mSignalSwitch == 255)
                    for (byte i = 0; i < mySignal.NumberOfSignals; i++)
                    {
                        if (mySignal.GetSignal(i))
                            strResp += "1";
                        else
                            strResp += "0";
                    }
                else
                {
                    //change status
                    mySignal.ChangeSignal(mSignalSwitch, bValue);
                    strResp += strOK;
                }
            }

            return strResp;
        }

        private GetResponse ProcessSignal(string rawURL)
        {

            return new GetResponse(GetResponse.ResponseStatus.OK, DecryptSignal(rawURL));
        }

        static private bool DecryptCombo(string StrDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(StrDecrypt);
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            int mChannel = Param.CheckConvertInt32(Params, paramChannel);
            if (!((mChannel >= (int)LegoChannel.CH1) && (mChannel <= (int)LegoChannel.CH4)))
                isvalid = false;
            int mComboBlue = Param.CheckConvertInt32(Params, paramComboBlue);
            if (!((mComboBlue == (int)LegoSpeed.BLUE_BRK) || (mComboBlue == (int)LegoSpeed.BLUE_FLT) ||
                (mComboBlue == (int)LegoSpeed.BLUE_FWD) || (mComboBlue == (int)LegoSpeed.BLUE_REV)))
                isvalid = false;
            int mComboRed = Param.CheckConvertInt32(Params, paramComboRed);
            if (!((mComboRed >= (int)LegoSpeed.RED_FLT) && (mComboRed <= (int)LegoSpeed.RED_BRK)))
                isvalid = false;

            // check if all params are correct

            if (isvalid)
            {
                isvalid = myLego.ComboMode((LegoSpeed)mComboBlue, (LegoSpeed)mComboRed, (LegoChannel)mChannel);
            }
            else
                isvalid = false;
            return isvalid;
        }

        static private bool DecryptComboAll(string StrDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(StrDecrypt);
            LegoSpeed[] mComboBlue = new LegoSpeed[4];
            LegoSpeed[] mComboRed = new LegoSpeed[4];
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            for (int a = 0; a < 4; a++)
            {

                mComboBlue[a] = (LegoSpeed)Param.CheckConvertInt32(Params, paramComboBlue + a.ToString());
                if (!((mComboBlue[a] == LegoSpeed.BLUE_BRK) || (mComboBlue[a] == LegoSpeed.BLUE_FLT) ||
                    (mComboBlue[a] == LegoSpeed.BLUE_FWD) || (mComboBlue[a] == LegoSpeed.BLUE_REV)))
                    isvalid = false;
                mComboRed[a] = (LegoSpeed)Param.CheckConvertInt32(Params, paramComboRed + a.ToString());
                if (!((mComboRed[a] >= LegoSpeed.RED_FLT) && (mComboRed[a] <= LegoSpeed.RED_BRK)))
                    isvalid = false;
            }
            // check if all params are correct
            if (isvalid)
            {
                isvalid = myLego.ComboModeAll(mComboBlue, mComboRed);
            }
            else
                isvalid = false;

            return isvalid;

        }

        private GetResponse ProcessCombo(string rawURL)
        {
            string strResp = "";
            if (DecryptCombo(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        private GetResponse ProcessComboAll(string rawURL)
        {
            string strResp = "";
            if (DecryptComboAll(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        static private bool DecryptSinglePWM(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            int mChannel = Param.CheckConvertInt32(Params, paramChannel);
            if (!((mChannel >= (int)LegoChannel.CH1) && (mChannel <= (int)LegoChannel.CH4)))
                isvalid = false;
            int mPWM = Param.CheckConvertInt32(Params, paramSinglePWM);
            if (!((mPWM >= (int)LegoPWM.FLT) && (mPWM <= (int)LegoPWM.REV1)))
                isvalid = false;
            int mOutPut = Param.CheckConvertInt32(Params, paramSingleOutput);
            if (!((mOutPut >= (int)LegoPWMOutput.RED) || (mOutPut <= (int)LegoPWMOutput.BLUE)))
                isvalid = false;

            // check if all params are correct

            if (isvalid)
            {
                isvalid = myLego.SingleOutputPWM((LegoPWM)mPWM, (LegoPWMOutput)mOutPut, (LegoChannel)mChannel);
            }
            else
                isvalid = false;

            return isvalid;

        }

        static private bool DecryptSinglePWMAll(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            LegoPWM[] mPWM = new LegoPWM[4];
            LegoPWMOutput[] mOutPut = new LegoPWMOutput[4];
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            for (int a = 0; a < 4; a++)
            {
                mPWM[a] = (LegoPWM)Param.CheckConvertInt32(Params, paramSinglePWM + a.ToString());
                if (!((mPWM[a] >= LegoPWM.FLT) && (mPWM[a] <= LegoPWM.REV1)))
                    isvalid = false;
                mOutPut[a] = (LegoPWMOutput)Param.CheckConvertInt32(Params, paramSingleOutput + a.ToString());
                if (!((mOutPut[a] >= LegoPWMOutput.RED) || (mOutPut[a] <= LegoPWMOutput.BLUE)))
                    isvalid = false;
            }

            // check if all params are correct
            if (isvalid)
            {
                isvalid = myLego.SingleOutputPWMAll(mPWM, mOutPut);
            }
            else
                isvalid = false;

            return isvalid;
        }

        private GetResponse ProcessSinglePWM(string rawURL)
        {
            string strResp = "";
            if (DecryptSinglePWM(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        private GetResponse ProcessSinglePWMAll(string rawURL)
        {
            string strResp = "";
            if (DecryptSinglePWMAll(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        static private bool DecryptSingleCST(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            int mChannel = Param.CheckConvertInt32(Params, paramChannel);
            if (!((mChannel >= (int)LegoChannel.CH1) && (mChannel <= (int)LegoChannel.CH4)))
                isvalid = false;
            int mPWM = Param.CheckConvertInt32(Params, paramSinglePWM);
            if (!((mPWM >= (int)LegoCST.CLEARC1_CLEARC2) && (mPWM <= (int)LegoCST.TOGGLE_FWD_BKD)))
                isvalid = false;
            int mOutPut = Param.CheckConvertInt32(Params, paramSingleOutput);
            if (!((mOutPut >= (int)LegoPWMOutput.RED) || (mOutPut <= (int)LegoPWMOutput.BLUE)))
                isvalid = false;

            if (isvalid)
            {
                isvalid = myLego.SingleOutputCST((LegoCST)mPWM, (LegoPWMOutput)mOutPut, (LegoChannel)mChannel);
            }
            else
                isvalid = false;
            return isvalid;
        }

        private GetResponse ProcessSingleCST(string rawURL)
        {
            string strResp = "";
            if (DecryptSingleCST(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        static private bool DecryptComboPWM(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            int mChannel = Param.CheckConvertInt32(Params, paramChannel);
            if (!((mChannel >= (int)LegoChannel.CH1) && (mChannel <= (int)LegoChannel.CH4)))
                isvalid = false;
            int mPWM1 = Param.CheckConvertInt32(Params, paramComboPWM1);
            if (!((mPWM1 >= (int)LegoPWM.FLT) && (mPWM1 <= (int)LegoPWM.REV1)))
                isvalid = false;
            int mPWM2 = Param.CheckConvertInt32(Params, paramComboPWM2);
            if (!((mPWM2 >= (int)LegoPWM.FLT) && (mPWM2 <= (int)LegoPWM.REV1)))
                isvalid = false;

            if (isvalid)
            {
                isvalid = myLego.ComboPWM((LegoPWM)mPWM1, (LegoPWM)mPWM1, (LegoChannel)mChannel);
            }
            else
                isvalid = false;

            return isvalid;
        }

        private GetResponse ProcessComboPWM(string rawURL)
        {
            string strResp = "";
            if (DecryptComboPWM(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        static private bool DecryptComboPWMAll(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            LegoPWM[] mPWMR = new LegoPWM[4];
            LegoPWM[] mPWMB = new LegoPWM[4];

            for (int i = 0; i < 4; i++)
            {
                int pwr = Param.CheckConvertInt32(Params, paramComboPWMR + i);
                if (!((pwr >= (int)LegoPWM.FLT) && (pwr <= (int)LegoPWM.REV1)))
                    isvalid = false;
                else
                    mPWMR[i] = (LegoPWM)pwr;
                int pwb = Param.CheckConvertInt32(Params, paramComboPWMB + i);
                if (!((pwb >= (int)LegoPWM.FLT) && (pwb <= (int)LegoPWM.REV1)))
                    isvalid = false;
                else
                    mPWMB[i] = (LegoPWM)pwb;
            }
            if (isvalid)
            {
                isvalid = myLego.ComboPWMAll(mPWMR, mPWMB);
            }
            else
                isvalid = false;

            return isvalid;
        }

        private GetResponse ProcessComboPWMAll(string rawURL)
        {
            string strResp = "";
            if (DecryptComboPWMAll(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        static private bool DecryptContinuous(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            int mChannel = Param.CheckConvertInt32(Params, paramChannel);
            if (!((mChannel >= (int)LegoChannel.CH1) && (mChannel <= (int)LegoChannel.CH4)))
                isvalid = false;
            int mFunction = Param.CheckConvertInt32(Params, paramContinuousFct);
            if (!((mFunction >= (int)LegoFunction.NO_CHANGE) && (mFunction <= (int)LegoFunction.TOGGLE)))
                isvalid = false;
            int mOutPut = Param.CheckConvertInt32(Params, paramSingleOutput);
            if (!((mOutPut >= (int)LegoOutput.RED_PINC1) || (mOutPut <= (int)LegoOutput.BLUE_PINC2)))
                isvalid = false;

            if (isvalid)
            {
                isvalid = myLego.SingleOutputContinuous((LegoFunction)mFunction, (LegoOutput)mOutPut, (LegoChannel)mChannel);
            }
            else
                isvalid = false;

            return isvalid;
        }

        static private bool DecryptContinuousAll(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            LegoFunction[] mFunction = new LegoFunction[4];
            LegoOutput[] mOutPut = new LegoOutput[4];
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            for (int a = 0; a < 4; a++)
            {
                mFunction[a] = (LegoFunction)Param.CheckConvertInt32(Params, paramContinuousFct + a.ToString());
                if (!((mFunction[a] >= LegoFunction.NO_CHANGE) && (mFunction[a] <= LegoFunction.TOGGLE)))
                    isvalid = false;
                mOutPut[a] = (LegoOutput)Param.CheckConvertInt32(Params, paramSingleOutput + a.ToString());
                if (!((mOutPut[a] >= LegoOutput.RED_PINC1) || (mOutPut[a] <= LegoOutput.BLUE_PINC2)))
                    isvalid = false;
            }
            // check if all params are correct
            if (isvalid)
            {
                isvalid = myLego.SingleOutputContinuousAll(mFunction, mOutPut);
            }
            else
                isvalid = false;

            return isvalid;
        }

        private GetResponse ProcessContinuous(string rawURL)
        {
            string strResp = "";
            if (DecryptContinuous(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        private GetResponse ProcessContinuousAll(string rawURL)
        {
            string strResp = "";
            if (DecryptContinuousAll(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        static private bool DecryptSingleTimeout(string strDecrypt)
        {
            // decode params
            List<Param> Params = Param.decryptParam(strDecrypt);
            bool isvalid = true;
            //check if Params contains anything and is valid
            if (Params == null)
                return false;
            if (Params.Count == 0)
                return false;

            int mChannel = Param.CheckConvertInt32(Params, paramChannel);
            if (!((mChannel >= (int)LegoChannel.CH1) && (mChannel <= (int)LegoChannel.CH4)))
                isvalid = false;
            int mFunction = Param.CheckConvertInt32(Params, paramContinuousFct);
            if (!((mFunction >= (int)LegoFunction.NO_CHANGE) && (mFunction <= (int)LegoFunction.TOGGLE)))
                isvalid = false;
            int mOutPut = Param.CheckConvertInt32(Params, paramSingleOutput);
            if (!((mOutPut >= (int)LegoOutput.RED_PINC1) || (mOutPut <= (int)LegoOutput.BLUE_PINC2)))
                isvalid = false;

            // check if all params are correct
            if (isvalid)
            {
                isvalid = myLego.SingleOutputTimeout((LegoFunction)mFunction, (LegoOutput)mOutPut, (LegoChannel)mChannel);
            }
            else
                isvalid = false;

            return isvalid;
        }

        private GetResponse ProcessSingleTimeout(string rawURL)
        {
            string strResp = "";
            if (DecryptSingleTimeout(rawURL))
                strResp = strOK;
            else
                strResp = strProblem;
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }


        private GetResponse ProcessDisplayAll(string rawURL)
        {
            string strResp = "";
            // Start HTML document
            strResp = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            strResp += "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title></title></head><body>";
            // first form is for Combo mode
            strResp += "<form method=\"get\" action=\"combo.aspx\" target=\"_blank\"><p>Combo Mode<br />Speed Red<select id=\"RedSpeed\" name=\"rd\">";
            strResp += "<option label='RED_FLT'>0</option><option label='RED_FWD'>1</option><option label='RED_REV'>2</option><option label='RED_BRK'>3</option>";
            strResp += "</select> Speed Blue<select id=\"BlueSpeed\" name=\"bl\"><option label='BLUE_FLT'>0</option><option label='BLUE_FWD'>4</option><option label='BLUE_REV'>8</option><option label='BLUE_BRK'>12</option>";
            strResp += "</select> Channel<select id=\"Channel\" name=\"ch\"><option label='CH1'>0</option><option label='CH2'>1</option><option label='CH3'>2</option><option label='CH4'>3</option>";
            strResp += "</select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit1\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //strResp = WebServer.OutPutStream(response, strResp);
            //second form is singlepwm
            strResp += "<form method=\"get\" action=\"singlepwm.aspx\" target=\"_blank\"><p>SinglePWM Mode<br />PWM<select id=\"PWM\" name=\"pw\">";
            strResp += "<option label='FLT'>0</option>";
            strResp += "<option label='FWD1'>1</option>";
            strResp += "<option label='FWD2'>2</option>";
            strResp += "<option label='FWD3'>3</option>";
            strResp += "<option label='FWD4'>4</option>";
            strResp += "<option label='FWD5'>5</option>";
            strResp += "<option label='FWD6'>6</option>";
            strResp += "<option label='FWD7'>7</option>";
            strResp += "<option label='BRK'>8</option>";
            strResp += "<option label='REV7'>9</option>";
            strResp += "<option label='REV6'>10</option>";
            strResp += "<option label='REV5'>11</option>";
            strResp += "<option label='REV4'>12</option>";
            strResp += "<option label='REV3'>13</option>";
            strResp += "<option label='REV2'>14</option>";
            strResp += "<option label='REV1'>15</option>";
            strResp += "</select> Output<select id=\"Output\" name=\"op\"><option label='RED'>0</option><option label='BLUE'>1</option>";
            //strResp = WebServer.OutPutStream(response, strResp);
            strResp += "</select> Channel<select id=\"Channel\" name=\"ch\"><option label='CH1'>0</option><option label='CH2'>1</option><option label='CH3'>2</option><option label='CH4'>3</option>";
            strResp += "</select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit2\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //3rd is continuous
            strResp += "<form method=\"get\" action=\"continuous.aspx\" target=\"_blank\"><p>SingleCountinuous Mode";
            strResp += "<br />Function<select id=\"Function\" name=\"fc\">";
            strResp += "<option label='NO_CHANGE'>0</option><option label='CLEAR'>1</option><option label='SET'>2</option><option label='TOGGLE'>3</option>";
            strResp += "</select> Output<select id=\"Output\" name=\"op\"><option label='RED_PINC1'>0</option><option label='RED_PINC2'>1</option>";
            strResp += "<option label='BLUE_PINC1'>2</option><option label='BLUE_PINC2'>3</option>";
            //strResp = WebServer.OutPutStream(response, strResp);
            strResp += "</select> Channel<select id=\"Channel\" name=\"ch\"><option label='CH1'>0</option><option label='CH2'>1</option><option label='CH3'>2</option><option label='CH4'>3</option>";
            strResp += "</select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit3\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //4 ComboPWM
            strResp += "<form method=\"get\" action=\"combopwm.aspx\" target=\"_blank\"><p>ComboPWM Mode<br />PWM Red<select id=\"PWM1\" name=\"p1\">";
            strResp += "<option label='FLT'>0</option>";
            strResp += "<option label='FWD1'>1</option>";
            strResp += "<option label='FWD2'>2</option>";
            strResp += "<option label='FWD3'>3</option>";
            strResp += "<option label='FWD4'>4</option>";
            strResp += "<option label='FWD5'>5</option>";
            strResp += "<option label='FWD6'>6</option>";
            strResp += "<option label='FWD7'>7</option>";
            strResp += "<option label='BRK'>8</option>";
            strResp += "<option label='REV7'>9</option>";
            strResp += "<option label='REV6'>10</option>";
            strResp += "<option label='REV5'>11</option>";
            strResp += "<option label='REV4'>12</option>";
            strResp += "<option label='REV3'>13</option>";
            strResp += "<option label='REV2'>14</option>";
            strResp += "<option label='REV1'>15</option></select>";
            strResp += "PWM Blue<select id=\"PWM2\" name=\"p2\">";
            strResp += "<option label='FLT'>0</option>";
            strResp += "<option label='FWD1'>1</option>";
            strResp += "<option label='FWD2'>2</option>";
            strResp += "<option label='FWD3'>3</option>";
            strResp += "<option label='FWD4'>4</option>";
            strResp += "<option label='FWD5'>5</option>";
            strResp += "<option label='FWD6'>6</option>";
            strResp += "<option label='FWD7'>7</option>";
            strResp += "<option label='BRK'>8</option>";
            strResp += "<option label='REV7'>9</option>";
            strResp += "<option label='REV6'>10</option>";
            strResp += "<option label='REV5'>11</option>";
            strResp += "<option label='REV4'>12</option>";
            strResp += "<option label='REV3'>13</option>";
            strResp += "<option label='REV2'>14</option>";
            strResp += "<option label='REV1'>15</option>";
            strResp += "</select> Channel<select id=\"Channel\" name=\"ch\"><option label='CH1'>0</option><option label='CH2'>1</option><option label='CH3'>2</option><option label='CH4'>3</option>";
            strResp += "</select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit4\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //4 ComboPWMAll
            // TODO TODO TODO TODO
            strResp += "<form method=\"get\" action=\"combopwmall.aspx\" target=\"_blank\"><p>ComboPWMAll Mode<br />";
            for (int i = 0; i < 4; i++)
            {
                strResp += "PWM Red<select id=\"PWM1\" name=\"pwr" + i + "\">";
                strResp += "<option label='FLT'>0</option>";
                strResp += "<option label='FWD1'>1</option>";
                strResp += "<option label='FWD2'>2</option>";
                strResp += "<option label='FWD3'>3</option>";
                strResp += "<option label='FWD4'>4</option>";
                strResp += "<option label='FWD5'>5</option>";
                strResp += "<option label='FWD6'>6</option>";
                strResp += "<option label='FWD7'>7</option>";
                strResp += "<option label='BRK'>8</option>";
                strResp += "<option label='REV7'>9</option>";
                strResp += "<option label='REV6'>10</option>";
                strResp += "<option label='REV5'>11</option>";
                strResp += "<option label='REV4'>12</option>";
                strResp += "<option label='REV3'>13</option>";
                strResp += "<option label='REV2'>14</option>";
                strResp += "<option label='REV1'>15</option></select>";
                strResp += "PWM Blue<select id=\"PWM2\" name=\"pwb" + i + "\">";
                strResp += "<option label='FLT'>0</option>";
                strResp += "<option label='FWD1'>1</option>";
                strResp += "<option label='FWD2'>2</option>";
                strResp += "<option label='FWD3'>3</option>";
                strResp += "<option label='FWD4'>4</option>";
                strResp += "<option label='FWD5'>5</option>";
                strResp += "<option label='FWD6'>6</option>";
                strResp += "<option label='FWD7'>7</option>";
                strResp += "<option label='BRK'>8</option>";
                strResp += "<option label='REV7'>9</option>";
                strResp += "<option label='REV6'>10</option>";
                strResp += "<option label='REV5'>11</option>";
                strResp += "<option label='REV4'>12</option>";
                strResp += "<option label='REV3'>13</option>";
                strResp += "<option label='REV2'>14</option>";
                strResp += "<option label='REV1'>15</option>";
                //strResp += "</select> Channel<select id=\"Channel\" name=\"ch\"><option label='CH1'>0</option><option label='CH2'>1</option><option label='CH3'>2</option><option label='CH4'>3</option>";
                strResp += "</select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            strResp = "<input id=\"Submit4\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //strResp = WebServer.OutPutStream(response, strResp);
            //5 Single CST
            strResp += "<form method=\"get\" action=\"singlecst.aspx\" target=\"_blank\"><p>SingleCST Mode<br />CST<select id=\"CST\" name=\"pw\">";
            strResp += "<option label='CLEARC1_CLEARC2'>0</option>";
            strResp += "<option label='SETC1_SETC2'>1</option>";
            strResp += "<option label='CLEARC1_SETC2'>2</option>";
            strResp += "<option label='SETC1_CLEARC2'>3</option>";
            strResp += "<option label='INC_PWM'>4</option>";
            strResp += "<option label='DEC_PWM'>5</option>";
            strResp += "<option label='FULLFWD'>6</option>";
            strResp += "<option label='FULLBKD'>7</option>";
            strResp += "<option label='TOGGLE_FWD_BKD'>8</option>";
            strResp += "</select> Output<select id=\"Output\" name=\"op\"><option label='RED'>0</option><option label='BLUE'>1</option>";
            //strResp = WebServer.OutPutStream(response, strResp);
            strResp += "</select> Channel<select id=\"Channel\" name=\"ch\"><option label='CH1'>0</option><option label='CH2'>1</option><option label='CH3'>2</option><option label='CH4'>3</option>";
            strResp += "</select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit5\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //6 Single Timeout
            strResp += "<form method=\"get\" action=\"timeout.aspx\" target=\"_blank\"><p>SingleTimeout Mode";
            strResp += "<br />Function<select id=\"Function\" name=\"fc\">";
            strResp += "<option label='NO_CHANGE'>0</option><option label='CLEAR'>1</option><option label='SET'>2</option><option label='TOGGLE'>3</option>";
            strResp += "</select> Output<select id=\"Output\" name=\"op\"><option label='RED_PINC1'>0</option><option label='RED_PINC2'>1</option>";
            strResp += "<option label='BLUE_PINC1'>2</option><option label='BLUE_PINC2'>3</option>";
            strResp += "</select> Channel<select id=\"Channel\" name=\"ch\"><option label='CH1'>0</option><option label='CH2'>1</option><option label='CH3'>2</option><option label='CH4'>3</option>";
            strResp += "</select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit6\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //strResp = WebServer.OutPutStream(response, strResp);
            // 7 form is for Combo mode all
            strResp += "<form method=\"get\" action=\"comboall.aspx\" target=\"_blank\"><p>Combo Mode All";
            for (int i = 0; i < 4; i++)
            {
                strResp += "<br />Speed Red channel " + (i + 1) + "<select id=\"RedSpeed\" name=\"rd" + i + "\">";
                strResp += "<option label='RED_FLT'>0</option><option label='RED_FWD'>1</option><option label='RED_REV'>2</option><option label='RED_BRK'>3</option>";
                strResp += "</select> Speed Blue channel " + (i + 1) + "<select id=\"BlueSpeed\" name=\"bl" + i + "\"><option label='BLUE_FLT'>0</option><option label='BLUE_FWD'>4</option><option label='BLUE_REV'>8</option><option label='BLUE_BRK'>12</option>";
                strResp += "</select>";
                //strResp = WebServer.OutPutStream(response, strResp);
            }
            strResp += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit7\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //8 is continuous
            strResp += "<form method=\"get\" action=\"continuousall.aspx\" target=\"_blank\"><p>SingleCountinuous Mode All";
            for (int i = 0; i < 4; i++)
            {
                strResp += "<br />Function channel " + (i + 1) + "<select id=\"Function\" name=\"fc" + i + "\">";
                strResp += "<option label='NO_CHANGE'>0</option><option label='CLEAR'>1</option><option label='SET'>2</option><option label='TOGGLE'>3</option>";
                strResp += "</select> Output channel " + (i + 1) + "<select id=\"Output\" name=\"op" + i + "\"><option label='RED_PINC1'>0</option><option label='RED_PINC2'>1</option>";
                strResp += "<option label='BLUE_PINC1'>2</option><option label='BLUE_PINC2'>3</option></select>";
                //strResp = WebServer.OutPutStream(response, strResp);
            }
            strResp += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit8\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            // 9 form is singlepwm
            strResp += "<form method=\"get\" action=\"singlepwmall.aspx\" target=\"_blank\"><p>SinglePWM Mode All";
            for (int a = 0; a < 4; a++)
            {
                strResp += "<br />PWM channel " + (a + 1) + "<select id=\"PWM\" name=\"pw" + a + "\">";
                strResp += "<option label='FLT'>0</option>";
                strResp += "<option label='FWD1'>1</option>";
                strResp += "<option label='FWD2'>2</option>";
                strResp += "<option label='FWD3'>3</option>";
                strResp += "<option label='FWD4'>4</option>";
                strResp += "<option label='FWD5'>5</option>";
                strResp += "<option label='FWD6'>6</option>";
                strResp += "<option label='FWD7'>7</option>";
                strResp += "<option label='BRK'>8</option>";
                strResp += "<option label='REV7'>9</option>";
                strResp += "<option label='REV6'>10</option>";
                strResp += "<option label='REV5'>11</option>";
                strResp += "<option label='REV4'>12</option>";
                strResp += "<option label='REV3'>13</option>";
                strResp += "<option label='REV2'>14</option>";
                strResp += "<option label='REV1'>15</option>";
                strResp += "</select> Output channel " + (a + 1) + "<select id=\"Output\" name=\"op" + a + "\"><option label='RED'>0</option><option label='BLUE'>1</option></select>";
            }
            //strResp = WebServer.OutPutStream(response, strResp);
            strResp += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit9\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            // Signal list
            strResp += "<p><a href=\"" + pageSignal + ParamStart + securityKey + "\" target=\"_blank\">List all signals</a></p>";
            // 10 Change on signal
            strResp += "<form method=\"get\" action=\"signal.aspx\" target=\"_blank\"><p>Change one signal";
            strResp += "<select id=\"SIG\" name=\"si\">";
            for (int a = 0; a < mySignal.NumberOfSignals; a++)
            {
                strResp += "<option label='Signal " + a + "'>" + a + "</option>";
            }
            strResp += "</select> Value <select id=\"ValueSignal\" name=\"md\"><option label='Off'>0</option><option label='On'>1</option></select>";
            strResp += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit11\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            //strResp = WebServer.OutPutStream(response, strResp);
            // Switch list
            strResp += "<p><a href=\"" + pageSwitch + ParamStart + securityKey + "\" target=\"_blank\">List all switchs</a></p>";
            // 11 Change one switch
            strResp += "<form method=\"get\" action=\"switch.aspx\" target=\"_blank\"><p>Change one signal";
            strResp += "<select id=\"SIG\" name=\"si\">";
            for (int a = 0; a < mySwitch.NumberOfSwitch; a++)
            {
                strResp += "<option label='Switch " + a + "'>" + a + "</option>";
            }
            strResp += "</select> Value <select id=\"ValueSwitch\" name=\"md\"><option label='Straight'>0</option><option label='Turned'>1</option></select>";
            //strResp = WebServer.OutPutStream(response, strResp);
            strResp += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id=\"Submit11\" type=\"submit\" value=\"Send\" /></p>";
            strResp += "<input type=\"hidden\" name=\"" + paramSecurityKey + "\" value=\"" + MySecurityKey + "\"></form>";
            strResp += "</body></html>";
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        private GetResponse ProcessDisplayCircuit(string rawURL)
        {
            string strResp = "";
            // Start HTML document
            strResp += "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            strResp += "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Gestion des trains</title>";
            //this is the css to make it nice :-)
            strResp += "<link href=\"file/" + pageCSS + "\" rel=\"stylesheet\" type=\"text/css\" />";
            strResp += "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head><body onLoad=\"getswitches();\">";
            strResp += "<meta http-equiv=\"Cache-control\" content=\"no-cache\"/>";
            //create the script part
            strResp += "<SCRIPT language=\"JavaScript\">";
            strResp += "var xhr = new XMLHttpRequest(); function btnclicked(boxMSG, cmdSend) { boxMSG.innerHTML=\"Waiting\";";
            strResp += "xhr.open('GET', cmdSend + '&" + securityKey + "&_=' + Math.random());";
            strResp += "xhr.send(null); xhr.onreadystatechange = function() {if (xhr.readyState == 4) {boxMSG.innerHTML=xhr.responseText;}};}";

            strResp += "function findTop(iobj) { ttop = 0; while (iobj) { ttop += iobj.offsetTop; iobj = iobj.offsetParent; } return ttop; }";
            strResp += "function findLeft(iobj) { tleft = 0; while (iobj) { tleft += iobj.offsetLeft; iobj = iobj.offsetParent; } return tleft; }";
            //request switch change
            strResp += "var xhr = new XMLHttpRequest(); function swclicked(boxMSG, cmdSend) { var mycmd = cmdSend +'&md='; ";
            strResp += "if (boxMSG.src.indexOf('turn.png')>=0) mycmd+='0'; else mycmd += '1';";
            strResp += "xhr.open('GET', 'switch.aspx?' + mycmd + '&" + securityKey + "&_='+Math.random());";
            strResp += "xhr.send(null); xhr.onreadystatechange = function() {if (xhr.readyState == 4) { if(xhr.responseText.indexOf('OK')>=0)";
            strResp += "if (boxMSG.src.indexOf('turn')>=0) boxMSG.src = 'file/str.png'; else boxMSG.src = 'file/turn.png';}};}";
            //strResp = WebServer.OutPutStream(response, strResp);
            //create the initial value for the switches
            strResp += "function buildSwitch(boxMSG, num) {var obj = document.getElementById(boxMSG); ";
            strResp += "if (num == 1) obj.src = 'file/str.png'; else obj.src = 'file/turn.png';}";
            strResp += "var NumberSwitch=" + mySwitch.NumberOfSwitch + ";";
            strResp += "function getswitches() {xhr.open('GET', 'switch.aspx?" + securityKey + "&no=1&_=' + Math.random()); xhr.send(null); xhr.onreadystatechange = function () {";
            strResp += "if (xhr.readyState == 4) {";
            strResp += "for (inc = 0; inc < NumberSwitch; inc++) { if(xhr.responseText.charAt(inc)=='0') mynum = 0; else mynum = 1; buildSwitch('switch'+inc, mynum);}}};}";
            strResp += "</SCRIPT>";
            //strResp = WebServer.OutPutStream(response, strResp);
            // Create one section for each train
            strResp += "<h1>Gestion des trains</h1><TABLE BORDER=\"0\" >";
            for (byte i = 0; i < myParamRail.NumberOfTrains; i++)
            {
                strResp += "<TR><FORM><TD>" + myParamRail.Trains[i].TrainName + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleOutput + "?pw=" + (16 - myParamRail.Trains[i].Speed);
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\"<<\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleCST + "?pw=5";
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\"<\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleOutput + "?pw=8";
                //strResp = WebServer.OutPutStream(response, strResp);
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\"Stop\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleCST + "?pw=4";
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\">\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleOutput + "?pw=" + myParamRail.Trains[i].Speed;
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\">>\"></TD>";
                strResp += "<TD><span id='train" + i + "'></span></FORM></TD></TR>";
                //strResp = WebServer.OutPutStream(response, strResp);
            }
            strResp += "</TABLE><br>";
            for (byte i = 0; i < myParamRail.NumberOfSwitchs; i++)
            {
                strResp += "<span style='position: absolute;margin-left:" + myParamRail.Switchs[i].Left + "px; margin-top:" + myParamRail.Switchs[i].Top + "px; top:findTop(document.all.MyImage); left:findLeft(document.all.MyImage);' >";
                strResp += "<img border='0' src=\"\" id='switch" + i + "' onClick=\"swclicked(document.getElementById('switch" + i + "'),'si=" + i + "&no=1');\" /></span>";


                //strResp += "<TR><FORM><TD>" + myParamRail.Switchs[i].Name + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=0&no=1')\" value=\"|\"></TD>";
                //strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=1&no=1')\" value=\"/\"></TD>";
                //strResp += "<TD><span id='switch" + i + "'></span></FORM></TD></TR>";
            }
            strResp += "<img alt='Map of the city' id='MyImage' src='file/circuit.png' /><a href='all.aspx?" + securityKey + "'>Display all page</a>";
            strResp += "</body></html>";
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }

        private GetResponse ProcessDisplayDefault(string rawUrl)
        {
            string strResp = "";
            // Start HTML document
            strResp += "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            strResp += "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Gestion des trains</title>";
            //this is the css to make it nice :-)
            strResp += "<link href=\"file/" + pageCSS + "\" rel=\"stylesheet\" type=\"text/css\" />";
            strResp += "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head><body>";
            strResp += "<meta http-equiv=\"Cache-control\" content=\"no-cache\"/>";
            strResp += "<meta http-equiv=\"EXPIRES\" content=\"0\" />";
            //create the script part
            strResp += "<SCRIPT language=\"JavaScript\">";
            strResp += "var xhr = new XMLHttpRequest(); function btnclicked(boxMSG, cmdSend) { boxMSG.innerHTML=\"Waiting\";";
            strResp += "xhr.open('GET', cmdSend + '&" + securityKey + "&_=' + Math.random());";
            strResp += "xhr.onreadystatechange = function() {if (xhr.readyState == 4) {boxMSG.innerHTML=xhr.responseText;}}; xhr.send();}";
            strResp += "</SCRIPT>";
            //strResp = WebServer.OutPutStream(response, strResp);
            // Create one section for each train
            strResp += "<h1>Gestion des trains</h1><TABLE BORDER=\"0\" >";
            for (byte i = 0; i < myParamRail.NumberOfTrains; i++)
            {
                strResp += "<TR><FORM><TD>" + myParamRail.Trains[i].TrainName + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleOutput + "?pw=" + (16 - myParamRail.Trains[i].Speed);
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\"<<\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleCST + "?pw=5";
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\"<\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleOutput + "?pw=8";
                //strResp = WebServer.OutPutStream(response, strResp);
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\"Stop\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleCST + "?pw=4";
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\">\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('train" + i + "'),'" + pageSingleOutput + "?pw=" + myParamRail.Trains[i].Speed;
                strResp += "&op=" + (int)myParamRail.Trains[i].RedBlue + "&ch=" + (myParamRail.Trains[i].Channel - 1) + "')\" value=\">>\"></TD>";
                strResp += "<TD><span id='train" + i + "'></span></FORM></TD></TR>";
                //strResp = WebServer.OutPutStream(response, strResp);
            }

            for (byte i = 0; i < myParamRail.NumberOfSwitchs; i++)
            {
                strResp += "<TR><FORM><TD>" + myParamRail.Switchs[i].Name + "</TD><TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=0&no=1')\" value=\"|\"></TD>";
                strResp += "<TD><INPUT type=\"button\" onClick=\"btnclicked(document.getElementById('switch" + i + "'),'" + pageSwitch + "?si=" + i + "&md=1&no=1')\" value=\"/\"></TD>";
                strResp += "<TD><span id='switch" + i + "'></span></FORM></TD></TR>";
            }
            strResp += "</TABLE><br><a href='all.aspx?" + securityKey + "'>Display all page</a><br><a href='circ.aspx?" + securityKey + "'>Circuit</a>";
            strResp += "</body></html>";
            return new GetResponse(GetResponse.ResponseStatus.OK, strResp);
        }
    }
}
