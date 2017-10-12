//////////////////////////////////////////////////////////////////////////////////
// Copyrights @2017 Sameer Khandekar
// License: MIT
//////////////////////////////////////////////////////////////////////////////////
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>

// SSID of your home network
const char* ssid = "YOUR SSID";
// password for your network
const char* password = "YOUR PASSWORD";

// pin where hall effect sensor is connected
const int hallEffectPin = 16;

// instantiate server
ESP8266WebServer server(80);

// on board led
const int led = 13;

// displays text for root path
void handleRoot() {
  digitalWrite(led, 1);
  server.send(200, "text/plain", "hello from esp8266!");
  digitalWrite(led, 0);
}

// handles query for /IsOpen path
void handleIsOpen() {
  // turn the led on
  digitalWrite(led, 1);
  // read the hall effect pin and return 1 or 0
  if (digitalRead(hallEffectPin) == HIGH)
  {
    server.send(200, "text/plain", "1");
    Serial.println("High");
  }
  else
  {
    server.send(200, "text/plain", "0");  
    Serial.println("Low");
  }
  
  digitalWrite(led, 0);
}

// displays 404
void handleNotFound(){
  digitalWrite(led, 1);
  String message = "File Not Found\n\n";
  message += "URI: ";
  message += server.uri();
  message += "\nMethod: ";
  message += (server.method() == HTTP_GET)?"GET":"POST";
  message += "\nArguments: ";
  message += server.args();
  message += "\n";
  for (uint8_t i=0; i<server.args(); i++){
    message += " " + server.argName(i) + ": " + server.arg(i) + "\n";
  }
  server.send(404, "text/plain", message);
  digitalWrite(led, 0);
}

// set up the server
void setup(void){
  // set output mode for led
  pinMode(led, OUTPUT);
  digitalWrite(led, 0);
  // set input mode for the hall effect sensor pin
  pinMode(hallEffectPin,INPUT);  
  // initialize the serial port monitor
  Serial.begin(115200);
  Serial.println("Setup started...");
  // init the WiFi
  WiFi.begin(ssid, password);
  Serial.println("");

  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  // display IP address. User will register it with UWP app
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  if (MDNS.begin("esp8266")) {
    Serial.println("MDNS responder started");
  }

  // sest handlers for various paths
  server.on("/", handleRoot);

  server.on("/IsOpen", handleIsOpen);

  server.on("/inline", [](){
    server.send(200, "text/plain", "this works as well");
  });

  server.onNotFound(handleNotFound);

  server.begin();
  Serial.println("HTTP server started");
  MDNS.addService("http", "tcp", 80);
}

void loop(void){
  server.handleClient();
}
