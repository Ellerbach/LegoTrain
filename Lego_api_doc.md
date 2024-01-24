# Lego Infrared and Signal/Switch API documentation

## Purpose

This documentation has the purpose to explain how to pilot and interact with the Lego Infrared and Lego Signal and Switch boards and pilot either a Lego train, or any other FPS Lego element. This project requires to use the  either a [Lego Infrared](./LegoInfrared/) module, either a [Lego Singal/Switch](./SignalSwitch/) one.

Note that the code is built for ESP32 devices. This should be able to work with any ESP32 device.

## Introduction

The board can be piloted either thru HTTP or serial. Both are using a very similar protocol and are command (URL) based with arguments.

## Web server configuration

The WebServer is configured by default on port 80. The only way to change it is in the code. If you need to change the port because you want to expose it on the internet, I recommend you to change the route in your router and not the port on the device.

## Configuration of the elements

 You can go to the [<http://ipaddress/configuration/config>](http://ipaddress/configuration/config). Depending on the type of board, you'll get a different configuration page.

## HTTP Communication protocol

The communication protocol looks like: [<http://ipaddress/api/combo?rd=3&bl=0&ch=1>](http://ipaddress/api/combo?rd=3&bl=0&ch=1)

The answer is either 200 OK either Bad Request 400.

* ipaddress: This is the IP Address or name of your device.
* combo This is the name of the page called.
* There is a series of parameters depending on what you are calling.

Here is the list of all available arguments. Please note that none of the arguments or the page name is case sensitive. They **must** be lowercase.

## Piloting a Lego FPS

The Lego communication is quite complex and there are many options. I do recommend you first get familiar with the Lego communication protocol before trying to pilot your own elements.

## Pre reading and important notice

Please read the [Lego protocol](./Assets/LEGO_Power_Functions_RC_v110.pdf) before reading the next section. Note that those documents are Lego property and they are checked in for convenience. Please refer to the copyright notice in the document.

The infrared protocol is over the air and it may happen that the receptor won't see it correctly. **You are never sure that the order you send will arrive**. This is not due to my system but due to the fact that infrared communications are very sensitive with other infrared activities. That as it's lights, even if they may be reflected on walls, an obstacle can stop it and avoid the signal to be received.

Due to the Lego protocol itself any order takes about 1 second to be output totally. So don't try to pilot too many elements with single commands at the same time or with a short timing. I've implemented special functions to be able to pilot up to 8 elements during the same command (so in 1 second).

You can also read those 2 blog posts to understand the way the board is working: [Using Netduino and .NET Microframework to pilot a Lego Power Function (part 1)](https://ellerbach.github.io/blog-posts/2012-04-07-Using-netduino-and-.NET-Microframework-to-pilot-any-Lego-Power-Function-thru-Infrared-(part-1).html), [Using Netduino and .NET Microframework to pilot a Lego Power Function (part 2)](https://ellerbach.github.io/blog-posts/2012-04-17-Using-netduino-and-.NET-Microframework-to-pilot-any-Lego-Power-Function-thru-Infrared-(part-2).html) and [Using Netduino and .NET Microframework to pilot a Lego Power Function (part 2)](https://ellerbach.github.io/blog-posts/2012-04-26-Using-netduino-and-.NET-Microframework-to-pilot-any-Lego-Power-Function-thru-Infrared-(part-3).html). The code in this repository is the successor of this more than 10 years old .NET Microframework code!

They also explain a bit how the Lego protocol is working.

## Differences between HTTP and Serial

As for the signals and switches piloting there are only few differences between the web server and serial piloting.

A web server request looks like [http://ipaddress/combo?rd=0&bl=0&ch=0&sec=key123](http://ipaddress/combo?rd=0&bl=0&ch=0&sec=key123)

The same serial request looks like combo?rd=0&bl=0&ch=0

In both cases the Return values is either OK\r or Problem\r as pure text without any http header. \r is the return character.

OK is returned if everything was correct. Problem if a value is not correct, or any other error appeared.

As for signals and switches protocols, the command and arguments are not case sensitive. I do recommend to use lowercase characters.

In the description of following sections, I will use the simplified serial version as the 'sec' argument has to be present in all the calls for HTTP.

## Combo Mode

This mode is able to control: Two outputs float/forward/backward/brake.

This is a combo command controlling the state of both output Red and Blue at the same time.

This mode has timeout for lost IR.

Usage: `combo?rd=1&bl=8&ch=2`

### rd

this argument control the red output. Accepted values are the following:

* No change = 0 (0x0)
* Forward = 1 (0x1)
* backward = 2 (0x2)
* Break = 3 (0x3)

### bl

this argument control the blue output. Accepted values are the following:

* No change = 0 (0x0)
* Forward = 4 (0x4)
* backward = 8 (0x8)
* Break = 12 (0xC)

### ch

this argument selects the channel of the receptor. Accepted values goes from 0 to 3 for channel 1 to 4.

## Single pin continuous Mode

This mode is able to control: Clear/set/toggle of an individual pin C1 or C2 on output red or blue.

This mode has no timeout for lost IR.

Usage: `continuous?fc=2&op=2&ch=2`

### fc

this argument select what you want to do with the specific pin output, accepted values are the following:

* No change = 0 (0x0)
* Clear = 1 (0x1)
* Set = 2 (0x2)
* Toggle = 3 (0x3)

### op

this argument select the output and pin you want to control, accepted values are the following:

* Red PINC1 = 0 (0x0)
* Red PINC2 = 1 (0x1)
* Blue PINC1 = 2 (0x2)
* Blue PINC2 = 3 (0x3)

### ch

this argument select the channel of the receptor. Accepted values goes from 0 to 3 for channel 1 to 4.

## Single pin timeout Mode

This mode is able to control: Clear/set/toggle of an individual pin C1 or C2 on output red or blue.

This mode has timeout for lost IR.

Usage: `timeout?fc=1&op=1&ch=1`

### fc

this argument select what you want to do with the specific pin output, accepted values are the following:

* No change = 0 (0x0)
* Clear = 1 (0x1)
* Set = 2 (0x2)
* Toggle = 3 (0x3)

### op

this argument select the output and pin you want to control, accepted values are the following:

* Red PINC1 = 0 (0x0)
* Red PINC2 = 1 (0x1)
* Blue PINC1 = 2 (0x2)
* Blue PINC2 = 3 (0x3)

### ch

this argument select the channel of the receptor. Accepted values goes from 0 to 3 for channel 1 to 4.

## Single output Mode with PWM

This mode is able to control: One output at a time with PWM.

This mode has no timeout for lost IR.

This function is most likely use to set a nominative speed to a motor.

Usage: `singlepwm?pw=4&op=1&ch=3`

### pw

this argument indicate which PWM value the motor (or lamp) has to take, accepted values are the following:

* Float = 0 (0x0)
* PWM forward step 1 = 1 (0x1)
* PWM forward step 2 = 2 (0x2)
* PWM forward step 3 = 3 (0x3)
* PWM forward step 4 = 4 (0x4)
* PWM forward step 5 = 5 (0x5)
* PWM forward step 6 = 6 (0x6)
* PWM forward step 7 = 7 (0x7)
* Brake = 8 (0x8)
* PWM backward step 7 = 9 (0x9)
* PWM backward step 6 = 10 (0xA)
* PWM backward step 5 = 11 (0xB)
* PWM backward step 4 = 12 (0xC)
* PWM backward step 3 = 13 (0xD)
* PWM backward step 2 = 14 (0xE)
* PWM backward step 1 = 15 (0xF)

### op

this argument select the output either red = 0 or blue = 1.

### ch

this argument select the channel of the receptor. Accepted values goes from 0 to 3 for channel 1 to 4.

## Single output Mode Clear Set Toggle

This mode is able to control: One output at a time with clear/set/toggle control pins.

This mode has no timeout for lost IR on all commands except "full forward" and "full backward".

Usage: `singlecst?pw=4&op=1&ch=2`

### pw

this argument is used to select the right behavior on pins. The following values are accepted:

* Clear C1 + Clear C2 = 0 (0x0)
* Set C1 + Clear C2 = 1 (0x1)
* Clear C1 + Set C2 = 2 (0x2)
* Set C1 + Set C2 = 3 (0x3)
* Increment PWM = 4 (0x4)
* Decrement PWM = 5 (0x5)
* Full forward (timeout) = 6 (0x6)
* Full backward (timeout) = 7 (0x7)
* Toggle full forward/backward (0x8 - default forward)

### op

this argument select the output either red = 0 or blue = 1.

### ch

this argument select the channel of the receptor. Accepted values goes from 0 to 3 for channel 1 to 4.

## Combo PWM Mode

This mode is able to control: Two outputs with PWM in 7 steps forward and backward.

This is a combo command controlling the state of both output red and blue at the same time.

This mode has timeout for lost IR.

Usage: `combopwm?p1=4&p2=10&ch=1`

### p1

this argument indicate which PWM value the motor (or lamp) has to take, accepted values are the following:

* Float = 0 (0x0)
* PWM forward step 1 = 1 (0x1)
* PWM forward step 2 = 2 (0x2)
* PWM forward step 3 = 3 (0x3)
* PWM forward step 4 = 4 (0x4)
* PWM forward step 5 = 5 (0x5)
* PWM forward step 6 = 6 (0x6)
* PWM forward step 7 = 7 (0x7)
* Brake = 8 (0x8)
* PWM backward step 7 = 9 (0x9)
* PWM backward step 6 = 10 (0xA)
* PWM backward step 5 = 11 (0xB)
* PWM backward step 4 = 12 (0xC)
* PWM backward step 3 = 13 (0xD)
* PWM backward step 2 = 14 (0xE)
* PWM backward step 1 = 15 (0xF)

### p2

this argument indicate which PWM value the motor (or lamp) has to take, accepted values are the following:

* Float = 0 (0x0)
* PWM forward step 1 = 1 (0x1)
* PWM forward step 2 = 2 (0x2)
* PWM forward step 3 = 3 (0x3)
* PWM forward step 4 = 4 (0x4)
* PWM forward step 5 = 5 (0x5)
* PWM forward step 6 = 6 (0x6)
* PWM forward step 7 = 7 (0x7)
* Brake = 8 (0x8)
* PWM backward step 7 = 9 (0x9)
* PWM backward step 6 = 10 (0xA)
* PWM backward step 5 = 11 (0xB)
* PWM backward step 4 = 12 (0xC)
* PWM backward step 3 = 13 (0xD)
* PWM backward step 2 = 14 (0xE)
* PWM backward step 1 = 15 (0xF)

### ch

this argument select the channel of the receptor. Accepted values goes from 0 to 3 for channel 1 to 4.

## Combo PWM All Mode

This mode is able to control the 8 outputs with PWM in 7 steps forward and backward at the same time!

This is a combo command controlling the state of both output red and blue at the same time.

This mode has timeout for lost IR.

Usage: `combopwmall?pwr0=4&pwb0=10&pwr1=3&pwb1=0&pwr2=5&pwb2=1&pwr3=3&pwb3=10`

### pwrX

X = channel from 0 to 3
this argument indicate which PWM value the motor (or lamp) has to take, accepted values are the following:

* Float = 0 (0x0)
* PWM forward step 1 = 1 (0x1)
* PWM forward step 2 = 2 (0x2)
* PWM forward step 3 = 3 (0x3)
* PWM forward step 4 = 4 (0x4)
* PWM forward step 5 = 5 (0x5)
* PWM forward step 6 = 6 (0x6)
* PWM forward step 7 = 7 (0x7)
* Brake = 8 (0x8)
* PWM backward step 7 = 9 (0x9)
* PWM backward step 6 = 10 (0xA)
* PWM backward step 5 = 11 (0xB)
* PWM backward step 4 = 12 (0xC)
* PWM backward step 3 = 13 (0xD)
* PWM backward step 2 = 14 (0xE)
* PWM backward step 1 = 15 (0xF)

### pwbX

X = channel from 0 to 3
this argument indicate which PWM value the motor (or lamp) has to take, accepted values are the following:

* Float = 0 (0x0)
* PWM forward step 1 = 1 (0x1)
* PWM forward step 2 = 2 (0x2)
* PWM forward step 3 = 3 (0x3)
* PWM forward step 4 = 4 (0x4)
* PWM forward step 5 = 5 (0x5)
* PWM forward step 6 = 6 (0x6)
* PWM forward step 7 = 7 (0x7)
* Brake = 8 (0x8)
* PWM backward step 7 = 9 (0x9)
* PWM backward step 6 = 10 (0xA)
* PWM backward step 5 = 11 (0xB)
* PWM backward step 4 = 12 (0xC)
* PWM backward step 3 = 13 (0xD)
* PWM backward step 2 = 14 (0xE)
* PWM backward step 1 = 15 (0xF)

Note : if any channel is missing the information sent will be Float (0x0).

### Combo Mode All

This special function allow to control at the same time all 4 channels and all red and blue output at 1 time! This is something you cannot do with regular remote control.

As for the normal Combo Mode, this function has timeout on lost IR.

Usage: `comboall?rd0=1&bl0=0&rd1=2&bl1=4&rd2=3&bl2=8&rd3=0&bl3=12`

## rdX

X from 0 to 3 representing channel 1 to 4.

this argument control the red output. Accepted values are the following:

* No change = 0 (0x0)
* Forward = 1 (0x1)
* backward = 2 (0x2)
* Break = 3 (0x3)

### blX

X from 0 to 3 representing channel 1 to 4.

this argument control the blue output. Accepted values are the following:

* No change = 0 (0x0)
* Forward = 4 (0x4)
* backward = 8 (0x8)
* Break = 12 (0xC)

## Single Countinuous Mode All

As for the Single Continuous Mode, this mode is able to control: Clear/set/toggle of an individual pin C1 or C2 on output red or blue but on 4 channels in 1 time. This is something you cannot do with a regular remote control. Like for the normal function, there is no timeout for lost IR.

Usage: `continuousall?fc0=2&op0=0&fc1=1&op1=1&fc2=0&op2=2&fc3=3&op3=2`

### fcX

X from 0 to 3 representing channel 1 to 4.

this argument select what you want to do with the specific pin output, accepted values are the following:

* No change = 0 (0x0)
* Clear = 1 (0x1)
* Set = 2 (0x2)
* Toggle = 3 (0x3)

### opX

X from 0 to 3 representing channel 1 to 4.

this argument select the output and pin you want to control, accepted values are the following:

* Red PINC1 = 0 (0x0)
* Red PINC2 = 1 (0x1)
* Blue PINC1 = 2 (0x2)
* Blue PINC2 = 3 (0x3)

### Single PWM Mode All

As for the normal function, this mode is able to control: One output at a time with clear/set/toggle control pins but on 4 channels at the same time. This is something you can't do with a regular remote control.

This mode has no timeout for lost IR on all commands except "full forward" and "full backward".

Usage: `singlepwmall?pw0=0&op0=0&pw1=6&op1=1&pw2=12&op2=1&pw3=2&op3=0`

### pwX

X from 0 to 3 representing channel 1 to 4.

this argument is used to select the right behavior on pins. The following values are accepted:

* Clear C1 + Clear C2 = 0 (0x0)
* Set C1 + Clear C2 = 1 (0x1)
* Clear C1 + Set C2 = 2 (0x2)
* Set C1 + Set C2 = 3 (0x3)
* Increment PWM = 4 (0x4)
* Decrement PWM = 5 (0x5)
* Full forward (timeout) = 6 (0x6)
* Full backward (timeout) = 7 (0x7)
* Toggle full forward/backward (default forward)

### opX

X from 0 to 3 representing channel 1 to 4.

this argument select the output either red = 0 or blue = 1.

## Signals

You can control the signals to change individual signals.

usage: `signal?si=2&md=0`

### si

This is the signal number id.

### md

This is the color of the led, 0 for red, 1 for green.

## Switches

You can control the switches to change.

usage: `switch?si=2&md=0`

### si

This is the switch number id.

### md

This is how the switch is, 0 for straight, 1 for turned.
