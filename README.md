# ZoneLighting

## PROJECT SUMMARY

ZoneLighting is a .NET application to create pixel-based lighting for your home or office (or any other space). It abstracts out the logical view of the lights in "zones", which can each run a "zone program". Each zone has a "lighting controller", which is a C# wrapper for lighting controllers such as the FadeCandy board. Zone programs can output any pattern of lights using a zone's lighting controller. The program running on the zone must take the physical configuration into account. Hardware support is currently limited to FadeCandy boards with WS2812 RGB LEDs, but the architecture is designed and intended for extensibility.


* **ORIGINAL AUTHOR**: Anshul Vishwakarma
* **EMAIL**: shootdaj@gmail.com
* **REQUIRED SOFTWARE**: Windows with .NET runtime installed (latest version - 4.5 as of writing). Requires C# 6.0 (Visual Studio 2015 Preview) to compile.
* **REQUIRED HARDWARE**: Lighting Hardware such as FadeCandy with WS2812 strips or (currently unsupported) an Arduino-based LPD8806 RGB LED Strip
* **LICENSE**: MIT (See file LICENSE)


## USER/PROGRAMMER GUIDE:

###Basic Concepts

This application is a software layer that is designed to give the user very powerful, modular, customizable, and high-level control over lights. This statement may bring up more questions than it answers. What kind of lights? Currently it only supports the the WS2812 light strips (http://www.adafruit.com/product/1138) through the FadeCandy board. FadeCandy is a PCB that connects to a computer/microcontroller over USB and receives lighting commands which are then sent to the light strips attached to the PCB. But I've written this application in a way that allows addition of other lighting controllers. I plan on adding support for LPD8806 (http://www.adafruit.com/product/306) through Arduino or Raspberry Pi. 

If you are wondering why you would use this application instead of just writing your own code on top of FadeCandy, then I'd ask you why you want to reinvent the wheel. This application does exactly that, it's a software layer on top of lighting controller stacks and it gives you, as a programmer of lights, the ability to program them on a much higher level using zones rather than controlling individual lights. FadeCandy, for example, is not just a PCB. It's a stack. It needs multiple parts to function - it needs the PCB, the light strips attached to the PCB, the USB connection to the computer, and the server executable running that relays websocket requests to the PCB, which are then relayed to the light strips. This entire stack is abstracted in ZoneLighting as a single class called FadeCandyController. This allows you to focus on the higher level functionality of your application and algorithms. If you want to create a program that scrolls a dot across all the lights, the program can be written to work on any lighting controller stack without modifying the program by just changing the lighting controller on which the program is running. 

###Zones and Lights

This application is based on the concept of zones .A zone is a group of lights that can be controlled as a unit. Imagine you are creating a display made of LEDs or other form of light (bulbs, black lights, stage lighting etc.). You want to be able to control what goes on that display. Zones allow you to group lights together and execute various lighting programs (such as loops or reactive programs like notifications from other sources) on those zones. This technique may sound familiar to video game programmers, where sprites are used instead of pixel manipulation to create the illusion of movement within the games. Zones can be seen as a way to control the lights in your display using a higher level abstraction that gives your lighting application more structure. Using individual lights to construct the entirety of your lighting application would be akin to building a LEGO structure entirely out of a single kind of LEGO brick - the smallest kind. It would be possible, but it would be unmanageable and inefficient, and it pollutes the higher-level structures with lower-level complications. It is for the same reason that an architect does not think of a building in terms of bricks and cement, but rather in terms of components such as floors, rooms, and supporting structures. Building complex structures requires a degree of modularity, or zoning, to allow for a more holistic view of the structure. That is the concept of a zone - a way to encapsulate many lights into a structure that acts as a unit. These can be seen as the unit building blocks of your lighting application, but there is no restriction for zones to be a certain way so you can create zones of any "shape" (following the LEGO analogy).

Q: What does a zone look like? 
A: In the most basic sense, it is an enumerable collection of lights. A light, as defined in this application, is nothing more than a contract - an interface - defined as IRGBLight. This tells us a few things about what the light is and what it can do. First, it tells us that the interface expects an RGB color to be representable by its implementation, but if a light doesn't support full RGB, it can throw an exception during its API calls when the RGB values are invalid. It's important to note that this is a logical representation of a light. The implementation of this interface does not require the implementer to interact with the physical light. That is the job of the lighting controller (ILightingController). This interface has three methods that it expects the subclass to define: 

1. bool SetColor(Color color) - Expects the subclass to define how to set the color of the given light. Note that this does not mean how to physically set the color of the light, but how to set it in the logical view of the light. There is a logical view of the lights and a physical view. The logical view acts as a filter to filter out invalid values of color, for example (or other kinds of filtering). If the subclass of this interface is used to represent an incandescent light, which can only display lights of a certain color (yellowish to whiteish), then the SetColor method can be programmed to return false or throw an error when the requested color is not in the range of colors that can be produced on the light. In this way, we do need to know something about the physical lights that the subclass will represent, but this interface's implementation does not need to know how to physically change the color of the light. That will be done using the physical definition of the light in terms of a specific controller as we will see in a bit.

2. int LogicalIndex - This property defines the address of the light in the logical view. This is useful in things like sorting the lights etc. This number is different from the physical index that will be defined in the concrete implementation of the light. The physical index will be specific to the lighting controller, but the logical index is not.

3. Color GetColor() - Expects the subclass to define how to get the current color of the light.

4. Color Color - This is just a placeholder at the moment, but it may be

Now let's look at an implementation of this interface called LED. The LED class represents a single LED in a zone. The way I've programmed this, it needs to implement interfaces for each type of lighting controller. So for  It contains an instance of FadeCandyPixel, which contains all information required by the FadeCandyController to output this light on a FadeCandy board.








 Each light needs to be addressable and these addresses should be usable by the lighting controller that is outputting the zone's lights. These addresses are specific to the type of lighting controller. So for the FadeCandyController, the addresses are stored in a property of LED each LED has a property FadeCandyPixel which is used by FadeCandyController to locate the LED using the address defined by the 


 If you plan on moving the zone to another lighting controller,

###Zone Programs

I mentioned earlier that as a user, you can execute programs on a zone. What exactly is a program? It can be anything ranging from a static loop to a responsive program. A few examples are programs that scroll a dot 



I believe that the best way to explain such an abstract concept is to show an example of how it is intended to be used. So I will do just that:

I created this application Zone Lighting to allow for creating some mood lighting in my basement. The lighting is laid out as such:



		[------------------]												[-------------------]
		[                  ]												[					]
		[    Left Wing     ]												[	  Right Wing	]
		[                  ]												[					]
		[------------------]												[-------------------]

				\																/
				  \															  /	
					\														/
					  \													  /	
						\												/
						  \											  /
							\										/
							  \									  /
								\	[-------------------------] /
								  \	[						  ]
									[		FadeCandy 		  ]
									[	Lighting Controller   ]
									[						  ]
									[-------------------------]



Left Wing and Right Wing are sets of lights that sit on the left and right side of my computer monitor, respectively. They are both cut from a long strip of WS2812s. WS2812 is an electronic component that is a combination of an RGB LED and a driver chip that handles PWM, I/O buffering and propagation, timing issues, and other details that are required for a high-level control of an RGB LED or a series of RGB LEDs. 

**Note**: WS2812s are are sometimes (incorrectly) referred to as "WS2811". Even though for most practical purposes the names WS2811 and WS2812 are interchangeable, it's important to realize that the WS2811 (http://www.adafruit.com/datasheets/WS2811.pdf) is the name for the driver chip that drives the RGB LED, and WS2812 (http://www.adafruit.com/datasheets/WS2812.pdf) is the name for the entire component, including the LED and the driver chip. But since the WS2811 driver chip is essentially useless without the LED, the name "WS2811" is many times used interchangeably with WS2812. Many vendors sell WS2812s as WS2811, so if searching vendor sites to buy the WS2812, try searching for WS2811 also. The Adafruit product known as NeoPixel is the same as the WS2812, rebranded for Adafruit.

**This project was started as an abstraction on top of the FadeCandy board** (https://github.com/scanlime/fadecandy). FadeCandy is a product created by Github user scanlime. It consists of a physical hardware board that is able to control 512 WS2812s as 8 strips of 64 WS2812s and the software component which consists of a program called FCServer that connects to the physical board over USB and listens for requests over WebSockets in the format formalized by the Open Packet Control protocol (which was also created by scanlime). FCServer redirects these WebSocket requests to the physical FadeCandy board. These may be requests to change the color of the lights or administrative requests like enabling/disabling dithering or interpolation. **I wanted to create a software layer that abstracts out the entire FadeCandy stack as a generic lighting controller**. This generalization is intended to allow for adding facilities to control non-WS2812 lighting such as the LPD8806 (individually-addressable RGB LED strip that needs manual PWM and a clock signal), regular LEDs using a board like Arduino or similar, or regular lamps that plug into the wall using X10 or Insteon. This also allows multi-layered configurations and to "transfer" a zone configuration to a different physical configuration without changing anything in the logical layer. In short, this application heavily enforces separation of concerns by using a layered architecture.

Currently, the application only supports FadeCandy as the lighting controller and what I call a FadeCandyPixel as the physical unit of a "pixel". A FadeCandyPixel represents a single WS2812 in a series of WS2812s. These are contained in the logical class LED such that each instance of an LED contains an instance of a FadeCandyPixel. In essence, the LED class is a wrapper around the various physical representations of an LED, such as FadeCandyPixel. In the future, if support for LPD8806 is added, a new class could be created called LP8806Pixel (inheriting from PhysicalRGBLight), which would represent a physical LED on the LPD8806. An instance of this class would be added as a property to the LED class to allow the LED be output on the LPD8806. This property would be used by the corresponding lighting controller class LPD8806Controller to output the LED to the LPD8806. Many of the properties contained within the FadeCandyPixel class would also be contained in the LP8806Pixel class, such as PhysicalIndex, RedIndex, GreenIndex, and BlueIndex, but the implementations of these would be different based on how the various positions and colors on the LPD8806 are addressed. In some RGB strips, the positions of blue and green colors are switched, so that would be a case where the implementation of the BlueIndex and GreenIndex would differ from the FadeCandyPixel class.

**I've designed this project with the intention of other people extending it**. To add a new lighting controller, all one needs to do is extend the abstract LightingController class. To add new types of lights, all that's required is to extend the abstract PhysicalRGBLight class. For examples of how to do these things, look at the implementations of the FadeCandyController and FadeCandyPixel classes, respectively.

The FadeCandyController class is implemented as a singleton, simply because at the time of writing this code, it was anticipated that only a single FadeCandy board will be used with this program. If someone finds that using more than one board is necessary, it's very easy to convert it from a singleton to a non-singleton class. The instances will live inside the ZoneLightingManager class as a property.

Speaking of ZoneLightingManager (ZLM), it is worth talking a bit about this class and its function. This is what you can call the "runner" class for the entire application. All high-level operations like initializing/uninitializing zone programs, zones, or lighting controllers are done using this class. It is implemented as a singleton. To access the singleton instance, use ZoneLightingManager.Instance (this is the practice followed throughout the entire application for accessing singleton instances for any given class). ZLM is also responsible for adding/removing zone data. Currently it is adding data in the LoadSampleZoneData() method based on the configuration I have in the ASCII diagram above. In LoadSampleZoneData(), it adds two zones that are to be controlled by a FadeCandy board, leftWingZone and rightWingZone. These zones are added to the Zones property of ZLM, which is a list of Zones that ZLM can manage. When the Initialize() method is called on ZLM, it initializes all lighting controllers, loads zone data, and initializes all zones contained in the Zones property. Initializing zones entails selecting a zone program for each zone and starting it with some parameters.

What exactly is a zone program? **A zone program is essentially a piece of code (program) that can output anything it needs to a zone's lights**. An example of a zone program is some code that loops the colors of a rainbow on a zone (or any other collection of colors). Another example is a program that checks your email every so often and blinks the lights in a zone in a certain pattern when you get a new email. Another example is a program that just "scrolls" a dot across the lights in a zone such by turning the appropriate lights on and off such that a "dot" appears to be moving across the lights of that zone. So really, a zone program in this sense is any program that outputs a lighting pattern on a given zone. This pattern may or may not be based on external stimuli.

## FINAL NOTES AND FUTURE PLANS

This is my first open-source project, and therefore there are bound to be issues, errors, and incompatibilities. This project also has no tests mainly because it was something I hacked up in a few days just to get the FadeCandy board working. My intention is to add tests in the functional parts of the application where things can potentially break. This is one of the main priorities. Other priorities are to add support for more hardware controllers and more kinds of lighting. Also need to add support to be able to load programs that are not hard-coded but are instead either loaded using reflection or using configuration files (or a combination of both). This will allow the application to be modular and programs should eventually be able to be downloaded and "installed". It is also planned to add zone subtypes, and enforce which programs can run on which zone subtypes. This will introducing a much more predictable way programs run on zones, because that essentially would allow the person programming the zone program to specify what kind of zones a program can run on. The "type" of zone will entail things like how many pixels are available to display on, what configuration the pixels are in, and other information about the zone configuration.


Please feel free to create issues in the Github issue tracker, and if you have any specific questions directed towards me, please feel free to email me. I will try to respond as timely as possible.