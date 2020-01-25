#include <Ethernet.h>
#include <PubSubClient.h>
#include <ESP8266WiFi.h>

#include "HX711.h"

HX711 scale;
char ssid[] = "GL-MT300N-V2-2ae"; 
char pass[] = "goodlife"; 

WiFiClient espClient;
PubSubClient client(espClient);


float calibration_factor = 400; 
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
  Serial.print("Wi-Fi connected successfully:");Serial.println(WiFi.localIP());
}

void _setupMQT(){
  Serial.println("MQTT setup");
  client.setServer("192.168.8.169", 1883);

}
void _connectToMQTT(){
   while (!client.connected()) {  
     if (client.connect("arduinoClient")) {
       return ;
     }
    Serial.print("failed, rc=");
    Serial.print(client.state());
    Serial.println(" try again in 5 seconds");
    delay(1000);    
  }
}
void _setupScale(){
  scale.begin(4, 0);
  scale.set_scale();
  scale.tare();  //Reset the scale to 0

  long zero_factor = scale.read_average(); //Get a baseline reading
  
  scale.set_scale(calibration_factor); 
}

void setup() {
  Serial.begin(115200);

  _connectToWIFI();
  _setupMQT();
  _setupScale(); 
  
  Serial.println("All ok");  
}

void loop() {
  units = scale.get_units(), 10;
 
  Serial.println(units);
  
  if (units > 1){
    Serial.println(units);
    if (!client.connect("arduinoClient")) {
      _connectToMQTT();
    } 

    if (client.connect("arduinoClient")) {   
      String s =  "{\"Weight\":";
      s = s + units + "}";
      client.publish("pf/scale", s.c_str());     
    } 
  }
  delay(1000);
}
