# ZoneLighting

## PROJECT SUMMARY

ZoneLighting is a .NET application to create pixel-based lighting for your home or office (or any other space). It abstracts out the logical view of the lights in "zones", which can each run a "zone program". Each zone has a lighting controller, which is a C# wrapper for lighting controllers such as the FadeCandy board. Zone programs can output any pattern of lights using a zone's lighting controller. The program running on the zone must take the physical configuration into account. Hardware support is currently limited to FadeCandy boards with WS2812 RGB LEDs, but the architecture is designed and intended for extensibility.

ORIGINAL AUTHOR: Anshul Vishwakarma
EMAIL: shootdaj@gmail.com
REQUIRED SOFTWARE: Windows with .NET runtime installed (latest version - 4.5 as of writing)
REQUIRED HARDWARE: Lighting Hardware such as FadeCandy with WS2812 strips or (currently unsupported) an Arduino-based LPD8806 RGB LED Strip
LICENSE: MIT (See file LICENSE)


## USER/PROGRAMMER GUIDE:

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

Note: WS2812s are are sometimes (incorrectly) referred to as "WS2811". Even though for most practical purposes the names WS2811 and WS2812 are interchangeable, it's important to realize that the WS2811 (http://www.adafruit.com/datasheets/WS2811.pdf) is the name for the driver chip that drives the RGB LED, and WS2812 (http://www.adafruit.com/datasheets/WS2812.pdf) is the name for the entire component, including the LED and the driver chip. But since the WS2811 driver chip is essentially useless without the LED, the name "WS2811" is many times used interchangeably with WS2812. Many vendors sell WS2812s as WS2811, so if searching vendor sites to buy the WS2812, try searching for WS2811 also. The Adafruit product known as NeoPixel is the same as the WS2812, rebranded for Adafruit.

This project was started as an abstraction on top of the FadeCandy board (https://github.com/scanlime/fadecandy). FadeCandy is a product created by Github user scanlime. It consists of a physical hardware board that is able to control 512 WS2812s as 8 strips of 64 WS2812s and the software component which consists of a program called FCServer that connects to the physical board over USB and listens for requests over WebSockets in the format formalized by the Open Packet Control protocol (which was also created by scanlime). FCServer redirects these WebSocket requests to the physical FadeCandy board. These may be requests to change the color of the lights or administrative requests like enabling/disabling dithering or interpolation. I wanted to create a software layer that abstracts out the entire FadeCandy stack as a generic lighting controller. This generalization is intended to allow for adding facilities to control non-WS2812 lighting such as the LPD8806 (individually-addressable RGB LED strip that needs manual PWM and a clock signal), regular LEDs using a board like Arduino or similar, or regular lamps that plug into the wall using X10 or Insteon. This also allows multi-layered configurations and to "transfer" a zone configuration to a different physical configuration without changing anything in the logical layer. In short, this application heavily enforces separation of concerns by using a layered architecture.

Currently, the application only supports FadeCandy as the lighting controller and what I call a FadeCandyPixel as the physical unit of a "pixel". A FadeCandyPixel represents a single WS2812 in a series of WS2812s. These are contained in the logical class LED such that each instance of an LED contains an instance of a FadeCandyPixel. In essence, the LED class is a wrapper around the various physical representations of an LED, such as FadeCandyPixel. In the future, if support for LPD8806 is added, a new class could be created called LP8806Pixel (inheriting from PhysicalRGBLight), which would represent a physical LED on the LPD8806. An instance of this class would be added as a property to the LED class to allow the LED be output on the LPD8806. This property would be used by the corresponding lighting controller class LPD8806Controller to output the LED to the LPD8806. Many of the properties contained within the FadeCandyPixel class would also be contained in the LP8806Pixel class, such as PhysicalIndex, RedIndex, GreenIndex, and BlueIndex, but the implementations of these would be different based on how the various positions and colors on the LPD8806 are addressed. In some RGB strips, the positions of blue and green colors are switched, so that would be a case where the implementation of the BlueIndex and GreenIndex would differ from the FadeCandyPixel class.

I've designed this project with the intention of other people extending it. To add a new lighting controller, all one needs to do is extend the abstract LightingController class. To add new types of lights, all that's required is to extend the abstract PhysicalRGBLight class. For examples of how to do these things, look at the implementations of the FadeCandyController and FadeCandyPixel classes, respectively.

The FadeCandyController class is implemented as a singleton, simply because at the time of writing this code, it was anticipated that only a single FadeCandy board will be used with this program. If someone finds that using more than one board is necessary, it's very easy to convert it from a singleton to a non-singleton class. The instances will live inside the ZoneLightingManager class as a property.

Speaking of ZoneLightingManager (ZLM), it is worth talking a bit about this class and its function. This is what you can call the "runner" class for the entire application. All high-level operations like initializing/uninitializing zone programs, zones, or lighting controllers are done using this class. It is implemented as a singleton. To access the singleton instance, use ZoneLightingManager.Instance (this is the practice followed throughout the entire application for accessing singleton instances for any given class). ZLM is also responsible for adding/removing zone data. Currently it is adding data in the LoadSampleZoneData() method based on the configuration I have in the ASCII diagram above. In LoadSampleZoneData(), it adds two zones that are to be controlled by a FadeCandy board, leftWingZone and rightWingZone. These zones are added to the Zones property of ZLM, which is a list of Zones that ZLM can manage. When the Initialize() method is called on ZLM, it initializes all lighting controllers, loads zone data, and initializes all zones contained in the Zones property. Initializing zones entails selecting a zone program for each zone and starting it with some parameters.

What exactly is a zone program? A zone program is essentially a piece of code (program) that can output anything it needs to a zone's lights. An example of a zone program is some code that loops the colors of a rainbow on a zone (or any other collection of colors). Another example is a program that checks your email every so often and blinks the lights in a zone in a certain pattern when you get a new email. Another example is a program that just "scrolls" a dot across the lights in a zone such by turning the appropriate lights on and off such that a "dot" appears to be moving across the lights of that zone. So really, a zone program in this sense is any program that outputs a lighting pattern on a given zone. This pattern may or may not be based on external stimuli.

## FINAL NOTES AND FUTURE PLANS

This is my first open-source project, and therefore there are bound to be issues, errors, and incompatibilities. This project also has no tests mainly because it was something I hacked up in a few days just to get the FadeCandy board working. My intention is to add tests in the functional parts of the application where things can potentially break. This is one of the main priorities. Other priorities are to add support for more hardware controllers and more kinds of lighting. Also need to add support to be able to load programs that are not hard-coded but are instead either loaded using reflection or using configuration files (or a combination of both). This will allow the application to be modular and programs should eventually be able to be downloaded and "installed". It is also planned to add zone subtypes, and enforce which programs can run on which zone subtypes. This will introducing a much more predictable way programs run on zones, because that essentially would allow the person programming the zone program to specify what kind of zones a program can run on. The "type" of zone will entail things like how many pixels are available to display on, what configuration the pixels are in, and other information about the zone configuration.


Please feel free to create issues in the Github issue tracker, and if you have any specific questions directed towards me, please feel free to email me. I will try to respond as timely as possible.