#include <Ethernet.h>
#include <PubSubClient.h>

#include "HX711.h"

HX711 scale;
char ssid[] = "GL-MT300N-V2-2ae"; 
char pass[] = "goodlife"; 

IPAddress server(192, 168, 8, 169);
float calibration_factor = 101; 
float units;
float ounces;

void _connectToWIFI(){
  Serial.println();
  Serial.println();
  Serial.print("Connecting to...");
  Serial.println(ssid);

  WiFi.begin(ssid, pass);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("Wi-Fi connected successfully");
   Serial.println("LocalIP:");  Serial.print(WiFi.localIP(););
}

void _setupMQT(){
  Serial.println("MQTT setup"):
  client.setServer(server, 1883);
  client.setCallback(callback);
}

void _setupScale(){

  scale.begin(4, 0);
  scale.set_scale();
  scale.tare();  //Reset the scale to 0

  long zero_factor = scale.read_average(); //Get a baseline reading
  
  scale.set_scale(calibration_factor); 
}

void setup() {
  Serial.begin(38400);

  _connectToWIFI();
  _setupMQT();
  _setupScale(); 
  
  Serial.print("Started");  
}

void loop() {
  units = scale.get_units(), 10;
  if (units < 0)
  {
    units = 0.00;
  }
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");

    if (client.connect("arduinoClient")) {
      Serial.println("connected");     
      client.publish("pf/scale","{\"Weight\":0}");      
    } 
  }
}
