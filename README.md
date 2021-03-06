# LegoTrain
IR Lego Power Function for train, switch control with servo motor, light signal. 

This project allow to control any Lego Power Function and support all Lego modes including exclusive modes which can't be done thru any of the Lego remote control. 
A specific module allow to control servo motors used to pilot Lego switches. Another module allow to pilot red/green signal lights.
Check [this video](https://onedrive.live.com/redir?resid=ED3080C73007CBED!161097&authkey=!AG2N5T7Qx6PtG3A&ithint=video%2cMP4) to see in action the project in a real Lego city!

## Infrared
The infrared module require a bit of electronic. You'll need a to drive enough power in the infrared led to be able to control trains or other Lego Power Functions in large rooms or with a lot of lights.
For this, you'll need to build the equivalent electronic:
![Electronic for Infrared](/Assets/infrared.jpg)

The MOSI pin is 19 and Enabled is pin number 24 on the Rpi2. Those are the 2 pins you need for the infrared part.

## Switches
Piloting switches require as well specific electronic. In this case multiplexing. Schema is quite simple for this:
[schema of the 74HC4515](/http://www.nxp.com/documents/data_sheet/74HC_HCT4515_CNV.pdf)
* A0 -> GPIO16 (pin 36)
* A1 -> GPIO20 (pin 38)
* A2 -> GPIO21 (pin 40)
* A3 -> GPIO19 (pin 35)
* E -> Ground
* LE -> GPIO13 (pin 33)
All output to be plugged to the driving pin of every servo motor. Servo motor to be at +5V voltage. The level of high for the 4515 is higher than the 3.3V delivered by the RPI, so no need of level converter to 5V.

For the hardware part, you'll need simple servo motor, here is a basic view on how to integrate them with the Lego switch:
![Servo motor integration](/Assets/switches.jpg)

##Signals
Piloting signals is quite straight forward using the a 74HC595. You can virtually chain as many as you want to have as many signals.
Please note that so far, it is limited in the code to 16. 
![Electronic for signals](/Assets/signal.jpg)
Every 74HC595 can handle 4 signals, chain them as for any 74HC595.
* Pin 8 is ground
* Pin 16 is VCC (+5V recommended)
* Pin 14 to MOSI (pin 19) for the first 74HC595 of the chain. For the next one it has to be linked to the Pin 9 of the previous 74HC595
* Pin 13 to ground (used to activate or not the 8 output)
* Pin 12 to GPIO7 (pin 26), used to select the SPI
* Pin 11 to GPIO11 pin 23 (called SCLK)
* Pin 10 to +VSS (this one is used to reset if needed)
* Pin 15, 1, 2, 3, 4, 5, 6, 7 are the output pin
* Pin 9 has to be linked to the next 74HC595 of the chain if there are more than 1.

## Setup files
The project is using a setup file which contains all the train name, position of the switches, IR information. This is done thru the Excel file you can find [here](/Docs/Creating%20setup%20file.xlsx).
Just copy the generated string and paste it to the file named [ParamTrain](/To%20deploy%20on%20device/ParamTrain.txt). Save it and deployit to the Raspberry.
![File to copy](/Assets/file_to_copy.png)
Copy the files from this location to the Raspberry into the user Package LocalState folder.
You can create your own circuit picture using for [example PowerPoint](/Docs/Creating%20the%20circuit%20file.pptx), exporting it as an image (name must be circuit.png).
Please note you can change all the train names, switches names as well. So far only use Ascii 7 codes, so accent and other non core Ascii caracters.
The security key is required, you can leave it empty if you don't want any but need to add it to the URL in all cases like ?sec=

## Loading the project and referencing other projects
When loading the LegoTrain.sln solution, you'll need as well the following projects:
* [RPI-Win10-Helpers](https://github.com/Ellerbach/RPI-Win10-Helpers)
* My own fork of the excellent [Restup webserver](https://github.com/Ellerbach/restup)
Note: working on using only the master of the Restup project. This will be done later on.

## Running the project
Once deployed on the Raspberry, you can make your project default project and it will be launched at boot time.
Access it like http://IPaddress/circ.aspx?sec=Key234
If all is correct, you should see a page like that:
![Train](/Assets/train.jpg)

## Using the API
API are available. Documentation in [this file](/Docs/Lego%20Infrared%20API%20doc.md). This does allow to pilot any Lego IR element, more than just trains. A test fuction for all the elements is available using the page which displays all the elements.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
