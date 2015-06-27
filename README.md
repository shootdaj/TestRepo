# ZoneLighting

## PROJECT SUMMARY

ZoneLighting is a .NET application to create pixel-based lighting. It abstracts out the logical view of the lights in "zones", each of which can run a "zone program". Each zone has a "lighting controller", which is a C# wrapper for lighting controllers such as the FadeCandy board or an Arduino. Zone programs can output any pattern of lights using a zone's lighting controller. The program running on the zone must take the physical configuration into account. Hardware support is currently limited to FadeCandy boards with WS2812 RGB LEDs, but the architecture is designed and intended for extensibility.


* **ORIGINAL AUTHOR**: Anshul Vishwakarma
* **EMAIL**: shootdaj@gmail.com
* **REQUIRED SOFTWARE**: Windows with .NET runtime installed (latest version - 4.5 as of writing). Requires C# 6.0 (Visual Studio 2015 Preview) to compile.
* **REQUIRED HARDWARE**: Lighting Hardware such as FadeCandy with WS2812 strips or (currently unsupported) an Arduino-based LPD8806 RGB LED Strip
* **LICENSE**: MIT (See file *LICENSE*)


## USER GUIDE:

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

Program Sets are sets of programs that are logically grouped together and function as a unit. The advantage of using a Program Set over programs is that a Program Set can be run over many zones. A program set is a set of programs on a set of zones.

Zone programs are loaded from external assemblies. The location of the folder where the assemblies are searched for is provided in the configuration parameter "ProgramDLLFolder". In the way the project is organized, the project ExternalPrograms houses all the programs that are available.  The WebController project loads a copy of the assembly in the folder ExternalPrograms/bin/Debug/Programs (by default). 

To create a new zone program, create a new class in the ExternalPrograms project. If the program needs to loop something over and over, make the new class inherit from LoopingZoneProgram. If the program is simply doing things based on external stimuli, make the new class inherit from ReactiveZoneProgram. If in doubt, use LoopingZoneProgram. Add some inputs using the AddMappedInput method if you wish to control parameters inside the program. An input needs a delegate that will determine what happens with the input value. A mapped input simply assigns the input value to a variable. See the example programs Rainbow, ScrollDot, and StaticColor for more details.

To create a new zone program assembly, create a new C# class library project, and add a reference to the ZoneLighting assembly. This assembly gives access to the LoopingZoneProgram and ReactiveZoneProgram classes. Now create at least one program in here and to have this assembly be loaded by the application, put the compiled .dll for this project in the folder specified in the configuration parameter "ProgramDLLFolder".

###Synchronization

The concept of synchronization is central to this application. The user can run programs on zones but these programs have no knowledge of each other. So what would you do if you wanted two programs to move in lock-step? That is where the concept of synchronization comes in, in the form of SyncContext. The user just has to provide whether or not they want the programs to be synced when creating a program set. The SyncContext is managed internally by the program set.

###ZoneLightingManager

ZoneLightingManager is what you can call the "runner" class for the entire application. All high-level operations like creating/destroying program sets are done using this class. To use ZLM, simply create an instance of it. WebController already creates one and keeps it in the ASP.NET Application State and displays it on the index page.

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

## FINAL NOTES AND FUTURE PLANS

This application is still in the early phases of architecture and therefore there are bound to be issues, errors, and incompatibilities. Please feel free to create issues in the Github issue tracker, and if you have any specific questions directed towards me, please feel free to email me. I will try to respond as timely as possible.