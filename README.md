# LegoTrain

IR Lego Power Function for train, switch control with servo motor, light signal using remote wifi controlled MCU based on [ESP32-C3 super mini](https://www.bing.com/search?q=esp32-c3+super+mini&qs=n&form=QBRE&sp=-1&lq=0&pq=esp32-c3+super+mini&sc=9-19&sk=&cvid=465952776C5548268A0AC7C84DE5E692&ghsh=0&ghacc=0&ghpl=) running [.NET nanoFramework](https://www.nanoframework.net)!

One LegoInfrared module allows to control any Lego Power Function and support all Lego modes including exclusive modes which can't be done thru any of the Lego remote control through API. Please refer to the [LegoInfrared project](https://github.com/Ellerbach/LegoInfrared) for more details. In our case, this module will control trains.

Specific module allow to control servo motors used to pilot Lego switches and to pilot red/green signal lights.
Check [this video](https://onedrive.live.com/redir?resid=ED3080C73007CBED!161097&authkey=!AG2N5T7Qx6PtG3A&ithint=video%2cMP4) to see in action the project in a real Lego city!

## Setting up the wifi for each module

Once the module flashed, you will be able to set the wifi to each module. For this, connect to the `LegoInfrared` wifi that is available, then go to the URL [<http://192.168.4.1>](http://192.168.4.1) from a browser, set the values and you'll be good to go! If you've done something wrong, the wifi hot spot will still be present and you can reenter the credentials.

In case of success, the IP address of the module will be displayed. You can now connect back to your wifi and connect to the IP address of your module. You still need to give the module an ID in the case of the switches/signals ones. Each ID **must** be unique. You will also have to select if you want just the switch, the signal or both.

## Infrared

The infrared module require a bit of electronic. You'll need a to drive enough power in the infrared led to be able to control trains or other Lego Power Functions in large rooms or with a lot of lights.
For this, you'll need to build the equivalent electronic:

![Electronic for Infrared](/Assets/infrared.jpg)

The module is configured. You can change the default values if you're using a different model and make your own electronic.

## Switches

Servo motor to be at +5V voltage. The control pin should be connected to the configured pin.

For the hardware part, you'll need simple servo motor, here is a basic view on how to integrate them with the Lego switch:

![Servo motor integration](/Assets/switches.jpg)

## Signals

Piloting signals is quite straight forward and you need a little bit of electronic:

![Electronic for signals](/Assets/signal.jpg)

## Circuit, signals and switches file

You have a default circuit file that you can edit. A PowerPoint can be used to quickly create the circuit and the elements. Note [the name of the files](./LegoTrain/config) which **must** be the same.

## Running the project

To be done later. This section will include how to package the project and run it on a Raspberry PI or equivalent. Also how to run it on Docker and the necessary settings.

## Using the API

API are available. Documentation in [this file](./Lego_api_doc.md). This does allow to pilot any Lego IR element, more than just trains.
