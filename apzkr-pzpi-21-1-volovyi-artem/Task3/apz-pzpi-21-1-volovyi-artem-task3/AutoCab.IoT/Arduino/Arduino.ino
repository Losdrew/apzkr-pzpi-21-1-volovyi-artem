#include <SoftwareSerial.h>
#include <ArduinoUniqueID.h>
#include <ArduinoJson.h>
#include <TinyGPS++.h>
#include <Wire.h>
#include "carstatus.h"
#include "commands.h"

const int SPEAKER_PIN = A0;

const int DEFAULT_DELAY = 3000;
const int FUEL_DECREMENT_INTERVAL = 15000;

const int MPU_ADDRESS = 0x68;  // I2C address of the MPU-6050

SoftwareSerial espSerial(8, 9); // Serial to transfer data to ESP8266 module
SoftwareSerial gpsSerial(11, 12); // Serial to transfer data to GPS module

TinyGPSPlus gps;

String deviceId;

int currentDelay;
CarStatus currentStatus;
String doorStatus;

bool isTripActive;
bool isDestinationReached;
bool isVoiceAssistantOn;

unsigned long currentMillis = 0;
unsigned long previousMillis = 0;

int fuelLevel = 100;

int16_t acX, acY, acZ, tmp, gyX, gyY, gyZ;

void setup() {
  Serial.begin(115200);
  espSerial.begin(115200);
  gpsSerial.begin(9600);

  while (!Serial) {
  ; // wait for serial port to connect. Needed for native USB port only
  }

  pinMode(SPEAKER_PIN, OUTPUT);

  Wire.begin();
  Wire.beginTransmission(MPU_ADDRESS);
  Wire.write(0x6B);  // PWR_MGMT_1 register
  Wire.write(0);     // set to zero (wakes up the MPU-6050)
  Wire.endTransmission(true);

  deviceId = getDeviceId();
  currentDelay = DEFAULT_DELAY;
  currentStatus = CarStatus::Idle;
  doorStatus = DOOR_CLOSED_COMMAND;
}

void loop() {
  currentMillis = millis();
  handleCarStatus();
  handleFuel();
  handleAccelerometer();
  handleServices();
  smartDelay(currentDelay);
  sendCarData();
}

void handleCarStatus() {
  Serial.println("Current status: " + String((int)currentStatus));

  if (currentStatus == CarStatus::Idle) {
    if (isTripActive) {
      Serial.println("Trip started");
      currentStatus = CarStatus::EnRoute;
    }
  }

  if (currentStatus == CarStatus::EnRoute) {
    if (isDestinationReached) {
      Serial.println("Waiting for passenger");
      currentStatus = CarStatus::WaitingForPassenger;
    }
  }

  if (currentStatus == CarStatus::WaitingForPassenger) {
    isDoorOpen();
  }
}

bool isDoorOpen() { 
  if (doorStatus == DOOR_OPEN_COMMAND) {
    Serial.println("Door open");
    return true;
  }
  if (doorStatus == DOOR_CLOSED_COMMAND) {
    Serial.println("Door closed");
    return false;
  }
  return false;
}

void smartDelay(unsigned long interval) {
  if (currentMillis - previousMillis >= interval) {
    previousMillis = currentMillis;
    unsigned long start = millis();
    do {
      readFromGPS();
      readFromESP();
    } while (millis() - start < interval);
  }
}

void readFromESP() {
  if (!Serial.available()) {
    return;
  }

  String response = Serial.readString();
  response.trim();

  if (response.length() > 0) {
    Serial.println("Received data: " + response);

    if (response.indexOf(DOOR_OPEN_COMMAND) != - 1 || response.indexOf(DOOR_CLOSED_COMMAND) != -1) {
      doorStatus = response.substring(response.indexOf(DOOR_OPEN_COMMAND), response.indexOf("\n"));
    }
    if (response.indexOf(TRIP_ACTIVE_COMMAND) != -1) {
      isTripActive = true;
    }
    if (response.indexOf(DESTINATION_REACHED_COMMAND) != -1) {
      isDestinationReached = true;
    }
    if (response.indexOf(CAR_STOPPED_COMMAND) != -1) {
      currentStatus = CarStatus::WaitingForPassenger;
    }
    if (response.indexOf(CAR_STARTED_COMMAND) != -1) {
      currentStatus = CarStatus::OnTrip;
    }
    if (response.indexOf(LOW_FUEL_COMMAND) != -1) {
      currentStatus = CarStatus::Maintenance;
    }
    if (response.indexOf(SERVICES_COMMANDS[0]) != -1) {
      isVoiceAssistantOn = true;
    }
  }
}

void readFromGPS() {
  while (gpsSerial.available()) {
    gps.encode(gpsSerial.read());
  }
}

void handleFuel() {
  static unsigned long previousFuelDecrement = 0;
  if (currentMillis - previousFuelDecrement >= FUEL_DECREMENT_INTERVAL) {
    previousFuelDecrement = currentMillis;
    if (fuelLevel > 0) {
      fuelLevel -= 1; 
    } else {
      Serial.println("Out of fuel!");
      currentStatus = CarStatus::Inactive;
    }
  }
}

void handleAccelerometer() {
  Wire.beginTransmission(MPU_ADDRESS);
  Wire.write(0x3B);  // starting with register 0x3B (ACCEL_XOUT_H)
  Wire.endTransmission(false);
  Wire.requestFrom(MPU_ADDRESS, 14, true);  // request a total of 14 registers
  acX=Wire.read()<<8|Wire.read();  // 0x3B (ACCEL_XOUT_H) & 0x3C (ACCEL_XOUT_L)     
  acY=Wire.read()<<8|Wire.read();  // 0x3D (ACCEL_YOUT_H) & 0x3E (ACCEL_YOUT_L)
  acZ=Wire.read()<<8|Wire.read();  // 0x3F (ACCEL_ZOUT_H) & 0x40 (ACCEL_ZOUT_L)
  tmp=(Wire.read()<<8|Wire.read()) / 340.00 + 36.53;  // 0x41 (TEMP_OUT_H) & 0x42 (TEMP_OUT_L)
  gyX=Wire.read()<<8|Wire.read();  // 0x43 (GYRO_XOUT_H) & 0x44 (GYRO_XOUT_L)
  gyY=Wire.read()<<8|Wire.read();  // 0x45 (GYRO_YOUT_H) & 0x46 (GYRO_YOUT_L)
  gyZ=Wire.read()<<8|Wire.read();  // 0x47 (GYRO_ZOUT_H) & 0x48 (GYRO_ZOUT_L)
}

void handleServices() {
  if (isVoiceAssistantOn) {
    tone(SPEAKER_PIN, 300, 600);
  }
}

void sendCarData() {
  StaticJsonDocument<96> doc;

  doc["deviceId"] = deviceId;
  doc["status"] = (int)currentStatus;
  doc["isDoorOpen"] = (doorStatus == DOOR_OPEN_COMMAND);
  doc["fuel"] = fuelLevel;
  doc["temperature"] = tmp;

  JsonObject location = doc.createNestedObject("location");
  
  location["x"] = gps.location.lng();
  location["y"] = gps.location.lat();

  char jsonBuffer[192]; 
  serializeJson(doc, jsonBuffer, 192);
  Serial.println(jsonBuffer);
  espSerial.println(jsonBuffer);
}

String getDeviceId() {
  String deviceId;
  for (size_t i = 0; i < UniqueIDsize; i++) {
    deviceId += String(UniqueID[i], HEX);
  }
  deviceId.toUpperCase();
  return deviceId;
}