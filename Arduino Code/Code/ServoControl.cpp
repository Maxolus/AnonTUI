#include "ServoControl.h"
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

static const uint8_t controllerAddresses[NUM_CONTROLLERS] = {0x40, 0x41, 0x42};
static Adafruit_PWMServoDriver pwmControllers[NUM_CONTROLLERS] = {
  Adafruit_PWMServoDriver(0x40),
  Adafruit_PWMServoDriver(0x41),
  Adafruit_PWMServoDriver(0x42)
};

// PWM limits per servo
uint16_t pwmMin[NUM_SERVOS] = {
  110, 110, 110, 
  120, 110, 115, 
  120, 120, 110, 
  120, 110, 110, 
  110, 110, 125, 
  110, 110, 80,         // Module 6
  110, 110, 60, 
  110, 100, 
  110, 80, 
  110, 120,             // Module 10
  110, 90, 
  110, 70,              // Module 12
  110, 80, 
  110, 60
};

uint16_t pwmMax[NUM_SERVOS] = {
  525, 525, 560, 
  525, 530, 525, 
  525, 525, 555, 
  505, 525, 530, 
  525, 525, 525, 
  520, 520, 620,        // Module 6
  525, 510, 540, 
  525, 550, 
  520, 550, 
  525, 580,             // Module 10
  525, 585,             // Module 11
  500, 555, 
  525, 555, 
  525, 580
};

// Angle limits per servo
float angleMin[NUM_SERVOS] = {
  15, 10, 0, 
  15, 10, 0, 
  15, 10, 0, 
  15, 10, 0, 
  15, 10, 0, 
  15, 10, 0, 
  15, 10, 0, 
  10, 0,
  10, 0, 
  10, 0, 
  10, 0, 
  10, 0, 
  10, 0, 
  10, 0
};

float angleMax[NUM_SERVOS] = {
  165, 90, 180, 
  165, 90, 180, 
  165, 90, 180, 
  165, 90, 180, 
  165, 90, 180, 
  165, 90, 180, 
  165, 90, 180, 
  90, 180, 
  90, 180, 
  90, 180, 
  90, 180, 
  90, 180, 
  90, 180, 
  90, 180
};

// Start angle per servo
float servoTargetAngle[NUM_SERVOS] = {
  90, 0, 90,              //0,1,2
  90, 0, 90,              //3,4,5
  90, 0, 90,              //6,7,8
  90, 0, 90,              //9,10,11
  90, 0, 90,              //12,13,14
  90, 0, 90,              //15,16,17
  90, 0, 90,              //18,19,20
  0, 90,                  //21,22
  0, 90,                  //23,24            
  0, 90,                  //25,26 
  0, 90,                  //27,28 
  0, 90,                  //29,30 
  0, 90,                  //31,32 
  0, 90                   //33,34
};


float servoStartAngle[NUM_SERVOS] = {
  90, 0, 90, 
  90, 0, 90, 
  90, 0, 90, 
  90, 0, 90, 
  90, 0, 90, 
  90, 0, 90, 
  90, 0, 90, 
  0, 90, 
  0, 90, 
  0, 90, 
  0, 90, 
  0, 90, 
  0, 90, 
  0, 90
};

static float servoCurrentAngle[NUM_SERVOS];

float speedDegPerSec = 200.0;      //min: 60 / max: 600
static unsigned long lastUpdate = 0;
static const unsigned long updateInterval = 20; // ms

static void writeServo(uint8_t index, uint16_t pwmValue);
static uint16_t angleToPWM(uint8_t index, float angle);



void servoControlBegin() {
  Wire.begin();

  for (uint8_t i = 0; i < NUM_CONTROLLERS; i++) {
    pwmControllers[i].begin();
    pwmControllers[i].setPWMFreq(50); // 50Hz for Servos
  }

  for (uint8_t i = 0; i < NUM_SERVOS; i++) {
    servoCurrentAngle[i] = servoStartAngle[i]; // start at this angle
    writeServo(i, angleToPWM(i, servoCurrentAngle[i])); // set initial value directly
  }

  delay(500); // short pause for initialization

  // Move all servos to target position
  lastUpdate = millis();
  while (true) {
    servoControlUpdate();
    bool allReached = true;
    for (uint8_t i = 0; i < NUM_SERVOS; i++) {
      if (abs(servoCurrentAngle[i] - servoTargetAngle[i]) > 1.0) {
        allReached = false;
        break;
      }
    }
    if (allReached) break;
    delay(updateInterval);
  }
}


void servoControlUpdate() {
  unsigned long now = millis();
  if (now - lastUpdate < updateInterval) return;

  float dt = (now - lastUpdate) / 1000.0f;
  lastUpdate = now;

  float maxStep = speedDegPerSec * dt;

  for (uint8_t i = 0; i < NUM_SERVOS; i++) {
    float diff = servoTargetAngle[i] - servoCurrentAngle[i];
    if (abs(diff) > maxStep) {
      servoCurrentAngle[i] += (diff > 0 ? maxStep : -maxStep);
    } else {
      servoCurrentAngle[i] = servoTargetAngle[i];
    }
    writeServo(i, angleToPWM(i, servoCurrentAngle[i]));
  }
}


void servoControlSetTargetAngle(uint8_t index, float angle) {
  if (index >= NUM_SERVOS) return;
  servoTargetAngle[index] = constrain(angle, angleMin[index], angleMax[index]);
}




static void writeServo(uint8_t index, uint16_t pwmValue) {
  uint8_t controllerIndex = index / 16;
  uint8_t channel = index % 16;
  pwmControllers[controllerIndex].setPWM(channel, 0, pwmValue);
}


static uint16_t angleToPWM(uint8_t index, float angle) {
  angle = constrain(angle, angleMin[index], angleMax[index]);
  float ratio = (angle - 0) / (180 - 0);      //float ratio = (angle - angleMin[index]) / (angleMax[index] - angleMin[index]);
  return pwmMin[index] + ratio * (pwmMax[index] - pwmMin[index]);
}
