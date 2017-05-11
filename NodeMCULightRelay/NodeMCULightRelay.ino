#include <WiFiUdp.h>
#include <WiFiServer.h>
#include <WiFiClientSecure.h>
#include <ESP8266WiFiType.h>
#include <ESP8266WiFiSTA.h>
#include <ESP8266WiFiScan.h>
#include <ESP8266WiFiGeneric.h>
#include <ESP8266WiFiAP.h>
#include <WebSocketsClient.h>
#include <WebSockets.h>
#include <ws2812_i2s.h>
#include <WebSocketsServer.h>
#include <Arduino.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266WebServer.h>
#include <FS.h>

#define NUM_LEDS 64

const char* ssid = "*****";
const char* password = "******";
const char* mDNSid = "WebsocketsTest";

static WS2812 ledstrip;
static Pixel_t pixels[NUM_LEDS];

ESP8266WiFiMulti WiFiMulti;
WebSocketsServer webSocket = WebSocketsServer(81);
int ticks = 0;
int ms = 0;

void setup() {
	pinMode(LED_BUILTIN, OUTPUT);
	Serial.begin(115200);
	ledstrip.init(NUM_LEDS);

	//connect to wifi
	WiFiMulti.addAP(ssid, password);
	while (WiFiMulti.run() != WL_CONNECTED) {
		delay(100);
	}

	Serial.printf("Connected to WIFI\nIP address: ");
	Serial.println(WiFi.localIP());

	//start websocket
	webSocket.begin();
	webSocket.onEvent(webSocketEvent);
}

void loop() {
	webSocket.loop();	
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t lenght) {

	//switch (type) {
	//case WStype_DISCONNECTED:
	//	Serial.printf("[%u] Disconnected!\n", num);
	//	break;
	//case WStype_CONNECTED: {
	//	IPAddress ip = webSocket.remoteIP(num);
	//	Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n", num, ip[0], ip[1], ip[2], ip[3], payload);

	//	// send message to client
	//	webSocket.sendTXT(num, "Connected");
	//}
	//					   break;
	//}

	/*
	Serial.printf("payload 0: %o\n", payload[0]);
	Serial.printf("payload 1: %o\n", payload[1]);
	Serial.printf("payload 2: %o\n", payload[2]);
	Serial.printf("payload 3: %o\n", payload[3]);
	Serial.printf("payload 4: %o\n", payload[4]);
	Serial.printf("payload 5: %o\n", payload[5]);
	*/


	for (int i = 0; i < NUM_LEDS; i++)
	{
		pixels[i].R = payload[i * 3];
		pixels[i].G = payload[i * 3 + 1];
		pixels[i].B = payload[i * 3 + 2];
		/*Serial.printf("");
		Serial.printf("");
		Serial.printf("");*/
		
	
	} 

	delay(20);

	ticks++;

	if (millis() - ms > 999)
	{
		Serial.println(ticks);
		ms = millis();		
		ticks = 0;	
	}

	ledstrip.show(pixels);
}
