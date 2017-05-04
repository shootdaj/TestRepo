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

#define NUM_LEDS 6

const char* ssid = "**";
const char* password = "**";
const char* mDNSid = "WebsocketsTest";

static WS2812 ledstrip;
static Pixel_t pixels[NUM_LEDS];

File fsUploadFile;
ESP8266WiFiMulti WiFiMulti;
ESP8266WebServer server(80);
WebSocketsServer webSocket = WebSocketsServer(81);

void setup() {
	pinMode(LED_BUILTIN, OUTPUT);
	Serial.begin(115200);
	ledstrip.init(NUM_LEDS);

	/*Serial.printf("WStype_DISCONNECTED = %o\n", WStype_DISCONNECTED);
	Serial.printf("WStype_CONNECTED = %o\n", WStype_CONNECTED);
	Serial.printf("WStype_DISCONNECTED = %o\n", WStype_TEXT);
	Serial.printf("WStype_DISCONNECTED = %o\n", WStype_DISCONNECTED);*/

	//start file server
	SPIFFS.begin();
	{
		Dir dir = SPIFFS.openDir("/");
		Serial.printf("asdfasdfadsf");
		while (dir.next()) {
			String fileName = dir.fileName();
			size_t fileSize = dir.fileSize();
			Serial.printf("FS File: %s, size: %s\n", fileName.c_str(), formatBytes(fileSize).c_str());
		}
		Serial.printf("\n");
	}



	//connect to wifi
	WiFiMulti.addAP(ssid, password);
	while (WiFiMulti.run() != WL_CONNECTED) {
		delay(100);
	}

	Serial.print("Connected! IP address: ");
	Serial.println(WiFi.localIP());

	//start websocket
	webSocket.begin();
	webSocket.onEvent(webSocketEvent);

	//SERVER INIT
	//list directory
	server.on("/list", HTTP_GET, handleFileList);
	//load editor
	server.on("/edit", HTTP_GET, []() {
		if (!handleFileRead("/edit.htm")) server.send(404, "text/plain", "FileNotFound");
	});
	//create file
	server.on("/edit", HTTP_PUT, handleFileCreate);
	//delete file
	server.on("/edit", HTTP_DELETE, handleFileDelete);
	//first callback is called after the request has ended with all parsed arguments
	//second callback handles file uploads at that location
	server.on("/edit", HTTP_POST, []() { server.send(200, "text/plain", ""); }, handleFileUpload);

	//called when the url is not defined here
	//use it to load content from SPIFFS
	server.onNotFound([]() {
		if (!handleFileRead(server.uri()))
			server.send(404, "text/plain", "FileNotFound");
	});

	server.begin();
	Serial.println("HTTP server started");

}

void loop() {
	webSocket.loop();
	server.handleClient();
}

void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t lenght) {

	Serial.printf("[%u] get Text: %o\n", num, payload);
	Serial.printf("[%u] Type: %u\n", num, type);

	switch (type) {
	case WStype_DISCONNECTED:
		Serial.printf("[%u] Disconnected!\n", num);
		break;
	case WStype_CONNECTED: {
		IPAddress ip = webSocket.remoteIP(num);
		Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n", num, ip[0], ip[1], ip[2], ip[3], payload);

		// send message to client
		webSocket.sendTXT(num, "Connected");
	}
						   break;
	case WStype_TEXT: {
		Serial.printf("[%u] get Text: %s\n", num, payload);
		//if (payload[0] == '#') {
		  // we get RGB data
		  // decode rgb data

		uint8_t l = NUM_LEDS * 1;
		while (l--) {
			pixels[l].R = payload[l * 3];
			Serial.printf("r:[%o]", pixels[l].R);
			pixels[l].G = payload[l * 3 + 1];
			Serial.printf("g:[%o]", pixels[l].G);
			pixels[l].B = payload[l * 3 + 2];
			Serial.printf("b:[%o]", pixels[l].B);
		}
		ledstrip.show(pixels);

		break;
	}
	}
	/*
	  Serial.printf("payload 0: %o\n", payload[0]);
	  Serial.printf("payload 1: %o\n", payload[1]);
	  Serial.printf("payload 2: %o\n", payload[2]);
	  Serial.printf("payload 3: %o\n", payload[3]);
	  Serial.printf("payload 4: %o\n", payload[4]);
	  Serial.printf("payload 5: %o\n", payload[5]);
	  */
	uint32_t rgb = (uint32_t)strtol((const char *)&payload[1], NULL, 16);

	for (int i = 0; i < NUM_LEDS; i++)
	{
		pixels[i].R = payload[i * 3];
		pixels[i].G = payload[i * 3 + 1];
		pixels[i].B = payload[i * 3 + 2];
		Serial.printf("r:[%o]", pixels[i].R);
		Serial.printf("g:[%o]", pixels[i].G);
		Serial.printf("b:[%o]", pixels[i].B);
	}

	ledstrip.show(pixels);

	//}

	//if (payload[0] == '*') {
	//  // we get Pixel number
	//  uint32_t PixelNumber = (uint32_t)strtol((const char *)&payload[1], NULL, 16);
	//  // NeoPixels
	//  /*for (int i = 0; i < pixelCount; i++) {
	//    strip.SetPixelColor(i, RgbColor(0x00, 0x00, 0x00));
	//  }
	//  strip.SetPixelColor(PixelNumber, RgbColor(0xff, 0xff, 0xff));
	//  strip.Show();*/
	// pixels[0].R = ((rgb >> 16) & 0xFF);
	//  pixels[0].G = ((rgb >> 8) & 0xFF);
	//  pixels[0].B = ((rgb >> 0) & 0xFF);
	//  ledstrip.show(pixels);
	//}

	//break;

}

String getContentType(String filename) {
	if (server.hasArg("download")) return "application/octet-stream";
	else if (filename.endsWith(".htm")) return "text/html";
	else if (filename.endsWith(".html")) return "text/html";
	else if (filename.endsWith(".css")) return "text/css";
	else if (filename.endsWith(".js")) return "application/javascript";
	else if (filename.endsWith(".png")) return "image/png";
	else if (filename.endsWith(".gif")) return "image/gif";
	else if (filename.endsWith(".jpg")) return "image/jpeg";
	else if (filename.endsWith(".ico")) return "image/x-icon";
	else if (filename.endsWith(".xml")) return "text/xml";
	else if (filename.endsWith(".pdf")) return "application/x-pdf";
	else if (filename.endsWith(".zip")) return "application/x-zip";
	else if (filename.endsWith(".gz")) return "application/x-gzip";
	return "text/plain";
}

bool handleFileRead(String path) {
	Serial.println("handleFileRead: " + path);
	if (path.endsWith("/")) path += "index.htm";
	String contentType = getContentType(path);
	String pathWithGz = path + ".gz";
	if (SPIFFS.exists(pathWithGz) || SPIFFS.exists(path)) {
		if (SPIFFS.exists(pathWithGz))
			path += ".gz";
		File file = SPIFFS.open(path, "r");
		size_t sent = server.streamFile(file, contentType);
		file.close();
		return true;
	}
	return false;
}

void handleFileUpload() {
	if (server.uri() != "/edit") return;
	HTTPUpload& upload = server.upload();
	if (upload.status == UPLOAD_FILE_START) {
		String filename = upload.filename;
		if (!filename.startsWith("/")) filename = "/" + filename;
		Serial.print("handleFileUpload Name: "); Serial.println(filename);
		fsUploadFile = SPIFFS.open(filename, "w");
		filename = String();
	}
	else if (upload.status == UPLOAD_FILE_WRITE) {
		//Serial.print("handleFileUpload Data: "); Serial.println(upload.currentSize);
		if (fsUploadFile)
			fsUploadFile.write(upload.buf, upload.currentSize);
	}
	else if (upload.status == UPLOAD_FILE_END) {
		if (fsUploadFile)
			fsUploadFile.close();
		Serial.print("handleFileUpload Size: "); Serial.println(upload.totalSize);
	}
}

void handleFileDelete() {
	if (server.args() == 0) return server.send(500, "text/plain", "BAD ARGS");
	String path = server.arg(0);
	Serial.println("handleFileDelete: " + path);
	if (path == "/")
		return server.send(500, "text/plain", "BAD PATH");
	if (!SPIFFS.exists(path))
		return server.send(404, "text/plain", "FileNotFound");
	SPIFFS.remove(path);
	server.send(200, "text/plain", "");
	path = String();
}

void handleFileCreate() {
	if (server.args() == 0)
		return server.send(500, "text/plain", "BAD ARGS");
	String path = server.arg(0);
	Serial.println("handleFileCreate: " + path);
	if (path == "/")
		return server.send(500, "text/plain", "BAD PATH");
	if (SPIFFS.exists(path))
		return server.send(500, "text/plain", "FILE EXISTS");
	File file = SPIFFS.open(path, "w");
	if (file)
		file.close();
	else
		return server.send(500, "text/plain", "CREATE FAILED");
	server.send(200, "text/plain", "");
	path = String();
}

void handleFileList() {
	if (!server.hasArg("dir")) { server.send(500, "text/plain", "BAD ARGS"); return; }

	String path = server.arg("dir");
	Serial.println("handleFileList: " + path);
	Dir dir = SPIFFS.openDir(path);
	path = String();

	String output = "[";
	while (dir.next()) {
		File entry = dir.openFile("r");
		if (output != "[") output += ',';
		bool isDir = false;
		output += "{\"type\":\"";
		output += (isDir) ? "dir" : "file";
		output += "\",\"name\":\"";
		output += String(entry.name()).substring(1);
		output += "\"}";
		entry.close();
	}

	output += "]";
	server.send(200, "text/json", output);
}

String formatBytes(size_t bytes) {
	if (bytes < 1024) {
		return String(bytes) + "B";
	}
	else if (bytes < (1024 * 1024)) {
		return String(bytes / 1024.0) + "KB";
	}
	else if (bytes < (1024 * 1024 * 1024)) {
		return String(bytes / 1024.0 / 1024.0) + "MB";
	}
	else {
		return String(bytes / 1024.0 / 1024.0 / 1024.0) + "GB";
	}
}