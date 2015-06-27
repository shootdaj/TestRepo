# ZoneLighting

## PROJECT SUMMARY

ZoneLighting is a .NET application to create pixel-based lighting. It abstracts out the logical view of the lights in "zones", each of which can run a "zone program". Each zone has a "lighting controller", which is a C# wrapper for lighting controllers such as the FadeCandy board or an Arduino. Zone programs can output any pattern of lights using a zone's lighting controller. The program running on the zone must take the physical configuration into account. Hardware support is currently limited to FadeCandy boards with WS2812 RGB LEDs, but the architecture is designed and intended for extensibility.


* **ORIGINAL AUTHOR**: Anshul Vishwakarma
* **EMAIL**: shootdaj@gmail.com
* **REQUIRED SOFTWARE**: Windows with .NET runtime installed (latest version - 4.5 as of writing). Requires C# 6.0 (Visual Studio 2015 Preview) to compile.
* **REQUIRED HARDWARE**: Lighting Hardware such as FadeCandy with WS2812 strips or (currently unsupported) an Arduino-based LPD8806 RGB LED Strip
* **LICENSE**: MIT (See file *LICENSE*)


## USER/PROGRAMMER GUIDE:

**This guide is incomplete. I will be adding to it as I get time and the project takes shape. I'm still in the experimentation phase and this project may not always be in a buildable state and it may not be usable without setting up some external things. I can help anyone set it up, so if needed, please email me or post it in the issue tracker, and I will help you get your environment setup. This is a preliminary documentation and as the project matures, it will change and become more comprehensive.**

###Project Structure

The project is organized in the following manner:

ZoneLighting.sln
	| - ZoneLighting.csproj - This is the main class library that does all the work.
	| - WebController.csproj - Web interface for the application
	| - Console - A simple runner used mainly for debugging and/or testing
	| - ExternalPrograms - A sample external program library provided for examples and to get started
	| - ExternalZones - A sample external zone library provided for examples
	| - ZoneLightingTests - Tests for the project ZoneLighting.csproj
	
###Basic Concepts

This application is a software layer that is designed to give the user very powerful, modular, customizable, and high-level control over lights. A light in this context is anything that can be controlled (turned on and off or dim) using a C# wrapper. A C# wrapper can be written on top of pretty much any kind of light, so this application can be used to control any kind of light ranging from regular LEDs, RGB LEDs, lamps that plug into the wall (using X10 or another smart home technology), or any other light that can be controlled by a microcontroller. LEDs and RGB LEDs can be controlled using any microcontroller such as Arduino, Raspberry Pi, or FadeCandy.

Currently this application only supports WS2812 lights (http://www.adafruit.com/product/1138) through the FadeCandy board. FadeCandy is a PCB that connects to a computer/microcontroller over USB and receives lighting commands which are then sent to the light strips attached to the PCB. But I've written this application in a way that allows addition of other lighting controllers. I plan on adding support for LPD8806 (http://www.adafruit.com/product/306) through Arduino or Raspberry Pi. 

An alternate way of looking at this application is as a software layer on top of lighting controller stacks. It gives you, as a programmer of lights, the ability to program lights and lighting effects on a much higher level using zones rather than individual lights. FadeCandy, for example, is not just a PCB. It's a stack. It needs multiple parts to function - it needs the PCB, the light strips attached to the PCB, the USB connection to the computer, and the server executable that relays websocket requests to the PCB, which are then relayed to the light strips. This entire FadeCandy stack is abstracted in ZoneLighting as a single class called FadeCandyController. This allows you to focus on the higher-level functionality of your application and algorithms. If you want to create a program that scrolls a dot across all the lights (which is provided as a stock program), the program can be written to work on any lighting controller stack without modifying the program by just changing the lighting controller on which the program is running (barring any inherent incompatibilities such as the interpolate and dither modes in FadeCandy).

###Zones and Lights

This application is based on the concept of zones. A zone is a group of lights that can be controlled as a unit. Imagine you are creating a display made of LEDs or other form of light like bulbs, black lights, or DMX stage lighting. You want to be able to control what goes on that display. Zones allow you to group those lights together and execute various lighting programs such as loops or reactive programs like notifications from other sources on those zones. This technique of grouping lights together may sound similar to video game sprites, where groups of pixels are used instead of individual pixel manipulation to create the illusion of movement within the games. Zones can be seen as a way to control the lights in your display using a higher-level abstraction that gives your lighting application more structure. Using individual lights to construct the entirety of your lighting application would be akin to building a LEGO structure entirely out of a smallest kind of LEGO brick. It would be possible, but it would be unmanageable and inefficient, and it pollutes the higher-level structures with lower-level complications. It is for the same reason that an architect does not think of a building in terms of bricks and cement, but in terms of floors, rooms, and supporting structures. Building complex structures requires a degree of modularity, or zoning, to allow for a more holistic view of the structure. That is the concept of a zone - a way to encapsulate many lights into a structure that acts as a unit. Zones can be seen as the unit building blocks of your lighting application, but there is no restriction for zones to be a certain way so you can create zones of any "shape" (following the LEGO analogy) and size. The purpose of this application is to quite literally let you project a "light movie" on top of your zones with the movies in this case being zone programs.

There are currently a few things that can be done on a zone:

1. Run program(s) on zone
2. Set color of entire zone

###Zone Programs and Program Sets

A Zone Program can be anything ranging from a static loop to a responsive program. A few examples are programs that scroll a dot in a loop, or output a rainbow loop, or output a notification using a light effect. Many stock programs are included with the application under the folder StockPrograms. Zone programs have the concept of Inputs, which are parameters that can be changed by the user of a program to manipulate things inside a program. A program is responsible for driving the lights in a zone. The zones and lighting controllers are static objects. A zone program runs on a zone and uses a lighting controller to output the lights in a zone. Examples are included with the project for a Rainbow program, a ScrollDot program, and a StaticColor program (among others).

Zone programs are loaded from external assemblies. The location of the folder where the assemblies are searched for is provided in the configuration parameter "ProgramDLLFolder". In the way the project is organized, the project ExternalPrograms houses all the programs that are available.  The WebController project loads a copy of the assembly in the folder ExternalPrograms/bin/Debug/Programs (by default). 

To create a new zone program, create a new class in the ExternalPrograms project. If the program needs to loop something over and over, make the new class inherit from LoopingZoneProgram. If the program is simply doing things based on external stimuli, make the new class inherit from ReactiveZoneProgram. If in doubt, use LoopingZoneProgram. Add some inputs using the AddMappedInput method if you wish to control parameters inside the program. An input needs a delegate that will determine what happens with the input value. A mapped input simply assigns the input value to a variable. See the example programs Rainbow, ScrollDot, and StaticColor for more details.

To create a new zone program assembly, create a new C# class library project, and add a reference to the ZoneLighting assembly. This assembly gives access to the LoopingZoneProgram and ReactiveZoneProgram classes. Now create at least one program in here and to have this assembly be loaded by the application, put the compiled .dll for this project in the folder specified in the configuration parameter "ProgramDLLFolder".

###Synchronization

The concept of synchronization is central to this application. The user can run programs on zones but these programs have no knowledge of each other. So what would you do if you wanted two programs to move in lock-step? That is where the concept of synchronization comes in, in the form of SyncContext. 
























Q: What does a zone look like? 
A: In the most basic sense, it is an enumerable collection of lights. A light, as defined in this application, is nothing more than a contract - an interface - defined as ILogicalRGBLight. This tells us a few things about what the light is and what it can do. First, it tells us that the interface expects an RGB color to be representable by its implementation, but if a light doesn't support full RGB, the implementation can throw an exception during its API calls when the RGB values are invalid. It's important to note that this is a logical representation of a light. The implementation of this interface does not require the implementer to interact with the physical light. That is the job of the lighting controller (ILightingController). ILogicalRGBLight has three methods that it expects the subclass to define: 

1. bool SetColor(Color color) - Expects the subclass to define how to set the color of the given light. Note that this does not mean how to physically set the color of the light, but how to set it in the logical view of the light. There is a logical view of the lights and a physical view. The logical view acts as a filter to filter out invalid values of color, for example (or other kinds of filtering). If the subclass of this interface is used to represent an incandescent light, which can only display lights of a certain color (yellowish to whiteish), then the SetColor method can be programmed to return false or throw an error when the requested color is not in the range of colors that can be produced on the light. In this way, we do need to know something about the physical lights that the subclass will represent, but this interface's implementation does not need to know how to physically change the color of the light. That will be done using the physical definition of the light in terms of a specific controller as we will see in a bit.

2. int LogicalIndex - This property defines the address of the light in the logical view. This is useful in things like sorting the lights. This number is different from the physical index that will be defined in the concrete implementation of the light. The physical index will be specific to the lighting controller, but the logical index is not.

3. Color GetColor() - Expects the subclass to define how to get the current color of the light.

Now let's look at an implementation of this interface called LED. The LED class represents a single LED in a zone. The way I've programmed this, it needs to implement interfaces for each type of physical representation of a light that it can be output on. So for example, it implements IFadeCandyPixel which requires an instance of FadeCandyPixel which contains all information required by the FadeCandyController to output this light to a FadeCandy board. IFadeCandyPixel is required to output the light using FadeCandyController because IFadeCandyPixel is the PixelType of FadeCandyController. To create an instance of an LED that needs to be output on FadeCandy, it needs the physical index of the LED on the FadeCandy which is passed in through the constructor. Then the color of the LED is set using SetColor. This changes the color of the logical view which is then sent to the lighting controller using FadeCandyController.SendLEDs method.

Zones need to created by the user by subclassing the Zone class based on their lighting setup. For a FadeCandy zone, subclass the FadeCandyZone class, which sets the zone's lighting controller to FadeCandyController. Your custom zone classes can be created in an external assembly by creating a new C# class library project. This assembly needs to be stored in the folder specified by the ZonesDLLFolder configuration property. This folder will be scanned during initialization and those zones will be available to the zone programs to be manipulated.


###ZoneLightingManager

ZoneLightingManager is what you can call the "runner" class for the entire application. All high-level operations like initializing/uninitializing zone programs, zones, or lighting controllers are done using this class. It is implemented as a singleton. To access the singleton instance, use ZoneLightingManager.Instance (this is the practice followed throughout the entire application for accessing singleton instances for any given class). ZLM is also responsible for adding/removing zone data. It loads zones from other assemblies (which are user-created based on their setup) which are then available to zone programs to manipulate. These zones are added to the Zones property of ZLM, which is a list of Zones that ZLM can manage. When the Initialize() method is called on ZLM, it initializes all lighting controllers, loads zone data, and initializes all zones contained in the Zones property. Initializing zones entails selecting a zone program for each zone and starting it with some starting input values.


Here is an example setup that I created. The lighting is laid out as such:



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



Left Wing and Right Wing are zones that sit on the left and right side of my computer monitor, respectively. They are both cut from a long strip of WS2812s. WS2812 is an electronic component that is a combination of an RGB LED and a driver chip that handles PWM, I/O buffering and propagation, timing issues, and other details that are required for a high-level control of an RGB LED or a series of RGB LEDs. 

**Note**: WS2812s are are sometimes (incorrectly) referred to as "WS2811". Even though for most practical purposes the names WS2811 and WS2812 are interchangeable, it's important to realize that the WS2811 (http://www.adafruit.com/datasheets/WS2811.pdf) is the name for the driver chip that drives the RGB LED, and WS2812 (http://www.adafruit.com/datasheets/WS2812.pdf) is the name for the entire component, including the LED and the driver chip. But since the WS2811 driver chip is essentially useless without the LED, the name "WS2811" is many times used interchangeably with WS2812. Many vendors sell WS2812s as WS2811, so if searching vendor sites to buy the WS2812, try searching for WS2811 also. The Adafruit product known as NeoPixel is the same as the WS2812, rebranded for Adafruit.

The code for these zones is stored in the ExternalZones project and the example programs to run on this are in the ZonePrograms project. Please see those for an example on how to implement your own setup. 


## FINAL NOTES AND FUTURE PLANS

This is my first open-source project, and therefore there are bound to be issues, errors, and incompatibilities. This project also has almost no tests mainly because it was something I hacked up in a few days just to get the FadeCandy board working. My intention is to add tests in the functional parts of the application where things can potentially break. This is one of the main priorities. Other priorities are to add support for more hardware controllers and more kinds of lighting. It is also planned to add zone subtypes, and enforce which programs can run on which zone subtypes. This will introduce a much more predictable way programs run on zones, because that essentially would allow the person programming the zone program to specify what kind of zones a program can run on. The "type" of zone will entail things like how many pixels are available to display on, what configuration the pixels are in, and other information about the zone configuration.


Please feel free to create issues in the Github issue tracker, and if you have any specific questions directed towards me, please feel free to email me. I will try to respond as timely as possible.