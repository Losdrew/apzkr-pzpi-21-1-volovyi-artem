#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
#include <ESP8266HTTPClient.h>
#include <ArduinoJson.h>
#include "certificate.h"
#include "carstatus.h"
#include "commands.h"

const String WIFI_SSID = "Artem";
const String WIFI_PASS = "12345678";

const String HOST = "https://autocabserver20240420160507.azurewebsites.net";
const String GET_ACTIVE_TRIP_LOCATION_ENDPOINT = "/api/trip/active-trip-location?deviceId=";
const String UPDATE_CAR_ENDPOINT = "/api/car/update";
const String GET_DOOR_STATUS_ENDPOINT = "/api/car/door-status?deviceId=";
const String GET_CAR_STATUS_ENDPOINT = "/api/car/status?deviceId=";
const String GET_TRIP_ROUTE_ENDPOINT = "/api/geolocation/route/";
const String GET_NEAREST_GAS_STATION_ROUTE_ENDPOINT = "/api/geolocation/gas-station-route?deviceId=";
const String GET_ACTIVE_TRIP_SERVICES_ENDPOINT = "/api/trip/active-trip-services?deviceId=";

const int DEFAULT_DELAY = 3000;

WiFiClientSecure client;
HTTPClient https;

String currentData;
DynamicJsonDocument currentDataJSON(256);

String activeTripLocation;
DynamicJsonDocument activeTripLocationJSON(48);

String activeTripServices;
DynamicJsonDocument activeTripServicesJSON(48);

String currentRoute;
DynamicJsonDocument currentRouteJSON(8192);

CarStatus currentStatus;
bool isDoorOpen;

void setup() {
  connectToWiFi();
  updateSystemTime();

  client.setTrustAnchors(&certificate);

  Serial.begin(115200);
  Serial1.begin(115200);
  
  while (!Serial) {
  ; // wait for serial port to connect. Needed for native USB port only
  }
}

void loop() {
  receiveCurrentData();

  if (!currentData.isEmpty()) {
    if (deserializeCurrentData()) {
      currentStatus = (CarStatus)(currentDataJSON["status"].as<int>());
      
      if (currentStatus == CarStatus::Idle) {
        handleActiveTrip();
        if (!isEnoughFuel()) {
          handleNearestGasStationRoute();
        }
      }

      if (currentStatus == CarStatus::EnRoute) {
        handleActiveTrip();
        handleTripRoute();
        handleDestination();
      }

      if (currentStatus == CarStatus::WaitingForPassenger) {
        handleDoorStatus();
        handleCarStatus();
      }

      if (currentStatus == CarStatus::OnTrip) {
        handleCarStatus();
        handleActiveTrip();
        handleTripRoute();
        handleDestination();
      }
      
      sendCarUpdate();
    }
  }
  
  delay(DEFAULT_DELAY);
}

void connectToWiFi() {
  WiFi.begin(WIFI_SSID, WIFI_PASS);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nConnected to " + WiFi.localIP().toString());
}

void updateSystemTime() {
  // Set time via NTP, as required for x.509 validation
  configTime(3 * 3600, 0, "pool.ntp.org", "time.nist.gov");
  time_t now = time(nullptr);
  while (now < 8 * 3600 * 2) {
    delay(500);
    now = time(nullptr);
  }
  struct tm timeinfo;
  gmtime_r(&now, &timeinfo);
}

void receiveCurrentData() {
  if (Serial.available()) {
    currentData = Serial.readString();
    currentData.trim();
    Serial.println("Received data: " + currentData);
  }
}

bool isEnoughFuel() {
  return (int)currentDataJSON["fuel"] > 25;
}

void handleDoorStatus() {
  String response = getDoorStatus();
  String command;
  if (response == DOOR_OPEN_COMMAND) {
    isDoorOpen = true;
    command = DOOR_OPEN_COMMAND;
  } else if (response == DOOR_CLOSED_COMMAND) {
    isDoorOpen = false;
    command = DOOR_CLOSED_COMMAND;
  }
  Serial1.println(command);
}

void handleCarStatus() {
  String response = getCarStatus();
  String command;
  if ((CarStatus)response.toInt() == CarStatus::WaitingForPassenger) {
    command = CAR_STOPPED_COMMAND;
  }
  else if ((CarStatus)response.toInt() == CarStatus::OnTrip) {
    command = CAR_STARTED_COMMAND;
  }
  Serial1.println(command);
}

void handleActiveTrip() {
  activeTripLocation = getActiveTripLocation();
  if (deserializeActiveTripLocation()) {
    Serial1.println(TRIP_ACTIVE_COMMAND);
  }
  handleActiveTripServices();
}

void handleActiveTripServices() {
  activeTripServices = getActiveTripServices();
  Serial.println("Active trip services: " + activeTripServices);
  
  if (deserializeActiveTripServices()) {
    String commands = activeTripServicesJSON["commands"];
    commands.remove(0, 1);
    commands.remove(commands.length() - 1, 1);

    int startPos = 0;
    int commaPos;
    while (startPos < commands.length()) {
      commaPos = commands.indexOf(", ", startPos);
      if (commaPos == -1) {
          commaPos = commands.length();
      }
      String substring = commands.substring(startPos + 1, commaPos - 1);
      Serial.println(substring);
      for (int i = 0; i < sizeof(SERVICES_COMMANDS) / sizeof(SERVICES_COMMANDS[0]); i++) {
          if (SERVICES_COMMANDS[i] == substring) {
            Serial1.println(substring);
            break;
          }
      }
      startPos = commaPos + 2; 
    }
  }
}

void handleTripRoute() {
  if (currentRouteJSON.isNull() && !activeTripLocationJSON.isNull()) {
    currentRoute = getTripRoute();
    deserializeCurrentRoute();
    Serial.println("Current route: " + currentRoute);
  }
}

void handleDestination() {
  double currentX = currentDataJSON["location"]["x"];
  double currentY = currentDataJSON["location"]["y"];
  double destinationX = activeTripLocationJSON["x"];
  double destinationY = activeTripLocationJSON["y"];

  Serial.println("currentX: " + (String)currentX + ", currentY: " + currentY + ", destination: " + destinationX + ", destinationY: " + destinationY);
  
  double tolerance = 0.01;

  if (fabs(currentX - destinationX) <= tolerance && fabs(currentY - destinationY) <= tolerance) {
    Serial1.println(DESTINATION_REACHED_COMMAND);
  }
}

void handleNearestGasStationRoute() {
  currentRoute = getNearestGasStationRoute();
  deserializeCurrentRoute();
  Serial.println("Current route: " + currentRoute);
  String command = LOW_FUEL_COMMAND;
  Serial1.println(command);
}

String getActiveTripLocation() {
  String request = HOST + GET_ACTIVE_TRIP_LOCATION_ENDPOINT + currentDataJSON["deviceId"].as<String>();
  return performHttpGet(request);
}

String getActiveTripServices() {
  String request = HOST + GET_ACTIVE_TRIP_SERVICES_ENDPOINT + currentDataJSON["deviceId"].as<String>();
  return performHttpGet(request);
}

String getDoorStatus() {
  String request = HOST + GET_DOOR_STATUS_ENDPOINT + currentDataJSON["deviceId"].as<String>();
  return performHttpGet(request);
}

String getCarStatus() {
  String request = HOST + GET_CAR_STATUS_ENDPOINT + currentDataJSON["deviceId"].as<String>();
  return performHttpGet(request);
}

String getTripRoute() {
  String currentCoordinates = String(currentDataJSON["location"]["x"].as<float>()) + "," + 
                              String(currentDataJSON["location"]["y"].as<float>());

  String destinationCoordinates = String(activeTripLocationJSON["x"].as<float>()) + "," +
                                  String(activeTripLocationJSON["y"].as<float>());

  String request = HOST + GET_TRIP_ROUTE_ENDPOINT + currentCoordinates + ";" + destinationCoordinates;
  return performHttpGet(request);
}

String getNearestGasStationRoute() {
  String request = HOST + GET_NEAREST_GAS_STATION_ROUTE_ENDPOINT + currentDataJSON["deviceId"].as<String>();
  return performHttpGet(request);
}

String performHttpGet(String request) {
  if (WiFi.status() != WL_CONNECTED) {
    Serial.printf("[HTTPS] Unable to connect\n");
    return "";
  }

  Serial.print("[HTTPS] begin...\n");

  if (https.begin(client, request)) {
    Serial.print("[HTTPS] GET...\n");

    int httpCode = https.GET();

    // httpCode will be negative on error
    if (httpCode > 0) {
      Serial.printf("[HTTPS] GET... code: %d\n", httpCode);

      if (httpCode == HTTP_CODE_OK || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
        String payload = https.getString();
        Serial.println(payload);
        deserializeJson(currentRouteJSON, https.getStream());
        return payload;
      }
    } else {
      Serial.printf("[HTTPS] GET... failed, error: %s\n", https.errorToString(httpCode).c_str());
    }

    https.end();
  }
  return "";
}

void sendCarUpdate() {
  if (currentDataJSON["isDoorOpen"] != isDoorOpen) {
    return;
  }
  
  if ((WiFi.status() != WL_CONNECTED)) {
    Serial.printf("[HTTPS] Unable to connect\n");
    return;
  }

  Serial.print("[HTTPS] begin...\n");

  String request = HOST + UPDATE_CAR_ENDPOINT;

  if (https.begin(client, request)) {

    https.addHeader("Content-Type", "application/json");
    Serial.print("[HTTP] POST...\n");
    
    int httpCode = https.POST(currentData);

    // httpCode will be negative on error
    if (httpCode > 0) {
      Serial.printf("[HTTPS] POST... code: %d\n", httpCode);

      if (httpCode == HTTP_CODE_OK || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
        String payload = https.getString();
        Serial.println(payload);
      }
    } else {
      Serial.printf("[HTTPS] POST... failed, error: %s\n", https.errorToString(httpCode).c_str());
    }

    https.end();
  }
}

bool deserializeCurrentData() {
  return deserializeJsonData(currentDataJSON, currentData);
}

bool deserializeActiveTripLocation() {
  return deserializeJsonData(activeTripLocationJSON, activeTripLocation);
}

bool deserializeActiveTripServices() {
  return deserializeJsonData(activeTripServicesJSON, activeTripServices);
}

bool deserializeCurrentRoute() {
  return deserializeJsonData(currentRouteJSON, currentRoute);
}

bool deserializeJsonData(DynamicJsonDocument &doc, String &data) {
  Serial.println("Deserializing data:");
  Serial.println(data);
  DeserializationError error = deserializeJson(doc, data);
  if (error) {
    Serial.print(F("Deserialization failed: "));
    Serial.println(error.f_str());
    return false;
  }
  return true;
}