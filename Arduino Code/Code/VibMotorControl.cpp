#include "VibMotorControl.h"

const uint8_t vibMotorPins[NUM_VIBMOTORS] = {
  2, 3, 4, 5, 6, 7, 8,
  9, 10, 11, 12, 46, 44, 45
};

bool vibMotorActive[NUM_VIBMOTORS] = {
  0, 0, 0, 0, 0, 0, 0,
  0, 0, 0, 0, 0, 0, 0
};

unsigned long vibMotorOnTime[NUM_VIBMOTORS] = {
  500, 500, 500, 500, 500, 500, 500,
  500, 500, 500, 500, 500, 500, 500
};

unsigned long vibMotorOffTime[NUM_VIBMOTORS] = {
  500, 500, 500, 500, 500, 500, 500,
  500, 500, 500, 500, 500, 500, 500
};

uint8_t vibMotorIntensityPercent[NUM_VIBMOTORS] = {
  100, 100, 100, 100, 100, 100, 100,                       
  100, 100, 100, 100, 100, 100, 100
};

// Internal state (only needed in this file)
static bool vibMotorIsOn[NUM_VIBMOTORS] = { false };
static unsigned long vibMotorLastToggleTime[NUM_VIBMOTORS] = { 0 };

void setupVibMotors() {
  for (uint8_t i = 0; i < NUM_VIBMOTORS; i++) {
    pinMode(vibMotorPins[i], OUTPUT);
    analogWrite(vibMotorPins[i], 0);
    vibMotorIsOn[i] = false;
    vibMotorLastToggleTime[i] = millis();
  }
}

void updateVibMotors() {
  unsigned long currentMillis = millis();

  for (uint8_t i = 0; i < NUM_VIBMOTORS; i++) {
    if (!vibMotorActive[i]) {
      analogWrite(vibMotorPins[i], 0);
      vibMotorIsOn[i] = false;
      continue;
    }

    if (vibMotorIsOn[i]) {
      if (currentMillis - vibMotorLastToggleTime[i] >= vibMotorOnTime[i]) {
        analogWrite(vibMotorPins[i], 0);
        vibMotorIsOn[i] = false;
        vibMotorLastToggleTime[i] = currentMillis;
      }
    } else {
      if (currentMillis - vibMotorLastToggleTime[i] >= vibMotorOffTime[i]) {
        uint8_t intensityPWM = map(vibMotorIntensityPercent[i], 0, 100, 0, 255);
        analogWrite(vibMotorPins[i], intensityPWM);
        vibMotorIsOn[i] = true;
        vibMotorLastToggleTime[i] = currentMillis;
      }
    }
  }
}