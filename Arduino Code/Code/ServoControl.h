#ifndef SERVO_CONTROL_H
#define SERVO_CONTROL_H

#include <Arduino.h>

#define NUM_SERVOS 35
#define NUM_CONTROLLERS 3

extern float servoTargetAngle[NUM_SERVOS];
extern float speedDegPerSec;      //min: 60 / max: 600

void servoControlBegin();
void servoControlUpdate();
void servoControlSetTargetAngle(uint8_t index, float angle);

void servoControlSetPWMLimits(uint8_t index, uint16_t minPWM, uint16_t maxPWM);

#endif
