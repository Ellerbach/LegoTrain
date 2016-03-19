# LegoTrain
IR Lego Power Function for train, switch control with servo motor, light signal. 

This project allow to control any Lego Power Function and support all Lego modes including exclusive modes which can't be done thru any of the Lego remote control. 
A specific module allow to control servo motors used to pilot Lego switches. Another module allow to pilot red/green signal lights.

## Infrared
The infrared module require a bit of electronic. You'll need a to drive enough power in the infrared led to be able to control trains or other Lego Power Functions in large rooms or with a lot of lights.
For this, you'll need to build the equivalent electronic:
![Electronic for Infrared](/Assets/infrared.jpg)

The MOSI pin is 19 and Enabled is pin number 24 on the Rpi2. Those are the 2 pins you need for the infrared part.

## Switches
Piloting switches require as well specific electronic. In this case multiplexing. Schema is quite simple for this:
![Electronic for switches](/Assets/74HC4515.jpg)
A0 -> GPIO16 (pin 36)
A1 -> GPIO20 (pin 38)
A2 -> GPIO21 (pin 40)
A3 = Ground
E -> GPIO13 (pin 33)
All output to be plugged to the driving pin of every servo motor. Servo motor to be at +5V voltage. The level of high for the 4515 is higher than the 3.3V delivered by the RPI, so no need of level converter to 5V.

For the hardware part, you'll need simple servo motor, here is a basic view on how to integrate them with the Lego switch:
![Servo motor integration](/Assets/switches.jpg)